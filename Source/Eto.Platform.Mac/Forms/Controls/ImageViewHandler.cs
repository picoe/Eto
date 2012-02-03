using System;
using MonoMac.AppKit;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Platform.Mac
{
	public class ImageViewHandler : MacControl<NSImageView, ImageView>, IImageView
	{
		IImage image;
		bool sizeSet;
		
		public ImageViewHandler ()
		{
			Control = new NSImageView ();
			AutoSize = false;
		}
		
		public override Size Size {
			get {
				return base.Size;
			}
			set {
				base.Size = value;
				sizeSet = true;
			}
		}
		
		#region IImageView implementation
		
		public IImage Image {
			get {
				return image;
			}
			set {
				image = value;
				Control.Image = (NSImage)value.ControlObject;
				if (!sizeSet)
					Control.SetFrameSize (Generator.ConvertF (value.Size));
			}
		}
		
		#endregion
	}
}

