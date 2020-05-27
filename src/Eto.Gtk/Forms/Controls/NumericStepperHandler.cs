using System;
using Eto.Forms;
using Eto.Drawing;
using System.Globalization;
using System.ComponentModel;
using Gtk;
using System.Text.RegularExpressions;

namespace Eto.GtkSharp.Forms.Controls
{
	public class NumericStepperHandler : GtkControl<Gtk.SpinButton, NumericStepper, NumericStepper.ICallback>, NumericStepper.IHandler
	{
		public NumericStepperHandler()
		{
			Control = new Gtk.SpinButton(double.MinValue, double.MaxValue, 1);
			Control.WidthChars = 5; // default to show 5 characters
			Value = 0;
		}

		protected override void SetSize(Size size)
		{
			// if a width is set, we allow it to shrink as small as possible..
			Control.WidthChars = size.Width > 0 ? 0 : 5;
			base.SetSize(size);
		}

		static readonly object SuppressValueChanged_Key = new object();

		int SuppressValueChanged
		{
			get { return Widget.Properties.Get<int>(SuppressValueChanged_Key); }
			set { Widget.Properties.Set(SuppressValueChanged_Key, value); }
		}

		protected override void Initialize()
		{
			base.Initialize();
			Control.ValueChanged += Connector.HandleValueChanged;
			Control.Input += Connector.HandleInput;
			Control.Output += Connector.HandleOutput;
		}

		protected new NumericStepperConnector Connector { get { return (NumericStepperConnector)base.Connector; } }

		protected override WeakConnector CreateConnector()
		{
			return new NumericStepperConnector();
		}

		protected class NumericStepperConnector : GtkControlConnector
		{
			public new NumericStepperHandler Handler { get { return (NumericStepperHandler)base.Handler; } }

			public void HandleValueChanged(object sender, EventArgs e)
			{
				if (Handler.SuppressValueChanged <= 0)
				{
					Handler.UpdateRequiredDigits();
					Handler.Callback.OnValueChanged(Handler.Widget, EventArgs.Empty);
				}
			}

			string TrimNumericString(string text) => Regex.Replace(text, $"[ ]|({Regex.Escape(Handler.CultureInfo.NumberFormat.NumberGroupSeparator)})", "");

			bool NumberStringsMatch(string num1, string num2) => string.Compare(TrimNumericString(num1), TrimNumericString(num2), Handler.CultureInfo, CompareOptions.IgnoreCase) == 0;

			[GLib.ConnectBefore]
			public void HandleInput(object o, InputArgs args)
			{
				var h = Handler;
				if (h.NeedsFormat)
				{
					var text = h.Text;
					if (h.HasFormatString)
						text = Regex.Replace(text, $@"(?!\d|{Regex.Escape(h.CultureInfo.NumberFormat.NumberDecimalSeparator)}|{Regex.Escape(h.CultureInfo.NumberFormat.NegativeSign)}).", ""); // strip any non-numeric value

					double result;
					if (double.TryParse(text, NumberStyles.Any, h.CultureInfo, out result))
					{
						if (h.HasFormatString && result > 0 && NumberStringsMatch((-result).ToString(h.FormatString, h.CultureInfo), h.Text))
							result = -result;

						args.NewValue = result;
						args.RetVal = 1;
						return;
					}
				}
				args.NewValue = h.Control.Adjustment.Value;
				args.RetVal = 0;
			}

			[GLib.ConnectBefore]
			public void HandleOutput(object o, OutputArgs args)
			{
				var h = Handler;
				if (h.NeedsFormat)
				{
					var val = h.Control.Adjustment.Value;
					var format = h.CurrentFormatString;
					var text = format == null ? val.ToString(h.CultureInfo) : val.ToString(format, h.CultureInfo);
					h.Control.Text = text;
					args.RetVal = 1;
					return;
				}
				args.RetVal = 0;
			}
		}

		public override string Text
		{
			get { return Control.Text; }
			set { Control.Text = value; }
		}

		public bool ReadOnly
		{
			get { return !Control.IsEditable; }
			set
			{
				Control.IsEditable = !value; 
				SetIncrement();
			}
		}

		public double Value
		{
			get { return HasFormatString ? Control.Value : Math.Round(Control.Value, MaximumDecimalPlaces); }
			set { Control.Value = Math.Max(MinValue, Math.Min(MaxValue, value)); }
		}

		public double MaxValue
		{
			get { return Control.Adjustment.Upper == double.MaxValue ? double.PositiveInfinity : Control.Adjustment.Upper; }
			set
			{ 
				Control.Adjustment.Upper = double.IsPositiveInfinity(value) ? double.MaxValue : value; 
				Value = Value;
			}
		}

