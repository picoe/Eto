using Eto.Drawing;
using swm = System.Windows.Media;

namespace Eto.Wpf.Drawing
{
	/// <summary>
	/// Handler for <see cref="ITextureBrush"/>
	/// </summary>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class TextureBrushHandler : TextureBrush.IHandler
	{
		public IMatrix GetTransform (TextureBrush widget)
		{
			return ((swm.ImageBrush)widget.ControlObject).Transform.ToEtoMatrix ();
		}

		public void SetTransform (TextureBrush widget, IMatrix transform)
		{
			((swm.ImageBrush)widget.ControlObject).Transform = transform.ToWpfTransform ();
		}

		public object Create (Image image, float opacity)
		{
			var rect = new System.Windows.Rect (0, 0, image.Size.Width, image.Size.Height);
			return new swm.ImageBrush (image.ToWpf ()) {
				TileMode = swm.TileMode.Tile,
				Opacity = opacity,
				Stretch = swm.Stretch.None,
				ViewboxUnits = swm.BrushMappingMode.Absolute,
				Viewbox = rect,
				ViewportUnits = swm.BrushMappingMode.Absolute,
				Viewport = rect
			};
		}


		public void SetOpacity (TextureBrush widget, float opacity)
		{
			((swm.ImageBrush)widget.ControlObject).Opacity = opacity;
		}
	}
}
