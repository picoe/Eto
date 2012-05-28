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
		Image image;
		bool sizeSet;
		
		public ImageViewHandler ()
		{
			Control = new SWF.PictureBox {
				BorderStyle = SWF.BorderStyle.None,
				SizeMode = SWF.PictureBoxSizeMode.CenterImage
			};
		}

		public override Size Size
		{
			get
			{
				return base.Size;
			}
			set
			{
				base.Size = value;
				sizeSet = true;
			}
		}

		#region IImageView implementation
		public Image Image {
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
				if (!sizeSet && Control.Image != null)
					Control.Size = Control.Image.Size;
			}
		}
		#endregion
		

	}
}

