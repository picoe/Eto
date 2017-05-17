using swc = System.Windows.Controls;
using sw = System.Windows;
using swm = System.Windows.Media;
using Eto.Forms;
using Eto.Drawing;
using Eto.Wpf.Drawing;
using Eto.Wpf.CustomControls;

namespace Eto.Wpf.Forms.Controls
{
	public class TabPageHandler : WpfPanel<swc.TabItem, TabPage, TabPage.ICallback>, TabPage.IHandler
	{
		Image image;
		readonly swc.DockPanel content;
		readonly swc.Image headerImage;
		readonly swc.TextBlock headerText;
		int spacing = 4;

		public int ImageSpacing
		{
			get { return spacing; }
			set
			{
				spacing = value;
				SetMargin();
			}
		}

		public TabPageHandler()
		{
			Control = new swc.TabItem();
			var header = new swc.StackPanel { Orientation = swc.Orientation.Horizontal };
			headerImage = new swc.Image();
			headerText = new swc.TextBlock();
			header.Children.Add(headerImage);
			header.Children.Add(headerText);
			Control.Header = header;

			Control.Content = content = new swc.DockPanel
			{
				LastChildFill = true,
				VerticalAlignment = sw.VerticalAlignment.Stretch,
				HorizontalAlignment = sw.HorizontalAlignment.Stretch
			};
		}

		public string Text
		{
			get { return headerText.Text; }
			set
			{
				headerText.Text = value;
				SetMargin();
			}
		}

		public override Color BackgroundColor
		{
			get { return Control.Background.ToEtoColor(); }
			set { Control.Background = value.ToWpfBrush(Control.Background); }
		}

		public Image Image
		{
			get { return image; }
			set
			{
				image = value;
				SetSource();
				SetMargin();
			}
		}

		void SetSource()
		{
			headerImage.Source = image.ToWpf(ParentScale);
		}

		void SetMargin()
		{
			if (image != null && !string.IsNullOrEmpty(Text))
				headerImage.Margin = new sw.Thickness(0, 0, spacing, 0);
			else
				headerImage.Margin = new sw.Thickness();
		}

		protected override bool NeedsPixelSizeNotifications => true;

		protected override void OnLogicalPixelSizeChanged()
		{
			base.OnLogicalPixelSizeChanged();
			SetSource();
		}

		public override Size ClientSize
		{
			get { return new Size((int)content.Width, (int)content.Height); }
			set
			{
				content.Width = value.Width;
				content.Height = value.Height;
				UpdatePreferredSize();
			}
		}

		public override void SetContainerContent(sw.FrameworkElement content)
		{
			this.content.Children.Clear();
			this.content.Children.Add(content);
		}
	}
}
