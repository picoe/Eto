using Eto.Wpf.Drawing;
using swd = System.Windows.Documents;

namespace Eto.Wpf.Forms.Controls
{
	public class LinkButtonHandler : WpfFrameworkElement<swc.TextBlock, LinkButton, LinkButton.ICallback>, LinkButton.IHandler
	{
		public swd.Hyperlink Hyperlink { get; private set; }

		// note: a TextBlock seals MeasureOverride, so a link cannot get the extra width EtoAccessLabel adds to work
		// around WPF wrapping text that measures as exactly fitting under Display (GDI) rendering at some DPI scales.
		// Trimming, or Wrap = None, avoids that here instead.
		public LinkButtonHandler()
		{
			Hyperlink = new swd.Hyperlink();
			Control = new swc.TextBlock { Inlines = { Hyperlink }, TextWrapping = WrapMode.Word.ToWpf() };
		}

		public WrapMode Wrap
		{
			get => Control.TextWrapping.ToEto();
			set
			{
				if (value != Wrap)
				{
					Control.TextWrapping = value.ToWpf();
					UpdatePreferredSize();
				}
			}
		}

		public TextTrimming Trimming
		{
			get => Control.TextTrimming.ToEto();
			set
			{
				if (value != Trimming)
				{
					Control.TextTrimming = value.ToWpf();
					UpdatePreferredSize();
				}
			}
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

				// A trimming ellipsis is drawn by the TextBlock, from its own properties, not from the
				// Hyperlink whose text it is shortening - so unless the TextBlock carries the same colours
				// the ellipsis comes out in the default text colour beside link-coloured text.
				var textBlockStyle = new sw.Style();
				textBlockStyle.Setters.Add(new sw.Setter(swc.TextBlock.ForegroundProperty, (textColor ?? TextColor).ToWpfBrush()));
				textBlockStyle.Triggers.Add(new sw.Trigger
				{
					Property = swc.TextBlock.IsEnabledProperty,
					Value = false,
					Setters = { new sw.Setter(swc.TextBlock.ForegroundProperty, DisabledTextColor.ToWpfBrush()) }
				});
				Control.Style = textBlockStyle;
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

		static readonly object FontKey = new object();
		public Font Font
		{
			get { return Widget.Properties.Create<Font>(FontKey, () => new Font(new FontHandler(Control))); }
			set
			{
				if (Widget.Properties.Get<Font>(FontKey) != value)
				{
					Widget.Properties[FontKey] = value;
					Control.SetEtoFont(value);
				}
			}
		}

		public string Text
		{
			get { return Hyperlink.Inlines.GetText(); }
			set
			{
				Hyperlink.Inlines.Clear();
				if (value != null)
					Hyperlink.Inlines.Add(value);
			}
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case TextControl.TextChangedEvent:
					// doesn't change
					break;
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
