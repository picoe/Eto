namespace Eto.Wpf.Forms.Controls
{
	public class EtoButton : swc.Button, IEtoWpfControl
	{
		public IWpfFrameworkElement Handler { get; set; }

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
		where TControl: swc.Primitives.ButtonBase
		where TWidget: Button
		where TCallback: Button.ICallback
	{
		public swc.Image ImagePart { get; private set; }

		public swc.Label LabelPart { get; private set; }

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
			Control.Click += (sender, e) => Callback.OnClick(Widget, EventArgs.Empty);
			LabelPart = new swc.Label
			{
				IsHitTestVisible = false,
				VerticalAlignment = sw.VerticalAlignment.Center,
				HorizontalAlignment = sw.HorizontalAlignment.Center,
				Padding = new sw.Thickness(0),
				Visibility = sw.Visibility.Collapsed
			};
			swc.Grid.SetColumn(LabelPart, 1);
			swc.Grid.SetRow(LabelPart, 1);
			ImagePart = new swc.Image();
			ImagePart.Visibility = sw.Visibility.Collapsed;
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

			SetImagePosition();
			base.Initialize();
		}

		public override bool UseMousePreview => true;

		public override bool UseKeyPreview => true;

		public string Text
		{
			get { return (LabelPart.Content as string).ToEtoMnemonic(); }
			set
			{
				LabelPart.Content = value.ToPlatformMnemonic();
				SetImagePosition();
			}
		}
		static readonly object Image_Key = new object();

		protected override bool NeedsPixelSizeNotifications => true;

		protected override void OnLogicalPixelSizeChanged()
		{
			base.OnLogicalPixelSizeChanged();
			SetImage();
		}

		void SetImage()
		{
			ImagePart.Source = Image.ToWpf(ParentScale);
		}

		public Image Image
		{
			get { return Widget.Properties.Get<Image>(Image_Key); }
			set
			{
				if (Widget.Properties.TrySet(Image_Key, value))
				{
					SetImage();
					ImagePart.Visibility = value != null ? sw.Visibility.Visible : sw.Visibility.Collapsed;
					SetImagePosition();
				}
			}
		}

		void SetImagePosition()
		{
			bool hideLabel = string.IsNullOrEmpty(Text);
			int col, row;
			sw.Thickness imageSpacing;
			switch (ImagePosition)
			{
				case ButtonImagePosition.Left:
					col = 0; row = 1;
					Control.HorizontalContentAlignment = sw.HorizontalAlignment.Stretch;
					Control.VerticalContentAlignment = sw.VerticalAlignment.Center;
					imageSpacing = new sw.Thickness(ImageLabelSpacing, 0, 0, 0);
					break;
				case ButtonImagePosition.Right:
					col = 2; row = 1;
					Control.HorizontalContentAlignment = sw.HorizontalAlignment.Stretch;
					Control.VerticalContentAlignment = sw.VerticalAlignment.Center;
					imageSpacing = new sw.Thickness(0, 0, ImageLabelSpacing, 0);
					break;
				case ButtonImagePosition.Above:
					col = 1; row = 0;
					Control.HorizontalContentAlignment = sw.HorizontalAlignment.Center;
					Control.VerticalContentAlignment = hideLabel ? sw.VerticalAlignment.Center : sw.VerticalAlignment.Stretch;
					imageSpacing = new sw.Thickness(0, ImageLabelSpacing, 0, 0);
					break;
				case ButtonImagePosition.Below:
					col = 1; row = 2;
					Control.HorizontalContentAlignment = sw.HorizontalAlignment.Center;
					Control.VerticalContentAlignment = hideLabel ? sw.VerticalAlignment.Center : sw.VerticalAlignment.Stretch;
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
			LabelPart.Visibility = hideLabel ? sw.Visibility.Collapsed : sw.Visibility.Visible;
			LabelPart.Margin = ImagePart.Visibility == sw.Visibility.Visible ? imageSpacing : new sw.Thickness(0, 0, 0, 0);
		}

		static readonly object ImagePosition_Key = new object();

		public ButtonImagePosition ImagePosition
		{
			get { return Widget.Properties.Get<ButtonImagePosition>(ImagePosition_Key); }
			set { Widget.Properties.Set(ImagePosition_Key, value, SetImagePosition); }
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

		public override  Color TextColor
		{
			get { return LabelPart.Foreground.ToEtoColor(); }
			set { LabelPart.Foreground = value.ToWpfBrush(Control.Foreground); }
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
	}
}
