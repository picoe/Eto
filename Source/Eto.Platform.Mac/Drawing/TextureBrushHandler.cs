using Eto.Drawing;
using sd = System.Drawing;

#if DESKTOP
using MonoMac.CoreGraphics;

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
			readonly float [] alpha = { 1f };
			CGPattern pattern;
			static readonly CGColorSpace patternColorSpace = CGColorSpace.CreatePattern (null);

			public void Apply (GraphicsHandler graphics, float x, float y)
			{
				graphics.Control.SetFillColorSpace (patternColorSpace);
				graphics.Control.SetPatternPhase(new sd.SizeF(x, y));
				if (pattern == null)
				{
					ClearPattern();
					pattern = new CGPattern(new sd.RectangleF(0, 0, image.Width, image.Height), transform, image.Width, image.Height, CGPatternTiling.ConstantSpacing, true, DrawPattern);					
				}
				graphics.Control.SetFillPattern (pattern, alpha);
			}
			
			public CGImage Image
			{
				get { return image; }
				set
				{
					image = value;
					ClearPattern();
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
					ClearPattern();
				}
			}

			void ClearPattern()
			{
				if (pattern != null)
					pattern.Dispose();
					pattern = null;
				}

			void DrawPattern (CGContext context)
			{
				var destRect = new sd.RectangleF(0, 0, image.Width, image.Height);
				context.ConcatCTM (new CGAffineTransform (1, 0, 0, -1, 0, image.Height));
				context.DrawImage (destRect, image);
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

		public override void Apply(object control, GraphicsHandler graphics, float x, float y)
		{
			((BrushObject)control).Apply (graphics, x, y);
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

