using Eto.Mac.Drawing;
using Eto.Mac.Forms.Controls;


namespace Eto.Mac.Forms.Cells
{
	public class ImageTextCellHandler : CellHandler<ImageTextCell, ImageTextCell.ICallback>, ImageTextCell.IHandler, IMacText
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
			if (Widget.TextBinding != null && !ColumnHandler.DataViewHandler.SuppressUpdate)
			{
				var str = value as NSString;
				if (str != null)
					Widget.TextBinding.SetValue(dataItem, str.ToString());
				else
					Widget.TextBinding.SetValue(dataItem, null);
			}
		}

		static readonly CellView field = new CellView();
		static readonly NSFont defaultFont = field.TextField.Font;

		public override nfloat GetPreferredWidth(object value, CGSize cellSize, int row, object dataItem)
		{
			SetDefaults(field);
			field.SetFrameSize(cellSize);

			field.ObjectValue = value as NSObject;
			
			var args = new MacCellFormatArgs(ColumnHandler.Widget, dataItem, row, field);
			ColumnHandler.DataViewHandler.OnCellFormatting(args);

			return field.FittingSize.Width;
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

		public override Color GetBackgroundColor(NSView view) => ((CellView)view).BetterBackgroundColor.ToEto();
		public override void SetBackgroundColor(NSView view, Color color) => ((CellView)view).BetterBackgroundColor = color.ToNSUI();

		public override Color GetForegroundColor(NSView view) => ((CellView)view).TextField.TextColor.ToEto();
		public override void SetForegroundColor(NSView view, Color color) => ((CellView)view).TextField.TextColor = color.ToNSUI();

		public override Font GetFont(NSView view) => ((CellView)view).TextField.Font.ToEto();
		public override void SetFont(NSView view, Font font) => ((CellView)view).TextField.Font = font.ToNS();

		private void SetDefaults(CellView view)
		{
			var field = view.TextField;
			field.TextColor = NSColor.ControlText;
			field.Font = NSFont.SystemFontOfSize(NSFont.SystemFontSize);
			view.BetterBackgroundColor = null;
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

		class CellView : MacImageTextView, IMacControl
		{
			NSObject _objectValue;
			public CellView(IntPtr handle) : base(handle) { }

			public CellView()
			{
			}

			protected override NSTextField CreateTextField() => new EtoCellTextField
			{
				Cell = new EtoLabelFieldCell
				{
					Wraps = false,
					Scrollable = true,
					UsesSingleLineMode = false // true prevents proper vertical alignment 
				},
				Selectable = false,
				DrawsBackground = false,
				Bezeled = false,
				Bordered = false
			};

			public EtoLabelFieldCell TextCell => (EtoLabelFieldCell)TextField.Cell;

			public new EtoCellTextField TextField => (EtoCellTextField)base.TextField;

			[Export("objectValue")]
			public NSObject ObjectValue
			{
				get => _objectValue;
				set
				{
					var val = value as MacImageData;

					TextField.ObjectValue = (NSString)(val?.Text ?? string.Empty);
					Image = val?.Image;
					_objectValue = value;
				}
			}

			[Export("item")]
			public NSObject Item { get; set; }

			public WeakReference WeakHandler
			{
				get => TextField.WeakHandler;
				set => TextField.WeakHandler = value;
			}

			[Export("tag")]
			public new nint Tag
			{
				get => TextField.Tag;
				set => TextField.Tag = value;
			}
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
					AutoresizingMask = NSViewResizingMask.HeightSizable | NSViewResizingMask.WidthSizable
				};

				var col = Array.IndexOf(tableView.TableColumns(), tableColumn);
				view.TextField.BecameFirstResponder += (sender, e) =>
				{
					var control = (CellView)(sender as NSView)?.Superview;
					var r = (int)control.Tag;
					var item = getItem(control.Item, r);

					var ee = MacConversions.CreateCellEventArgs(ColumnHandler.Widget, tableView, r, col, item);
					ColumnHandler.DataViewHandler.OnCellEditing(ee);
				};
				view.TextField.EditingEnded += (sender, e) =>
				{
					var notification = (NSNotification)sender;
					var control = (CellView)(notification.Object as NSView)?.Superview;
					var r = (int)control.Tag;
					var item = getItem(control.Item, r);
					SetObjectValue(item, control.TextField.ObjectValue);

					var ee = MacConversions.CreateCellEventArgs(ColumnHandler.Widget, tableView, r, col, item);
					ColumnHandler.DataViewHandler.OnCellEdited(ee);
					control.ObjectValue = GetObjectValue(item);
				};
				bool isResigning = false;
				view.TextField.ResignedFirstResponder += (sender, e) =>
				{
					if (isResigning)
						return;
					isResigning = true;
					var control = (CellView)(sender as NSView)?.Superview;
					var r = (int)control.Tag;
					var item = getItem(control.Item, r);
					SetObjectValue(item, control.TextField.ObjectValue);

					var ee = MacConversions.CreateCellEventArgs(ColumnHandler.Widget, tableView, r, col, item);
					ColumnHandler.DataViewHandler.OnCellEdited(ee);
					isResigning = false;
				};
				view.TextField.Bind(editableBinding, tableColumn, "editable", null);
			}
			view.ImageInterpolation = ImageInterpolation.ToNS();

			var cell = view.TextCell;
			cell.VerticalAlignment = VerticalAlignment;
			cell.Alignment = TextAlignment.ToNS();

			view.Tag = row;
			view.Item = obj;
			SetDefaults(view);
			var args = new MacCellFormatArgs(ColumnHandler.Widget, getItem(obj, row), row, view);
			ColumnHandler.DataViewHandler.OnCellFormatting(args);
			return view;
		}

		public void SetLastSelection(Range<int>? range)
		{
			// do nothing?
		}
	}
}

