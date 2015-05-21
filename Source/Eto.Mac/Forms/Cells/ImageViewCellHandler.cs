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
	public class ImageViewCellHandler : CellHandler<ImageViewCellHandler.EtoCell, ImageViewCell, ImageViewCell.ICallback>, ImageViewCell.IHandler
	{
		public class EtoCell : NSImageCell, IMacControl
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

			public Color BackgroundColor { get; set; }

			public bool DrawsBackground { get; set; }

			public NSImageInterpolation ImageInterpolation { get; set; }

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
					context.SetFillColor(BackgroundColor.ToCG());
					context.FillRect(cellFrame);
				}

				nscontext.ImageInterpolation = ImageInterpolation;

				base.DrawInteriorWithFrame(cellFrame, inView);
			}
		}

		public override bool Editable
		{
			get { return base.Editable; }
			set { Control.Editable = value; }
		}

		public ImageViewCellHandler()
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
			var c = (EtoCell)cell;
			return c.BackgroundColor;
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
				var img = Widget.Binding.GetValue(dataItem) as Image;
				if (img != null)
				{
					var imgHandler = ((IImageSource)img.Handler);
					return imgHandler.GetImage();
				}
			}
			return new NSImage();
		}

		public override void SetObjectValue(object dataItem, NSObject value)
		{
		}

		public override nfloat GetPreferredSize(object value, CGSize cellSize, NSCell cell)
		{
			var img = value as Image;
			if (img != null)
			{
				return (float)(cellSize.Height / (float)img.Size.Height * (float)img.Size.Width);
			}
			return 16;
		}

		public ImageInterpolation ImageInterpolation
		{
			get { return Control.ImageInterpolation.ToEto(); }
			set { Control.ImageInterpolation = value.ToNS(); }
		}
	}
}

