using System;
using Eto.Forms;
using Eto.Drawing;
using Eto.Platform.GtkSharp.Drawing;

namespace Eto.Platform.GtkSharp
{
	public class CheckBoxHandler : GtkControl<Gtk.CheckButton, CheckBox>, ICheckBox
	{
		Font font;
		bool toggling;
		
		public CheckBoxHandler ()
		{
			Control = new Gtk.CheckButton ();
			Control.Toggled += HandleControlToggled;
		}

		void HandleControlToggled (object sender, EventArgs e)
		{
			if (toggling)
				return;
			
			toggling = true;
			if (ThreeState) {
				if (!Control.Inconsistent && Control.Active)
					Control.Inconsistent = true;
				else if (Control.Inconsistent) {
					Control.Inconsistent = false;
					Control.Active = true;
				}
			}
			Widget.OnCheckedChanged (EventArgs.Empty);
			toggling = false;
		}

		public override string Text {
			get { return MnuemonicToString (Control.Label); }
			set { Control.Label = StringToMnuemonic(value); }
		}

		public override Font Font
		{
			get {
				if (font == null)
					font = new Font (Widget.Generator, new FontHandler (Control.Child));
				return font;
			}
			set
			{
				font = value;
				if (font != null)
					Control.Child.ModifyFont (((FontHandler)font.Handler).Control);
				else
					Control.Child.ModifyFont (null);
			}
		}


		public bool? Checked {
			get { return Control.Inconsistent ? null : (bool?)Control.Active; }
			set { 
				if (value == null)
					Control.Inconsistent = true;
				else {
					Control.Inconsistent = false;
					Control.Active = value.Value;
				}
			}
		}
		
		public bool ThreeState {
			get;
			set;
		}
	}
}
