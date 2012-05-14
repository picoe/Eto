using System;
using Eto.Forms;
using Eto.Drawing;
using Eto.Platform.GtkSharp.Drawing;

namespace Eto.Platform.GtkSharp
{
	public class RadioButtonHandler : GtkControl<Gtk.RadioButton, RadioButton>, IRadioButton
	{
		Font font;
		Gtk.AccelLabel label;
		
		public void Create(RadioButton controller)
		{
			if (controller != null) Control = new Gtk.RadioButton((Gtk.RadioButton)controller.ControlObject);
			else Control = new Gtk.RadioButton((Gtk.RadioButton)null);
			label = new Gtk.AccelLabel("");
			Control.Add(label); //control.AddMnemonicLabel(label);
			Control.Toggled += control_Toggled;
		}
		
		void control_Toggled(Object sender, EventArgs e)
		{
			Widget.OnCheckedChanged(e);
		}

		public override Font Font
		{
			get { return font; }
			set
			{
				font = value;
				if (font != null)
					Control.Child.ModifyFont (((FontHandler)font.Handler).Control);
				else
					Control.Child.ModifyFont (null);
			}
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
