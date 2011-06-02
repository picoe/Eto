using System;
using SWF = System.Windows.Forms;
using SD = System.Drawing;
using Eto.Forms;
using Eto.Drawing;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Eto.Platform.Windows.Drawing;

namespace Eto.Platform.Windows.Forms
{
	public class ImageViewHandler : WindowsControl<SWF.PictureBox, ImageView>, IImageView
	{
		IImage image;
		
		public ImageViewHandler ()
		{
			Control = new SWF.PictureBox();
			Control.SizeMode = SWF.PictureBoxSizeMode.CenterImage;
		}
	

		#region IImageView implementation
		public IImage Image {
			get {
				return image;
			}
			set {
				image = value;
				var sdimage = image.ControlObject as SD.Image;
				if (sdimage != null) Control.Image = sdimage;
				else
				{
					var icon = image.Handler as IconHandler;
					Control.Image = icon.GetLargestIcon().ToBitmap();
				}
			}
		}
		#endregion
		

	}
}

