namespace Eto.Mac.Forms.Menu
{
	class ContextHandler : NSMenuDelegate
	{
		WeakReference handler;
		public ContextMenuHandler Handler
		{
			get { return (ContextMenuHandler)handler.Target; }
			set { handler = new WeakReference(value); }
		}

		public override void MenuWillHighlightItem(NSMenu menu, NSMenuItem item)
		{
		}

		public override void MenuWillOpen(NSMenu menu)
		{
			var h = Handler;
			if (h == null)
				return;
			h.Callback.OnOpening(h.Widget, EventArgs.Empty);
		}

		public override void MenuDidClose(NSMenu menu)
		{
			var h = Handler;
			if (h == null)
				return;
			h.Callback.OnClosing(h.Widget, EventArgs.Empty);

			Application.Instance.AsyncInvoke(() => h.Callback.OnClosed(h.Widget, EventArgs.Empty));
		}
	}

	public class ContextMenuHandler : WidgetHandler<NSMenu, ContextMenu, ContextMenu.ICallback>, ContextMenu.IHandler
	{
		protected override NSMenu CreateControl() => new EtoMenu();
		ContextHandler _delegate;

		public ContextMenuHandler()
		{
		}

		public ContextMenuHandler(NSMenu control)
		{
			Control = control;
		}
		
		internal static IEnumerable<MenuItem> GetMenuItems(NSMenu menu)
		{
			for (nint i = 0; i < menu.Count; i++)
			{
				var item = menu.ItemAt(i);
				
				if (item.HasSubmenu)
				{
					yield return new SubMenuItem(new SubMenuItemHandler(item), GetMenuItems(item.Submenu));
				}
				else if (item.IsSeparatorItem)
				{
					yield return new SeparatorMenuItem(new SeparatorMenuItemHandler(item));
				}
				else
				{
					yield return new ButtonMenuItem(new ButtonMenuItemHandler(item));
				}
			}
		}

		protected override void Initialize()
		{
			if (Control is EtoMenu etoMenu)
				etoMenu.WorksWhenModal = true;
			Control.AutoEnablesItems = false;
			Control.ShowsStateColumn = true;
			if (Control.WeakDelegate == null)
				Control.Delegate = _delegate = new ContextHandler { Handler = this };

			base.Initialize();
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case ContextMenu.OpeningEvent:
				case ContextMenu.ClosedEvent:
				case ContextMenu.ClosingEvent:
					// handled by delegate
					break;

				default:
					base.AttachEvent(id);
					break;
			}
		}

		public void AddMenu(int index, MenuItem item)
		{
			Control.InsertItem((NSMenuItem)item.ControlObject, index);
		}

		public void RemoveMenu(MenuItem item)
		{
			Control.RemoveItem((NSMenuItem)item.ControlObject);
		}

		public void Clear()
		{
			Control.RemoveAllItems();
		}

		public ContextMenuSystemItems IncludeSystemItems
		{
			// NSMenu.AllowsContextMenuPlugIns defaults to true, which matches ContextMenuSystemItems.All.
			// Note All has every bit set, so test against None rather than using HasFlag(All) here.
			get => Control.AllowsContextMenuPlugIns ? ContextMenuSystemItems.All : ContextMenuSystemItems.None;
			set => Control.AllowsContextMenuPlugIns = value != ContextMenuSystemItems.None;
		}

		public void Show(Control relativeTo, PointF? location)
		{
			var view = relativeTo?.GetContainerView();
			MacView.CancelMouseTracking();

			if (location != null || view == null)
			{
				CGPoint cglocation;
				if (view != null && location != null)
				{
					cglocation = location.Value.ToNS();
					if (!view.IsFlipped)
						cglocation.Y = view.Frame.Height - cglocation.Y;
				}
				else
				{
					cglocation = (location ?? Mouse.Position).ToNS();
					var origin = NSScreen.Screens[0].Frame.Bottom;
					cglocation.Y = origin - cglocation.Y;
				}

				Control.PopUpMenu(null, cglocation, view);
			}
			else
			{
				NSEvent nsevent = NSApplication.SharedApplication.CurrentEvent;
				if (nsevent == null)
				{
					var keyWindow = NSApplication.SharedApplication.KeyWindow;
					var mouseLocation = NSEvent.CurrentMouseLocation;
					mouseLocation = keyWindow.ConvertPointFromScreen(mouseLocation);

					var time = DateTime.Now.ToOADate();
					var windowNumber = keyWindow.WindowNumber;

					nsevent = NSEvent.MouseEvent(NSEventType.RightMouseDown, mouseLocation, 0, time, windowNumber, null, 0, 0, 0.1f);
				}

				NSMenu.PopUpContextMenu(Control, nsevent, view);
			}
		}
	}
}