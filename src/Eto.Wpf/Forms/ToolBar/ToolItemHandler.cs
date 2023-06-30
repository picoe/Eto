namespace Eto.Wpf.Forms.ToolBar
{
	public static class ToolItemHandler
	{
		public static Size? DefaultImageSize = new Size(16, 16);
		internal static readonly object ImageSize_Key = new object();
	}
	
	public abstract class ToolItemHandler<TControl, TWidget> : WidgetHandler<TControl, TWidget>, ToolItem.IHandler
		where TControl : System.Windows.UIElement
		where TWidget : ToolItem
	{
		swc.Image swcImage;
		swc.TextBlock label;
		Image image;

		public virtual string Text
		{
			get { return label.Text.ToEtoMnemonic(); }
			set
			{
				label.Text = value.ToPlatformMnemonic();
				label.Visibility = string.IsNullOrEmpty(label.Text) ? sw.Visibility.Collapsed : sw.Visibility.Visible;
			}
		}

		public abstract string ToolTip { get; set; }

		public virtual Image Image
		{
			get { return image; }
			set
			{
				image = value;
				if (swcImage != null)
				{
					OnImageSizeChanged();
					swcImage.Visibility = swcImage.Source == null ? sw.Visibility.Collapsed : sw.Visibility.Visible;
				}
			}
		}

		public virtual bool Enabled
		{
			get { return Control.IsEnabled; }
			set
			{
				Control.IsEnabled = value;
				if (swcImage != null) 
					swcImage.IsEnabled = value;
			}
		}


		public Size? ImageSize
		{
			get => Widget.Properties.Get<Size?>(ToolItemHandler.ImageSize_Key, ToolItemHandler.DefaultImageSize);
			set
			{
				if (Widget.Properties.TrySet(ToolItemHandler.ImageSize_Key, value, ToolItemHandler.DefaultImageSize))
				{
					OnImageSizeChanged();
				}
			}
		}

		protected virtual void OnImageSizeChanged()
		{
			if (swcImage != null)
			{
				var size = ImageSize;
				swcImage.MaxHeight = size?.Height ?? double.PositiveInfinity;
				swcImage.MaxWidth = size?.Width ?? double.PositiveInfinity;
				swcImage.Source = image.ToWpf(Screen.PrimaryScreen.LogicalPixelSize, size);
			}
		}

		protected override void Initialize()
		{
			base.Initialize();
			OnImageSizeChanged();
		}

		protected virtual sw.FrameworkElement CreateContent(sw.FrameworkElement extra = null)
		{
			swcImage = new swc.Image();
			swcImage.Visibility = sw.Visibility.Collapsed;
			label = new swc.TextBlock();
			label.VerticalAlignment = sw.VerticalAlignment.Center;
			label.Margin = new sw.Thickness(2, 0, 2, 0);
			label.Visibility = sw.Visibility.Collapsed;
			var panel = new swc.StackPanel { Orientation = swc.Orientation.Horizontal };
			panel.Children.Add(swcImage);
			panel.Children.Add(label);
			if (extra != null)
				panel.Children.Add(extra);

			sw.Automation.AutomationProperties.SetLabeledBy(Control, label);
			return panel;
		}

		public bool Visible
		{
			get => Control.Visibility == sw.Visibility.Visible;
			set => Control.Visibility = value ? sw.Visibility.Visible : sw.Visibility.Collapsed;
		}

		public virtual void CreateFromCommand(Command command)
		{
		}

		public virtual void OnLoad(EventArgs e)
		{
		}

		public virtual void OnPreLoad(EventArgs e)
		{
		}

		public virtual void OnUnLoad(EventArgs e)
		{
		}
	}
}