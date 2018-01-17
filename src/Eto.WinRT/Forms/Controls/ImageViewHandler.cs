#if TODO_XAML
using swc = Windows.UI.Xaml.Controls;
using swm = Windows.UI.Xaml.Media;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.WinRT.Forms.Controls
{
	public class ImageViewHandler : WpfFrameworkElement<CustomControls.MultiSizeImage, ImageView>, IImageView
	{
		Image image;

		public override Color BackgroundColor
		{
			get { return Control.Background.ToEtoColor(); }
			set { Control.Background = value.ToWpfBrush(Control.Background); }
		}

		public ImageViewHandler()
		{
			Control = new CustomControls.MultiSizeImage
			{
				UseSmallestSpace = true,
				Stretch = swm.Stretch.Uniform,
				StretchDirection = swc.StretchDirection.Both
			};
		}

		public Image Image
		{
			get { return image; }
			set
			{
				image = value;
				Control.Source = image != null ? image.ControlObject as swm.ImageSource : null;
			}
		}
	}
}
#endif