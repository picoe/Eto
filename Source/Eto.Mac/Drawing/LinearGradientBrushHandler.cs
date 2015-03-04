using System;
using Eto.Drawing;

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
using Eto.Mac;
using CoreGraphics;
using ImageIO;

namespace Eto.iOS.Drawing
#endif
{
	/// <summary>
	/// Handler for <see cref="LinearGradientBrush"/>
	/// </summary>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class LinearGradientBrushHandler : BrushHandler, LinearGradientBrush.IHandler
	{
		class BrushObject
		{
			CGAffineTransform transform = CGAffineTransform.MakeIdentity();
			CGAffineTransform viewTransform = CGAffineTransform.MakeIdentity();
			readonly nfloat[] alpha = { 1 };
			CGPattern pattern;
			GradientWrapMode wrap;
			CGSize tileSize;
			CGSize sectionSize;

			public CGGradient InverseGradient { get; set; }

			public CGGradient Gradient { get; set; }

			public CGPoint StartPoint { get; set; }

			public CGPoint EndPoint { get; set; }

			public GradientWrapMode Wrap
			{
				get { return wrap; }
				set
				{
					wrap = value;
					pattern = null;
				}
			}

			public void Apply(GraphicsHandler graphics)
			{
				graphics.SetFillColorSpace();

				#if OSX
				graphics.SetPhase();
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
					pattern = null;
				}
			}

			void DrawPattern(CGContext context)
			{
				var start = new CGPoint(0, 0);
				var end = start + sectionSize;

				context.ClipToRect(new CGRect(start, tileSize).Inset(-4, -4));

				if (Wrap == GradientWrapMode.Reflect)
				{
					for (int i = 0; i < 2; i++)
					{
						context.DrawLinearGradient(Gradient, start, end, 0);
						context.DrawLinearGradient(InverseGradient, end, end + sectionSize, 0);
						start = start + sectionSize + sectionSize;
						end = end + sectionSize + sectionSize;
					}
				}
				else
				{
					for (int i = 0; i < 2; i++)
					{
						context.DrawLinearGradient(Gradient, start, end, 0);
						start = start + sectionSize;
						end = end + sectionSize;
					}
				}
			}

			void SetPattern()
			{
				sectionSize = new CGSize((EndPoint.X - StartPoint.X) + 1, (EndPoint.Y - StartPoint.Y) + 1);
				if (Wrap == GradientWrapMode.Reflect)
					tileSize = new CGSize(sectionSize.Width * 4, sectionSize.Height * 4);
				else
					tileSize = new CGSize(sectionSize.Width * 2, sectionSize.Height * 2);
				var rect = new CGRect(StartPoint, tileSize);
				var t = CGAffineTransform.Multiply(transform, viewTransform);
				pattern = new CGPattern(rect, t, rect.Width, rect.Height, CGPatternTiling.NoDistortion, true, DrawPattern);
			}
		}

		public object Create(Color startColor, Color endColor, PointF startPoint, PointF endPoint)
		{
			return new BrushObject
			{
				Gradient = new CGGradient(CGColorSpace.CreateDeviceRGB(), new CGColor []
				{
					startColor.ToCGColor(),
					endColor.ToCGColor()
				}),
				InverseGradient = new CGGradient(CGColorSpace.CreateDeviceRGB(), new CGColor []
				{
					endColor.ToCGColor(),
					startColor.ToCGColor()
				}),
				StartPoint = startPoint.ToNS(),
				EndPoint = endPoint.ToNS()
			};
		}

		public object Create(RectangleF rectangle, Color startColor, Color endColor, float angle)
		{
			return null;
		}

		public IMatrix GetTransform(LinearGradientBrush widget)
		{
			return ((BrushObject)widget.ControlObject).Transform.ToEto();
		}

		public void SetTransform(LinearGradientBrush widget, IMatrix transform)
		{
			((BrushObject)widget.ControlObject).Transform = transform.ToCG();
		}

		public GradientWrapMode GetGradientWrap(LinearGradientBrush widget)
		{
			return ((BrushObject)widget.ControlObject).Wrap;
		}

		public void SetGradientWrap(LinearGradientBrush widget, GradientWrapMode gradientWrap)
		{
			((BrushObject)widget.ControlObject).Wrap = gradientWrap;
		}

		public override void Apply(object control, GraphicsHandler graphics)
		{
			((BrushObject)control).Apply(graphics);
		}
	}
}

