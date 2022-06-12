using swc = System.Windows.Controls;
using sw = System.Windows;
using swd = System.Windows.Data;
using Eto.Forms;
using Eto.Drawing;
using Eto.Wpf.Drawing;
using System;
using System.Windows;

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
		sw.Thickness? headerPadding;
		public swc.Label Header { get; set; }
		swc.AccessText AccessText => (swc.AccessText)Header.Content;

		public GroupBoxHandler()
		{
			Control = new EtoGroupBox { Handler = this };
			Control.Loaded += Control_Loaded;
			Header = new swc.Label { Content = new swc.AccessText(), Padding = new sw.Thickness(0) };
			Control.Header = Header;
		}

		private void Control_Loaded(object sender, RoutedEventArgs e)
		{
			SetHeaderPadding();
		}

		protected virtual swc.Border HeaderControl => Control.FindChild<swc.Border>("Header");

		private void SetHeaderPadding()
		{
			var header = HeaderControl;
			if (header == null)
				return;
			if (headerPadding == null)
				headerPadding = header.Padding;
			var noHeader = string.IsNullOrEmpty(Text);
			header.Padding = noHeader ? new sw.Thickness(0) : headerPadding.Value;
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
				if (Control.IsLoaded)
					SetHeaderPadding();
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
