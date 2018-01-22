#if TODO_XAML
using swc = Windows.UI.Xaml.Controls;
using sw = Windows.UI.Xaml;
using Eto.Forms;

namespace Eto.WinRT.Forms.Menu
{
	public class ContextMenuHandler : WidgetHandler<swc.ContextMenu, ContextMenu>, IContextMenu
	{
		public ContextMenuHandler ()
		{
			Control = new swc.ContextMenu ();
		}

		public void AddMenu (int index, MenuItem item)
		{
			Control.Items.Insert (index, item.ControlObject);
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
#endif