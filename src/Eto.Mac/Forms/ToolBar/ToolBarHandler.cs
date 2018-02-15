using System;
using Eto.Forms;
using System.Linq;
using System.Collections.Generic;

#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
#if Mac64
using nfloat = System.Double;
using nint = System.Int64;
using nuint = System.UInt64;
#else
using nfloat = System.Single;
using nint = System.Int32;
using nuint = System.UInt32;
#endif
#if SDCOMPAT
using CGSize = System.Drawing.SizeF;
using CGRect = System.Drawing.RectangleF;
using CGPoint = System.Drawing.PointF;
#endif
#endif

namespace Eto.Mac.Forms.ToolBar
{
	public class ToolBarHandler : WidgetHandler<NSToolbar, Eto.Forms.ToolBar>, Eto.Forms.ToolBar.IHandler
	{
		ToolBarDock dock = ToolBarDock.Top;
		readonly List<IToolBarBaseItemHandler> items = new List<IToolBarBaseItemHandler>();

		class TBDelegate : NSToolbarDelegate
		{
			WeakReference handler;

			public ToolBarHandler Handler { get { return (ToolBarHandler)handler.Target; } set { handler = new WeakReference(value); } }

			public override string[] SelectableItemIdentifiers(NSToolbar toolbar)
			{
				return Handler.items.Where(r => r.Selectable).Select(r => r.Identifier).ToArray();
			}

			public override void WillAddItem(NSNotification notification)
			{
				
			}

			public override void DidRemoveItem(NSNotification notification)
			{
			}

			public override NSToolbarItem WillInsertItem(NSToolbar toolbar, string itemIdentifier, bool willBeInserted)
			{
				var item = Handler.items.FirstOrDefault(r => r.Identifier == itemIdentifier);
				return item == null ? null : item.Control;
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
						SeparatorToolItemHandler.DividerIdentifier,
						NSToolbar.NSToolbarSeparatorItemIdentifier, 
						NSToolbar.NSToolbarSpaceItemIdentifier,
						NSToolbar.NSToolbarFlexibleSpaceItemIdentifier,
						NSToolbar.NSToolbarCustomizeToolbarItemIdentifier
					}).ToArray();
			}
		}

		protected override NSToolbar CreateControl()
		{
			return new NSToolbar("main");
		}

		protected override void Initialize()
		{
			Control.SizeMode = NSToolbarSizeMode.Default;
			Control.Visible = true;
			Control.ShowsBaselineSeparator = true;
			//Control.AllowsUserCustomization = true;
			Control.DisplayMode = NSToolbarDisplayMode.IconAndLabel;
			Control.Delegate = new TBDelegate { Handler = this };

			base.Initialize();
		}

		public ToolBarDock Dock
		{
			get { return dock; }
			set { dock = value; }
		}

		public void AddButton(ToolItem item, int index)
		{
			var handler = (IToolBarBaseItemHandler)item.Handler;
			items.Insert(index, handler);
			Control.InsertItem(handler.Identifier, index);
			if (handler != null)
				handler.ControlAdded(this);
			//Control.ValidateVisibleItems();
		}

		public void RemoveButton(ToolItem item)
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
				}
			}
		}

		public void Clear()
		{
			for (int i = Control.Items.Length - 1; i >= 0; i--)
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
