using swc = System.Windows.Controls;
using sw = System.Windows;
using swm = System.Windows.Media;
using Eto.Forms;
using Eto.Drawing;
using Eto.Wpf.Drawing;

namespace Eto.Wpf.Forms.Controls
{
	public class TabPageHandler : WpfPanel<swc.TabItem, TabPage, TabPage.ICallback>, TabPage.IHandler
	{
		Image image;
		readonly swc.DockPanel content;
		readonly swc.Image headerImage;
		readonly swc.TextBlock headerText;

		public TabPageHandler()
		{
			Control = new swc.TabItem();
			var header = new swc.StackPanel { Orientation = swc.Orientation.Horizontal };
			headerImage = new swc.Image { MaxHeight = 16, MaxWidth = 16 };
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
			set { headerText.Text = value; }
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
			}
		}

		void SetSource()
		{
			headerImage.Source = image.ToWpf(ParentScale);
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
