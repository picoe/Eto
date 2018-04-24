using swc = System.Windows.Controls;
using sw = System.Windows;
using swi = System.Windows.Input;
using Eto.Forms;
using System;
using System.Linq;
using System.ComponentModel;

namespace Eto.Wpf.Forms.Menu
{
	public interface IInputBindingHost
	{
		swi.InputBindingCollection InputBindings { get; }
	}

	public class ContextMenuHandler : WidgetHandler<swc.ContextMenu, ContextMenu, ContextMenu.ICallback>, ContextMenu.IHandler, IInputBindingHost
	{
		public ContextMenuHandler()
		{
			Control = new swc.ContextMenu();
		}

		public swi.InputBindingCollection InputBindings
		{
			get { return Control.InputBindings; }
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case ContextMenu.OpeningEvent:
					Control.Opened += (sender, e) => Callback.OnOpening(Widget, EventArgs.Empty);
					break;

				case ContextMenu.ClosedEvent:
					// eto's Closed event should happen before click of menu item to be consistent with other platforms.
					// wpf's Closed event fires after, so look at IsOpen property changes instead
					PropertyChangeNotifier.Register(swc.ContextMenu.IsOpenProperty, HandleIsOpenChanged, Control);
					break;

				default:
					base.AttachEvent(id);
					break;
			}
		}

		void HandleIsOpenChanged(object sender, EventArgs e)
		{
			if (!Control.IsOpen)
				Callback.OnClosed(Widget, EventArgs.Empty);
		}

		public void AddMenu(int index, MenuItem item)
		{
			Control.Items.Insert(index, item.ControlObject);
			Control.InputBindings.AddKeyBindings(item.ControlObject as sw.UIElement);
		}

		public void RemoveMenu(MenuItem item)
		{
			Control.InputBindings.RemoveKeyBindings(item.ControlObject as sw.UIElement);
			Control.Items.Remove(item.ControlObject);
		}

		public void Clear()
		{
			Control.InputBindings.RemoveKeyBindings(Control.Items);
			Control.Items.Clear();
		}

		public void Show(Control relativeTo)
		{
			Control.Placement = swc.Primitives.PlacementMode.MousePoint;
			if (relativeTo != null)
			{
				Control.PlacementTarget = relativeTo.ControlObject as sw.UIElement;
			}
			Control.IsOpen = true;
			WpfFrameworkElementHelper.ShouldCaptureMouse = false;
		}
	}
}
