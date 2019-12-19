using System;
using Eto.Drawing;
#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
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

namespace Eto.Mac
{
	class ColorizeView : IDisposable
	{
		NSImage _image;
		CGSize? _realSize;

		public Color Color { get; set; }

		static readonly NSString CIOutputImage = new NSString("outputImage");
		static readonly bool supportsConvertSizeToBacking = ObjCExtensions.InstancesRespondToSelector<NSView>("convertSizeToBacking:");

		public static void Create(ref ColorizeView colorize, Color? color)
		{
			if (color == null || color.Value.A <= 0)
			{
				colorize?.Dispose();
				colorize = null;
				return;
			}

			colorize = colorize ?? new ColorizeView();

			colorize.Color = color.Value;
		}

		public void Begin(CGRect frame, NSView controlView)
		{
			var size = controlView.Frame.Size;
			if (size.Width <= 0 || size.Height <= 0)
			{
				_realSize = null;
				return;
			}
			if (_image == null || size != _image.Size)
			{
				if (_image != null)
					_image.Dispose();

				_image = new NSImage(size);
			}

			if (supportsConvertSizeToBacking)
				_realSize = controlView.ConvertSizeToBacking(size);
			else
				_realSize = controlView.ConvertSizeToBase(size);

			_image.LockFocusFlipped(!controlView.IsFlipped);
			NSGraphicsContext.CurrentContext.GraphicsPort.ClearRect(new CGRect(CGPoint.Empty, size));
		}

		public void End()
		{
			if (_image == null || _realSize == null)
				return;

			_image.UnlockFocus();

			var ciImage = CIImage.FromCGImage(_image.CGImage);


#pragma warning disable CS0618 // Image => InputImage in Xamarin.Mac 6.6
			var filter2 = new CIColorControls();
			filter2.SetDefaults();
			filter2.Image = ciImage;
			filter2.Saturation = 0.0f;
			ciImage = (CIImage)filter2.ValueForKey(CIOutputImage);

			var filter3 = new CIColorMatrix();
			filter3.SetDefaults();
			filter3.Image = ciImage;
#pragma warning restore CS0618

			var cgColor = Color.ToCG();
			var components = cgColor.Components;
			if (components.Length >= 3)
			{
				filter3.RVector = new CIVector(0, components[0], 0);
				filter3.GVector = new CIVector(components[1], 0, 0);
				filter3.BVector = new CIVector(0, 0, components[2]);
				filter3.AVector = new CIVector(0, 0, 0, cgColor.Alpha);
			}
			ciImage = (CIImage)filter3.ValueForKey(CIOutputImage);

			// create separate context so we can force using the software renderer, which is more than fast enough for this
			var ciContext = CIContext.FromContext(NSGraphicsContext.CurrentContext.GraphicsPort, new CIContextOptions { UseSoftwareRenderer = true });
			ciContext.DrawImage(ciImage, new CGRect(CGPoint.Empty, _image.Size), new CGRect(CGPoint.Empty, _realSize.Value));

			ciImage.Dispose();
			ciContext.Dispose();
			filter2.Dispose();
			filter3.Dispose();
		}

		public void Dispose()
		{
			_image?.Dispose();
			_image = null;
		}
	}
}

