using System;
using Eto.Forms;
using Eto.Drawing;
using Eto.Mac.Forms.Controls;
using Eto.Mac.Drawing;

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
	public class TextBoxCellHandler : CellHandler<TextBoxCell, TextBoxCell.ICallback>, TextBoxCell.IHandler, IMacText
	{
		static readonly CellView field = new CellView();
		static readonly NSFont defaultFont = field.Font;

		public override nfloat GetPreferredWidth(object value, CGSize cellSize, int row, object dataItem)
		{
			field.Font = defaultFont;

			var args = new MacCellFormatArgs(ColumnHandler.Widget, dataItem, row, field);
			ColumnHandler.DataViewHandler.OnCellFormatting(args);

			if (args.FontSet)
				field.Font = args.Font.ToNS();
			field.ObjectValue = value as NSObject ?? new NSString(string.Empty);
			return field.Cell.CellSizeForBounds(new CGRect(0, 0, nfloat.MaxValue, cellSize.Height)).Width;
		}

		public override NSObject GetObjectValue(object dataItem)
		{
			if (Widget.Binding != null)
			{
				var val = Widget.Binding.GetValue(dataItem);
				return val != null ? new NSString(Convert.ToString(val)) : NSString.Empty;
			}
			return NSString.Empty;
		}

		public override void SetObjectValue(object dataItem, NSObject value)
		{
			if (Widget.Binding != null && !ColumnHandler.DataViewHandler.SuppressUpdate)
			{
				var str = value as NSString;
				if (str != null)
					Widget.Binding.SetValue(dataItem, str.ToString());
				else
					Widget.Binding.SetValue(dataItem, null);
			}
		}

		public override Color GetBackgroundColor(NSView view)
		{
			return ((CellView)view).Cell.BetterBackgroundColor.ToEto();
		}

		public override void SetBackgroundColor(NSView view, Color color)
		{
			((CellView)view).Cell.BetterBackgroundColor = color.ToNSUI();
		}

		public override Color GetForegroundColor(NSView view)
		{
			return ((CellView)view).TextColor.ToEto();
		}

		public override void SetForegroundColor(NSView view, Color color)
		{
			((CellView)view).TextColor = color.ToNSUI();
		}

		public override Font GetFont(NSView view)
		{
			return ((CellView)view).Font.ToEto();
		}

		public override void SetFont(NSView view, Font font)
		{
			((CellView)view).Font = font.ToNS();
		}

		TextAlignment _textAlignment;
		public TextAlignment TextAlignment
		{
			get { return _textAlignment; }
			set
			{
				if (_textAlignment != value)
				{
					_textAlignment = value;
					ReloadColumnData();
				}
			}
		}

		VerticalAlignment _verticalAlignment = VerticalAlignment.Center;
		public VerticalAlignment VerticalAlignment
		{
			get { return _verticalAlignment; }
			set
			{
				if (_verticalAlignment != value)
				{
					_verticalAlignment = value;
					ReloadColumnData();
				}
			}
		}

		public AutoSelectMode AutoSelectMode { get; set; }

		class CellView : EtoCellTextField
		{
			[Export("item")]
			public NSObject Item { get; set; }

			public CellView()
			{
				base.Cell = new EtoLabelFieldCell
				{
					Wraps = false,
					Scrollable = true,
					UsesSingleLineMode = false // true prevents proper vertical alignment 
				};
				Selectable = false;
				DrawsBackground = false;
				Bezeled = false;
				Bordered = false;
				AutoresizingMask = NSViewResizingMask.HeightSizable | NSViewResizingMask.WidthSizable;
			}
			public CellView(IntPtr handle) : base(handle) { }

			public new EtoLabelFieldCell Cell => (EtoLabelFieldCell)base.Cell;
		}

		static readonly NSString editableBinding = new NSString("editable");

		public override NSView GetViewForItem(NSTableView tableView, NSTableColumn tableColumn, int row, NSObject obj, Func<NSObject, int, object> getItem)
		{
			var view = tableView.MakeView(tableColumn.Identifier, tableView) as CellView;
			if (view == null)
			{
				view = new CellView
				{
					WeakHandler = new WeakReference(this),
					Identifier = tableColumn.Identifier,
				};

				var col = Array.IndexOf(tableView.TableColumns(), tableColumn);
				view.BecameFirstResponder += (sender, e) =>
				{
					var control = (CellView)sender;
					var r = (int)control.Tag;
					var item = getItem(control.Item, r);
					var ee = MacConversions.CreateCellEventArgs(ColumnHandler.Widget, tableView, r, col, item);
					ColumnHandler.DataViewHandler.OnCellEditing(ee);
				};
				view.EditingEnded += (sender, e) =>
				{
					var notification = (NSNotification)sender;
					var control = (CellView)notification.Object;
					var r = (int)control.Tag;
					var item = getItem(control.Item, r);
					SetObjectValue(item, control.ObjectValue);

					var ee = MacConversions.CreateCellEventArgs(ColumnHandler.Widget, tableView, r, col, item);
					ColumnHandler.DataViewHandler.OnCellEdited(ee);
					control.ObjectValue = GetObjectValue(item) ?? new NSString(string.Empty);
				};
				view.ResignedFirstResponder += (sender, e) =>
				{
					var control = (CellView)sender;
					var r = (int)control.Tag;
					var item = getItem(control.Item, r);
					SetObjectValue(item, control.ObjectValue);

					var ee = MacConversions.CreateCellEventArgs(ColumnHandler.Widget, tableView, r, col, item);
					ColumnHandler.DataViewHandler.OnCellEdited(ee);
				};
				view.Bind(editableBinding, tableColumn, "editable", null);
			}

			var cell = view.Cell;
			cell.VerticalAlignment = VerticalAlignment;
			cell.Alignment = TextAlignment.ToNS();

			view.Item = obj;
			view.Tag = row;
			var args = new MacCellFormatArgs(ColumnHandler.Widget, getItem(obj, row), row, view);
			ColumnHandler.DataViewHandler.OnCellFormatting(args);
			return view;
		}

		public void SetLastSelection(Range<int>? range)
		{

		}
	}
}

