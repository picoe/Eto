using Eto.Drawing;
using Eto.Mac.Forms;



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
		where TControl : class
		where TWidget : Image
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


		public virtual void DrawTemplateImage(GraphicsHandler graphics, RectangleF source, RectangleF destination)
		{
			var imageSize = Size;
			// draw as a template image, and ignore color data
			var ctx = graphics.Control;
			ctx.SaveState();

			var image = GetImage();

			if (!image.Flipped)
			{
				// draw flipped
				ctx.ConcatCTM(new CGAffineTransform(1, 0, 0, -1, 0, graphics.ViewHeight));
				destination.Y = graphics.ViewHeight - destination.Y - destination.Height;
			}

			RectangleF destMask;
			if (destination.Size != source.Size)
			{
				// scale and position
				var scale = destination.Size / source.Size;
				destMask = new RectangleF(destination.Location - source.Location * scale, imageSize * scale);
			}
			else
			{
				// just position
				destMask = new RectangleF(destination.Location - source.Location, imageSize);
			}

			var destRect = destination.ToNS();
			var cgImage = image.AsCGImage(ref destRect, graphics.GraphicsContext, null);

			// clip to the image as a mask, using only alpha channel
			ctx.ClipToMask(destMask.ToNS(), cgImage);

			// set fill color based on current dark/light theme	
			// this is the best approximation I can find to get it to draw the same as NSImageView
			// thus far..
			NSColor color;
			if (graphics.DisplayView.HasDarkTheme())
				color = NSColor.FromWhite(1f, .55f);
			else
				color = NSColor.FromWhite(0, .5f);
			color.SetFill();

			ctx.FillRect(destRect);
			ctx.RestoreState();
		}
	}
}
