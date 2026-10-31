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
			CGAffineTransform transform = CGAffineTransform.MakeIdentity();

			public CGImage Image { get; set; }

			public float Opacity { get; set; }

			public BrushObject()
			{
				Opacity = 1;
			}

			public CGAffineTransform Transform
			{
				get { return transform; }
				set { transform = value; }
			}

			public void Draw(GraphicsHandler graphics, bool stroke, FillMode fillMode, bool clip)
			{
				var context = graphics.Control;
				if (stroke)
					context.ReplacePathWithStrokedPath();

				var tileRect = new CGRect(0, 0, Image.Width, Image.Height);
				var flip = new CGAffineTransform(1, 0, 0, -1, 0, (nfloat)Image.Height);
				var coverage = clip ? GetCoverage(context, tileRect) : TileCoverage.Multiple;
				var imageRect = tileRect;
				if (coverage != TileCoverage.Multiple)
				{
					var imageToDevice = CGAffineTransform.Multiply(CGAffineTransform.Multiply(flip, transform), context.GetUserSpaceToDeviceSpaceTransform());
					imageRect = SnapToPixels(imageToDevice, tileRect);
					// snapping moves the image off the shape's edges, so it needs the clip again
					if (coverage == TileCoverage.Exact && !IsClose(imageRect, tileRect, 0.001f))
						coverage = TileCoverage.Single;
				}

				if (coverage == TileCoverage.Exact)
					context.BeginPath(); // the image is the shape, so no clip is needed
				else if (clip)
					graphics.Clip(fillMode);

				context.SaveState();
				context.ConcatCTM(transform);
				context.ConcatCTM(flip);

				if (Opacity < 1f)
				{
					context.SetBlendMode(CGBlendMode.Normal);
					context.SetAlpha(Opacity);
				}
				// Core Animation draws a plain image on the GPU, but rebuilds a tiled image's pattern on the CPU for every fill
				if (coverage == TileCoverage.Multiple)
					context.DrawTiledImage(tileRect, Image);
				else
					context.DrawImage(imageRect, Image);
				context.RestoreState();
			}

			// DrawTiledImage aligns unrotated tiles to device pixels, so do the same to keep edges crisp
			static CGRect SnapToPixels(CGAffineTransform toDevice, CGRect rect)
			{
				var origin = toDevice.TransformPoint(rect.Location);
				var right = toDevice.TransformPoint(new CGPoint(rect.Right, rect.Y));
				var bottom = toDevice.TransformPoint(new CGPoint(rect.X, rect.Bottom));
				const float tolerance = 0.001f;
				if (Math.Abs(right.Y - origin.Y) > tolerance || Math.Abs(bottom.X - origin.X) > tolerance)
					return rect;

				var snapped = new CGPoint(Math.Floor(origin.X + 0.5), Math.Ceiling(origin.Y - 0.5));
				rect.Location = toDevice.Invert().TransformPoint(snapped);
				return rect;
			}

			enum TileCoverage
			{
				Multiple,
				Single,
				Exact
			}

			TileCoverage GetCoverage(CGContext context, CGRect tileRect)
			{
				if (context.IsPathEmpty())
					return TileCoverage.Multiple;

				// the context's own bounding box is measured in device space, which grows when rotated
				using (var path = context.CopyPath())
				{
					if (path == null)
						return TileCoverage.Multiple;

					const float tolerance = 0.01f;
					var pathBounds = path.PathBoundingBox;
					var bounds = transform.Invert().TransformRect(pathBounds);
					if (bounds.X < -tolerance || bounds.Y < -tolerance || bounds.Right > tileRect.Width + tolerance || bounds.Bottom > tileRect.Height + tolerance)
						return TileCoverage.Multiple;

					// the image fills the shape exactly when the shape is a rectangle that the tile maps onto
					if (bounds.Width < tileRect.Width - tolerance || bounds.Height < tileRect.Height - tolerance)
						return TileCoverage.Single;
					if (!IsClose(transform.TransformRect(tileRect), pathBounds, tolerance) || !path.IsRect(out _))
						return TileCoverage.Single;
					return TileCoverage.Exact;
				}
			}

			static bool IsClose(CGRect a, CGRect b, float tolerance)
			{
				return Math.Abs(a.X - b.X) <= tolerance
					&& Math.Abs(a.Y - b.Y) <= tolerance
					&& Math.Abs(a.Right - b.Right) <= tolerance
					&& Math.Abs(a.Bottom - b.Bottom) <= tolerance;
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

		public float GetOpacity(TextureBrush widget)
		{
			return ((BrushObject)widget.ControlObject).Opacity;
		}

		public void SetOpacity(TextureBrush widget, float opacity)
		{
			((BrushObject)widget.ControlObject).Opacity = opacity;
		}

		public override void Draw(object control, GraphicsHandler graphics, bool stroke, FillMode fillMode, bool clip)
		{
			((BrushObject)control).Draw(graphics, stroke, fillMode, clip);
		}
	}
}

