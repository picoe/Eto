using System;
using SD = System.Drawing;
using Eto.Drawing;

namespace Eto.Platform.Windows.Forms
{
	public static class Extensions
	{
		public static void AddImage (this System.Windows.Forms.ImageList list, Image image, string key)
		{
			var sdimage = image.ControlObject as SD.Image;
			if (sdimage != null) {
				list.Images.Add (key, sdimage);
				return;
			}
			var icon = image.ControlObject as SD.Icon;
			if (icon != null) {
				list.Images.Add (key, icon);
				return;
			}
		}
	}
}

