using System;
using Eto.Drawing;
using swc = System.Windows.Controls;
using sw = System.Windows;
using Eto.Forms;
using mwc = Xceed.Wpf.Toolkit;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Eto.Wpf.Forms.Controls
{
	public class EtoDoubleUpDown : mwc.DoubleUpDown, IEtoWpfControl
	{
		public new swc.TextBox TextBox { get { return base.TextBox; } }
		public new mwc.Spinner Spinner { get { return base.Spinner; } }

		public IWpfFrameworkElement Handler { get; set; }

		NumericStepperHandler StepperHandler => Handler as NumericStepperHandler;

		protected override sw.Size MeasureOverride(sw.Size constraint)
		{
			return Handler?.MeasureOverride(constraint, base.MeasureOverride) ?? base.MeasureOverride(constraint);
		}

		string TrimNumericString(string text) => Regex.Replace(text, $"[ ]|({Regex.Escape(StepperHandler.CultureInfo.NumberFormat.NumberGroupSeparator)})", "");

		bool NumberStringsMatch(string num1, string num2) => string.Compare(TrimNumericString(num1), TrimNumericString(num2), StepperHandler.CultureInfo, CompareOptions.IgnoreCase) == 0;

		protected override double? ConvertTextToValue(string text)
		{
			var h = StepperHandler;
			var trimmedText = text;
			if (h?.HasFormatString == true && trimmedText != null)
				trimmedText = Regex.Replace(trimmedText, $@"(?!\d|{Regex.Escape(h.CultureInfo.NumberFormat.NumberDecimalSeparator)}|{Regex.Escape(h.CultureInfo.NumberFormat.NegativeSign)}).", ""); // strip any non-numeric value
			try
			{
				var result = base.ConvertTextToValue(trimmedText);

				// test if the text matches the negative format
				if (h.HasFormatString && result > 0 && NumberStringsMatch((-result.Value).ToString(FormatString, CultureInfo), text))
					result = -result;

				return result;
			}
			catch
			{
				return null;
			}
		}

		protected override string ConvertValueToText()
		{
			return base.ConvertValueToText();
		}
	}

	public class NumericStepperHandler : WpfControl<EtoDoubleUpDown, NumericStepper, NumericStepper.ICallback>, NumericStepper.IHandler
	{
		double lastValue;

		protected override sw.Size DefaultSize => new sw.Size(80, double.NaN);

		protected override bool PreventUserResize { get { return true; } }

		public NumericStepperHandler()
		{
			Control = new EtoDoubleUpDown
			{
				Handler = this,
				FormatString = "0",
				Value = 0
			};
			Control.ValueChanged += (sender, e) =>
			{
				var val = Math.Max(MinValue, Math.Min(MaxValue, double.Parse((Control.Value ?? 0).ToString())));
				Control.Value = val;
				TriggerValueChanged();
				var textBox = Control.TextBox;
				if (val != Control.Value && textBox != null)
				{
					// callback set a different value, but it still shows just whatever the user typed in.
					// possibly due to some async call in Xceed.Wpf.Toolkit.
					// so, we set the text specifically.
					textBox.Text = Control.Value.Value.ToString(Control.FormatString, Control.CultureInfo);
					textBox.SelectionStart = textBox.Text.Length;
					textBox.SelectionLength = 0;
				}
			};
			Control.Loaded += Control_Loaded;
		}

		void Control_Loaded(object sender, sw.RoutedEventArgs e)
		{
			// ensure changed event fires when changing the text, not just when focus is lost
			if (Control.TextBox != null)
			{
				Control.TextBox.TextChanged += (sender2, e2) => TriggerValueChanged();
				if (Control.Spinner != null)
					Control.Spinner.GotKeyboardFocus += (sender2, e2) => Control.TextBox?.Focus();
				Control.Loaded -= Control_Loaded;
			}
		}

		void TriggerValueChanged()
		{
			var val = Value;
			if (lastValue != val)
			{
				lastValue = val;
				Callback.OnValueChanged(Widget, EventArgs.Empty);
			}
		}

		public override bool UseMousePreview { get { return true; } }

		public override bool UseKeyPreview { get { return true; } }

		public bool ReadOnly
		{
			get { return Control.IsReadOnly; }
			set { Control.IsReadOnly = value; }
		}

		public double Value
		{
			get { return HasFormatString ? Control.Value ?? 0 : Math.Round(Control.Value ?? 0, MaximumDecimalPlaces); }
			set { Control.Value = Math.Max(MinValue, Math.Min(MaxValue, value)); }
		}

		public double MinValue
		{
			get { return Control.Minimum ?? double.MinValue; }
			set
			{
				Control.Minimum = value;
				Control.Value = Math.Max(value, Value);
			}
		}

		public double MaxValue
		{
			get { return Control.Maximum ?? double.MaxValue; }
			set
			{
				Control.Maximum = value;
				Control.Value = Math.Min(value, Value);
			}
		}

		static readonly object DecimalPlaces_Key = new object();

		public int DecimalPlaces
		{
			get { return Widget.Properties.Get<int>(DecimalPlaces_Key); }
			set
			{
				Widget.Properties.Set(DecimalPlaces_Key, value, () =>
				{
					if (MaximumDecimalPlaces < value)
						MaximumDecimalPlaces = value;
					UpdateRequiredDigits();
				});
			}
		}

		public double Increment
		{
			get { return Control.Increment ?? 1; }
			set { Control.Increment = value; }
		}

		public override void Focus()
		{
			// focus the inner text box
			if (Control.IsLoaded)
				Control.TextBox?.Focus();
			else
				Control.Loaded += HandleFocus;
		}

		void HandleFocus(object sender, sw.RoutedEventArgs e)
		{
			Control.TextBox?.Focus();
			Control.Loaded -= HandleFocus;
		}

		static readonly object MaxiumumDecimalPlaces_Key = new object();

		public int MaximumDecimalPlaces
		{
			get { return Widget.Properties.Get<int>(MaxiumumDecimalPlaces_Key); }
			set
			{
				Widget.Properties.Set(MaxiumumDecimalPlaces_Key, value, () =>
				{
					if (DecimalPlaces > value)
						DecimalPlaces = value;
					UpdateRequiredDigits();
				});
			}
		}

		internal bool HasFormatString => !string.IsNullOrEmpty(FormatString);

		static readonly object FormatString_Key = new object();

		public string FormatString
		{
			get { return Widget.Properties.Get<string>(FormatString_Key); }
			set
			{
				Widget.Properties.Set(FormatString_Key, value, UpdateRequiredDigits);
			}
		}

		public CultureInfo CultureInfo
		{
			get { return Control.CultureInfo; }
			set { Control.CultureInfo = value; }
		}

		void UpdateRequiredDigits()
		{
			if (HasFormatString)
				Control.FormatString = FormatString;
			else if (MaximumDecimalPlaces > 0 || DecimalPlaces > 0)
			{
				var format = new StringBuilder();
				format.Append("0.");
				if (DecimalPlaces > 0)
					format.Append(new string('0', DecimalPlaces));
				if (MaximumDecimalPlaces > DecimalPlaces)
					format.Append(new string('#', MaximumDecimalPlaces - DecimalPlaces));
				Control.FormatString = format.ToString();
			}
			else
				Control.FormatString = "0";
		}
	}
}
