using System;
using Eto.Drawing;
using swc = System.Windows.Controls;
using sw = System.Windows;
using Eto.Forms;
using mwc = Xceed.Wpf.Toolkit;
using System.Text;

namespace Eto.Wpf.Forms.Controls
{
	public class EtoDoubleUpDown : mwc.DoubleUpDown, IEtoWpfControl
	{
		public new swc.TextBox TextBox { get { return base.TextBox; } }
		public new mwc.Spinner Spinner { get { return base.Spinner; } }

		public IWpfFrameworkElement Handler { get; set; }

		protected override sw.Size MeasureOverride(sw.Size constraint)
		{
			return Handler?.MeasureOverride(constraint, base.MeasureOverride) ?? base.MeasureOverride(constraint);
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
				Control.Value = Math.Max(MinValue, Math.Min(MaxValue, double.Parse((Control.Value ?? 0).ToString())));
				TriggerValueChanged();
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
					Control.Spinner.GotKeyboardFocus += (sender2, e2) => Control.TextBox.Focus();
				Control.Loaded -= Control_Loaded;
			}
		}

		void TriggerValueChanged()
		{
			var val = Value;
			if (lastValue != val)
			{
				Callback.OnValueChanged(Widget, EventArgs.Empty);
				lastValue = val;
			}
		}

		public override bool UseMousePreview { get { return true; } }

		public override bool UseKeyPreview { get { return true; } }

		public bool ReadOnly
		{
			get { return !Control.IsEnabled; }
			set { Control.IsEnabled = !value; }
		}

		public double Value
		{
			get { return Math.Round(Control.Value ?? 0, MaximumDecimalPlaces); }
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
				Control.TextBox.Focus();
			else
				Control.Loaded += HandleFocus;
		}

		void HandleFocus(object sender, sw.RoutedEventArgs e)
		{
			Control.TextBox.Focus();
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

		void UpdateRequiredDigits()
		{
			if (MaximumDecimalPlaces > 0 || DecimalPlaces > 0)
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
