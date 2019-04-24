using System;
using Eto.Forms;
using System.Collections.Generic;
using System.Linq;
using Eto.Drawing;
using Eto.Mac.Forms.Controls;


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

namespace Eto.Mac.Forms.Cells
{
	public class ComboBoxCellHandler : CellHandler<ComboBoxCell, ComboBoxCell.ICallback>, ComboBoxCell.IHandler
	{
		CollectionHandler collection;
		NSMenu menu = new NSMenu();

		public class EtoCell : NSPopUpButtonCell, IMacControl
		{
			public WeakReference WeakHandler { get; set; }

			public object Handler
			{ 
				get { return WeakHandler.Target; }
				set { WeakHandler = new WeakReference(value); } 
			}

			public EtoCell()
			{
			}

			public EtoCell(IntPtr handle) : base(handle)
			{
			}

			public NSColor TextColor { get; set; }

			public bool DrawsBackground { get; set; }

			public override void DrawBorderAndBackground(CGRect cellFrame, NSView controlView)
			{
				if (DrawsBackground)
				{
					var nscontext = NSGraphicsContext.CurrentContext;
					var context = nscontext.GraphicsPort;

					BackgroundColor.SetFill();
					context.FillRect(cellFrame);
				}

				base.DrawBorderAndBackground(cellFrame, controlView);
			}

			public override CGRect DrawTitle(NSAttributedString title, CGRect frame, NSView controlView)
			{
				if (TextColor != null)
				{
					var newtitle = (NSMutableAttributedString)title.MutableCopy();
					var range = new NSRange(0, (int)title.Length);
					newtitle.RemoveAttribute(NSStringAttributeKey.ForegroundColor, range);
					newtitle.AddAttribute(NSStringAttributeKey.ForegroundColor, TextColor, range);
					title = newtitle;
				}
				var rect = base.DrawTitle(title, frame, controlView);
				return rect;
			}
		}

		class CollectionHandler : EnumerableChangedHandler<object>
		{
			public ComboBoxCellHandler Handler { get; set; }

			ILookup<string, int> indexLookupByKey;

			public ILookup<string, int> IndexLookup
			{
				get { return indexLookupByKey ?? (indexLookupByKey = Collection.ToLookup(e => Handler.Widget.ComboKeyBinding.GetValue(e), e => IndexOf(e))); }
			}

			public override void AddItem(object item)
			{
				var menu = Handler.menu;
				var textBinding = Handler.Widget.ComboTextBinding;
				menu.AddItem(new NSMenuItem(textBinding.GetValue(item)));
				menu.Title = Guid.NewGuid().ToString();
				indexLookupByKey = null;
			}

			public override void InsertItem(int index, object item)
			{
				var menu = Handler.menu;
				var textBinding = Handler.Widget.ComboTextBinding;
				menu.InsertItem(new NSMenuItem(textBinding.GetValue(item)), index);
				menu.Title = Guid.NewGuid().ToString();
				indexLookupByKey = null;
			}

			public override void RemoveItem(int index)
			{
				var menu = Handler.menu;
				menu.RemoveItemAt(index);
				menu.Title = Guid.NewGuid().ToString();
				indexLookupByKey = null;
			}

			public override void RemoveAllItems()
			{
				var menu = Handler.menu;
				menu.RemoveAllItems();
				menu.Title = Guid.NewGuid().ToString();
				indexLookupByKey = null;
			}
		}

		public IEnumerable<object> DataStore
		{
			get { return collection != null ? collection.Collection : null; }
			set
			{
				if (collection != null)
					collection.Unregister();
				collection = new CollectionHandler { Handler = this };
				collection.Register(value);
			}
		}

		public override void SetObjectValue(object dataItem, NSObject value)
		{
			if (Widget.Binding != null && !ColumnHandler.DataViewHandler.SuppressUpdate)
			{
				var row = ((NSNumber)value).Int32Value;
				var item = collection.ElementAt(row);
				var itemValue = item != null ? Widget.ComboKeyBinding.GetValue(item) : null;
				Widget.Binding.SetValue(dataItem, itemValue);
			}
		}

		public override NSObject GetObjectValue(object dataItem)
		{
			if (Widget.Binding != null)
			{
				var val = Widget.Binding.GetValue(dataItem);
				var key = Convert.ToString(val);
				var lookup = collection.IndexLookup[key].ToArray();
				var index = lookup.Length > 0 ? lookup[0] : -1;
				return new NSNumber(index);
			}
			return null;
		}

