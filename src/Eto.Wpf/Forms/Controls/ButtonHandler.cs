#nullable enable

namespace Eto.Wpf.Forms.Controls
{
	public class EtoButton : swc.Button, IEtoWpfControl
	{
		public IWpfFrameworkElement? Handler { get; set; }

		protected override sw.Size MeasureOverride(sw.Size constraint)
		{
			return Handler?.MeasureOverride(constraint, base.MeasureOverride) ?? base.MeasureOverride(constraint);
		}
	}

	public class ButtonHandler : ButtonHandler<swc.Button, Button, Button.ICallback>, Button.IHandler
	{
		public static Size DefaultMinimumSize = new Size(80, 23);

		internal static readonly object ImageLabelSpacing_Key = new object();

		protected override Size GetDefaultMinimumSize() => DefaultMinimumSize;

		protected override swc.Button CreateControl() => new EtoButton { Handler = this };
	}

	/// <summary>
	/// Button handler.
	/// </summary>
	/// <copyright>(c) 2014 by Vivek Jhaveri</copyright>
	/// <copyright>(c) 2012-2019 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class ButtonHandler<TControl, TWidget, TCallback> : WpfControl<TControl, TWidget, TCallback>, Button.IHandler
		where TControl : swc.Primitives.ButtonBase
		where TWidget : Button
		where TCallback : Button.ICallback
	{
		EtoAccessLabel? _labelPart;

		public swc.Image? ImagePart { get; private set; }

		public EtoAccessLabel LabelPart => _labelPart ??= new EtoAccessLabel
		{
			TextAlignment = sw.TextAlignment.Center
		};

		/// <summary>
		/// Gets or sets the spacing between the image and the label when both are present
		/// </summary>
		public int ImageLabelSpacing
		{
			get => Widget.Properties.Get<int>(ButtonHandler.ImageLabelSpacing_Key, 2);
			set
			{
				if (Widget.Properties.TrySet(ButtonHandler.ImageLabelSpacing_Key, value, 2))
				{
					SetImagePosition();
				}
			}
		}

		protected override sw.Size DefaultSize => MinimumSize.ToWpf();

		protected virtual Size GetDefaultMinimumSize() => Size.Empty;

		protected override void Initialize()
		{
			Control.HorizontalAlignment = sw.HorizontalAlignment.Stretch;
			Control.VerticalContentAlignment = sw.VerticalAlignment.Stretch;
			Control.Click += (sender, e) => Callback.OnClick(Widget, EventArgs.Empty);
			CreateContent();

			base.Initialize();
		}

		private void CreateContent()
		{
			if (Control.Content is swc.Grid g)
			{
				if (_labelPart != null)
					g.Children.Remove(_labelPart);
				if (ImagePart != null)
					g.Children.Remove(ImagePart);
			}
			Control.Content = null;

			if (ImagePart == null)
			{
				// no image, so just use the label
				Control.Content = _labelPart;
				sw.Automation.AutomationProperties.SetLabeledBy(Control, _labelPart);
				return;
			}


			if (string.IsNullOrEmpty(Text))
			{
				// no label, so just use the image
				sw.Automation.AutomationProperties.SetLabeledBy(Control, null);
				Control.Content = ImagePart;
				return;
			}

			// we have an image and text
			if (Control.Content is not swc.Grid)
			{
				swc.Grid.SetColumn(LabelPart, 1);
				swc.Grid.SetRow(LabelPart, 1);
				var grid = new swc.Grid();
				grid.ColumnDefinitions.Add(new swc.ColumnDefinition { Width = sw.GridLength.Auto });
				grid.ColumnDefinitions.Add(new swc.ColumnDefinition { Width = new sw.GridLength(1, sw.GridUnitType.Star) });
				grid.ColumnDefinitions.Add(new swc.ColumnDefinition { Width = sw.GridLength.Auto });
				grid.RowDefinitions.Add(new swc.RowDefinition { Height = sw.GridLength.Auto });
				grid.RowDefinitions.Add(new swc.RowDefinition { Height = new sw.GridLength(1, sw.GridUnitType.Star) });
				grid.RowDefinitions.Add(new swc.RowDefinition { Height = sw.GridLength.Auto });
				grid.Children.Add(ImagePart);
				grid.Children.Add(LabelPart);

				Control.Content = grid;
				sw.Automation.AutomationProperties.SetLabeledBy(Control, LabelPart);
			}
			SetImagePosition();

		}


		public override bool UseMousePreview => true;

		public override bool UseKeyPreview => true;

		public string? Text
		{
			get => _labelPart?.Text;
			set
			{
				if (value == Text)
					return;
				var wasEmpty = string.IsNullOrEmpty(Text);
				var isEmpty = string.IsNullOrEmpty(value);

				if (wasEmpty && !isEmpty)
				{
					LabelPart.Text = value;
					CreateContent();
				}
				else if (!wasEmpty && isEmpty)
				{
					// don't kill LabelPart, it holds some state
					LabelPart.Text = null;
					CreateContent();
				}
				else if (!wasEmpty && !isEmpty)
				{
					LabelPart.Text = value;
				}
			}
		}


		static readonly object Image_Key = new object();

		protected override bool NeedsPixelSizeNotifications => true;

		protected override void OnLogicalPixelSizeChanged()
		{
			base.OnLogicalPixelSizeChanged();
			SetImage();
		}

		bool SetImage()
		{
			if (ImagePart == null)
				return false;
			ImagePart.Source = Image.ToWpf(ParentScale);
			return true;
		}

		public Image Image
		{
			get { return Widget.Properties.Get<Image>(Image_Key); }
			set
			{
				if (Widget.Properties.TrySet(Image_Key, value))
				{
					if (ImagePart == null && value != null)
					{
						ImagePart = new swc.Image { Source = Image.ToWpf(ParentScale) };
						CreateContent();
					}
					else if (ImagePart != null && value == null)
					{
						ImagePart = null;
						CreateContent();
					}
					else if (ImagePart != null && value != null)
					{
						SetImage();
					}
				}
			}
		}

		void SetImagePosition()
		{
			if (ImagePart == null || LabelPart == null)
				return;
			// when hugging the text, only give the grid its desired size so the image ends up beside the label
			// instead of the label taking up all the leftover space.
			var fill = ImageNextToText ? sw.HorizontalAlignment.Center : sw.HorizontalAlignment.Stretch;
			var fillVertical = ImageNextToText ? sw.VerticalAlignment.Center : sw.VerticalAlignment.Stretch;
			int col, row;
			sw.Thickness imageSpacing;
			switch (ImagePosition)
			{
				case ButtonImagePosition.Left:
					col = 0; row = 1;
					Control.HorizontalContentAlignment = fill;
					Control.VerticalContentAlignment = sw.VerticalAlignment.Center;
					imageSpacing = new sw.Thickness(ImageLabelSpacing, 0, 0, 0);
					break;
				case ButtonImagePosition.Right:
					col = 2; row = 1;
					Control.HorizontalContentAlignment = fill;
					Control.VerticalContentAlignment = sw.VerticalAlignment.Center;
					imageSpacing = new sw.Thickness(0, 0, ImageLabelSpacing, 0);
					break;
				case ButtonImagePosition.Above:
					col = 1; row = 0;
					Control.HorizontalContentAlignment = sw.HorizontalAlignment.Center;
					Control.VerticalContentAlignment = fillVertical;
					imageSpacing = new sw.Thickness(0, ImageLabelSpacing, 0, 0);
					break;
				case ButtonImagePosition.Below:
					col = 1; row = 2;
					Control.HorizontalContentAlignment = sw.HorizontalAlignment.Center;
					Control.VerticalContentAlignment = fillVertical;
					imageSpacing = new sw.Thickness(0, 0, 0, ImageLabelSpacing);
					break;
				case ButtonImagePosition.Overlay:
					col = 1; row = 1;
					Control.HorizontalContentAlignment = sw.HorizontalAlignment.Center;
					Control.VerticalContentAlignment = sw.VerticalAlignment.Center;
					imageSpacing = new sw.Thickness(0);
					break;
				default:
					throw new NotSupportedException();
			}

			swc.Grid.SetColumn(ImagePart, col);
			swc.Grid.SetRow(ImagePart, row);
			LabelPart.Margin = imageSpacing;
		}

		static readonly object ImagePosition_Key = new object();

		public ButtonImagePosition ImagePosition
		{
			get { return Widget.Properties.Get<ButtonImagePosition>(ImagePosition_Key); }
			set { Widget.Properties.Set(ImagePosition_Key, value, SetImagePosition); }
		}

		static readonly object ImageNextToText_Key = new object();

		public bool ImageNextToText
		{
			get { return Widget.Properties.Get<bool>(ImageNextToText_Key); }
			set { Widget.Properties.Set(ImageNextToText_Key, value, SetImagePosition); }
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case Eto.Forms.Control.MouseUpEvent:
#if WPF
					ContainerControl.PreviewMouseDown += (sender, e) =>
					{
						// don't swallow mouse up events for right click and middle click
						e.Handled |= e.ChangedButton != sw.Input.MouseButton.Left;
					};
#endif
					base.AttachEvent(id);
					break;
				case Button.TextChangedEvent:
					// text is never changed
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		public override Color TextColor
		{
			get => (_labelPart?.Foreground ?? Control.Foreground).ToEtoColor();
			set
			{
				LabelPart.Foreground = value.ToWpfBrush();
			}
		}

		static readonly object MinimumSize_Key = new object();

		public Size MinimumSize
		{
			get { return Widget.Properties.Get<Size?>(MinimumSize_Key) ?? GetDefaultMinimumSize(); }
			set
			{
				if (MinimumSize != value)
				{
					Widget.Properties[MinimumSize_Key] = value;
					SetSize();
					Control.InvalidateMeasure();
				}
			}
		}

		public bool UseMnemonic
		{
			get => _labelPart?.UseMnemonic ?? true;
			set => LabelPart.UseMnemonic = value;
		}

		public bool AlwaysShowMnemonic
		{
			get => _labelPart?.AlwaysShowMnemonic ?? false;
			set => LabelPart.AlwaysShowMnemonic = value;
		}

		public bool EnableMnemonic
		{
			get => _labelPart?.EnableMnemonic ?? true;
			set => LabelPart.EnableMnemonic = value;
		}

	}
}
