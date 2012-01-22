using System;
using swc = System.Windows.Controls;
using Eto.Forms;

namespace Eto.Platform.Wpf.Forms.Menu
{
	public class ContextMenuHandler : WidgetHandler<swc.ContextMenu, ContextMenu>, IContextMenu
	{
		public ContextMenuHandler ()
		{
			Control = new swc.ContextMenu ();
		}

		public void AddMenu (int index, MenuItem item)
		{
			Control.Items.Add (item.ControlObject);
		}

		public void RemoveMenu (MenuItem item)
		{
			Control.Items.Remove (item.ControlObject);
		}

		public void Clear ()
		{
			Control.Items.Clear ();
		}
	}
}
