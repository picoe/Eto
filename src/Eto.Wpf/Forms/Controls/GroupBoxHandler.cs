using swc = System.Windows.Controls;
using sw = System.Windows;
using swd = System.Windows.Data;
using Eto.Forms;
using Eto.Drawing;
using Eto.Wpf.Drawing;
using System;

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
		swc.Label Header { get; set; }
		swc.AccessText AccessText { get { return (swc.AccessText)Header.Content; } }

		public GroupBoxHandler()
		{
			Control = new EtoGroupBox { Handler = this };
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
				Control.Header = string.IsNullOrEmpty(value) ? null : Header;
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
