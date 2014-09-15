using Eto.Drawing;
using Eto.Forms;
using Eto.Wpf.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sw = System.Windows;
using swc = System.Windows.Controls;
using swd = System.Windows.Documents;
using swm = System.Windows.Media;

namespace Eto.Wpf.Forms.Controls
{
	public class LinkButtonHandler : WpfFrameworkElement<swc.TextBlock, LinkButton, LinkButton.ICallback>, LinkButton.IHandler
	{
		public swd.Hyperlink Hyperlink { get; private set; }

		public LinkButtonHandler()
		{
			Hyperlink = new swd.Hyperlink();
			Control = new swc.TextBlock { Inlines = { Hyperlink }, TextWrapping = sw.TextWrapping.Wrap };
		}

		public override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			SetStyle(true);
		}

		void SetStyle(bool force = false)
		{
			if (Widget.Loaded || force)
			{
				var style = new sw.Style();
				if (textColor != null)
				{
					style.Setters.Add(new sw.Setter(swd.Hyperlink.ForegroundProperty, textColor.Value.ToWpfBrush()));
				}
				/**/
				style.Triggers.Add(new sw.Trigger
				{
					Property = swd.Hyperlink.IsMouseOverProperty,
					Value = true,
					Setters = { new sw.Setter(swd.Hyperlink.ForegroundProperty, (textColor ?? TextColor).ToWpfBrush()) }
				});
				style.Triggers.Add(new sw.Trigger
				{
					Property = swd.Hyperlink.IsEnabledProperty,
					Value = false,
					Setters = { new sw.Setter(swd.Hyperlink.ForegroundProperty, DisabledTextColor.ToWpfBrush()) }
				});
				/**
				style.Triggers.Add(new sw.Trigger
				{
					Property = swd.Hyperlink.IsMouseCaptureWithinProperty,
					Value = true,
					Setters = { new sw.Setter(swd.Hyperlink.ForegroundProperty, Colors.Red.ToWpfBrush()) }
				});/**/
				Hyperlink.Style = style;
			}
		}

		Color? textColor;
		public Color TextColor
		{
			get { return textColor ?? Hyperlink.Foreground.ToEtoColor(); }
			set
			{
				if (textColor != value)
				{
					textColor = value;
					SetStyle();
				}
			}
		}

		public override Color BackgroundColor
		{
			get { return Control.Background.ToEtoColor(); }
			set { Control.Background = value.ToWpfBrush(); }
		}

		public override bool Enabled
		{
			get { return Hyperlink.IsEnabled; }
			set { Hyperlink.IsEnabled = value; }
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
					FontHandler.Apply(Control, null, value);
				}
			}
		}

		public string Text
		{
			get { return Control.Text; }
			set
			{
				Hyperlink.Inlines.Clear();
				Hyperlink.Inlines.Add(value);
			}
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case LinkButton.ClickEvent:
					Hyperlink.Click += (sender, e) => Callback.OnClick(Widget, EventArgs.Empty);
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		static readonly object DisabledTextColorKey = new object();

		public Color DisabledTextColor
		{
			get { return Widget.Properties.Get<Color?>(DisabledTextColorKey) ?? sw.SystemColors.GrayTextColor.ToEto(); }
			set
			{
				if (value != DisabledTextColor)
				{
					Widget.Properties[DisabledTextColorKey] = value;
					SetStyle();
				}
			}
		}
	}
}
