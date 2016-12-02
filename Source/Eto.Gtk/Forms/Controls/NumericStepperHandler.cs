using System;
using Eto.Forms;
using Eto.Drawing;
using System.Globalization;

namespace Eto.GtkSharp.Forms.Controls
{
	public class NumericStepperHandler : GtkControl<Gtk.SpinButton, NumericStepper, NumericStepper.ICallback>, NumericStepper.IHandler
	{
		class EtoSpinButton : Gtk.SpinButton
		{
			public EtoSpinButton()
				: base(double.MinValue, double.MaxValue, 0)
			{
			}
		}

		public NumericStepperHandler()
		{
			Control = new EtoSpinButton();
			Control.WidthRequest = 120;
			Control.Wrap = true;
			Control.Adjustment.StepIncrement = 1;
			Control.Adjustment.PageIncrement = 1;
			Value = 0;
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
			get { return Math.Round(Control.Value, MaximumDecimalPlaces); }
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
			var idx = str.IndexOf(CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator);
			return idx > 0 ? str.Length - idx - 1 : 0;
		}

		void UpdateRequiredDigits()
		{
			SuppressValueChanged++;
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
	}
}
