using System;
using Eto.Forms;

namespace Eto.Platform.GtkSharp
{
	public class NumericUpDownHandler : GtkControl<Gtk.SpinButton, NumericUpDown>, INumericUpDown
	{
		//private Gtk.ScrolledWindow scroll;
		
		public NumericUpDownHandler()
		{
			Control = new Gtk.SpinButton(0, 100, 1);
			//control.WrapMode = Gtk.WrapMode.None;
			Control.Changed += control_Changed;
			//scroll.Add(control);
		}
		
		public override string Text
		{
			get { return Control.Text; }
			set { Control.Text = value; }
		}
		
		public bool ReadOnly
		{
			get { return !Control.IsEditable; }
			set { Control.IsEditable = !value; }
		}
		
		public decimal Value
		{
			get { return (decimal)Control.Value; }
			set { Control.Value = (double)value; }
		}
		
		
		private void control_Changed(object sender, EventArgs e)
		{
			Widget.OnTextChanged(e);
		}
	}
}
