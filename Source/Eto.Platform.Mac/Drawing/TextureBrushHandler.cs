using System;
using Eto.Drawing;
using sd = System.Drawing;

#if DESKTOP
using MonoMac.CoreGraphics;
using MonoMac.ImageIO;

namespace Eto.Platform.Mac.Drawing
#else
using MonoTouch.CoreGraphics;
using MonoTouch.ImageIO;

namespace Eto.Platform.iOS.Drawing
#endif
{
	/// <summary>
	/// Handler for <see cref="ITextureBrush"/>
	/// </summary>
	/// <copyright>(c) 2012 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class TextureBrushHandler : BrushHandler, ITextureBrush
	{
		class BrushObject
		{
			CGImage image;
			CGAffineTransform transform = CGAffineTransform.MakeIdentity();
			float [] alpha = new float[] { 1f };
			CGPattern pattern;

			public void Apply (GraphicsHandler graphics)
			{
				using (var patternSpace = CGColorSpace.CreatePattern (null)) {
					graphics.Control.SetFillColorSpace (patternSpace);
				}
				graphics.Control.SetFillPattern (pattern, alpha);
			}
			
			public CGImage Image
			{
				get { return image; }
				set
				{
					image = value;
					SetPattern ();
				}
			}

			public float Opacity
			{
				get { return alpha[0]; }
				set { alpha[0] = value; }
			}
			
			public CGAffineTransform Transform
			{
				get { return transform; }
				set {
					transform = value;					
					SetPattern ();
				}
			}

			void DrawPattern (CGContext context)
			{
				var destRect = new sd.RectangleF(0, 0, image.Width, image.Height);
				context.TranslateCTM (0, image.Height / 2);
				context.ScaleCTM (1f, -1f);
				context.TranslateCTM (0, -image.Height / 2);
				context.DrawImage (destRect, image);
			}

			void SetPattern ()
			{
				var t = transform;
				t.Scale(1f, -1f);
				pattern = new CGPattern(new sd.RectangleF(0, 0, image.Width, image.Height), t, image.Width, image.Height, CGPatternTiling.ConstantSpacing, true, DrawPattern);
			}
		}

		public object Create (Image image, float opacity)
		{
			return new BrushObject {
				Image = image.ToCG (),
				Opacity = opacity
			};
		}

		public IMatrix GetTransform (TextureBrush widget)
		{
			return ((BrushObject)widget.ControlObject).Transform.ToEto ();
		}

		public void SetTransform (TextureBrush widget, IMatrix transform)
		{
			((BrushObject)widget.ControlObject).Transform = transform.ToCG ();
		}

		public override void Apply (object control, GraphicsHandler graphics)
		{
			((BrushObject)control).Apply (graphics);
		}

		public float GetOpacity (TextureBrush widget)
		{
			return ((BrushObject)widget.ControlObject).Opacity;
		}

		public void SetOpacity (TextureBrush widget, float opacity)
		{
			((BrushObject)widget.ControlObject).Opacity = opacity;
		}
	}
}

