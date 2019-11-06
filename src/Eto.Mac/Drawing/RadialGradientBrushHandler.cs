using System;
using Eto.Drawing;
using System.Collections.Generic;
using System.Linq;

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

#if OSX

namespace Eto.Mac.Drawing
#else
using Eto.Mac;
using CoreGraphics;
using ImageIO;

namespace Eto.iOS.Drawing
#endif
{
	public class RadialGradientBrushHandler : BrushHandler, RadialGradientBrush.IHandler
	{
		class BrushObject
		{
			CGAffineTransform transform = CGAffineTransform.MakeIdentity();
			GradientWrapMode wrap;
			float lastScale;

			public CGGradient Gradient { get; set; }

			public PointF Center { get; set; }

			public PointF GradientOrigin { get; set; }

			public SizeF Radius { get; set; }

			public Color StartColor { get; set; }

			public Color EndColor { get; set; }

			public GradientWrapMode Wrap
			{
				get { return wrap; }
				set
				{
					wrap = value;
					Gradient = null;
				}
			}

			public void Apply(GraphicsHandler graphics)
			{
			}

			public CGAffineTransform Transform
			{
				get { return transform; }
				set
				{
					transform = value;
					Gradient = null;
				}
			}

			public void Draw(GraphicsHandler graphics, bool stroke, FillMode fillMode, bool clip)
			{
				var outerRadius = Radius.Width;
				var yscale = Radius.Height / Radius.Width;
				var center = Center;
				var origin = GradientOrigin;
				var scale = 1f;
				var rect = graphics.Control.GetPathBoundingBox().ToEto();
				if (stroke)
					graphics.Control.ReplacePathWithStrokedPath();
				if (clip)
					graphics.Clip(fillMode);

				if (wrap != GradientWrapMode.Pad)
				{
					// use eto's transformrectangle as it'll make the rect encompass the resulting transformed area
					var boundRect = transform.Invert().ToEto().TransformRectangle(rect);

					// find max number of iterations we need to fill the bounding rectangle
					scale = GradientHelper.GetRadialScale(Center, Radius, GradientOrigin, boundRect);
				}

				if (Gradient == null || scale > lastScale)
				{
					var stops = GradientHelper.GetGradientStops(StartColor.ToCG(), EndColor.ToCG(), scale, wrap).ToList();
					lastScale = scale;
					Gradient = new CGGradient(CGColorSpace.CreateDeviceRGB(), stops.Select(r => r.Item2).ToArray(), stops.Select(r => (nfloat)r.Item1).ToArray());
				}
				else
				{
					scale = lastScale;
				}
				
				var scaledRect = new RectangleF(GradientOrigin - (GradientOrigin - Center + Radius) * scale, GradientOrigin + (Center + Radius - GradientOrigin) * scale);
				center = scaledRect.Center;
				outerRadius *= scale;

				// adjust center based on ellipse scale from gradient origin
				center.Y = origin.Y - (origin.Y - center.Y) / yscale;

				// scale to draw elliptical gradient
				var t = new CGAffineTransform(1, 0f, 0f, yscale, 0, origin.Y - origin.Y * yscale);
				t.Multiply(transform);

				graphics.Control.SaveState();
				graphics.Control.ConcatCTM(t);
				graphics.Control.DrawRadialGradient(Gradient, origin.ToNS(), 0, center.ToNS(), outerRadius, CGGradientDrawingOptions.DrawsAfterEndLocation | CGGradientDrawingOptions.DrawsBeforeStartLocation);
				graphics.Control.RestoreState();
			}
		}

		public object Create(Color startColor, Color endColor, PointF center, PointF gradientOrigin, SizeF radius)
		{
			return new BrushObject
			{
				Center = center,
				GradientOrigin = gradientOrigin,
				Radius = radius,
				StartColor = startColor,
				EndColor = endColor
			};
		}

		public IMatrix GetTransform(RadialGradientBrush widget)
		{
			return ((BrushObject)widget.ControlObject).Transform.ToEto();
		}

		public void SetTransform(RadialGradientBrush widget, IMatrix transform)
		{
			((BrushObject)widget.ControlObject).Transform = transform.ToCG();
		}

		public GradientWrapMode GetGradientWrap(RadialGradientBrush widget)
		{
			return ((BrushObject)widget.ControlObject).Wrap;
		}

		public void SetGradientWrap(RadialGradientBrush widget, GradientWrapMode gradientWrap)
		{
			((BrushObject)widget.ControlObject).Wrap = gradientWrap;
		}

		public override void Draw(object control, GraphicsHandler graphics, bool stroke, FillMode fillMode, bool clip)
		{
			((BrushObject)control).Draw(graphics, stroke, fillMode, clip);
		}
	}
}

