using Eto.Mac.Drawing;
using Eto.Mac.Forms.Controls;




namespace Eto.Mac.Forms.Cells
{
	public class DrawableCellHandler : CellHandler<DrawableCell, DrawableCell.ICallback>, DrawableCell.IHandler
	{
		public class EtoCellValue : NSObject
		{
			public object Item { get; set; }

			public EtoCellValue()
			{
			}

			public EtoCellValue(IntPtr handle) : base(handle)
			{
			}

			[Export("copyWithZone:")]
			NSObject CopyWithZone(IntPtr zone)
			{
				var val = new EtoCellValue { Item = Item };
				val.DangerousRetain();
				return val;
			}
		}

		public class EtoCell : NSCell, IMacControl
		{
			public WeakReference WeakHandler { get; set; }

			public DrawableCellHandler Handler
			{ 
				get { return (DrawableCellHandler)WeakHandler.Target; }
				set { WeakHandler = new WeakReference(value); } 
			}

			public EtoCell()
			{
				Enabled = true;
			}

			public EtoCell(IntPtr handle) : base(handle)
			{
			}

			public Color BackgroundColor { get; set; }

			public bool DrawsBackground { get; set; }

			public override CGSize CellSizeForBounds(CGRect bounds)
			{
				return CGSize.Empty;
			}

			public override void DrawInteriorWithFrame(CGRect cellFrame, NSView inView)
			{
				var nscontext = NSGraphicsContext.CurrentContext;

				if (DrawsBackground)
				{
					var context = nscontext.CGContext;
					context.SetFillColor(BackgroundColor.ToCG());
					context.FillRect(cellFrame);
				}

				var handler = Handler;
				var graphicsHandler = new GraphicsHandler(inView, nscontext, (float)cellFrame.Height, cellFrame);
				using (var graphics = new Graphics(graphicsHandler))
				{
					var state = Highlighted ? CellStates.Selected : CellStates.None;
					var item = ObjectValue as EtoCellValue;
					#pragma warning disable 618
					var args = new DrawableCellPaintEventArgs(graphics, cellFrame.ToEto(), state, item != null ? item.Item : null);
					handler.Callback.OnPaint(handler.Widget, args);
					#pragma warning restore 618
				}
			}
		}

		public override nfloat GetPreferredWidth(object value, CGSize cellSize, int row, object dataItem)
		{
			return -1; // TODO: Add ability for DrawableCell to provide a preferred width for a specific item.
		}

		public override Color GetBackgroundColor(NSView view) => ((EtoCellView)view).BackgroundColor;

		public override void SetBackgroundColor(NSView view, Color color)
		{
			var field = ((EtoCellView)view);
			field.BackgroundColor = color;
			field.DrawsBackground = color.A > 0;
		}

		private void SetDefaults(EtoCellView view)
		{
			view.DrawsBackground = false;
		}

		public override NSObject GetObjectValue(object dataItem)
		{
			return new EtoCellValue { Item = dataItem };
		}

		public override void SetObjectValue(object dataItem, NSObject value)
		{
		}

		public class EtoCellView : NSControl
		{
			public WeakReference WeakHandler { get; set; }

			public DrawableCellHandler Handler
			{ 
				get { return (DrawableCellHandler)(WeakHandler != null ? WeakHandler.Target : null); }
				set { WeakHandler = new WeakReference(value); } 
			}

			public EtoCellView()
			{
			}

			public EtoCellView(IntPtr handle)
				: base(handle)
			{
			}

			public Color BackgroundColor { get; set; }

			public bool DrawsBackground { get; set; }

			NSObject val;

			public override NSObject ObjectValue
			{
				get
				{
					return base.ObjectValue;
				}
				set
				{
					base.ObjectValue = value;
					val = value;
				}
			}

			public override bool AcceptsFirstResponder()
			{
				return Enabled;
			}

			public override void DrawRect(CGRect dirtyRect)
			{
				var nscontext = NSGraphicsContext.CurrentContext;
				var isFirstResponder = Window?.FirstResponder == this;

				var cellFrame = Bounds;
				if (DrawsBackground)
				{
					var context = nscontext.CGContext;
					context.SetFillColor(BackgroundColor.ToCG());
					context.FillRect(cellFrame);
				}

				var handler = Handler;
				if (handler == null)
					return;
				var graphicsHandler = new GraphicsHandler(this, nscontext, (float)cellFrame.Height, cellFrame);
				using (var graphics = new Graphics(graphicsHandler))
				{
					var rowView = this.Superview as NSTableRowView;
					var state = CellStates.None;
					if (rowView != null && rowView.Selected)
						state |= CellStates.Selected;
					if (isFirstResponder)
					{
						state |= CellStates.Editing;
						SetKeyboardFocusRingNeedsDisplay(cellFrame);
						nscontext.SaveGraphicsState();
						GraphicsExtensions.SetFocusRingStyle(NSFocusRingPlacement.RingOnly);
						NSGraphics.RectFill(cellFrame);
						nscontext.RestoreGraphicsState();
					}
					var item = val as EtoCellValue;
					#pragma warning disable 618
					var args = new DrawableCellPaintEventArgs(graphics, cellFrame.ToEto(), state, item != null ? item.Item : null);
					handler.Callback.OnPaint(handler.Widget, args);
					#pragma warning restore 618
				}
			}
		}

		static NSString enabledBinding = new NSString("enabled");

		public override NSView GetViewForItem(NSTableView tableView, NSTableColumn tableColumn, int row, NSObject obj, Func<NSObject, int, object> getItem)
		{
			var view = tableView.MakeView(tableColumn.Identifier, tableView) as EtoCellView;
			if (view == null)
			{
				view = new EtoCellView { Handler = this, Identifier = tableColumn.Identifier, FocusRingType = NSFocusRingType.Exterior };
				view.Bind(enabledBinding, tableColumn, "editable", null);
			}
			SetDefaults(view);
			var args = new MacCellFormatArgs(ColumnHandler.Widget, getItem(obj, row), row, view);
			ColumnHandler.DataViewHandler.OnCellFormatting(args);
			return view;
		}
	}
}

