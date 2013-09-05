using System;
using swc = System.Windows.Controls;
using sw = System.Windows;
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

		public void Show (Control relativeTo)
		{
			Control.Placement = swc.Primitives.PlacementMode.MousePoint;
			if (relativeTo != null) {
				Control.PlacementTarget = relativeTo.ControlObject as sw.UIElement;
			}
			Control.IsOpen = true;
            WpfFrameworkElementHelper.ShouldCaptureMouse = false;
		}
	}
}
