using System;
#if XAMMAC2
using AppKit;
using CoreGraphics;
using Foundation;
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

namespace Eto.Mac.Forms.Controls
{
    public class MacImageTextView : NSView
    {
        public const int ImagePadding = 2;
        CGSize _imageSize;
        NSTextField _textField;
        NSImage _image;

        public NSTextField TextField => _textField ?? (_textField = CreateTextFieldInternal());

		[Export("image")]
        public NSImage Image
        {
            get => _image;
            set
            {
                _image = value;
                SetSizes(Bounds.Size);
            }
        }

        void SetSizes(CGSize bounds)
        {
            if (_image == null)
            {
                TextField.Frame = new CGRect(CGPoint.Empty, bounds);
                _imageSize = CGSize.Empty;
            }
            else
            {
                var imageSize = _image.Size;
                var scaledHeight = Math.Min(imageSize.Height, bounds.Height);
                var scaledWidth = imageSize.Width * scaledHeight / imageSize.Height;
                _imageSize = new CGSize(scaledWidth, scaledHeight);
                TextField.Frame = new CGRect(scaledWidth + ImagePadding, 0, bounds.Width - scaledWidth - ImagePadding, bounds.Height);
            }
        }

        public override CGSize FittingSize
        {
            get
            {
                var size = TextField.Cell.CellSizeForBounds(new CGRect(0, 0, nfloat.MaxValue, Bounds.Height));
                if (_image != null)
                {
                    size.Width += _imageSize.Width + ImagePadding;
                }
                return size;
            }
        }

        public override void SetFrameSize(CGSize newSize)
        {
            base.SetFrameSize(newSize);
            SetSizes(newSize);
        }

        public MacImageTextView(IntPtr handle) : base(handle)
        {
        }

        public MacImageTextView()
        {
        }

        NSTextField CreateTextFieldInternal()
        {
            var view = CreateTextField();
            view.AutoresizingMask = NSViewResizingMask.WidthSizable | NSViewResizingMask.HeightSizable;
            AddSubview(view);
            return view;
        }

        public NSColor BetterBackgroundColor { get; set; }

		public NSImageInterpolation ImageInterpolation { get; set; }

        public override void DrawRect(CGRect dirtyRect)
		{
            if (BetterBackgroundColor != null)
            {
                BetterBackgroundColor.SetFill();
                NSGraphics.RectFill(dirtyRect);
            }

            if (_image != null)
            {
                var context = NSGraphicsContext.CurrentContext;
                var bounds = Bounds;

                var imageRect = new CGRect(0, bounds.Y, _imageSize.Width, _imageSize.Height);
                imageRect.Y += (bounds.Height - _imageSize.Height) / 2;

                const float alpha = 1; //Enabled ? 1 : (nfloat)0.5;

                context.ImageInterpolation = ImageInterpolation;

                _image.Draw(imageRect, new CGRect(CGPoint.Empty, _image.Size), NSCompositingOperation.SourceOver, alpha, true, null);
            }

            base.DrawRect(dirtyRect);
		}

		protected virtual NSTextField CreateTextField() => new NSTextField();

        protected virtual NSImageView CreateImageView() => new NSImageView();
    }
}

