namespace Eto.GtkSharp.Drawing
{
	/// <summary>
	/// Handler for the <see cref="TextureBrush"/>
	/// </summary>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class TextureBrushHandler : BrushHandler, TextureBrush.IHandler
	{
		class TextureBrushObject
		{
			public Cairo.Matrix Transform { get; set; }
			public Cairo.Matrix InverseTransform { get; set; }
			public Image Image { get; set; }
			Bitmap opacityImage;
			float opacity = 1f;
			public float Opacity
			{
				get { return opacity; }
				set
				{
					opacity = value;
					opacityImage = null;
				}
			}

			public void Apply(Cairo.Context context)
			{
				if (!ReferenceEquals(Transform, null))
					context.Transform(Transform);

				if (opacityImage == null)
				{
					opacityImage = new Bitmap(Image.Size, PixelFormat.Format32bppRgba);
					using (var g = new Graphics(opacityImage))
					{
						g.DrawImage(Image, 0, 0);
					}
					using (var bd = opacityImage.Lock())
					{
						for (int x = 0; x < opacityImage.Width; x++)
							for (int y = 0; y < opacityImage.Height; y++)
							{
								var c = bd.GetPixel(x, y);
								bd.SetPixel(x, y, new Color(c, c.A * Opacity));
							}
					}
				}
				var img = opacityImage ?? Image;
				if (img.Handler is BitmapHandler bitmapHandler)
				{
					// reuse the bitmap's surface, as converting the image for every fill is slow
					var surface = bitmapHandler.GetCairoSurface();
					if (!ReferenceEquals(surface, patternSurface))
					{
						pattern?.Dispose();
						pattern = new Cairo.SurfacePattern(surface) { Extend = Cairo.Extend.Repeat };
						patternSurface = surface;
					}
					context.SetSource(pattern);
				}
				else
				{
					Gdk.CairoHelper.SetSourcePixbuf(context, img.ToGdk(), 0, 0);
					using (var source = context.GetSource())
					{
						if (source is Cairo.SurfacePattern surfacePattern)
							surfacePattern.Extend = Cairo.Extend.Repeat;
					}
				}
				if (!ReferenceEquals(InverseTransform, null))
					context.Transform(InverseTransform);
			}

			Cairo.SurfacePattern pattern;
			Cairo.Surface patternSurface;
		}

		public IMatrix GetTransform(TextureBrush widget)
		{
			return ((TextureBrushObject)widget.ControlObject).Transform.ToEto();
		}

		public void SetTransform(TextureBrush widget, IMatrix transform)
		{
			((TextureBrushObject)widget.ControlObject).Transform = transform.ToCairo();
			((TextureBrushObject)widget.ControlObject).InverseTransform = transform.Inverse().ToCairo();
		}

		public object Create(Image image, float opacity)
		{
			return new TextureBrushObject
			{
				Image = image,
				Opacity = opacity
			};
		}

		public override void Apply(object control, Cairo.Context context)
		{
			((TextureBrushObject)control).Apply(context);
		}

		public float GetOpacity(TextureBrush widget)
		{
			return ((TextureBrushObject)widget.ControlObject).Opacity;
		}

		public void SetOpacity(TextureBrush widget, float opacity)
		{
			((TextureBrushObject)widget.ControlObject).Opacity = opacity;
		}
	}
}

