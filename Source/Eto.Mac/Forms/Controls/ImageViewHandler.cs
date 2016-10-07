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
using CoreImage;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
using MonoMac.CoreImage;
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

namespace Eto.Mac.Forms.Controls
{
	public class ImageViewHandler : MacControl<NSImageView, ImageView, ImageView.ICallback>, ImageView.IHandler
	{
		Image image;

		public class EtoImageView : NSImageView, IMacControl
		{
			public WeakReference WeakHandler { get; set; }

			public object Handler
			{ 
				get { return WeakHandler.Target; }
				set { WeakHandler = new WeakReference(value); } 
			}

			public EtoImageView()
			{
				ImageScaling = NSImageScale.ProportionallyUpOrDown;
			}
		}

		protected override NSImageView CreateControl()
		{
			return new EtoImageView();
		}

		protected override SizeF GetNaturalSize(SizeF availableSize)
		{
			return image == null ? Size.Empty : image.Size;
		}

		public Image Image
		{
			get { return image; }
			set
			{
				if (image != value)
				{
					var oldSize = GetPreferredSize(Size.MaxValue);
					image = value;
					Control.Image = image.ToNS();
					LayoutIfNeeded(oldSize);
				}
			}
		}
	}
}

