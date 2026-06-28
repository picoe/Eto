namespace Eto.GirCore.Forms.Controls
{
	public class ToggleButtonHandler : GirControl<Gtk.ToggleButton, ToggleButton, ToggleButton.ICallback>, ToggleButton.IHandler
	{
		int suppressClick;
		readonly Gtk.Box content;
		readonly Gtk.Label label;
		string? text;
		Image? image;
		Gtk.Image? imageView;
		ButtonImagePosition imagePosition;
		Size minimumSize;
		bool useMnemonic = true;

		public ToggleButtonHandler()
		{
			Control = Gtk.ToggleButton.New();
			content = Gtk.Box.New(Gtk.Orientation.Horizontal, 6);
			label = Gtk.Label.New(null);
			content.Append(label);
			Control.SetChild(content);
			Control.OnToggled += (sender, e) => HandleToggled();
		}

		void HandleToggled()
		{
			Callback.OnCheckedChanged(Widget, EventArgs.Empty);

			if (suppressClick == 0)
				Callback.OnClick(Widget, EventArgs.Empty);
		}

		public bool Checked
		{
			get => Control.Active;
			set
			{
				suppressClick++;
				Control.Active = value;
				suppressClick--;
			}
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case ToggleButton.CheckedChangedEvent:
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		public string? Text
		{
			get => text;
			set
			{
				text = value;
				label.SetTextWithMnemonic((useMnemonic ? value : value?.Replace("&", string.Empty))?.ToPlatformMnemonic());
			}
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
				if (value == useMnemonic)
					return;
				useMnemonic = value;
				Text = text;
			}
		}

		public bool AlwaysShowMnemonic
		{
			get => false;
			set { }
		}

		void UpdateImage()
		{
			if (imageView != null)
			{
				content.Remove(imageView);
				imageView = null;
			}

			if (image != null)
				imageView = Drawing.GirImageHelper.CreateImage(image, new Size(16, 16));

			UpdateContentLayout();
		}

		void UpdateContentLayout()
		{
			content.SetOrientation(imagePosition is ButtonImagePosition.Above or ButtonImagePosition.Below ? Gtk.Orientation.Vertical : Gtk.Orientation.Horizontal);
			content.Remove(label);
			if (imageView != null)
				content.Remove(imageView);

			if (imageView == null)
			{
				content.Append(label);
				return;
			}

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
	}
}
