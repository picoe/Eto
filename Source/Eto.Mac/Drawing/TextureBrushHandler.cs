using System;
using Eto.Drawing;
using sd = System.Drawing;

#if XAMMAC2 
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
using CoreImage;
#elif OSX
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
using MonoMac.CoreImage;
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

#if OSX

namespace Eto.Mac.Drawing
#else
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
using CoreImage;
using ImageIO;

namespace Eto.iOS.Drawing
#endif
{
	/// <summary>
	/// Handler for <see cref="TextureBrush"/>
	/// </summary>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class TextureBrushHandler : BrushHandler, TextureBrush.IHandler
	{
		class BrushObject
		{
			CGImage image;
			CGAffineTransform transform = CGAffineTransform.MakeIdentity();
			CGAffineTransform viewTransform = CGAffineTransform.MakeIdentity();
			readonly nfloat[] alpha = { 1 };
			CGPattern pattern;

			public void Apply(GraphicsHandler graphics)
			{
				graphics.SetFillColorSpace();

				#if OSX
				if (graphics.DisplayView != null)
				{
					// adjust for position of the current view relative to the window
					var pos = graphics.DisplayView.ConvertPointToView(CGPoint.Empty, null);
					graphics.Control.SetPatternPhase(new CGSize(pos.X, pos.Y));
				}
				#endif

				// make current transform apply to the pattern
				var currentTransform = graphics.CurrentTransform;
				if (pattern == null || viewTransform != currentTransform)
				{
					viewTransform = currentTransform;
					SetPattern();
				}

				graphics.Control.SetFillPattern(pattern, alpha);
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
				get { return (float)alpha[0]; }
				set { alpha[0] = value; }
			}

			public CGAffineTransform Transform
			{
				get { return transform; }
				set
				{
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

			void DrawPattern(CGContext context)
			{
				var destRect = new CGRect(0, 0, image.Width, image.Height);
				context.ConcatCTM(new CGAffineTransform(1, 0, 0, -1, 0, image.Height));
				context.DrawImage(destRect, image);
			}

			void SetPattern()
			{
				var t = CGAffineTransform.Multiply(transform, viewTransform);
				ClearPattern();
				pattern = new CGPattern(new CGRect(0, 0, image.Width, image.Height), t, image.Width, image.Height, CGPatternTiling.NoDistortion, true, DrawPattern);
			}
		}

		public object Create(Image image, float opacity)
		{
			return new BrushObject
			{
				Image = image.ToCG(),
				Opacity = opacity
			};
		}

		public IMatrix GetTransform(TextureBrush widget)
		{
			return ((BrushObject)widget.ControlObject).Transform.ToEto();
		}

		public void SetTransform(TextureBrush widget, IMatrix transform)
		{
			((BrushObject)widget.ControlObject).Transform = transform.ToCG();
		}

		public override void Apply(object control, GraphicsHandler graphics)
		{
			((BrushObject)control).Apply(graphics);
		}

		public float GetOpacity(TextureBrush widget)
		{
			return ((BrushObject)widget.ControlObject).Opacity;
		}

		public void SetOpacity(TextureBrush widget, float opacity)
		{
			((BrushObject)widget.ControlObject).Opacity = opacity;
		}
	}
}

