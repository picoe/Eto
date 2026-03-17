namespace Eto.GirCore.Forms.Menu
{
	public class ContextMenuHandler : MenuHandler<Gtk.Popover, ContextMenu, ContextMenu.ICallback>, ContextMenu.IHandler, Eto.Forms.Menu.ISubmenuHandler, IGirMenuParentHandler
	{
		readonly Gtk.Box content;
		bool pendingClosed;

		public ContextMenuHandler()
		{
			Control = Gtk.Popover.New();
			Control.SetHasArrow(false);
			content = Gtk.Box.New(Gtk.Orientation.Vertical, 0);
			Control.SetChild(content);
			Control.OnClosed += (sender, e) =>
			{
				if (pendingClosed)
					Application.Instance.AsyncInvoke(() => Callback.OnClosed(Widget, EventArgs.Empty));
				else
					Callback.OnClosed(Widget, EventArgs.Empty);
				pendingClosed = false;
			};
		}

		void Rebuild()
		{
			GirMenuHelper.ClearBox(content);
			foreach (var item in Widget.Items)
			{
				GirMenuHelper.SetParent(item, this);
				var child = GirMenuHelper.GetWidget(item);
				if (child != null)
					content.Append(child);
			}
		}

		public void AddMenu(int index, MenuItem item)
		{
			Rebuild();
		}

		public void RemoveMenu(MenuItem item)
		{
			GirMenuHelper.SetParent(item, null);
			Rebuild();
		}

		public void Clear()
		{
			foreach (var item in Widget.Items)
				GirMenuHelper.SetParent(item, null);
			Rebuild();
		}

		public void Show(Control relativeTo, PointF? location)
		{
			var parent = relativeTo?.ControlObject as Gtk.Widget;
			if (parent == null)
				return;

			if (Control.GetParent() != parent)
			{
				if (Control.GetParent() != null)
					Control.Unparent();
				Control.SetParent(parent);
			}

			GirMenuHelper.ValidateItems(Widget.Items);
			Callback.OnOpening(Widget, EventArgs.Empty);

			if (location != null)
			{
				var rect = new Gdk.Rectangle
				{
					X = (int)location.Value.X,
					Y = (int)location.Value.Y,
					Width = 1,
					Height = 1
				};
				Control.SetPointingTo(rect);
			}

			Control.Present();
		}

		public void PrepareForChildActivation()
		{
			Callback.OnClosing(Widget, EventArgs.Empty);
			pendingClosed = true;
		}

		public void CloseHierarchy()
		{
			Control.Popdown();
		}

		public void ChildUpdated()
		{
			Rebuild();
		}
	}
}
