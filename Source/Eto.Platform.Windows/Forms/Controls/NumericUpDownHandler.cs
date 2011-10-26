using System;
using SD = System.Drawing;
using SWF = System.Windows.Forms;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Platform.Windows
{
	public class NumericUpDownHandler : WindowsControl<SWF.NumericUpDown, NumericUpDown>, INumericUpDown
	{
		public NumericUpDownHandler ()
		{
			Control = new SWF.NumericUpDown{
				Maximum = 100,
				Minimum = 0,
				Width = 40
			};
			this.Control.ValueChanged += delegate {
				Widget.OnValueChanged (EventArgs.Empty);
			};
		}
		
		public bool ReadOnly {
			get { return Control.ReadOnly; }
			set { Control.ReadOnly = value; }
		}
		
		public double Value {
			get { return (double)Control.Value; }
			set { Control.Value = (decimal)value; }
		}
		
		public double MinValue {
			get { return (double)Control.Minimum; }
			set { Control.Minimum = (decimal)value; }
		}

		public double MaxValue {
			get { return (double)Control.Maximum; }
			set { Control.Maximum = (decimal)value; }
		}
	}
}
