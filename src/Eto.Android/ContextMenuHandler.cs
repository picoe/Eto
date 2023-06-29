using aa = Android.App;
using ac = Android.Content;
using ao = Android.OS;
using ar = Android.Runtime;
using av = Android.Views;
using aw = Android.Widget;
using ag = Android.Graphics;
namespace Eto.Android
{
	internal class ContextMenuHandler : WidgetHandler<aw.PopupMenu, ContextMenu, ContextMenu.ICallback>, ContextMenu.IHandler
	{
		private List<MenuItem> items;
		private Boolean attachedOpeningEvent;
		private Boolean attachedClosingEvent;

		public ContextMenuHandler()
		{
			items = new List<MenuItem>();
		}

		private void Control_MenuItemClick(System.Object sender, aw.PopupMenu.MenuItemClickEventArgs e)
		{
			var Item = items.FirstOrDefault(i => ReferenceEquals(i.ControlObject, e.Item));
			var ItemHandler = (IMenuItemHandler)Item.Handler;
			ItemHandler.PerformClick();
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case ContextMenu.OpeningEvent:
					attachedOpeningEvent = true;
					break;
				case ContextMenu.ClosedEvent:
					attachedClosingEvent = true;
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		public void Show(Control relativeTo)
		{
			Show(relativeTo, new PointF(0, 0));
		}

		public void Show(Control relativeTo, PointF? location)
		{
			var Anchor = (av.View)relativeTo?.ControlObject;

			if (Anchor == null)
				Anchor = Forms.ApplicationHandler.Instance.MainActivity.FindViewById(global::Android.Resource.Id.Content);

			Show(Anchor, location);
		}

		internal void Show(av.View relativeTo, PointF? location)
		{ 
			Control = new aw.PopupMenu(Platform.AppContextThemed, relativeTo);

			var Index = 0;

			foreach(var item in items)
			{
				var ItemHandler = (IMenuItemHandler)item.Handler;

				if (item is ISubmenu subItem && subItem.Items.Any())
				{
					var Sub = Control.Menu.AddSubMenu(av.Menu.None, Index, Index, new Java.Lang.String(item.Text));
					ItemHandler.Control = Sub.Item;
					ItemHandler.SubControl = Sub;
				}

				ItemHandler.CreateControl(Control.Menu, Index);
				
				Index++;
			}

			if(attachedOpeningEvent)
				Callback?.OnOpening(Widget, EventArgs.Empty);

			if (attachedClosingEvent)
				Control.DismissEvent += (s, e) => Callback?.OnClosed(Widget, e);

			Control.MenuItemClick += Control_MenuItemClick;

			Control.Show();
		}

		public void AddMenu(System.Int32 index, MenuItem item)
		{
			items.Add(item);
		}

		public void RemoveMenu(MenuItem item)
		{
			items.Remove(item);
		}

		public void Clear()
		{
			items.Clear();
		}
	}
}