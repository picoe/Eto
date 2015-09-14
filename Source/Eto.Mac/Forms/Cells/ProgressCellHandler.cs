using System;
using Eto.Forms;
using Eto.Drawing;
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

namespace Eto.Mac.Forms.Cells
{
	public class ProgressCellHandler : CellHandler<ProgressCellHandler.EtoCell, ProgressCell, ProgressCell.ICallback>, ProgressCell.IHandler
	{
		public ProgressCellHandler()
		{
			Control = new EtoCell { Handler = this, Enabled = true, LevelIndicatorStyle = NSLevelIndicatorStyle.ContinuousCapacity, MinValue = 0, MaxValue = 1 };
		}

		public override bool Editable
		{
			get { return base.Editable; }
			set { Control.Editable = value; }
		}

		public override void SetBackgroundColor(NSCell cell, Color color)
		{
			var c = (EtoCell)cell;
			c.BackgroundColor = color;
		}

		public override Color GetBackgroundColor(NSCell cell)
		{
			return ((EtoCell)cell).BackgroundColor;
		}

		public override void SetForegroundColor(NSCell cell, Color color)
		{
			((EtoCell)cell).ForegroundColor = color;
		}

		public override Color GetForegroundColor(NSCell cell)
		{
			return ((EtoCell)cell).ForegroundColor;
		}

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

		public override nfloat GetPreferredSize(object value, CGSize cellSize, NSCell cell)
		{
			return 30f;
		}


		// The progress cell
		public class EtoCell : NSLevelIndicatorCell, IMacControl
		{
			public EtoCell()
			{
				ForegroundColor = Colors.Black;
				BackgroundColor = Colors.White;
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

			public Color BackgroundColor { get; set; }

			public Color ForegroundColor { get; set; }

			public override CGSize CellSizeForBounds(CGRect bounds)
			{
				return new CGSize(50f, 10f);
			}

			[Export("copyWithZone:")]
			NSObject CopyWithZone(IntPtr zone)
			{
				var ptr = Messaging.IntPtr_objc_msgSendSuper_IntPtr(
					SuperHandle,
					MacCommon.CopyWithZoneHandle,
					zone);

				return new EtoCell(ptr) { Handler = Handler };
			}

			public override void DrawWithFrame(CGRect cellFrame, NSView inView)
			{
				var progress = FloatValue;
				if (float.IsNaN((float)progress))
					return;

				base.DrawWithFrame(cellFrame, inView);

				string progressText = (int)(progress * 100f) + "%";
				var str = new NSMutableAttributedString(progressText, NSDictionary.FromObjectAndKey(ForegroundColor.ToNSUI(), NSStringAttributeKey.ForegroundColor));
				var range = new NSRange(0, str.Length);
				if (Font != null)
				{
					str.AddAttributes(NSDictionary.FromObjectAndKey(Font, NSStringAttributeKey.Font), range);
				}
				var size = FontExtensions.MeasureString(str, cellFrame.Size.ToEto());
				var rect = cellFrame.ToEto();
				rect.Offset((rect.Size - size) / 2);

				str.DrawString(rect.ToNS());
			}
		}
	}
}
