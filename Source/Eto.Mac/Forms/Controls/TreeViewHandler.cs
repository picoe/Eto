using System;
using Eto.Forms;
using System.Collections.Generic;
using Eto.Mac.Forms.Menu;
using System.Linq;
using sd = System.Drawing;
using Eto.Drawing;

#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
using CoreImage;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
using MonoMac.CoreImage;
#if Mac64
using CGSize = MonoMac.Foundation.NSSize;
using CGRect = MonoMac.Foundation.NSRect;
using CGPoint = MonoMac.Foundation.NSPoint;
using nfloat = System.Double;
using nint = System.Int64;
using nuint = System.UInt64;
#else
using CGSize = System.Drawing.SizeF;
using CGRect = System.Drawing.RectangleF;
using CGPoint = System.Drawing.PointF;
using nfloat = System.Single;
using nint = System.Int32;
using nuint = System.UInt32;
#endif
#endif

#if Mac64
using nnint = System.UInt64;
#else
using nnint = System.Int32;
#endif

namespace Eto.Mac.Forms.Controls
{
	public class TreeViewHandler : MacControl<NSOutlineView, TreeView, TreeView.ICallback>, TreeView.IHandler
	{
		ITreeStore top;
		ContextMenu contextMenu;
		readonly Dictionary<ITreeItem, EtoTreeItem> cachedItems = new Dictionary<ITreeItem, EtoTreeItem>();
		readonly Dictionary<int, EtoTreeItem> topitems = new Dictionary<int, EtoTreeItem>();
		bool selectionChanging;
		bool raiseExpandEvents = true;
		readonly NSTableColumn column;

		public NSScrollView Scroll { get; private set; }

		public class EtoTreeItem : MacImageData
		{
			Dictionary<int, EtoTreeItem> items;
			ITreeItem item;

			public EtoTreeItem()
			{
			}

			public EtoTreeItem(IntPtr ptr)
				: base(ptr)
			{
			}

			public EtoTreeItem(EtoTreeItem value)
				: base (value)
			{
				this.Item = value.Item;
				this.items = value.items;
			}

			public ITreeItem Item
			{
				get { return item; }
				set
				{
					item = value;
					if (item.Image != null)
						Image = Item.Image.ControlObject as NSImage;
					Text = (NSString)item.Text;
				}
			}

			public Dictionary<int, EtoTreeItem> Items
			{
				get
				{
					if (items == null)
						items = new Dictionary<int, EtoTreeItem>();
					return items;
				}
			}

			public override object Clone()
			{
				return new EtoTreeItem(this);
			}
		}

		public class EtoOutlineDelegate : NSOutlineViewDelegate
		{
			WeakReference handler;
			public TreeViewHandler Handler { get { return (TreeViewHandler)handler.Target; } set { handler = new WeakReference(value); } }

			public override bool ShouldEditTableColumn(NSOutlineView outlineView, NSTableColumn tableColumn, NSObject item)
			{
				var myitem = item as EtoTreeItem;
				if (myitem != null)
				{
					var args = new TreeViewItemCancelEventArgs(myitem.Item);
					Handler.Callback.OnLabelEditing(Handler.Widget, args);
					return !args.Cancel;
				}
				return true;
			}

			public override void WillDisplayCell(NSOutlineView outlineView, NSObject cell, NSTableColumn tableColumn, NSObject item)
			{
				var c = cell as NSTextFieldCell;
				if (c != null &&
					Handler.textColor != null)
					c.TextColor = Handler.textColor.Value.ToNSUI();				
			}

			public override void SelectionDidChange(NSNotification notification)
			{
				if (!Handler.selectionChanging)
					Handler.Callback.OnSelectionChanged(Handler.Widget, EventArgs.Empty);
			}

			public override void ItemDidCollapse(NSNotification notification)
			{
				if (Handler.raiseExpandEvents)
				{
					var myitem = notification.UserInfo[(NSString)"NSObject"] as EtoTreeItem;
					if (myitem != null && myitem.Item.Expanded)
					{
						myitem.Item.Expanded = false;
						Handler.Callback.OnCollapsed(Handler.Widget, new TreeViewItemEventArgs(myitem.Item));
					}
				}
			}

