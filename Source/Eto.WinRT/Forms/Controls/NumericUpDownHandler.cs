using System;
using swc = Windows.UI.Xaml.Controls;
using sw = Windows.UI.Xaml;
using Eto.Forms;
using mwc = WinRTXamlToolkit.Controls;
using Eto.Drawing;

namespace Eto.WinRT.Forms.Controls
{
	public class NumericUpDownHandler : WpfControl<mwc.NumericUpDown, NumericUpDown, NumericUpDown.ICallback>, NumericUpDown.IHandler
	{
		public NumericUpDownHandler()
		{
			Control = new mwc.NumericUpDown { ValueBarVisibility = mwc.NumericUpDownValueBarVisibility.Visible };
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

		int decimalPlaces;
		public int DecimalPlaces
		{
			get { return decimalPlaces; }
			set
			{
				if (value != decimalPlaces)
				{
					decimalPlaces = value;
					Control.ValueFormat = decimalPlaces == 0 ? "0" : "0." + new string('0', decimalPlaces);
				}
			}
		}
	}
}