		static EtoPopUpButton field = new EtoPopUpButton { Cell = new EtoCell() };
		static NSFont defaultFont = field.Font;
		static IntPtr sel_GetTitle = Selector.GetHandle("title");

		static bool IsDifferent(EtoPopUpButton field, NSMenu menu)
		{
			var fieldTitle = Messaging.IntPtr_objc_msgSend(field.Handle, sel_GetTitle);
			var menuTitle = Messaging.IntPtr_objc_msgSend(menu.Handle, sel_GetTitle);
			return fieldTitle != menuTitle;
		}

		public override nfloat GetPreferredWidth(object value, CGSize cellSize, int row, object dataItem)
		{
			var args = new MacCellFormatArgs(ColumnHandler.Widget, dataItem, row, field);
			ColumnHandler.DataViewHandler.OnCellFormatting(args);

			field.Font = defaultFont;
			if (args.FontSet)
				field.Font = args.Font.ToNS();

			if (IsDifferent(field, menu))
			{
				field.Menu = menu.Copy() as NSMenu;
			}
			field.ObjectValue = value as NSObject;
			return field.Cell.CellSizeForBounds(new CGRect(0, 0, nfloat.MaxValue, cellSize.Height)).Width;
		}

		public class EtoPopUpButton : NSPopUpButton
		{
			WeakReference handler;
			public ComboBoxCellHandler Handler
			{
				get => handler?.Target as ComboBoxCellHandler;
				set => handler = new WeakReference(value);
			}

			public EtoPopUpButton() { }

			public EtoPopUpButton(IntPtr handle) : base(handle) { }

			public override NSMenu MenuForEvent(NSEvent theEvent)
			{
				return null;
			}
		}

		public override Color GetBackgroundColor(NSView view)
		{
			return ((EtoPopUpButton)view).Cell.BackgroundColor.ToEto();
		}

		public override void SetBackgroundColor(NSView view, Color color)
		{
			var field = ((EtoCell)((EtoPopUpButton)view).Cell);
			field.BackgroundColor = color.ToNSUI();
			field.DrawsBackground = color.A > 0;
		}

		public override Color GetForegroundColor(NSView view)
		{
			return ((EtoCell)((EtoPopUpButton)view).Cell).TextColor.ToEto();
		}

		public override void SetForegroundColor(NSView view, Color color)
		{
			((EtoCell)((EtoPopUpButton)view).Cell).TextColor = color.ToNSUI();
		}

		public override Font GetFont(NSView view)
		{
			return ((EtoPopUpButton)view).Font.ToEto();
		}

		public override void SetFont(NSView view, Font font)
		{
			((EtoPopUpButton)view).Font = font.ToNS();
		}

		class CellView : EtoPopUpButton
		{
			[Export("item")]
			public NSObject Item { get; set; }
			public CellView() { }
			public CellView(IntPtr handle) : base(handle) { }
		}

		static NSString enabledBinding = new NSString("enabled");

		public override NSView GetViewForItem(NSTableView tableView, NSTableColumn tableColumn, int row, NSObject obj, Func<NSObject, int, object> getItem)
		{
			var view = tableView.MakeView(tableColumn.Identifier, tableView) as CellView;
			if (view == null)
			{
				var col = Array.IndexOf(tableView.TableColumns(), tableColumn);

				view = new CellView
				{
					Handler = this,
					Cell = new EtoCell(),
					Identifier = tableColumn.Identifier,
					Bordered = false,
					AutoresizingMask = NSViewResizingMask.HeightSizable | NSViewResizingMask.WidthSizable
				};
				view.Activated += (sender, e) =>
				{
					var control = (CellView)sender;
					var r = (int)control.Tag;
					var item = getItem(control.Item, r);
					var cellArgs = MacConversions.CreateCellEventArgs(ColumnHandler.Widget, tableView, r, col, item);
					ColumnHandler.DataViewHandler.OnCellEditing(cellArgs);
					SetObjectValue(item, control.ObjectValue);

					ColumnHandler.DataViewHandler.OnCellEdited(cellArgs);
					control.ObjectValue = GetObjectValue(item);

				};
				view.Bind(enabledBinding, tableColumn, "editable", null);
				view.Menu = menu.Copy() as NSMenu;
			}
			else if (IsDifferent(view, menu))
				view.Menu = menu.Copy() as NSMenu;

			view.Tag = row;
			view.Item = obj;
			var formatArgs = new MacCellFormatArgs(ColumnHandler.Widget, getItem(obj, row), row, view);
			ColumnHandler.DataViewHandler.OnCellFormatting(formatArgs);
			return view;
		}
	}
}

