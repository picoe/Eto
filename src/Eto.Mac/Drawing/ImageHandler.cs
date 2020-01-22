using Eto.Drawing;
using Eto.Mac.Forms;

using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
using CoreImage;

#if Mac64
using nfloat = System.Double;
using nint = System.Int64;
using nuint = System.UInt64;
#endif

namespace Eto.Mac.Drawing
{
	interface IImageSource
	{
		NSImage GetImage();
	}

	interface IImageHandler : IImageSource
	{
		void DrawImage(GraphicsHandler graphics, float x, float y);

		void DrawImage(GraphicsHandler graphics, float x, float y, float width, float height);

		void DrawImage(GraphicsHandler graphics, RectangleF source, RectangleF destination);
	}

	public abstract class ImageHandler<TControl, TWidget> : WidgetHandler<TControl, TWidget>, Image.IHandler, IImageHandler
		where TControl: class
		where TWidget: Image
	{
		public abstract Size Size { get; }

		public abstract NSImage GetImage();

		public virtual void DrawImage(GraphicsHandler graphics, float x, float y)
		{
			DrawImage(graphics, x, y, Size.Width, Size.Height);
		}

		public virtual void DrawImage(GraphicsHandler graphics, float x, float y, float width, float height)
		{
			DrawImage(graphics, new RectangleF(PointF.Empty, Size), new RectangleF(x, y, width, height));
		}

		public abstract void DrawImage(GraphicsHandler graphics, RectangleF source, RectangleF destination);
	}
}