			public override bool ShouldExpandItem(NSOutlineView outlineView, NSObject item)
			{
				if (Handler.raiseExpandEvents)
				{
					var myitem = item as EtoTreeItem;
					if (myitem != null && !myitem.Item.Expanded)
					{
						var args = new TreeViewItemCancelEventArgs(myitem.Item);
						Handler.Callback.OnExpanding(Handler.Widget, args);
						return !args.Cancel;
					}
				}
				return true;
			}

			public override bool ShouldCollapseItem(NSOutlineView outlineView, NSObject item)
			{
				if (Handler.raiseExpandEvents)
				{
					var myitem = item as EtoTreeItem;
					if (myitem != null && myitem.Item.Expanded)
					{
						var args = new TreeViewItemCancelEventArgs(myitem.Item);
						Handler.Callback.OnCollapsing(Handler.Widget, args);
						return !args.Cancel;
					}
				}
				return true;
			}

			public override void ItemDidExpand(NSNotification notification)
			{
				if (Handler.raiseExpandEvents)
				{
					var myitem = notification.UserInfo[(NSString)"NSObject"] as EtoTreeItem;
					if (myitem != null && !myitem.Item.Expanded)
					{
						myitem.Item.Expanded = true;
						Handler.Callback.OnExpanded(Handler.Widget, new TreeViewItemEventArgs(myitem.Item));
						Handler.ExpandItems(myitem);
					}
				}
			}
		}

		public class EtoDataSource : NSOutlineViewDataSource
		{
			WeakReference handler;
			public TreeViewHandler Handler { get { return (TreeViewHandler)handler.Target; } set { handler = new WeakReference(value); } }

			public override NSObject GetObjectValue(NSOutlineView outlineView, NSTableColumn tableColumn, NSObject item)
			{
				var myitem = item as EtoTreeItem;
				return myitem;
			}

			public override bool ItemExpandable(NSOutlineView outlineView, NSObject item)
			{
				var myitem = item as EtoTreeItem;
				return myitem != null && myitem.Item.Expandable;
			}

			public override NSObject GetChild(NSOutlineView outlineView, nint childIndex, NSObject item)
			{
				Dictionary<int, EtoTreeItem> items;
				var myitem = item as EtoTreeItem;
				items = myitem == null ? Handler.topitems : myitem.Items;
				
				EtoTreeItem etoItem;
				if (!items.TryGetValue((int)childIndex, out etoItem))
				{
					var parentItem = myitem != null ? myitem.Item : Handler.top;
					etoItem = new EtoTreeItem { Item = parentItem [(int)childIndex] };
					Handler.cachedItems[etoItem.Item] = etoItem;
					items[(int)childIndex] = etoItem;
				}
				return etoItem;
			}

			public override nint GetChildrenCount(NSOutlineView outlineView, NSObject item)
			{
				if (Handler.top == null)
					return 0;

				if (item == null)
					return Handler.top.Count;

				var myitem = item as EtoTreeItem;
				return myitem.Item.Count;
			}

			public override void SetObjectValue(NSOutlineView outlineView, NSObject theObject, NSTableColumn tableColumn, NSObject item)
			{
				var myitem = item as EtoTreeItem;
				if (myitem != null)
				{
					var args = new TreeViewItemEditEventArgs(myitem.Item, (string)(NSString)theObject);
					Handler.Callback.OnLabelEdited(Handler.Widget, args);
					if (!args.Cancel)
						myitem.Item.Text = args.Label;
				}
			}
		}

		public class EtoOutlineView : NSOutlineView, IMacControl
		{
			public WeakReference WeakHandler { get; set; }

			public TreeViewHandler Handler
			{ 
				get { return (TreeViewHandler)WeakHandler.Target; }
				set { WeakHandler = new WeakReference(value); } 
			}

			/// <summary>
			/// The area to the right and below the rows is not filled with the background
			/// color. This fixes that. See http://orangejuiceliberationfront.com/themeing-nstableview/
			/// </summary>
			public override void DrawBackground(CGRect clipRect)
			{
				var backgroundColor = Handler.BackgroundColor;
				if (backgroundColor != Colors.Transparent) {
					backgroundColor.ToNSUI ().Set ();
					NSGraphics.RectFill (clipRect);
				} else
					base.DrawBackground (clipRect);
			}
		}

		public override NSView ContainerControl
		{
			get { return Scroll; }
		}

