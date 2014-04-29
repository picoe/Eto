using System;
using swc = Windows.UI.Xaml.Controls;
using sw = Windows.UI.Xaml;
using Eto.Forms;
using mwc = WinRTXamlToolkit.Controls;
using Eto.Drawing;

namespace Eto.WinRT.Forms.Controls
{
	public class NumericUpDownHandler : WpfControl<mwc.NumericUpDown, NumericUpDown>, INumericUpDown
	{
		public NumericUpDownHandler ()
		{
			Control = new mwc.NumericUpDown { ValueBarVisibility = mwc.NumericUpDownValueBarVisibility.Visible};
			Control.ValueChanged += (sender, e) => Widget.OnValueChanged(EventArgs.Empty);
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
	}
}
