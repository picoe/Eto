using Eto.Drawing;
using System;
using sd = SharpDX.Direct2D1;

namespace Eto.Direct2D.Drawing
{
	public class TextureBrushHandler : TextureBrush.IHandler
	{
		public class TextureBrushData : TransformBrushData
		{
			public Image Image { get; set; }

			protected override sd.Brush Create(sd.RenderTarget target)
			{
				var brush = new sd.BitmapBrush(target, Image.ToDx(target));
				brush.ExtendModeX = brush.ExtendModeY = sd.ExtendMode.Wrap;
				if (Transform != null)
					brush.Transform = Transform.ToDx();
				return brush;
			}
		}

		public IMatrix GetTransform(TextureBrush widget)
		{
			var brush = (TextureBrushData)widget.ControlObject;
			return brush.Transform;
		}

		public void SetTransform(TextureBrush widget, IMatrix transform)
		{
			var brush = (TextureBrushData)widget.ControlObject;
			brush.Transform = transform;
		}

		public void SetOpacity(TextureBrush widget, float opacity)
		{
			var brush = (TextureBrushData)widget.ControlObject;
			brush.Alpha = opacity;
		}

		public object Create(Image image, float opacity)
		{
			return new TextureBrushData { Image = image, Alpha = opacity };
		}
	}
}
