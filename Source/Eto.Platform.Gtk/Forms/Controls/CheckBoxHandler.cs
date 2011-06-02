using System;
using Eto.Forms;

namespace Eto.Platform.GtkSharp
{
	public class CheckBoxHandler : GtkControl<Gtk.CheckButton, CheckBox>, ICheckBox
	{

		public CheckBoxHandler()
		{
			Control = new Gtk.CheckButton();
			Control.Toggled += control_CheckedChanged;
		}

		private void control_CheckedChanged(object sender, EventArgs e)
		{
			Widget.OnCheckedChanged(e);
		}

		public override string Text
		{
			get { return Control.Label; }
			set { Control.Label = value; }
		}

		#region ICheckBox Members

		public bool Checked
		{
			get { return Control.Active; }
			set { Control.Active = value; }
		}

		#endregion

	}
}
