using System;
using System.Web;
using SD = System.Drawing;
using SDI = System.Drawing.Imaging;
using Eto.Platform.Web.Forms;
using Eto.Forms;
using Eto.Web;

namespace Eto.Platform.Web
{
	public class ImageSource : Eto.Web.BasePage
	{

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			Response.Clear();

			//SD.Icon icon;
			
			/*
			SD.Bitmap bmp = icon.ToBitmap();
			bmp.Save(this.Response.OutputStream, SDI.ImageFormat.Gif);
			bmp.Dispose();
			 */
		}
	}
}