		public double MinValue
		{
			get { return Control.Adjustment.Lower == double.MinValue ? double.NegativeInfinity : Control.Adjustment.Lower; }
			set
			{
				Control.Adjustment.Lower = double.IsNegativeInfinity(value) ? double.MinValue : value; 
				Value = Value;
			}
		}

		static readonly object Increment_Key = new object();

		public double Increment
		{
			get { return Widget.Properties.Get<double>(Increment_Key, 1); }
			set { Widget.Properties.Set(Increment_Key, value, SetIncrement, 1); }
		}

		void SetIncrement()
		{
			var adjustment = Control.Adjustment;
			var inc = ReadOnly ? 0 : Increment;

			adjustment.StepIncrement = adjustment.PageIncrement = inc;
		}

		static readonly object DecimalPlaces_Key = new object();

		public int DecimalPlaces
		{
			get { return Widget.Properties.Get<int>(DecimalPlaces_Key); }
			set
			{ 
				Widget.Properties.Set(DecimalPlaces_Key, value, () =>
				{
					MaximumDecimalPlaces = Math.Max(value, MaximumDecimalPlaces);
					UpdateRequiredDigits();
				});
			}
		}

		int GetNumberOfDigits()
		{
			var str = Control.Value.ToString(CultureInfo.InvariantCulture);
			var idx = str.IndexOf(CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator, StringComparison.Ordinal);
			return idx > 0 ? str.Length - idx - 1 : 0;
		}

		void UpdateRequiredDigits()
		{
			SuppressValueChanged++;
			_formatString = null;
			if (MaximumDecimalPlaces > DecimalPlaces)
			{
				// prevent spinner from accumulating an inprecise value, which would eventually 
				// show values like 1.0000000000001 or 1.999999999998
				var val = double.Parse(Control.Value.ToString());
				Control.Adjustment.Value = val;
			}
			Control.Digits = (uint)Math.Max(Math.Min(GetNumberOfDigits(), MaximumDecimalPlaces), DecimalPlaces);
			SuppressValueChanged--;
		}

		public Color TextColor
		{
			get { return Control.GetTextColor(); }
			set { Control.SetTextColor(value); }
		}

		public override Color BackgroundColor
		{
			get { return Control.GetBase(); }
			set { Control.SetBase(value); }
		}

		static readonly object MaximumDecimalPlaces_Key = new object();

		public int MaximumDecimalPlaces
		{
			get { return Widget.Properties.Get<int>(MaximumDecimalPlaces_Key); }
			set
			{
				Widget.Properties.Set(MaximumDecimalPlaces_Key, value, () =>
				{
					DecimalPlaces = Math.Min(value, DecimalPlaces);
					UpdateRequiredDigits();
				}); 
			}
		}

		string _formatString;

		string CurrentFormatString
		{
			get
			{
				var format = FormatString;
				if (!string.IsNullOrEmpty(format))
					return format;
				if (_formatString != null)
					return _formatString;
				_formatString = "0.";
				if (DecimalPlaces > 0)
					_formatString += new string('0', DecimalPlaces);
				if (MaximumDecimalPlaces > DecimalPlaces)
					_formatString += new string('#', MaximumDecimalPlaces - DecimalPlaces);
				return _formatString;
			}
		}

		bool NeedsFormat => HasFormatString || CultureInfo != CultureInfo.CurrentCulture;
		bool HasFormatString => !string.IsNullOrEmpty(FormatString);

		static readonly object FormatString_Key = new object();

		public string FormatString
		{
			get { return Widget.Properties.Get<string>(FormatString_Key); }
			set {
				// ensure format is valid first, GTK crashes if the formating throws in the Output event, even if caught while setting this
				if (!string.IsNullOrEmpty(value))
					0.0.ToString(value);
				Widget.Properties.Set(FormatString_Key, value, UpdateFormat);
			}
		}

		void UpdateFormat()
		{
			_formatString = null;
			// GTK doesn't remember the value if the format changes as it tries to parse the old text with the new format
			Control.Value = Control.Value; 

			// update to the new text
			Control.Update();
		}

		static readonly object CultureInfo_Key = new object();

		public CultureInfo CultureInfo
		{
			get { return Widget.Properties.Get(CultureInfo_Key, CultureInfo.CurrentCulture); }
			set { Widget.Properties.Set(CultureInfo_Key, value, UpdateFormat, CultureInfo.CurrentCulture); }
		}
	}
}
