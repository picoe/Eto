using SWF = System.Windows.Forms;
using SD = System.Drawing;
using Eto.Forms;
using Eto.Drawing;
using Eto.WinForms.Drawing;

namespace Eto.WinForms.Forms.Controls
{
	public class ImageViewHandler : WindowsControl<SWF.PictureBox, ImageView, ImageView.ICallback>, ImageView.IHandler
	{
		Image image;
		bool sizeSet;
		
		public ImageViewHandler ()
		{
			Control = new SWF.PictureBox {
				BorderStyle = SWF.BorderStyle.None,
				SizeMode = SWF.PictureBoxSizeMode.Zoom
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
				SetImage ();
			}
		}
		void SetImage ()
		{
			if (image != null) {
				var handler = image.Handler as IWindowsImageSource;
				Control.Image = handler != null ? handler.GetImageWithSize(null) : null;
			}
			else
				Control.Image = null;

			if (!sizeSet && Control.Image != null)
				Control.Size = Control.Image.Size;
		}

		public Image Image {
			get {
				return image;
			}
			set {
				image = value;
				SetImage ();
			}
		}
	}
}

