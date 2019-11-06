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
	/// <summary>
	/// Handler for <see cref="LinearGradientBrush"/>
	/// </summary>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class LinearGradientBrushHandler : BrushHandler, LinearGradientBrush.IHandler
	{
		class BrushObject
		{
			IMatrix transform;
			GradientWrapMode wrap;
			float lastScale;

			public CGGradient Gradient { get; set; }

			public PointF StartPoint { get; set; }

			public PointF EndPoint { get; set; }

			public CGColor StartColor { get; set; }

			public CGColor EndColor { get; set; }

			public GradientWrapMode Wrap
			{
				get { return wrap; }
				set
				{
					wrap = value;
					Reset();
				}
			}

			public IMatrix Transform
			{
				get { return transform; }
				set
				{
					transform = value;
					if (wrap != GradientWrapMode.Pad)
						Reset();
				}
			}

			void Reset()
			{
				Gradient = null;
			}

			public void Draw(GraphicsHandler graphics, bool stroke, FillMode fillMode, bool clip)
			{
				var start = StartPoint;
				var end = EndPoint;
				var rect = graphics.Control.GetPathBoundingBox().ToEto();
				if (stroke)
					graphics.Control.ReplacePathWithStrokedPath();
				if (clip)
					graphics.Clip(fillMode);

				if (transform != null)
				{
					start = transform.TransformPoint(start);
					end = transform.TransformPoint(end);
				}

				if (wrap == GradientWrapMode.Pad)
				{
					if (Gradient == null)
						Gradient = new CGGradient(CGColorSpace.CreateDeviceRGB(), new [] { StartColor, EndColor }, new nfloat[] { (nfloat)0f, (nfloat)1f });
				}
				else
				{
					var scale = GradientHelper.GetLinearScale(ref start, ref end, rect, lastScale, wrap == GradientWrapMode.Reflect ? 2f : 1f);

					if (Gradient == null || scale > lastScale)
					{
						var stops = GradientHelper.GetGradientStops(StartColor, EndColor, scale, wrap).ToList();
						Gradient = new CGGradient(CGColorSpace.CreateDeviceRGB(), stops.Select(r => r.Item2).ToArray(), stops.Select(r => (nfloat)r.Item1).ToArray());
						lastScale = scale;
					}
				}

				var context = graphics.Control;

				context.DrawLinearGradient(Gradient, start.ToNS(), end.ToNS(), CGGradientDrawingOptions.DrawsAfterEndLocation | CGGradientDrawingOptions.DrawsBeforeStartLocation);
			}
		}

		public object Create(Color startColor, Color endColor, PointF startPoint, PointF endPoint)
		{
			return new BrushObject
			{
				StartColor = startColor.ToCG(),
				EndColor = endColor.ToCG(),
				StartPoint = startPoint,
				EndPoint = endPoint
			};
		}

		public object Create(RectangleF rectangle, Color startColor, Color endColor, float angle)
		{
			return new BrushObject
			{
				StartColor = startColor.ToCG(),
				EndColor = endColor.ToCG(),
				StartPoint = rectangle.TopLeft,
				EndPoint = rectangle.TopRight // TODO
			};
		}

		public IMatrix GetTransform(LinearGradientBrush widget)
		{
			return ((BrushObject)widget.ControlObject).Transform;
		}

		public void SetTransform(LinearGradientBrush widget, IMatrix transform)
		{
			((BrushObject)widget.ControlObject).Transform = transform;
		}

		public GradientWrapMode GetGradientWrap(LinearGradientBrush widget)
		{
			return ((BrushObject)widget.ControlObject).Wrap;
		}

		public void SetGradientWrap(LinearGradientBrush widget, GradientWrapMode gradientWrap)
		{
			((BrushObject)widget.ControlObject).Wrap = gradientWrap;
		}

		public override void Draw(object control, GraphicsHandler graphics, bool stroke, FillMode fillMode, bool clip)
		{
			((BrushObject)control).Draw(graphics, stroke, fillMode, clip);
		}
	}
}

