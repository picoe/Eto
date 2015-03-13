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
	// TO DO: Add native ProgressBar and remove Draw code
	public class ProgressCellHandler : CellHandler<NSTextFieldCell, ProgressCell, ProgressCell.ICallback>, ProgressCell.IHandler
	{
		public ProgressCellHandler()
		{
			Control = new EtoCell { Handler = this, Enabled = true };
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
			c.DrawsBackground = color != Colors.Transparent;
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
		public class EtoCell : NSTextFieldCell, IMacControl
		{
			public EtoCell() : base()
			{
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

			public new Color BackgroundColor { get; set; }

			public Color ForegroundColor { get; set; }

			public override CGSize CellSizeForBounds(CGRect bounds)
			{
				return CGSize.Empty;
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

			public override void DrawInteriorWithFrame(CGRect cellFrame, NSView inView)
			{
				var nscontext = NSGraphicsContext.CurrentContext;

				if (DrawsBackground)
				{
					var context = nscontext.GraphicsPort;
					context.SetFillColor(BackgroundColor.ToCGColor());
					context.FillRect(cellFrame);
				}

				float? progress;
				if (float.IsNaN((float)(progress = FloatValue)))
					return;

				string progressText = (int)(progress * 100f) + "%";

				using (var graphics = new Graphics(new GraphicsHandler(null, nscontext, (float)cellFrame.Height, flipped: true)))
				{
					RectangleF barRect = new RectangleF((float)cellFrame.X + 2f, (float)cellFrame.Y, (float)cellFrame.Width - 4f, (float)cellFrame.Height - 2f);

					if (ForegroundColor == Colors.Transparent)
						ForegroundColor = Colors.Green;

					graphics.FillRectangle(Colors.Beige, barRect);
					graphics.FillRectangle(ForegroundColor, barRect.X, barRect.Y, (barRect.Width / 100f) * (float)(progress * 100f), barRect.Height);
					graphics.DrawRectangle(Colors.Black, barRect);
					float fontSize = Math.Max(13, (float)barRect.Height / 2f);
					graphics.DrawText(Fonts.Sans(fontSize), Colors.Black, new PointF(barRect.MiddleX - (((3f * (float)progressText.Length) / 9.45f) * fontSize), barRect.MiddleY - 1 -(fontSize / 2f)), progressText);
				}
			}
		}
	}
}
