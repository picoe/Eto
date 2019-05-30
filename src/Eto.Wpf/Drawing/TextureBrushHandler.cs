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
		static swm.ImageBrush Get(TextureBrush widget) => ((FrozenBrushWrapper)widget.ControlObject).Brush as swm.ImageBrush;

		static void SetFrozen(TextureBrush widget) => ((FrozenBrushWrapper)widget.ControlObject).SetFrozen();

		public IMatrix GetTransform(TextureBrush widget)
		{
			return Get(widget).Transform.ToEtoMatrix();
		}

		public void SetTransform(TextureBrush widget, IMatrix transform)
		{
			Get(widget).Transform = transform.ToWpfTransform();
			SetFrozen(widget);
		}

		public object Create(Image image, float opacity)
		{
			var rect = new System.Windows.Rect(0, 0, image.Size.Width, image.Size.Height);
			return new FrozenBrushWrapper(new swm.ImageBrush(image.ToWpf())
			{
				TileMode = swm.TileMode.Tile,
				Opacity = opacity,
				Stretch = swm.Stretch.None,
				ViewboxUnits = swm.BrushMappingMode.Absolute,
				Viewbox = rect,
				ViewportUnits = swm.BrushMappingMode.Absolute,
				Viewport = rect
			});
		}


		public void SetOpacity(TextureBrush widget, float opacity)
		{
			Get(widget).Opacity = opacity;
			SetFrozen(widget);
		}
	}
}
