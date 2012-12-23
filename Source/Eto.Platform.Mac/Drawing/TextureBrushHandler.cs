using System;
using Eto.Drawing;
using MonoMac.CoreGraphics;
using sd = System.Drawing;
using MonoMac.ImageIO;

namespace Eto.Platform.Mac.Drawing
{
	public class TextureBrushHandler : BrushHandler, ITextureBrushHandler
	{
		CGImage cgimage;
		Image image;
		IMatrix transform;
		float [] alpha = new float[] { 1f };
		CGPattern pattern;

		public void Create (Image image)
		{
			this.Image = image;
		}

		void DrawPattern (CGContext context)
		{
			var destRect = new sd.RectangleF(0, 0, cgimage.Width, cgimage.Height);
			context.TranslateCTM (0, cgimage.Height / 2);
			context.ScaleCTM (1f, -1f);
			context.TranslateCTM (0, -cgimage.Height / 2);
			context.DrawImage (destRect, cgimage);
		}

		public override void Apply (GraphicsHandler graphics)
		{
			using (var patternSpace = CGColorSpace.CreatePattern (null)) {
				graphics.Control.SetFillColorSpace (patternSpace);
			}
			graphics.Control.SetFillPattern (pattern, alpha);
		}

		public Image Image
		{
			get { return image; }
			set
			{
				image = value;
				cgimage = null;
				SetPattern ();
			}
		}

		public IMatrix Transform
		{
			get { return transform; }
			set {
				transform = value;
				SetPattern ();
			}
		}

		void SetPattern ()
		{
			if (cgimage == null)
				cgimage = Image.ToCG ();
			var transform = Transform.ToCG ();
			// we want angle and height transforms to be clockwise and top down
			transform.Scale (1f, -1f);

			pattern = new CGPattern(new sd.RectangleF(0, 0, cgimage.Width, cgimage.Height), transform, cgimage.Width, cgimage.Height, CGPatternTiling.ConstantSpacing, true, DrawPattern);
		}

		public void Dispose ()
		{
		}
	}
}

