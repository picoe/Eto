using Eto.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using swm = System.Windows.Media;

namespace Eto.Platform.Wpf.Drawing
{
	public class TextureBrushHandler : ITextureBrushHandler
	{
		Image image;
		swm.ImageBrush Control { get; set; }

		public void Create (Image image)
		{
			Control = new swm.ImageBrush(image.ToWpf());
			Control.TileMode = swm.TileMode.Tile;
			Control.Stretch = swm.Stretch.None;
			Control.ViewboxUnits = swm.BrushMappingMode.Absolute;
			Control.Viewbox = new System.Windows.Rect (0, 0, image.Size.Width, image.Size.Height);
			Control.ViewportUnits = swm.BrushMappingMode.Absolute;
			Control.Viewport = Control.Viewbox;
			this.image = image;
		}

		public Image Image
		{
			get { return image; }
		}

		public IMatrix Transform
		{
			get { return Control.Transform.ToEtoMatrix (); }
			set { Control.Transform = value.ToWpfTransform (); }
		}

		public object ControlObject
		{
			get { return Control; }
		}

		public void Dispose ()
		{
		}
	}
}
