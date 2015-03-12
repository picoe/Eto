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
		public class EtoCell : NSTextFieldCell, IMacControl
		{
			public WeakReference WeakHandler { get; set; }

			public ProgressCellHandler Handler
			{ 
				get { return (ProgressCellHandler)WeakHandler.Target; }
				set { WeakHandler = new WeakReference(value); } 
			}

			public EtoCell()
			{
			}

			public EtoCell(IntPtr handle) : base(handle)
			{
			}

			public new Color BackgroundColor { get; set; }

			//public bool DrawsBackground { get; set; }

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
					zone
				);
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

				var graphicsHandler = new GraphicsHandler(null, nscontext, (float)cellFrame.Height, flipped: true);
				using (var graphics = new Graphics(graphicsHandler))
				{
					int value = int.Parse(StringValue);
					RectangleF barRect = new RectangleF((float)cellFrame.X + 2f, (float)cellFrame.Y, (float)cellFrame.Width - 4f, (float)cellFrame.Height - 2f);

					graphics.FillRectangle(Colors.Beige, barRect);
					graphics.FillRectangle(Colors.Green, barRect.X, barRect.Y, (barRect.Width / 100f) * (float)value, barRect.Height);
					graphics.DrawRectangle(Colors.Black, barRect);
					string progressText = value + "%";
					graphics.DrawText(Fonts.Sans(13), Colors.Black, new PointF(barRect.X + (barRect.Width / 2) - (progressText.Length * 3), barRect.Y + (barRect.Height / 2) - 8), progressText);
				}
			}
		}

		public override bool Editable
		{
			get { return base.Editable; }
			set { Control.Editable = value; }
		}

		public ProgressCellHandler()
		{
			Control = new EtoCell { Handler = this, Enabled = true };
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
		}

		public override Color GetForegroundColor(NSCell cell)
		{
			return Colors.Transparent;
		}

		public override NSObject GetObjectValue(object dataItem)
		{
			if (Widget.Binding != null)
			{
				var val = Widget.Binding.GetValue(dataItem);
				return new NSNumber(val);
			}
			return new NSNumber(0);
		}

		public override void SetObjectValue(object dataItem, NSObject value)
		{
			if (Widget.Binding != null)
				Widget.Binding.SetValue(dataItem, ((NSNumber)value).Int32Value);
		}

		public override nfloat GetPreferredSize(object value, CGSize cellSize, NSCell cell)
		{
			return 10f;
		}
	}
}

