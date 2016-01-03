using swf = System.Windows.Forms;
using sd = System.Drawing;
using Eto.Forms;
using Eto.Drawing;
using Eto.WinForms.Drawing;

namespace Eto.WinForms.Forms.Controls
{
	public class ImageViewHandler : WindowsControl<swf.PictureBox, ImageView, ImageView.ICallback>, ImageView.IHandler
	{
		Image image;
		bool sizeSet;

		public ImageViewHandler()
		{
			Control = new swf.PictureBox
			{
				BorderStyle = swf.BorderStyle.None,
				SizeMode = swf.PictureBoxSizeMode.Zoom
			};
			Control.SizeChanged += Control_SizeChanged;
		}

		void Control_SizeChanged(object sender, System.EventArgs e)
		{
			if (!Widget.Loaded)
				return;
			var handler = image?.Handler as IWindowsImageSource;
			Control.Image = handler?.GetImageWithSize(Control.ClientSize.ToEto());
		}

		public override Size Size
		{
			get { return base.Size; }
			set
			{
				base.Size = value;
				sizeSet = true;
				SetImage();
			}
		}
		void SetImage(bool setSize = true)
		{
			var handler = image?.Handler as IWindowsImageSource;
			Control.Image = handler?.GetImageWithSize(Widget.Loaded || sizeSet ? (Size?)Size : null);

			if (setSize && !sizeSet && Control.Image != null)
				Control.Size = Control.Image.Size;
		}

		public Image Image
		{
			get { return image; }
			set
			{
				image = value;
				SetImage();
			}
		}
	}
}

