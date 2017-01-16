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

		protected override bool UseContentSize => false;

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
			set
			{
				font = Header.SetEtoFont(value, r => AccessText.TextDecorations = r);
				UpdatePreferredSize();
			}
		}

		public string Text
		{
			get { return AccessText.Text.ToEtoMnemonic(); }
			set
			{
				AccessText.Text = value.ToPlatformMnemonic();
				Control.Header = string.IsNullOrEmpty(value) ? null : Header;
				UpdatePreferredSize();
			}
		}

		public Color TextColor
		{
			get { return AccessText.Foreground.ToEtoColor(); }
			set { AccessText.Foreground = value.ToWpfBrush(AccessText.Foreground); }
		}

		protected override sw.Size GetContentPadding(sw.Size constraint)
		{
			sw.Size basePadding = base.GetContentPadding(constraint);

			double headerHeight = 2;

			sw.UIElement header = Control.Header as sw.UIElement;
			if (header != null)
			{
				header.Measure(constraint.Subtract(basePadding));
				// The default template for the GroupBox wraps the header in a border with Padding="3,1,3,0".
				// Note: There is a slight error in this calculation. The actual header height is less the a pixel smaller than the calculated value.
				headerHeight = header.DesiredSize.Height + 1;
			}

			// The default template for the GroupBox is a 4x4 grid with reserved 6 pixels on the left, right and bottom sides.
			sw.Size contentPadding = new sw.Size(basePadding.Width + 2*6, basePadding.Height + headerHeight + 6);

			return contentPadding;
		}
	}
}
