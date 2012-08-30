using System;
using System.IO;
using Eto.Drawing;
using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace Eto.Platform.iOS.Drawing
{
	public class IconHandler : WidgetHandler<UIImage, Icon>, IIcon
	{
		public IconHandler()
		{
		}
		
		public IconHandler(UIImage image)
		{
			Control = image;
		}
		
		#region IIcon Members

		public void Create (Stream stream)
		{
			var data = NSData.FromStream(stream);
			Control = new UIImage (data);
		}

		public void Create (string fileName)
		{
			Control = new UIImage (fileName);
		}

		#endregion
		
		public Size Size {
			get {
				return Generator.ConvertF(Control.Size);
			}
		}

	}
}
