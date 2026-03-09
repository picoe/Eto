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
				AutoSize = false,
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
				sizeSet = value.Width >= 0 || value.Height >= 0;
				SetImage();
			}
		}

		void SetImage()
		{
			var handler = image?.Handler as IWindowsImageSource;
			Control.Image = handler?.GetImageWithSize(Widget.Loaded || sizeSet ? (Size?)LogicalToDeviceUnits(Size).ToEto() : null);

			if (!sizeSet && image != null)
			{
				Control.Size = LogicalToDeviceUnits(image.Size);
			}
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

