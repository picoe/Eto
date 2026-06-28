namespace Eto.GirCore.Forms.Controls
{
	public class ButtonHandler : GirControl<Gtk.Button, Button, Button.ICallback>, Button.IHandler
	{
		readonly Gtk.Box content;
		readonly Gtk.Label label;
		Gtk.Image? imageView;
		Image? image;
		ButtonImagePosition imagePosition;
		Size minimumSize;
		bool useMnemonic = true;

		public ButtonHandler()
		{
			Control = Gtk.Button.New();
			content = Gtk.Box.New(Gtk.Orientation.Horizontal, 6);
			label = Gtk.Label.New(null);
			content.Append(label);
			Control.SetChild(content);
			Control.OnClicked += (sender, e) => Callback.OnClick(Widget, EventArgs.Empty);
		}

		public virtual string? Text
		{
			get { return label.GetText().ToEtoMnemonic(); }
			set { label.SetTextWithMnemonic((useMnemonic ? value : value?.Replace("&", string.Empty))?.ToPlatformMnemonic()); }
		}

		public Image Image
		{
			get => image!;
			set
			{
				image = value;
				UpdateImage();
			}
		}

		public ButtonImagePosition ImagePosition
		{
			get => imagePosition;
			set
			{
				imagePosition = value;
				UpdateContentLayout();
			}
		}

		public Size MinimumSize
		{
			get => minimumSize;
			set
			{
				minimumSize = value;
				Control.SetSizeRequest(value.Width, value.Height);
			}
		}

		public Color TextColor { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool UseMnemonic
		{
			get => useMnemonic;
			set
			{
				useMnemonic = value;
				Text = Text;
			}
		}
		public bool AlwaysShowMnemonic { get => false; set { } }

		void UpdateImage()
		{
			if (imageView != null)
			{
				content.Remove(imageView);
				imageView = null;
			}

			if (image != null)
			{
				imageView = Drawing.GirImageHelper.CreateImage(image, new Size(16, 16));
				UpdateContentLayout();
			}
			else
			{
				UpdateContentLayout();
			}
		}

		void UpdateContentLayout()
		{
			content.SetOrientation(imagePosition is ButtonImagePosition.Above or ButtonImagePosition.Below ? Gtk.Orientation.Vertical : Gtk.Orientation.Horizontal);
			if (imageView != null)
			{
				content.Remove(label);
				content.Remove(imageView);
				switch (imagePosition)
				{
					case ButtonImagePosition.Right:
					case ButtonImagePosition.Below:
						content.Append(label);
						content.Append(imageView);
						break;
					case ButtonImagePosition.Overlay:
					case ButtonImagePosition.Left:
					case ButtonImagePosition.Above:
					default:
						content.Append(imageView);
						content.Append(label);
						break;
				}
			}
			else
			{
				content.Remove(label);
				content.Append(label);
			}
		}
	}
}
