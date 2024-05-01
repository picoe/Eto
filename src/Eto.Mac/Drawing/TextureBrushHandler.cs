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
				if (stroke)
					graphics.Control.ReplacePathWithStrokedPath();
				if (clip)
					graphics.Clip(fillMode);

				var context = graphics.Control;

				context.SaveState();
				context.ConcatCTM(transform);
				context.ConcatCTM(new CGAffineTransform(1, 0, 0, -1, 0, (nfloat)Image.Height));
				//transform.ToEto().TransformRectangle(rect);

				if (Opacity < 1f)
				{
					context.SetBlendMode(CGBlendMode.Normal);
					context.SetAlpha(Opacity);
				}
				context.DrawTiledImage(new CGRect(0, 0, Image.Width, Image.Height), Image);
				context.RestoreState();
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

