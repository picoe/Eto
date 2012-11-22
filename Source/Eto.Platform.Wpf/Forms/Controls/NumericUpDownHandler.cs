using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using swc = System.Windows.Controls;
using sw = System.Windows;
using Eto.Forms;
using mwc = Microsoft.Windows.Controls;
using Eto.Drawing;
using Eto.Platform.Wpf.Drawing;

namespace Eto.Platform.Wpf.Forms.Controls
{
	public class NumericUpDownHandler : WpfControl<mwc.DoubleUpDown, NumericUpDown>, INumericUpDown
	{
		public NumericUpDownHandler ()
		{
			Control = new mwc.DoubleUpDown ();
			Control.ValueChanged += (sender, e) => {
				Widget.OnValueChanged (EventArgs.Empty);
			};
		}

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
