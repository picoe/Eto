using System;
using swf = System.Windows.Forms;
using sd = System.Drawing;
using Eto.Forms;
using Eto.Drawing;
using Eto.Platform.Windows.Drawing;

namespace Eto.Platform.Windows.Forms.Controls
{
	public class ImageCellHandler : CellHandler<swf.DataGridViewImageCell, ImageCell>, IImageCell
	{
		static sd.Bitmap transparent;
		public ImageCellHandler ()
		{
			Control = new swf.DataGridViewImageCell ();
			Control.ImageLayout = swf.DataGridViewImageCellLayout.Zoom;
		}

		static ImageCellHandler ()
		{
			transparent = new sd.Bitmap (1, 1);
			using (var g = sd.Graphics.FromImage (transparent)) {
				g.FillRectangle (sd.Brushes.Transparent, 0, 0, 1, 1);
			}
		}

		public override object GetCellValue (object itemValue)
		{
			var image = itemValue as Image;
			if (image != null) {
				var imageHandler = image.Handler as IWindowsImage;
				if (imageHandler != null) {
					return imageHandler.GetImageWithSize (Math.Max(32, this.Control.PreferredSize.Height));
				}
			}
			return transparent;
		}

		public override object GetItemValue (object cellValue)
		{
			return null;
		}
	}
}

