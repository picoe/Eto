using System;
using Eto.Forms;
using Eto.Drawing;
using sw = System.Windows;
using swm = System.Windows.Media;
using swc = System.Windows.Controls;
using xwt = Xceed.Wpf.Toolkit;

namespace Eto.Wpf.Forms
{
	public class XceedColorDialog : sw.Window
	{
		xwt.ColorCanvas canvas;

		public event sw.RoutedPropertyChangedEventHandler<swm.Color?> SelectedColorChanged
		{
			add { canvas.SelectedColorChanged += value; }
			remove { canvas.SelectedColorChanged -= value; }
		}

		public swm.Color Color
		{
			get { return canvas.SelectedColor ?? swm.Colors.Transparent; }
			set { canvas.SelectedColor = value; }
		}

		public bool UsingAlphaChannel
		{
			get { return canvas.UsingAlphaChannel; }
			set { canvas.UsingAlphaChannel = value; }
		}

		public XceedColorDialog()
		{
			canvas = new xwt.ColorCanvas();
			Background = sw.SystemColors.ControlBrush;

			var doneButton = new swc.Button { Content = "OK", IsDefault = true, MinWidth = 80, Margin = new sw.Thickness(5) };
			doneButton.Click += doneButton_Click;

			var cancelButton = new swc.Button { Content = "Cancel", IsCancel = true, MinWidth = 80, Margin = new sw.Thickness(5) };
			cancelButton.Click += cancelButton_Click;

			var buttons = new swc.StackPanel { Orientation = swc.Orientation.Horizontal, HorizontalAlignment = sw.HorizontalAlignment.Right };
			buttons.Children.Add(doneButton);
			buttons.Children.Add(cancelButton);

			var top = new swc.StackPanel { Orientation = swc.Orientation.Vertical };
			top.Children.Add(canvas);
			top.Children.Add(buttons);
			Content = top;
			SizeToContent = sw.SizeToContent.WidthAndHeight;
			ResizeMode = sw.ResizeMode.NoResize;
		}

		void cancelButton_Click(object sender, sw.RoutedEventArgs e)
		{
			DialogResult = false;
			Close();
		}

		void doneButton_Click(object sender, sw.RoutedEventArgs e)
		{
			DialogResult = true;
			Close();
		}
	}

	public class ColorDialogHandler : WidgetHandler<XceedColorDialog, ColorDialog, ColorDialog.ICallback>, ColorDialog.IHandler
	{
		public ColorDialogHandler()
		{
			this.Control = new XceedColorDialog();
		}

		public Color Color
		{
			get { return Control.Color.ToEto(); }
			set { Control.Color = value.ToWpf(); }
		}

		public bool AllowAlpha
		{
			get { return Control.UsingAlphaChannel; }
			set { Control.UsingAlphaChannel = value; }
		}

		public bool SupportsAllowAlpha => true;

		public DialogResult ShowDialog(Window parent)
		{
			if (parent != null)
			{
				var owner = parent.ControlObject as sw.Window;
				Control.Owner = owner;
				Control.WindowStartupLocation = sw.WindowStartupLocation.CenterOwner;
			}
			var result = Control.ShowDialog();
			if (result == true)
			{
				Callback.OnColorChanged(Widget, EventArgs.Empty);
				return DialogResult.Ok;
			}
			return DialogResult.Cancel;
		}
	}
}

