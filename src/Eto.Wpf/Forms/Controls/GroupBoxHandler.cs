using Eto.Wpf.Drawing;

namespace Eto.Wpf.Forms.Controls
{
	public class EtoGroupBox : swc.GroupBox, IEtoWpfControl
	{
		public IWpfFrameworkElement Handler { get; set; }

		protected override sw.Size MeasureOverride(sw.Size constraint)
		{
			return Handler?.MeasureOverride(constraint, base.MeasureOverride) ?? base.MeasureOverride(constraint);
		}
	}

	public class GroupBoxHandler : WpfPanel<swc.GroupBox, GroupBox, GroupBox.ICallback>, GroupBox.IHandler
	{
		public swc.Label Header { get; set; }
		swc.AccessText AccessText => (swc.AccessText)Header.Content;

		public GroupBoxHandler()
		{
			Control = new EtoGroupBox { Handler = this };
			Header = new swc.Label { Content = new swc.AccessText(), Padding = new sw.Thickness(0) };
			// Only attach the header when there is title text. Leaving Header null
			// when empty makes GroupBox.HasHeader false, so the template (and the
			// default WPF GroupBox template used by the other themes) collapses the
			// header area — keeping the content padding symmetric on all sides.
			UpdateHeader();
		}

		void UpdateHeader()
		{
			Control.Header = string.IsNullOrEmpty(AccessText.Text) ? null : Header;
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

		static readonly object Font_Key = new object();

		public Font Font
		{
			get => Widget.Properties.Get<Font>(Font_Key) ?? Widget.Properties.Create(Font_Key, () => Header.GetEtoFont());
			set
			{
				Widget.Properties.Set(Font_Key, Header.SetEtoFont(value, r => AccessText.TextDecorations = r));
				UpdatePreferredSize();
			}
		}

		public string Text
		{
			get { return AccessText.Text.ToEtoMnemonic(); }
			set
			{
				AccessText.Text = value.ToPlatformMnemonic();
				UpdateHeader();
				UpdatePreferredSize();
			}
		}

		public Color TextColor
		{
			get { return AccessText.Foreground.ToEtoColor(); }
			set { AccessText.Foreground = value.ToWpfBrush(AccessText.Foreground); }
		}
	}
}