		public TreeViewHandler()
		{
			Control = new EtoOutlineView
			{ 
				Handler = this,
				Delegate = new EtoOutlineDelegate{ Handler = this },
				DataSource = new EtoDataSource{ Handler = this },
				HeaderView = null,
				AutoresizesOutlineColumn = true,
				AllowsColumnResizing = false,
				FocusRingType = NSFocusRingType.None,
				ColumnAutoresizingStyle = NSTableViewColumnAutoresizingStyle.FirstColumnOnly
			};
			column = new NSTableColumn
			{
				DataCell = new MacImageListItemCell { 
					UsesSingleLineMode = true,
					Editable = true
				},
				Editable = false
			};
			
			
			Control.AddColumn(column);
			Control.OutlineTableColumn = column;
			
			Scroll = new EtoScrollView
			{
				Handler = this,
				HasVerticalScroller = true,
				HasHorizontalScroller = true,
				AutohidesScrollers = true,
				BorderType = NSBorderType.BezelBorder,
				DocumentView = Control
			};
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case TreeView.ExpandedEvent:
				case TreeView.ExpandingEvent:
				case TreeView.CollapsedEvent:
				case TreeView.CollapsingEvent:
				case TreeView.SelectionChangedEvent:
					// handled in delegate
					break;
				case TreeView.ActivatedEvent:
					Widget.KeyDown += (sender, e) => {
						if (!column.Editable && e.KeyData == Keys.Enter)
						{
							Callback.OnActivated(Widget, new TreeViewItemEventArgs(SelectedItem));
							e.Handled = true;
						}
					};
					Control.DoubleClick += HandleDoubleClick;
					break;
				case TreeView.LabelEditedEvent:
				case TreeView.LabelEditingEvent:
					// handled in delegate
					break;
				case TreeView.NodeMouseClickEvent:
                    /* TODO */
                //    Control.NodeMouseClick += (s, e) =>
                //    {
                //        this.Widget.OnNodeMouseClick(
                //            Generator.Convert(e));
                //    };
					break;

				default:
					base.AttachEvent(id);
					break;
			}
		}

		static void HandleDoubleClick (object sender, EventArgs e)
		{
			var handler = GetHandler(sender) as TreeViewHandler;
			if (handler != null)
			{
				if (handler.column.Editable)
					handler.Control.EditColumn(handler.Control.ClickedColumn, handler.Control.ClickedRow, new NSEvent(), true);
				else
					handler.Callback.OnActivated(handler.Widget, new TreeViewItemEventArgs(handler.SelectedItem));
			}
		}

		public ITreeStore DataStore
		{
			get { return top; }
			set
			{
				top = value;
				topitems.Clear();
				cachedItems.Clear();
				Control.ReloadData();
				ExpandItems(null);
			}
		}

		public ITreeItem SelectedItem
		{
			get
			{
				var row = Control.SelectedRow;
				if (row == -1)
					return null;
				var myitem = Control.ItemAtRow(row) as EtoTreeItem;
				return myitem == null ? null : myitem.Item;
			}
			set
			{
				PerformSelect(value, true);
			}
		}

		void PerformSelect(ITreeItem item, bool scrollToRow)
		{
			if (item == null)
				Control.DeselectAll(Control);
			else
			{
				
				EtoTreeItem myitem;
				if (cachedItems.TryGetValue(item, out myitem))
				{
					var cachedRow = Control.RowForItem(myitem);
					if (cachedRow >= 0)
					{
						if (scrollToRow)
							Control.ScrollRowToVisible(cachedRow);
						Control.SelectRow((nnint)cachedRow, false);
						return;
					}
				}
				
				var row = ExpandToItem(item);
				if (row != null)
				{
					if (scrollToRow)
						Control.ScrollRowToVisible(row.Value);
					Control.SelectRow((nnint)row.Value, false);
				}
			}
		}

		public ContextMenu ContextMenu
		{
			get { return contextMenu; }
			set
			{
				contextMenu = value;
				Control.Menu = contextMenu == null ? null : ((ContextMenuHandler)contextMenu.Handler).Control;
			}
		}

		static IEnumerable<ITreeItem> GetParents(ITreeItem item)
		{
			var parent = item.Parent;
			while (parent != null)
			{
				yield return parent;
				parent = parent.Parent;
			}
		}

		int CountRows(ITreeItem item)
		{
			if (!item.Expanded)
				return 0;
			
			var rows = 0;
			var container = item;
			if (container != null)
			{
				rows += container.Count;
				for (int i = 0; i < container.Count; i++)
				{
					rows += CountRows(container[i]);
				}
			}
			return rows;
		}

