using System;
using Eto.Forms;

namespace Eto.Platform.GtkSharp
{
	public class NumericUpDownHandler : GtkControl<Gtk.SpinButton, NumericUpDown>, INumericUpDown
	{
		public NumericUpDownHandler ()
		{
			Control = new Gtk.SpinButton (0, 100, 1);
			Control.WidthRequest = 80;
			Control.Wrap = true;
			Control.ValueChanged += delegate {
				Widget.OnValueChanged (EventArgs.Empty);
			};
		}
		
		public override string Text {
			get { return Control.Text; }
			set { Control.Text = value; }
		}
		
		public bool ReadOnly {
			get { return !Control.IsEditable; }
			set { Control.IsEditable = !value; }
		}
		
		public double Value {
			get { return Control.Value; }
			set { Control.Value = value; }
		}
		
		public double MaxValue {
			get { return Control.Adjustment.Upper; }
			set { Control.Adjustment.Upper = value; }
		}
		
		public double MinValue {
			get { return Control.Adjustment.Lower; }
			set { Control.Adjustment.Lower = value; }
		}
	}
}
