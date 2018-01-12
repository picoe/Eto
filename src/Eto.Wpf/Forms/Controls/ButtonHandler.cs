using System;
using Eto.Forms;
using Eto.Drawing;
#if WINRT
using sw = Windows.UI.Xaml;
using swc = Windows.UI.Xaml.Controls;
using wf = Windows.Foundation;

using WpfLabel = Windows.UI.Xaml.Controls.TextBlock;
using EtoImage = Windows.UI.Xaml.Controls.Image;

namespace Eto.WinRT.Forms.Controls
#else
using sw = System.Windows;
using swc = System.Windows.Controls;
using wf = System.Windows;

using WpfLabel = System.Windows.Controls.Label;

namespace Eto.Wpf.Forms.Controls
#endif
{
	public class EtoButton : swc.Button, IEtoWpfControl
	{
		public IWpfFrameworkElement Handler { get; set; }

		protected override wf.Size MeasureOverride(wf.Size constraint)
		{
			return Handler?.MeasureOverride(constraint, base.MeasureOverride) ?? base.MeasureOverride(constraint);
		}
	}

	/// <summary>
	/// Button handler.
	/// </summary>
	/// <copyright>(c) 2014 by Vivek Jhaveri</copyright>
	/// <copyright>(c) 2012-2015 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class ButtonHandler : WpfControl<swc.Button, Button, Button.ICallback>, Button.IHandler
	{
		readonly swc.Image swcimage;
		readonly WpfLabel label;

		public static Size DefaultMinimumSize = new Size(80, 23);

		protected override wf.Size DefaultSize => MinimumSize.ToWpf();

		public ButtonHandler()
		{
			Control = new EtoButton { Handler = this };
			Control.Click += (sender, e) => Callback.OnClick(Widget, EventArgs.Empty);
			label = new WpfLabel
			{
				IsHitTestVisible = false,
				VerticalAlignment = sw.VerticalAlignment.Center,
				HorizontalAlignment = sw.HorizontalAlignment.Center,
				Padding = new sw.Thickness(3, 0, 3, 0),
				Visibility = sw.Visibility.Collapsed
			};
			swc.Grid.SetColumn(label, 1);
			swc.Grid.SetRow(label, 1);
			swcimage = new swc.Image();
			var grid = new swc.Grid();
			grid.ColumnDefinitions.Add(new swc.ColumnDefinition { Width = sw.GridLength.Auto });
			grid.ColumnDefinitions.Add(new swc.ColumnDefinition { Width = new sw.GridLength(1, sw.GridUnitType.Star) });
			grid.ColumnDefinitions.Add(new swc.ColumnDefinition { Width = sw.GridLength.Auto });
			grid.RowDefinitions.Add(new swc.RowDefinition { Height = sw.GridLength.Auto });
			grid.RowDefinitions.Add(new swc.RowDefinition { Height = new sw.GridLength(1, sw.GridUnitType.Star) });
			grid.RowDefinitions.Add(new swc.RowDefinition { Height = sw.GridLength.Auto });
			grid.Children.Add(swcimage);
			grid.Children.Add(label);

			Control.Content = grid;
		}

		protected override void Initialize()
		{
			base.Initialize();
			SetImagePosition();
		}

		public override bool UseMousePreview { get { return true; } }

		public override bool UseKeyPreview { get { return true; } }

#if WINRT
		public string Text
		{
			get { return label.Text; }
			set
			{
				label.Text = value;
				SetImagePosition();
			}
		}
#else
		public string Text
		{
			get { return (label.Content as string).ToEtoMnemonic(); }
			set
			{
				label.Content = value.ToPlatformMnemonic();
				SetImagePosition();
			}
		}
#endif
		static readonly object Image_Key = new object();

#if WINRT
		void SetImage()
		{
			swcimage.Source = Image.ToWpf();
		}
#else
		protected override bool NeedsPixelSizeNotifications { get { return true; } }

		protected override void OnLogicalPixelSizeChanged()
		{
			base.OnLogicalPixelSizeChanged();
			SetImage();
		}

		void SetImage()
		{
			swcimage.Source = Image.ToWpf(ParentScale);
		}
#endif

		public Image Image
		{
			get { return Widget.Properties.Get<Image>(Image_Key); }
			set
			{
				Widget.Properties.Set(Image_Key, value, SetImage);
			}
		}

		void SetImagePosition()
		{
			bool hideLabel = string.IsNullOrEmpty(Text);
			int col, row;
			switch (ImagePosition)
			{
				case ButtonImagePosition.Left:
					col = 0; row = 1;
					Control.HorizontalContentAlignment = sw.HorizontalAlignment.Stretch;
					Control.VerticalContentAlignment = sw.VerticalAlignment.Center;
					break;
				case ButtonImagePosition.Right:
					col = 2; row = 1;
					Control.HorizontalContentAlignment = sw.HorizontalAlignment.Stretch;
					Control.VerticalContentAlignment = sw.VerticalAlignment.Center;
					break;
				case ButtonImagePosition.Above:
					col = 1; row = 0;
					Control.HorizontalContentAlignment = sw.HorizontalAlignment.Center;
					Control.VerticalContentAlignment = hideLabel ? sw.VerticalAlignment.Center : sw.VerticalAlignment.Stretch;
					break;
				case ButtonImagePosition.Below:
					col = 1; row = 2;
					Control.HorizontalContentAlignment = sw.HorizontalAlignment.Center;
					Control.VerticalContentAlignment = hideLabel ? sw.VerticalAlignment.Center : sw.VerticalAlignment.Stretch;
					break;
				case ButtonImagePosition.Overlay:
					col = 1; row = 1;
					Control.HorizontalContentAlignment = sw.HorizontalAlignment.Center;
					Control.VerticalContentAlignment = sw.VerticalAlignment.Center;
					break;
				default:
					throw new NotSupportedException();
			}

			swc.Grid.SetColumn(swcimage, col);
			swc.Grid.SetRow(swcimage, row);
			label.Visibility = hideLabel ? sw.Visibility.Collapsed : sw.Visibility.Visible;
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

		public
#if WPF
		override 
#endif
		Color TextColor
		{
			get { return label.Foreground.ToEtoColor(); }
			set { label.Foreground = value.ToWpfBrush(Control.Foreground); }
		}

		static readonly object MinimumSize_Key = new object();

		public Size MinimumSize
		{
			get { return Widget.Properties.Get<Size>(MinimumSize_Key, DefaultMinimumSize); }
			set
			{
				if (MinimumSize != value)
				{
					Widget.Properties[MinimumSize_Key] = value;
					Control.InvalidateMeasure();
				}
			}
		}
	}
}
