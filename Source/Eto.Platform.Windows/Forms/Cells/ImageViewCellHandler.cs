using System;
using swf = System.Windows.Forms;
using sd = System.Drawing;
using Eto.Forms;
using Eto.Drawing;
using Eto.Platform.Windows.Drawing;

namespace Eto.Platform.Windows.Forms.Controls
{
	public class ImageViewCellHandler : CellHandler<swf.DataGridViewImageCell, ImageViewCell>, IImageViewCell
	{
		static sd.Bitmap transparent;
		public ImageViewCellHandler ()
		{
			Control = new swf.DataGridViewImageCell ();
			Control.ImageLayout = swf.DataGridViewImageCellLayout.Zoom;
		}

		static ImageViewCellHandler ()
		{
			transparent = new sd.Bitmap (1, 1);
			using (var g = sd.Graphics.FromImage (transparent)) {
				g.FillRectangle (sd.Brushes.Transparent, 0, 0, 1, 1);
			}
		}

		public override object GetCellValue (object dataItem)
		{
			if (Widget.Binding != null) {
				var image = Widget.Binding.GetValue (dataItem) as Image;
				if (image != null) {
					var imageHandler = image.Handler as IWindowsImage;
					if (imageHandler != null) {
						return imageHandler.GetImageWithSize (Math.Max (32, this.Control.PreferredSize.Height));
					}
				}
			}
			return transparent;
		}

		public override void SetCellValue (object dataItem, object value)
		{
			if (Widget.Binding != null) {
				Widget.Binding.SetValue (dataItem, value);
			}
		}
	}
}

