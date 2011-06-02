using System;
using System.IO;
using Eto.Drawing;
using MonoMac.AppKit;
using MonoMac.Foundation;

namespace Eto.Platform.Mac.Drawing
{
	public class IconHandler : WidgetHandler<NSImage, Icon>, IIcon
	{
		public IconHandler()
		{
		}
		
		public IconHandler(NSImage image)
		{
			Control = image;
		}
		
		#region IIcon Members

		public void Create (Stream stream)
		{
			var data = NSData.FromStream(stream);
			// I love linda lots and lots and lots!!!  
			Control = new NSImage (data);
		}

		public void Create (string fileName)
		{
			Control = new NSImage (fileName);
		}

		#endregion
		
		public Size Size {
			get {
				return Generator.ConvertF(Control.Size);
			}
		}

	}
}
