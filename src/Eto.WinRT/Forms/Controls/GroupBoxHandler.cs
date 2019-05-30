#if TODO_XAML
using swc = Windows.UI.Xaml.Controls;
using sw = Windows.UI.Xaml;
using swd = Windows.UI.Xaml.Data;
using Eto.Forms;
using Eto.Drawing;
using Eto.WinRT.Drawing;

namespace Eto.WinRT.Forms.Controls
{
	public class GroupBoxHandler : WpfPanel<swc.GroupBox, GroupBox>, IGroupBox
	{
		Font font;
		swc.Label Header { get; set; }
		swc.AccessText AccessText { get { return (swc.AccessText)Header.Content; } }

		public GroupBoxHandler()
		{
			Control = new swc.GroupBox();
			Header = new swc.Label { Content = new swc.AccessText() };
		}

		public override void SetContainerContent(sw.FrameworkElement content)
		{
			Control.Content = content;
		}

		public override Color BackgroundColor
		{
			get { return Control.Background.ToEtoColor(); }
			set { Control.Background = value.ToWpfBrush(Control.Background); }
		}

		public Font Font
		{
			get { return font; }
			set { font = FontHandler.Apply(Header, r => AccessText.TextDecorations = r, value); }
		}

		public string Text
		{
			get { return AccessText.Text.ToEtoMneumonic(); }
			set
			{
				AccessText.Text = value.ToWpfMneumonic();
				Control.Header = string.IsNullOrEmpty(value) ? null : Header;
			}
		}
	}
}
#endif