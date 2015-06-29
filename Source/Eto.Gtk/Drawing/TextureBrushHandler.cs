using Eto.Drawing;

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
			public Gdk.Pixbuf Pixbuf { get; set; }
			public float Opacity { get; set; }

			public TextureBrushObject ()
			{
				Opacity = 1.0f;
			}

			public void Apply (GraphicsHandler graphics)
			{
				if (!object.ReferenceEquals (Transform, null))
					graphics.Control.Transform (Transform);
				
				Gdk.CairoHelper.SetSourcePixbuf (graphics.Control, Pixbuf, 0, 0);
				var surfacePattern = graphics.Control.GetSource() as Cairo.SurfacePattern;
				if (surfacePattern != null)
					surfacePattern.Extend = Cairo.Extend.Repeat;
				if (Opacity < 1.0f)
				{
					graphics.Control.Clip();
					graphics.Control.PaintWithAlpha(Opacity);
				}
				else
					graphics.Control.Fill();
				if (EtoEnvironment.Platform.IsMac && surfacePattern != null)
					surfacePattern.Dispose();
			}
		}

		public IMatrix GetTransform (TextureBrush widget)
		{
			return ((TextureBrushObject)widget.ControlObject).Transform.ToEto ();
		}

		public void SetTransform (TextureBrush widget, IMatrix transform)
		{
			((TextureBrushObject)widget.ControlObject).Transform = transform.ToCairo ();
		}

		public object Create (Image image, float opacity)
		{
			return new TextureBrushObject {
				Pixbuf = image.ToGdk (),
				Opacity = opacity
			};
		}

		public override void Apply (object control, GraphicsHandler graphics)
		{
			((TextureBrushObject)control).Apply (graphics);
		}

		public float GetOpacity (TextureBrush widget)
		{
			return ((TextureBrushObject)widget.ControlObject).Opacity;
		}

		public void SetOpacity (TextureBrush widget, float opacity)
		{
			((TextureBrushObject)widget.ControlObject).Opacity = opacity;
		}
	}
}