		int FindRow(IDataStore<ITreeItem> container, ITreeItem item)
		{
			int row = 0;
			for (int i = 0; i < container.Count; i++)
			{
				var current = container[i];
				if (object.ReferenceEquals(current, item))
				{
					return row;
				}
				row ++;
				row += CountRows(current);
			}
			return -1;
		}

		int? ExpandToItem(ITreeItem item)
		{
			var parents = GetParents(item).Reverse();
			IDataStore<ITreeItem> lastParent = null;
			var row = 0;
			foreach (var parent in parents)
			{
				if (lastParent != null)
				{
					var foundRow = FindRow(lastParent, parent);
					if (foundRow == -1)
						return null;
					row += foundRow;
					var foundItem = Control.ItemAtRow(row) as EtoTreeItem;
					if (foundItem == null)
						return null;
					Control.ExpandItem(foundItem);
					foundItem.Item.Expanded = true;
					row ++;
				}
				lastParent = parent;
			}
			if (lastParent != null)
			{
				var foundRow = FindRow(lastParent, item);
				if (foundRow == -1)
					return null;
				
				return foundRow + row;
			}
			return null;
		}

		void SetItemExpansion(NSObject parent)
		{
			raiseExpandEvents = false;
			var item = parent as EtoTreeItem;
			if (item != null && item.Item.Expandable && item.Item.Expanded != Control.IsItemExpanded(item))
			{
				if (item.Item.Expanded)
					Control.ExpandItem(item);
				else
					Control.CollapseItem(item, false);
			}
			raiseExpandEvents = true;
		}

		void ExpandItems(NSObject parent)
		{
			raiseExpandEvents = false;
			var item = parent as EtoTreeItem;
			PerformExpandItems(item);
			raiseExpandEvents = true;
		}

		void PerformExpandItems(NSObject parent)
		{
			var ds = Control.DataSource;
			var count = ds.GetChildrenCount(Control, parent);
			for (int i=0; i<count; i++)
			{
				var item = ds.GetChild(Control, i, parent) as EtoTreeItem;
				if (item != null && item.Item.Expandable && item.Item.Expanded != Control.IsItemExpanded(item))
				{
					if (item.Item.Expanded)
					{
						Control.ExpandItem(item);
						PerformExpandItems(item);
					}
					else
						Control.CollapseItem(item, false);
				}
			}
		}

		public void RefreshData()
		{
			selectionChanging = true;
			var selectedItem = SelectedItem;
			
			var loc = Scroll.ContentView.Bounds.Location;
			if (!Control.IsFlipped)
				loc.Y = Control.Frame.Height - Scroll.ContentView.Frame.Height - loc.Y;

			topitems.Clear();
			cachedItems.Clear();
			Control.ReloadData();
			ExpandItems(null);
			PerformSelect(selectedItem, false);
			
			if (Control.IsFlipped)
				Scroll.ContentView.ScrollToPoint(loc);
			else
				Scroll.ContentView.ScrollToPoint(new CGPoint(loc.X, Control.Frame.Height - Scroll.ContentView.Frame.Height - loc.Y));
			
			Scroll.ReflectScrolledClipView(Scroll.ContentView);
			
			selectionChanging = false;
		}

		public void RefreshItem(ITreeItem item)
		{
			EtoTreeItem myitem;
			if (cachedItems.TryGetValue(item, out myitem))
			{
				var row = Control.RowForItem(myitem);
				if (row >= 0)
					topitems.Remove((int)row);
				myitem.Items.Clear();
				SetItemExpansion(myitem);
				Control.ReloadItem(myitem, true);
				ExpandItems(myitem);
			}
			else
				RefreshData();
		}

		public ITreeItem GetNodeAt(PointF point)
		{
			var row = Control.GetRow(point.ToNS());
			if (row >= 0)
			{
				var item = Control.ItemAtRow(row) as EtoTreeItem;
				if (item != null)
					return item.Item;
			}
			return null;
		}

		Color? textColor;
		public Color TextColor
		{
			get { return textColor ?? Colors.Transparent; }
			set { textColor = value; }
		}

		public bool LabelEdit
		{
			get { return column.Editable; }
			set { column.Editable = value; }
		}
	}
}

