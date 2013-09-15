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
			CGAffineTransform? viewTransform;
			float [] alpha = new float[] { 1f };
			CGPattern pattern;
			static CGColorSpace patternColorSpace = CGColorSpace.CreatePattern (null);

			public void Apply (GraphicsHandler graphics)
			{
				graphics.Control.SetFillColorSpace (patternColorSpace);

				// make current transform apply to the pattern
				var currentTransform = graphics.Control.GetCTM();

				if (graphics.DisplayView != null)
				{
					var pos = graphics.DisplayView.ConvertPointToView (sd.PointF.Empty, null);
					currentTransform.Translate(pos.X, pos.Y);
					graphics.Control.SetPatternPhase(new sd.SizeF(-pos.X, -pos.Y));
				}
				if (pattern == null || viewTransform != currentTransform) {
					viewTransform = currentTransform;
					SetPattern ();
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

			private void ClearPattern()
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

			void SetPattern ()
			{
				var t = this.transform;
				if (viewTransform != null)
					t.Multiply (viewTransform.Value);
				ClearPattern();
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

