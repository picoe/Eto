using Eto.Drawing;
using sd = System.Drawing;
using sd2 = System.Drawing.Drawing2D;
using sdi = System.Drawing.Imaging;

namespace Eto.WinForms.Drawing
{
	/// <summary>
	/// Handler for <see cref="TextureBrush"/>
	/// </summary>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class TextureBrushHandler : BrushHandler, TextureBrush.IHandler
	{
		public object Create(Image image, float opacity)
		{
			var sdimage = new sd.Bitmap(image.ToSD());
			var att = new sdi.ImageAttributes();
			att.SetWrapMode(sd2.WrapMode.Tile);
			if (opacity < 1.0f)
			{
				var colorMatrix = new sdi.ColorMatrix(new float[][] {
										  new float [] { 1.0f, 0.0f, 0.0f, 0.0f, 0.0f },
										  new float [] { 0.0f, 1.0f, 0.0f, 0.0f, 0.0f },
										  new float [] { 0.0f, 0.0f, 1.0f, 0.0f, 0.0f },
										  new float [] { 0.0f, 0.0f, 0.0f, opacity, 0.0f },
										  new float [] { 0.0f, 0.0f, 0.0f, 0.0f, 1.0f }
									  });
				att.SetColorMatrix(colorMatrix);
			}
			return new sd.TextureBrush(sdimage, new sd.RectangleF(0, 0, sdimage.Width, sdimage.Height), att);
		}

		public IMatrix GetTransform(TextureBrush widget)
		{
			return ((sd.TextureBrush)widget.ControlObject).Transform.ToEto();
		}

		public void SetTransform(TextureBrush widget, IMatrix transform)
		{
			((sd.TextureBrush)widget.ControlObject).Transform = transform.ToSD();
		}

		public void SetOpacity(TextureBrush widget, float opacity)
		{
			var brush = ((sd.TextureBrush)widget.ControlObject);
			widget.ControlObject = Create(widget.Image, opacity);
			var newbrush = ((sd.TextureBrush)widget.ControlObject);
			newbrush.Transform = brush.Transform;
		}

		public override sd.Brush GetBrush(Brush brush, RectangleF rect)
		{
			return (sd.Brush)brush.ControlObject;
		}
	}
}
