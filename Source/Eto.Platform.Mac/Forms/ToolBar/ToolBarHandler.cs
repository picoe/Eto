using System;
using Eto.Forms;
using MonoMac.AppKit;
using System.Linq;
using System.Collections.Generic;

namespace Eto.Platform.Mac
{
	public class ToolBarHandler : WidgetHandler<NSToolbar, ToolBar>, IToolBar
	{
		ToolBarDock dock = ToolBarDock.Top;
		List<IToolBarBaseItemHandler> items = new List<IToolBarBaseItemHandler>();

		class TBDelegate : NSToolbarDelegate
		{
			WeakReference handler;

			public ToolBarHandler Handler { get { return (ToolBarHandler)handler.Target; } set { handler = new WeakReference(value); } }

			public override string[] SelectableItemIdentifiers(NSToolbar toolbar)
			{
				return Handler.items.Where(r => r.Selectable).Select(r => r.Identifier).ToArray();
			}

			public override void WillAddItem(MonoMac.Foundation.NSNotification notification)
			{
				
			}

			public override void DidRemoveItem(MonoMac.Foundation.NSNotification notification)
			{
			}

			public override NSToolbarItem WillInsertItem(NSToolbar toolbar, string itemIdentifier, bool willBeInserted)
			{
				var item = Handler.items.FirstOrDefault(r => r.Identifier == itemIdentifier);
				if (item != null)
					return item.Control;
				else
					return null;
			}

			public override string[] DefaultItemIdentifiers(NSToolbar toolbar)
			{
				return Handler.items.Select(r => r.Identifier).ToArray();
			}

			public override string[] AllowedItemIdentifiers(NSToolbar toolbar)
			{
				return Handler.items.Select(r => r.Identifier)
				.Union(
					new string[]
				{ 
					NSToolbar.NSToolbarSeparatorItemIdentifier, 
					NSToolbar.NSToolbarSpaceItemIdentifier,
					NSToolbar.NSToolbarFlexibleSpaceItemIdentifier,
					NSToolbar.NSToolbarCustomizeToolbarItemIdentifier
				}).ToArray();
			}
		}

		public ToolBarHandler()
		{
			Control = new NSToolbar("main");
			Control.SizeMode = NSToolbarSizeMode.Default;
			Control.Visible = true;
			Control.ShowsBaselineSeparator = true;
			//Control.AllowsUserCustomization = true;
			Control.DisplayMode = NSToolbarDisplayMode.IconAndLabel;
			Control.Delegate = new TBDelegate { Handler = this };
		}

		public ToolBarDock Dock
		{
			get { return dock; }
			set { dock = value; }
		}

		public void AddButton(ToolBarItem item)
		{
			var handler = (IToolBarBaseItemHandler)item.Handler;
			items.Add(handler);
			Control.InsertItem(handler.Identifier, Control.Items.Length);
			if (handler != null)
				handler.ControlAdded(this);
			//Control.ValidateVisibleItems();
		}

		public void RemoveButton(ToolBarItem item)
		{
			var handler = item.Handler as IToolBarBaseItemHandler;
			var index = items.IndexOf(handler);
			items.Remove(handler);
			//var handler = item.Handler as IToolBarItemHandler;
			Control.RemoveItem(index);
			//Control.ValidateVisibleItems();
		}

		public ToolBarTextAlign TextAlign
		{
			get
			{
				/*switch (control.TextAlign)
				{
					case SWF.ToolBarTextAlign.Right:
						return ToolBarTextAlign.Right;
					default:
					case SWF.ToolBarTextAlign.Underneath:
						return ToolBarTextAlign.Underneath;
				}
				 */
				return ToolBarTextAlign.Underneath;
			}
			set
			{
				switch (value)
				{
					case ToolBarTextAlign.Right:
						//control.TextAlign = SWF.ToolBarTextAlign.Right;
						break;
					default:
					case ToolBarTextAlign.Underneath:
						//control.TextAlign = SWF.ToolBarTextAlign.Underneath;
						break;
				}
			}
		}

		public void Clear()
		{
			for (int i = Control.Items.Length - 1; i >=0; i--)
			{
				Control.RemoveItem(i);
			}
			items.Clear();
			// allow menu items to be GC'd
			var newitems = Control.Items;

			//Control.ValidateVisibleItems();
		}
	}
}
