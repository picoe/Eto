using System;
using Eto.Forms;

namespace Eto.Platform.GtkSharp
{
	public class TextBoxHandler : GtkControl<Gtk.Entry, TextBox>, ITextBox
	{
		//private Gtk.ScrolledWindow scroll;

		public TextBoxHandler()
		{
			Control = new Gtk.Entry();
			//control.WrapMode = Gtk.WrapMode.None;
			Control.Changed += control_Changed;
			Control.ActivatesDefault = true;
			//scroll.Add(control);
		}

		public override string Text
		{
			get { return Control.Text; }
			set { Control.Text = value; }
		}

		public bool ReadOnly
		{
			get { return Control.IsEditable; }
			set { Control.IsEditable = value; }
		}
		
		public int MaxLength {
			get { return Control.MaxLength; }
			set { Control.MaxLength = value; }
		}


		private void control_Changed(object sender, EventArgs e)
		{
			Widget.OnTextChanged(e);
		}
	}
}
