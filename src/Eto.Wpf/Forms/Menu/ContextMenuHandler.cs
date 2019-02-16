using swc = System.Windows.Controls;
using sw = System.Windows;
using swi = System.Windows.Input;
using Eto.Forms;
using System;
using System.Linq;
using System.ComponentModel;
using Eto.Drawing;
using System.Windows.Controls.Primitives;

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
					Control.Closed += (sender, e) => Callback.OnClosed(Widget, EventArgs.Empty);
					break;

				case ContextMenu.ClosingEvent:
					// wpf's Closed event fires after, so look at IsOpen property changes instead
					Widget.Properties.Set(swc.ContextMenu.IsOpenProperty, PropertyChangeNotifier.Register(swc.ContextMenu.IsOpenProperty, HandleIsOpenChanged, Control));
					break;

				default:
					base.AttachEvent(id);
					break;
			}
		}

		void HandleIsOpenChanged(object sender, EventArgs e)
		{
			if (!Control.IsOpen)
				Callback.OnClosing(Widget, EventArgs.Empty);
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

		public void Show(Control relativeTo, PointF? location)
		{
			if (relativeTo != null)
			{
				Control.PlacementTarget = relativeTo.ControlObject as sw.UIElement;
				if (location != null)
				{
					Control.HorizontalOffset = 0;
					Control.VerticalOffset = 0;

					var pt = location.Value;

					if (pt.X == 0 && pt.Y == relativeTo.Height)
					{
						// try to stay at the bottom and scroll if needed, e.g. useful for drop down menus
						// will show above control if necessary
						Control.Placement = PlacementMode.Bottom;
					}
					else if (pt.Y == 0 && pt.X == relativeTo.Width)
					{
						Control.Placement = PlacementMode.Right;
					}
					else
					{
						// Location should be the upper left corner, but PlacementMode.RelativePoint 
						// can show the menu from its upper right corner.
						Control.Placement = PlacementMode.Custom;
						var logicalPixelSize = relativeTo.ParentWindow?.LogicalPixelSize;
						if (logicalPixelSize != null)
							pt *= logicalPixelSize.Value;
						Control.CustomPopupPlacementCallback = (popupSize, targetSize, offset) =>
						{
							return new[] { new CustomPopupPlacement(pt.ToWpf(), PopupPrimaryAxis.Horizontal) };
						};
					}
				}
				else
				{
					Control.Placement = swc.Primitives.PlacementMode.MousePoint;
					Control.HorizontalOffset = 0;
					Control.VerticalOffset = 0;
				}
			}
			else if (location != null)
			{
				Control.Placement = PlacementMode.Absolute;
				Control.HorizontalOffset = location.Value.X;
				Control.VerticalOffset = location.Value.Y;
			}
			else
			{
				Control.Placement = swc.Primitives.PlacementMode.MousePoint;
				Control.HorizontalOffset = 0;
				Control.VerticalOffset = 0;
			}
			Control.IsOpen = true;
			WpfFrameworkElementHelper.ShouldCaptureMouse = false;
		}
	}
}
