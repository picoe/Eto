using swc = System.Windows.Controls;
using Eto.Forms;
using System;

namespace Eto.Platform.Wpf.Forms.Menu
{
	public class SeparatorMenuItemHandler : WidgetHandler<swc.Separator, SeparatorMenuItem>, ISeparatorMenuItem
	{
		public SeparatorMenuItemHandler ()
		{
			Control = new swc.Separator ();
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
			set { throw new NotSupportedException(); }
		}

		public void CreateFromCommand(Command command)
		{
		}
	}
}
