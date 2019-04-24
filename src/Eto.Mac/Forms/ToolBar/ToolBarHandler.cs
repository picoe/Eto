using System;
using Eto.Forms;
using System.Linq;
using System.Collections.Generic;
using Eto.Drawing;

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
		public static string DividerIdentifier = "Divider";

		int suppressVisibleUpdate;
		readonly List<IToolBarBaseItemHandler> items = new List<IToolBarBaseItemHandler>();

		public class DividerToolbarItem : NSToolbarItem
		{
			Drawable _drawable;
			Color _color;

			static readonly bool supportsSeparatorColor = ObjCExtensions.RespondsToSelector<NSColor>("separatorColor");

			public DividerToolbarItem(bool willBeInserted) : base(DividerIdentifier)
			{
				_drawable = new Drawable
				{
					Size = willBeInserted ? new Size(1, 20) : new Size(20, 20)
				};
				_drawable.Paint += Drawable_Paint;
				View = _drawable.ToNative(true);
				View.AutoresizingMask = NSViewResizingMask.HeightSizable | NSViewResizingMask.WidthSizable;
				PaletteLabel = Application.Instance.Localize(this, "Divider");
				MenuFormRepresentation = NSMenuItem.SeparatorItem;
#if MONOMAC || XAMMAC2
				if (supportsSeparatorColor)
					_color = NSColor.SeparatorColor.ToEto();
				else
					_color = new Color(SystemColors.WindowBackground, 0.5f);
#else
				_color = new Color(SystemColors.WindowBackground, 0.5f);
#endif
			}

			public override bool AllowsDuplicatesInToolbar => true;

			void Drawable_Paint(object sender, PaintEventArgs e)
			{
				var x = _drawable.Width / 2;

				e.Graphics.DrawLine(_color, x, 0, x, _drawable.Height);
			}
		}

		class EtoToolbarDelegate : NSToolbarDelegate
		{
			WeakReference handler;

			public ToolBarHandler Handler { get => (ToolBarHandler)handler?.Target; set => handler = new WeakReference(value); }

			public override string[] SelectableItemIdentifiers(NSToolbar toolbar)
			{
				var h = Handler;
				if (h == null)
					return new string[0];
				return h.items.Where(r => r.Selectable && r.Visible).Select(r => r.Identifier).ToArray();
			}

			public override void WillAddItem(NSNotification notification)
			{
				var h = Handler;
				if (h == null)
					return;
				if (notification.UserInfo[itemKey] is NSToolbarItem item)
					h.SetItemVisibility(item, true);
			}

			static readonly NSString itemKey = new NSString("item");
			public override void DidRemoveItem(NSNotification notification)
			{
				var h = Handler;
				if (h == null)
					return;
				if (notification.UserInfo[itemKey] is NSToolbarItem item)
					h.SetItemVisibility(item, false);
			}

			public override NSToolbarItem WillInsertItem(NSToolbar toolbar, string itemIdentifier, bool willBeInserted)
			{
				var h = Handler;
				if (h == null)
					return null;

				if (itemIdentifier == DividerIdentifier)
					return new DividerToolbarItem(willBeInserted);

				var item = h.items.FirstOrDefault(r => r.Identifier == itemIdentifier);
				if (willBeInserted)
				{
					item?.SetVisible(true);
				}
				return item?.Control;
			}

			public override string[] DefaultItemIdentifiers(NSToolbar toolbar)
			{
				var h = Handler;
				if (h == null)
					return new string[0];
				return h.items.Where(r => r.Visible).Select(r => r.Identifier).ToArray();
			}

			static string[] systemIdentifiers = {
				DividerIdentifier,
				NSToolbar.NSToolbarSeparatorItemIdentifier,
				NSToolbar.NSToolbarSpaceItemIdentifier,
				NSToolbar.NSToolbarFlexibleSpaceItemIdentifier,
				NSToolbar.NSToolbarCustomizeToolbarItemIdentifier
			};

			public override string[] AllowedItemIdentifiers(NSToolbar toolbar)
			{
				var h = Handler;
				if (h == null)
					return systemIdentifiers;
				return systemIdentifiers.Union(h.items.Select(r => r.Identifier)).Distinct().ToArray();
			}
		}

		private void SetItemVisibility(NSToolbarItem item, bool visible)
		{
			if (suppressVisibleUpdate > 0)
				return;
			// set visibility of item when added/removed from customization
			var handler = GetEtoItem(item)?.Handler as IToolBarBaseItemHandler;
			handler?.SetVisible(visible);
		}

		protected override NSToolbar CreateControl() => new NSToolbar(Guid.NewGuid().ToString());

		protected override void Initialize()
		{
			Control.SizeMode = NSToolbarSizeMode.Default;
			Control.Visible = true;
			Control.ShowsBaselineSeparator = true;
			Control.DisplayMode = NSToolbarDisplayMode.IconAndLabel;
			Control.Delegate = new EtoToolbarDelegate { Handler = this };

			//Control.AutosavesConfiguration = true;
			//Control.AllowsUserCustomization = true;

			base.Initialize();
		}

		public ToolBarDock Dock
		{
			get => ToolBarDock.Top;
#pragma warning disable RECS0029 // Warns about property or indexer setters and event adders or removers that do not use the value parameter
			set
			{
				// can't change dock position, but not detrimental so we don't throw.
			}
#pragma warning restore RECS0029 // Warns about property or indexer setters and event adders or removers that do not use the value parameter
		}

		public void AddButton(ToolItem item, int index)
		{
			var handler = (IToolBarBaseItemHandler)item.Handler;
			items.Insert(index, handler);
			var idx = GetIndex(item, true);
			if (idx >= 0 && idx <= Control.Items.Length)
			{
				suppressVisibleUpdate++;
				Control.InsertItem(handler.Identifier, idx);
				suppressVisibleUpdate--;
				OnControlItemsChanged();
			}
			handler.ControlAdded(this);
		}

		public void RemoveButton(ToolItem item, int index)
		{
			var handler = (IToolBarBaseItemHandler)item.Handler;
			var idx = GetIndex(item);
			if (idx >= 0 && idx < Control.Items.Length)
			{
				suppressVisibleUpdate++;
				Control.RemoveItem(idx);
				suppressVisibleUpdate--;
				OnControlItemsChanged();
			}

			items.Remove(handler);
		}

		internal void ChangeIdentifier(ToolItem item)
		{
			var idx = GetIndex(item);
			if (idx >= 0 && idx < Control.Items.Length)
			{
				suppressVisibleUpdate++;
				// re-add it!
				Control.RemoveItem(idx);
				Control.InsertItem(GetIdentifier(item), idx);
				suppressVisibleUpdate--;
				OnControlItemsChanged();
			}
		}

		void OnControlItemsChanged()
		{
#if !XAMMAC2
			// re-retrieve items so they aren't GC'd (only needed in MonoMac)
			var newitems = Control.Items;
#endif
		}

		public ToolBarTextAlign TextAlign
		{
			get => ToolBarTextAlign.Underneath;
#pragma warning disable RECS0029 // Warns about property or indexer setters and event adders or removers that do not use the value parameter
			set
			{
				// can't change on macOS, but that's okay.
			}
#pragma warning restore RECS0029 // Warns about property or indexer setters and event adders or removers that do not use the value parameter
		}

		public void Clear()
		{
			suppressVisibleUpdate++;
			for (int i = Control.Items.Length - 1; i >= 0; i--)
			{
				Control.RemoveItem(i);
			}
			suppressVisibleUpdate--;
			items.Clear();
			// allow menu items to be GC'd
			OnControlItemsChanged();
		}

		/// <summary>
		/// Gets the native index of the specified eto item
		/// </summary>
		/// <returns>The index to use.</returns>
		/// <param name="item">Item to find the index for.</param>
		/// <param name="forInsert">Set to true to specify that we want to find the index to insert to, and it doesn't exist in the items yet.</param>
		/// <param name="checkVisible">Set to true to check that the item is visible first, which will return -1 if it is not.</param>
		internal int GetIndex(ToolItem item, bool forInsert = false, bool checkVisible = true)
		{
			if (checkVisible && !item.Visible)
				return -1;

			var nativeItems = Control.Items;
			var idx = 0;
			for (int i = 0; i < Widget.Items.Count; i++)
			{
				var curitem = Widget.Items[i];
				if (ReferenceEquals(curitem, item))
				{
					if (curitem.Visible || forInsert)
						return idx;
					return -1;
				}

				if (idx >= nativeItems.Length)
				{
					if (forInsert)
						return idx;
				}
				else if (curitem.Visible)
				{
					var nativeItem = nativeItems[idx];
					if (nativeItem.Identifier == GetIdentifier(curitem))
						idx++;
				}
			}
			return -1;
		}

		internal void ChangeVisibility(ToolItem item, bool value)
		{
			suppressVisibleUpdate++;
			var nsitem = GetNSItem(item);
			var nativeItems = Control.Items;
			int idx;
			if (nsitem != null)
				idx = Array.IndexOf(nativeItems, nsitem);
			else
				idx = GetIndex(item, checkVisible: false); // lookup by index of eto item

			// lookup by index of native control
			if (value)
			{
				if (idx == -1)
				{
					idx = GetIndex(item, true, false);
					if (idx >= 0 && idx <= nativeItems.Length)
						Control.InsertItem(GetIdentifier(item), idx);
				}
			}
			else if (idx != -1 && idx < nativeItems.Length)
			{
				Control.RemoveItem(idx);
			}
			OnControlItemsChanged();
			suppressVisibleUpdate--;
		}

		string GetIdentifier(ToolItem item) => (item.Handler as IToolBarBaseItemHandler)?.Identifier;

		NSToolbarItem GetNSItem(ToolItem item) => item.ControlObject as NSToolbarItem;

		ToolItem GetEtoItem(NSToolbarItem item)
		{
			var itemIdentifier = item.Identifier;
			var etoItem = items.FirstOrDefault(r => r.Identifier == itemIdentifier);
			if (etoItem != null)
				return etoItem.Widget;
			var index = Array.IndexOf(Control.Items, item);

			var idx = 0;
			for (int i = 0; i < Widget.Items.Count; i++)
			{
				var curitem = Widget.Items[i];
				if (idx == index)
					return curitem;

				if (curitem.Visible)
					idx++;
			}
			return null;
		}
	}
}
