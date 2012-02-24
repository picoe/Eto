using System;
using Eto.Forms;

namespace Eto.Platform.GtkSharp
{
	public class CheckBoxHandler : GtkControl<Gtk.CheckButton, CheckBox>, ICheckBox
	{
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
			get { return Control.Label; }
			set { Control.Label = value; }
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
