using SD = System.Drawing;
using Eto.Drawing;
using Eto.WinForms.Drawing;

namespace Eto.WinForms.Forms
{
	public static class Extensions
	{
		public static void AddImage (this System.Windows.Forms.ImageList list, Image image, string key, int? size = null)
		{
			var imageHandler = image.Handler as IWindowsImageSource;
			if (imageHandler != null) {
				list.Images.Add (key, imageHandler.GetImageWithSize (size ?? list.ImageSize.Width));
				return;
			}

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

