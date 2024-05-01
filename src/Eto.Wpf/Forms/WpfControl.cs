﻿using Eto.Wpf.Drawing;

namespace Eto.Wpf.Forms
{
	public class WpfControl<TControl, TWidget, TCallback> : WpfFrameworkElement<TControl, TWidget, TCallback>, Control.IHandler
		where TControl : swc.Control
		where TWidget: Control
		where TCallback: Control.ICallback
	{
		public override Color BackgroundColor
		{
			get { return (ContainerControl as swc.Control ?? Control).Background.ToEtoColor(); }
			set { (ContainerControl as swc.Control ?? Control).Background = value.ToWpfBrush(Control.Background); }
		}

		public virtual bool ShowBorder
		{
			get { return true; }
			set { }
		}

		protected virtual void SetDecorations(sw.TextDecorationCollection decorations)
		{
		}

		static readonly object FontKey = new object();

		public Font Font
		{
			get { return Widget.Properties.Create<Font>(FontKey, () => new Font(new FontHandler(Control))); }
			set
			{
				if (Widget.Properties.Get<Font>(FontKey) != value)
				{
					Widget.Properties[FontKey] = value;
					Control.SetEtoFont(value, SetDecorations);
				}
			}
		}

		public virtual Color TextColor
		{
			get { return Control.Foreground.ToEtoColor(); }
			set { Control.Foreground = value.ToWpfBrush(Control.Foreground); }
		}

		public override sw.FrameworkElement TabControl => Control;
	}
}
