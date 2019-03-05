using sw = System.Windows;
using swc = System.Windows.Controls;
using Eto.Forms;
using System;

namespace Eto.Wpf.Forms.Menu
{
	public class SeparatorMenuItemHandler : WidgetHandler<swc.Separator, SeparatorMenuItem>, SeparatorMenuItem.IHandler
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

		public bool Visible
		{
			get => Control.Visibility == sw.Visibility.Visible;
			set => Control.Visibility = value ? sw.Visibility.Visible : sw.Visibility.Collapsed;
		}

		public void CreateFromCommand(Command command)
		{
		}
	}
}
