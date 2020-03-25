using swc = System.Windows.Controls;
using swm = System.Windows.Media;
using sw = System.Windows;
using Eto.Forms;
using Eto.Drawing;
using System.Windows;
using System;

namespace Eto.Wpf.Forms.Controls
{
	public class ImageViewHandler : WpfFrameworkElement<CustomControls.MultiSizeImage, ImageView, ImageView.ICallback>, ImageView.IHandler
	{
		Image image;
		swc.Border border;

		public override FrameworkElement ContainerControl => border;

		public override Color BackgroundColor
		{
			get { return border.Background.ToEtoColor(); }
			set { border.Background = value.ToWpfBrush(border.Background); }
		}

		public ImageViewHandler()
		{
			Control = new CustomControls.MultiSizeImage
			{
				Handler = this,
				UseSmallestSpace = true,
				Stretch = swm.Stretch.Uniform,
				StretchDirection = swc.StretchDirection.Both
			};
			Control.Loaded += Control_Loaded;
			Control.SizeChanged += Control_SizeChanged;
			border = new swc.Border { Child = Control };
		}

		private void Control_Loaded(object sender, RoutedEventArgs e)
		{
			if (image is Icon)
				SetSource();
		}

		void Control_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			// if we're using an icon, update the current icon when the image size is changed
			if (image is Icon)
				SetSource();
		}

		protected override bool NeedsPixelSizeNotifications => true;

		protected override void OnLogicalPixelSizeChanged() => SetSource();

		void SetSource()
		{
			var fittingSize = image?.Size;
			if (Control.IsLoaded)
			{
				// use actual size of the control to get the correct image
				var size = Size;
				if (size.Width > 0 && size.Height > 0)
					fittingSize = size;
			}
			Image img = image;
			if (image is Icon icon)
			{
				var frame = icon.GetFrame(ParentScale, fittingSize);
				Control.Source = frame.Bitmap.ToWpf();
			}
			else
			{
				Control.Source = img.ToWpf(ParentScale, fittingSize);
			}
		}

		public Image Image
		{
			get { return image; }
			set
			{
				image = value;
				SetSource();
				Control.PreferredSize = image?.Size.ToWpf();
				Control.InvalidateMeasure();
			}
		}
	}
}
