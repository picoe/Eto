using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using swc = System.Windows.Controls;
using swm = System.Windows.Media;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Platform.Wpf.Forms.Controls
{
	public class ImageViewHandler : WpfFrameworkElement<swc.Image, ImageView>, IImageView
	{
		IImage image;

		public override Color BackgroundColor
		{
			get
			{
				return Color.Transparent;
			}
			set
			{
				
			}
		}


		public ImageViewHandler ()
		{
			Control = new swc.Image ();
		}

		public IImage Image
		{
			get { return image; }
			set
			{
				image = value;
				if (image != null)
					Control.Source = image.ControlObject as swm.ImageSource;
				else
					Control.Source = null;
			}
		}
	}
}
