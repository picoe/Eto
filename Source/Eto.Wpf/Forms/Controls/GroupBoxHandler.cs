using swc = System.Windows.Controls;
using sw = System.Windows;
using swd = System.Windows.Data;
using Eto.Forms;
using Eto.Drawing;
using Eto.Wpf.Drawing;

namespace Eto.Wpf.Forms.Controls
{
	public class GroupBoxHandler : WpfPanel<swc.GroupBox, GroupBox, GroupBox.ICallback>, GroupBox.IHandler
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
			set { font = Header.SetEtoFont(value, r => AccessText.TextDecorations = r); }
		}

		public string Text
		{
			get { return AccessText.Text.ToEtoMnemonic(); }
			set
			{
				AccessText.Text = value.ToPlatformMnemonic();
				Control.Header = string.IsNullOrEmpty(value) ? null : Header;
			}
		}

		public Color TextColor
		{
			get { return AccessText.Foreground.ToEtoColor(); }
			set { AccessText.Foreground = value.ToWpfBrush(AccessText.Foreground); }
		}
	}
}
