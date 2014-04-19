#if TODO_XAML
using System;
using swc = Windows.UI.Xaml.Controls;
using sw = Windows.UI.Xaml;
using Eto.Forms;
using mwc = Xceed.Wpf.Toolkit;

namespace Eto.Platform.Xaml.Forms.Controls
{
	public class NumericUpDownHandler : WpfControl<mwc.DoubleUpDown, NumericUpDown>, INumericUpDown
	{
		public NumericUpDownHandler ()
		{
			Control = new mwc.DoubleUpDown ();
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
			get { return Control.Value ?? 0; }
			set { Control.Value = value; }
		}

		public double MinValue
		{
			get { return Control.Minimum ?? double.MinValue; }
			set { Control.Minimum = value; }
		}

		public double MaxValue
		{
			get { return Control.Maximum ?? double.MaxValue; }
			set { Control.Maximum = value; }
		}

	}
}
#endif