using swc = System.Windows.Controls;
using swm = System.Windows.Media;
using Eto.Forms;
using Eto.Drawing;
using System.Windows;
using System;

namespace Eto.Wpf.Forms.Controls
{
	public class ImageViewHandler : WpfFrameworkElement<CustomControls.MultiSizeImage, ImageView, ImageView.ICallback>, ImageView.IHandler
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
				Handler = this,
				UseSmallestSpace = true,
				Stretch = swm.Stretch.Uniform,
				StretchDirection = swc.StretchDirection.Both
			};
		}

		protected override bool NeedsPixelSizeNotifications {  get { return true; } }

		protected override void OnLogicalPixelSizeChanged()
		{
			SetSource();
		}

		void SetSource()
		{
			Control.Source = image.ToWpf(ParentScale, UserPreferredSize.ToEtoSize());
		}

		public Image Image
		{
			get { return image; }
			set
			{
				image = value;
				var size = image != null ? image.Size : Eto.Drawing.Size.Empty;
				var ps = UserPreferredSize;
				if (double.IsNaN(ps.Width))
					ps.Width = size.Width;
				if (double.IsNaN(ps.Height))
					ps.Height = size.Height;
				UserPreferredSize = ps;
				SetSource();
			}
		}
	}
}
