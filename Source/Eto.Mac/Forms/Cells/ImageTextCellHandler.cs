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
	public class ImageTextCellHandler : CellHandler<ImageTextCellHandler.EtoCell, ImageTextCell, ImageTextCell.ICallback>, ImageTextCell.IHandler
	{
		public class EtoCell : MacImageListItemCell, IMacControl
		{
			public WeakReference WeakHandler { get; set; }

			public object Handler
			{ 
				get { return WeakHandler.Target; }
				set { WeakHandler = new WeakReference(value); } 
			}

			public NSImageInterpolation ImageInterpolation { get; set; }

			public EtoCell()
			{
			}

			public EtoCell(IntPtr handle) : base(handle)
			{
			}

			[Export("copyWithZone:")]
			NSObject CopyWithZone(IntPtr zone)
			{
				var ptr = Messaging.IntPtr_objc_msgSendSuper_IntPtr(SuperHandle, MacCommon.CopyWithZoneHandle, zone);
				return new EtoCell(ptr) { Handler = Handler };
			}

			public override void DrawInteriorWithFrame(CGRect cellFrame, NSView inView)
			{
				var nscontext = NSGraphicsContext.CurrentContext;
				nscontext.ImageInterpolation = ImageInterpolation;
				base.DrawInteriorWithFrame(cellFrame, inView);
			}
		}

		public ImageTextCellHandler()
		{
			Control = new EtoCell
			{ 
				Handler = this, 
				UsesSingleLineMode = true
			};
		}

		public override void SetBackgroundColor(NSCell cell, Color color)
		{
			var c = (EtoCell)cell;
			c.BackgroundColor = color.ToNSUI();
			c.DrawsBackground = color != Colors.Transparent;
		}

		public override Color GetBackgroundColor(NSCell cell)
		{
			var c = (EtoCell)cell;
			return c.BackgroundColor.ToEto();
		}

		public override void SetForegroundColor(NSCell cell, Color color)
		{
			var c = (EtoCell)cell;
			c.TextColor = color.ToNSUI();
		}

		public override Color GetForegroundColor(NSCell cell)
		{
			var c = (EtoCell)cell;
			return c.TextColor.ToEto();
		}

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

		public override nfloat GetPreferredSize(object value, CGSize cellSize, NSCell cell)
		{
			var val = value as MacImageData;
			if (val == null)
				return 0;
			
			var font = cell.Font ?? NSFont.BoldSystemFontOfSize(NSFont.SystemFontSize);
			var str = val.Text;
			var attrs = NSDictionary.FromObjectAndKey(font, NSAttributedString.FontAttributeName);
			
			var size = str.StringSize(attrs).Width + 4 + 16 + MacImageListItemCell.ImagePadding * 2; // for border + image
			return (float)size;
			
		}

		public ImageInterpolation ImageInterpolation
		{
			get { return Control.ImageInterpolation.ToEto(); }
			set { Control.ImageInterpolation = value.ToNS(); }
		}
	}
}

