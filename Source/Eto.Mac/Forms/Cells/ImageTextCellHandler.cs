using System;
using Eto.Forms;
using Eto.Drawing;
using Eto.Mac.Drawing;
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
	public class ImageTextCellHandler : CellHandler<ImageTextCell, ImageTextCell.ICallback>, ImageTextCell.IHandler
	{
		public override NSObject GetObjectValue(object dataItem)
		{
			var result = new MacImageData();
			if (Widget.TextBinding != null)
			{
				result.Text = (NSString)Convert.ToString(Widget.TextBinding.GetValue(dataItem));
			}
			if (Widget.ImageBinding != null)
			{
				var image = Widget.ImageBinding.GetValue(dataItem) as Image;
				result.Image = image != null ? ((IImageSource)image.Handler).GetImage() : null;
			}
			else
				result.Image = null;
			return result;
		}

		public override void SetObjectValue(object dataItem, NSObject value)
		{
			if (Widget.TextBinding != null)
			{
				var str = value as NSString;
				if (str != null)
					Widget.TextBinding.SetValue(dataItem, str.ToString());
				else
					Widget.TextBinding.SetValue(dataItem, null);
			}
		}

		static EtoCellTextField field = new EtoCellTextField { Cell = new MacImageListItemCell() };
		static NSFont defaultFont = field.Font;

		public override nfloat GetPreferredWidth(object value, CGSize cellSize, int row, object dataItem)
		{
			field.Font = defaultFont;

			var args = new MacCellFormatArgs(ColumnHandler.Widget, dataItem, row, field);
			ColumnHandler.DataViewHandler.Callback.OnCellFormatting(ColumnHandler.DataViewHandler.Widget, args);

			if (args.FontSet)
				field.Font = args.Font.ToNS();
			field.ObjectValue = value as NSObject;
			return field.Cell.CellSizeForBounds(new CGRect(0, 0, nfloat.MaxValue, cellSize.Height)).Width;
		}

		ImageInterpolation _imageInterpolation;
		public ImageInterpolation ImageInterpolation
		{
			get { return _imageInterpolation; }
			set
			{
				if (_imageInterpolation != value)
				{
					_imageInterpolation = value;
					ReloadColumnData();
				}
			}
		}

		public override Color GetBackgroundColor(NSView view)
		{
			return ((EtoLabelFieldCell)((EtoCellTextField)view).Cell).BetterBackgroundColor.ToEto();
		}

		public override void SetBackgroundColor(NSView view, Color color)
		{
			((EtoLabelFieldCell)((EtoCellTextField)view).Cell).BetterBackgroundColor = color.ToNSUI();
		}

		public override Color GetForegroundColor(NSView view)
		{
			return ((EtoCellTextField)view).TextColor.ToEto();
		}

		public override void SetForegroundColor(NSView view, Color color)
		{
			((EtoCellTextField)view).TextColor = color.ToNSUI();
		}

		public override Font GetFont(NSView view)
		{
			return ((EtoCellTextField)view).Font.ToEto();
		}

		public override void SetFont(NSView view, Font font)
		{
			((EtoCellTextField)view).Font = font.ToNS();
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

		class CellView : EtoCellTextField
		{
			[Export("item")]
			public NSObject Item { get; set; }
			public CellView() { }
			public CellView(IntPtr handle) : base(handle) { }
		}

		public override NSView GetViewForItem(NSTableView tableView, NSTableColumn tableColumn, int row, NSObject obj, Func<NSObject, int, object> getItem)
		{
			var view = tableView.MakeView(tableColumn.Identifier, tableView) as CellView;
			if (view == null)
			{
				view = new CellView();
				view.Cell = new MacImageListItemCell
				{
					Wraps = false,
					Scrollable = true,
					UsesSingleLineMode = false // true prevents proper vertical alignment 
				};
				view.Identifier = tableColumn.Identifier;
				view.Selectable = false;
				view.DrawsBackground = false;
				view.Bezeled = false;
				view.Bordered = false;
				view.AutoresizingMask = NSViewResizingMask.HeightSizable | NSViewResizingMask.WidthSizable;

				var col = Array.IndexOf(tableView.TableColumns(), tableColumn);
				view.BecameFirstResponder += (sender, e) =>
				{
					var control = (CellView)sender;
					var r = (int)control.Tag;
					var item = getItem(control.Item, r);
					var ee = new GridViewCellEventArgs(ColumnHandler.Widget, r, (int)col, item);
					ColumnHandler.DataViewHandler.Callback.OnCellEditing(ColumnHandler.DataViewHandler.Widget, ee);
				};
				view.EditingEnded += (sender, e) =>
				{
					var notification = (NSNotification)sender;
					var control = (CellView)notification.Object;
					var r = (int)control.Tag;
					var item = getItem(control.Item, r);
					SetObjectValue(item, control.ObjectValue);

					var ee = new GridViewCellEventArgs(ColumnHandler.Widget, r, (int)col, item);
					ColumnHandler.DataViewHandler.Callback.OnCellEdited(ColumnHandler.DataViewHandler.Widget, ee);
					control.ObjectValue = GetObjectValue(item);
				};
				view.ResignedFirstResponder += (sender, e) =>
				{
					var control = (CellView)sender;
					var r = (int)control.Tag;
					var item = getItem(control.Item, r);
					SetObjectValue(item, control.ObjectValue);

					var ee = new GridViewCellEventArgs(ColumnHandler.Widget, r, (int)col, item);
					ColumnHandler.DataViewHandler.Callback.OnCellEdited(ColumnHandler.DataViewHandler.Widget, ee);
				};
				view.Bind("editable", tableColumn, "editable", null);
			}
			var cell = (MacImageListItemCell)view.Cell;
			cell.ImageInterpolation = ImageInterpolation.ToNS();
			cell.VerticalAlignment = VerticalAlignment;
			cell.Alignment = TextAlignment.ToNS();
			view.Tag = row;
			view.Item = obj;
			var args = new MacCellFormatArgs(ColumnHandler.Widget, getItem(obj, row), row, view);
			ColumnHandler.DataViewHandler.OnCellFormatting(args);
			return view;
		}
	}
}

