using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;

using aa = Android.App;
using ac = Android.Content;
using ao = Android.OS;
using ar = Android.Runtime;
using av = Android.Views;
using aw = Android.Widget;
using ag = Android.Graphics;
using Eto.Drawing;

namespace Eto.Android.Forms.Controls
{
	public class ImageViewHandler : AndroidControl<aw.ImageView, ImageView, ImageView.ICallback>, ImageView.IHandler
	{
		public override av.View ContainerControl { get { return Control; } }

		public ImageViewHandler()
		{
			Control = new aw.ImageView(aa.Application.Context);
		}

		Image image;
		public Image Image
		{
			get { return image; }
			set
			{
				image = value;
				Control.SetImageBitmap(image.ToAndroid());
			}
		}
	}
}