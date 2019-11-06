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
	public class ProgressCellHandler : CellHandler<ProgressCell, ProgressCell.ICallback>, ProgressCell.IHandler
	{
		public override NSObject GetObjectValue(object dataItem)
		{
			float? progress = Widget.Binding.GetValue(dataItem);
			if (Widget.Binding != null && progress.HasValue)
			{
				progress = progress < 0f ? 0f : progress > 1f ? 1f : progress;
				return new NSNumber((float)progress);
			}
			return new NSNumber(float.NaN);
		}

		public override void SetObjectValue(object dataItem, NSObject value)
		{
			if (Widget.Binding != null)
			{
				float? progress = ((NSNumber)value).FloatValue as float?;
				if (progress.HasValue)
					progress = progress < 0f ? 0f : progress > 1f ? 1f : progress;
				Widget.Binding.SetValue(dataItem, progress);
			}
		}

		static NSLevelIndicator field = new NSLevelIndicator { Cell = new EtoCell() };

		public override nfloat GetPreferredWidth(object value, CGSize cellSize, int row, object dataItem)
		{
			var args = new MacCellFormatArgs(ColumnHandler.Widget, dataItem, row, field);
			ColumnHandler.DataViewHandler.OnCellFormatting(args);

			field.Font = args.Font.ToNS() ?? NSFont.BoldSystemFontOfSize(NSFont.SystemFontSize);
			field.ObjectValue = value as NSObject;
			return field.Cell.CellSizeForBounds(new CGRect(0, 0, nfloat.MaxValue, cellSize.Height)).Width;
		}

		public override Color GetBackgroundColor(NSView view)
		{
			return ((EtoCell)((NSLevelIndicator)view).Cell).BackgroundColor.ToEto();
		}

		public override void SetBackgroundColor(NSView view, Color color)
		{
			var field = ((EtoCell)((NSLevelIndicator)view).Cell);
			field.BackgroundColor = color.ToNSUI();
			field.DrawsBackground = color.A > 0;
		}

		public override Color GetForegroundColor(NSView view)
		{
			return ((EtoCell)((NSLevelIndicator)view).Cell).TextColor.ToEto();
		}

		public override void SetForegroundColor(NSView view, Color color)
		{
			((EtoCell)((NSLevelIndicator)view).Cell).TextColor = color.ToNSUI();
		}

		public override Font GetFont(NSView view)
		{
			return ((NSLevelIndicator)view).Font.ToEto();
		}

		public override void SetFont(NSView view, Font font)
		{
			((NSLevelIndicator)view).Font = font.ToNS();
		}


		public override NSView GetViewForItem(NSTableView tableView, NSTableColumn tableColumn, int row, NSObject obj, Func<NSObject, int, object> getItem)
		{
			var view = tableView.MakeView(tableColumn.Identifier, tableView) as NSLevelIndicator;
			if (view == null)
			{
				view = new NSLevelIndicator();
				view.Identifier = tableColumn.Identifier;
				view.Cell = new EtoCell { MinValue = 0, MaxValue = 1 };
				view.Cell.LevelIndicatorStyle = NSLevelIndicatorStyle.ContinuousCapacity;
			}
			var args = new MacCellFormatArgs(ColumnHandler.Widget, getItem(obj, row), row, view);
			ColumnHandler.DataViewHandler.OnCellFormatting(args);
			return view;
		}

		FontUtility _fontUtility = new FontUtility();

		// The progress cell
		public class EtoCell : NSLevelIndicatorCell, IMacControl
		{
			public EtoCell()
			{
				TextColor = NSColor.ControlText;
			}

			public EtoCell(IntPtr handle) : base(handle)
			{
			}

			public WeakReference WeakHandler { get; set; }

			public ProgressCellHandler Handler
			{
				get { return (ProgressCellHandler)WeakHandler.Target; }
				set { WeakHandler = new WeakReference(value); }
			}

			[Export("backgroundColor")]
			public NSColor BackgroundColor { get; set; }

			[Export("drawsBackground")]
			public bool DrawsBackground { get; set; }

			[Export("textColor")]
			public NSColor TextColor { get; set; }

			public override CGSize CellSizeForBounds(CGRect bounds)
			{
				return new CGSize(50f, 10f);
			}

			public override void DrawWithFrame(CGRect cellFrame, NSView inView)
			{
				if (DrawsBackground && BackgroundColor != null && BackgroundColor.AlphaComponent > 0)
				{
					BackgroundColor.Set();
					NSGraphics.RectFill(cellFrame);
				}

				base.DrawWithFrame(cellFrame, inView);

				var progress = FloatValue;
				if (float.IsNaN((float)progress))
					return;

				string progressText = (int)(progress * 100f) + "%";
				var str = new NSMutableAttributedString(progressText, NSDictionary.FromObjectAndKey(TextColor, NSStringAttributeKey.ForegroundColor));
				var range = new NSRange(0, str.Length);
				if (Font != null)
				{
					str.AddAttributes(NSDictionary.FromObjectAndKey(Font, NSStringAttributeKey.Font), range);
				}
				var h = Handler;
				if (h == null)
					return;


				var size = h._fontUtility.MeasureString(str, cellFrame.Size.ToEto());
				var rect = cellFrame.ToEto();
				var offset = (rect.Size - size) / 2;
				if (!NSGraphicsContext.CurrentContext.IsFlipped)
					offset.Height = -offset.Height;
				rect.Offset(offset);

				str.DrawString(rect.ToNS());
			}
		}
	}
}
