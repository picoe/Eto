#if TODO_XAML
using swc = Windows.UI.Xaml.Controls;
using sw = Windows.UI.Xaml;
using swm = Windows.UI.Xaml.Media;
using Eto.Forms;
using Eto.Drawing;
using Eto.WinRT.Drawing;

namespace Eto.WinRT.Forms.Controls
{
	public class TabPageHandler : WpfPanel<swc.TabItem, TabPage>, ITabPage
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

			Control.Content = content = new swc.DockPanel { LastChildFill = true };
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
				headerImage.Source = image != null ? ((IWpfImage)image.Handler).GetImageClosestToSize(16) : null;
			}
		}

		public override Size ClientSize
		{
			get { return new Size((int)content.Width, (int)content.Height); }
			set
			{
				content.Width = value.Width;
				content.Height = value.Height;
			}
		}

		public override void SetContainerContent(sw.FrameworkElement content)
		{
			this.content.Children.Clear();
			this.content.Children.Add(content);
		}
	}
}
#endif