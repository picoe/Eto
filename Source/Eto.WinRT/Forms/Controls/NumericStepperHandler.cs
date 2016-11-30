using System;
using swc = Windows.UI.Xaml.Controls;
using sw = Windows.UI.Xaml;
using Eto.Forms;
using mwc = WinRTXamlToolkit.Controls;
using Eto.Drawing;
using System.Text;

namespace Eto.WinRT.Forms.Controls
{
	public class NumericStepperHandler : WpfControl<mwc.NumericUpDown, NumericStepper, NumericStepper.ICallback>, NumericStepper.IHandler
	{
		public NumericStepperHandler()
		{
			Control = new mwc.NumericUpDown { ValueBarVisibility = mwc.NumericUpDownValueBarVisibility.Visible, ValueFormat = "0" };
			Control.ValueChanged += (sender, e) => Callback.OnValueChanged(Widget, EventArgs.Empty);
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
			get { return Control.Value; }
			set { Control.Value = value; }
		}

		public double MinValue
		{
			get { return Control.Minimum; }
			set { Control.Minimum = value; }
		}

		public double MaxValue
		{
			get { return Control.Maximum; }
			set { Control.Maximum = value; }
		}

		public double Increment
		{
			get { return Control.SmallChange; }
			set { Control.SmallChange = value; }
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

		public Color TextColor
		{
			get { return Control.Foreground.ToEtoColor(); }
			set { Control.Foreground = value.ToWpfBrush(Control.Foreground); }
		}

		static readonly object MaxiumumDecimalPlaces_Key = new object();

		public int MaximumDecimalPlaces
		{
			get { return Widget.Properties.Get<int>(MaxiumumDecimalPlaces_Key); }
			set
			{
				Widget.Properties.Set(MaxiumumDecimalPlaces_Key, value, () =>
				{
					DecimalPlaces = Math.Min(DecimalPlaces, value);
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
				Control.ValueFormat = format.ToString();
			}
			else
				Control.ValueFormat = "0";
		}
	}
}
