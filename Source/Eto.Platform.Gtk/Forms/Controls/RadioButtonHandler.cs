using System;
using Eto.Forms;

namespace Eto.Platform.GtkSharp
{
	public class RadioButtonHandler : GtkControl<Gtk.RadioButton, RadioButton>, IRadioButton
	{
		Gtk.AccelLabel label;

		protected override Gtk.Widget FontControl
		{
			get { return label; }
		}
		
		public void Create (RadioButton controller)
		{
			if (controller != null)
				Control = new Gtk.RadioButton (RadioButtonHandler.GetControl (controller));
			else {
				Control = new Gtk.RadioButton ((Gtk.RadioButton)null);
				// make gtk work like others in that no radio button is initially selected
				var inactive = new Gtk.RadioButton (Control);
				inactive.Active = true;
			}
			label = new Gtk.AccelLabel("");
			Control.Add(label); //control.AddMnemonicLabel(label);
			Control.Toggled += control_Toggled;
		}
		
		void control_Toggled(Object sender, EventArgs e)
		{
			Widget.OnCheckedChanged(e);
		}

		public override string Text
		{
			get { return MnuemonicToString(label.Text); }
			set { label.TextWithMnemonic = StringToMnuemonic(value); }
		}
		
		public bool Checked
		{
			get { return Control.Active; }
			set { Control.Active = value; }
		}
		
	}
}
