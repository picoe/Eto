using System;
using Eto.Forms;

namespace Eto.Platform.GtkSharp
{
	public class SeparatorMenuItemHandler : MenuHandler<Gtk.SeparatorMenuItem, SeparatorMenuItem>, ISeparatorMenuItem
	{
		public SeparatorMenuItemHandler()
		{
			Control = new Gtk.SeparatorMenuItem();
		}

		public void CreateFromCommand(Command command)
		{
		}

		public string Text
		{
			get { return null; }
			set { throw new NotSupportedException(); }
		}

		public string ToolTip
		{
			get { return null; }
			set { throw new NotSupportedException(); }
		}

		public Keys Shortcut
		{
			get { return Keys.None; }
			set { throw new NotSupportedException(); }
		}

		public bool Enabled
		{
			get { return false; }
			set { }
		}
	}
}
