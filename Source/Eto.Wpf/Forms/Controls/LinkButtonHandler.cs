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
			Control = new swc.TextBlock { Inlines = { Hyperlink } };
			SetStyle();
		}

		void SetStyle()
		{
			var style = new sw.Style();
			if (textColor != null)
			{
				style.Setters.Add(new sw.Setter(swd.Hyperlink.ForegroundProperty, textColor.Value.ToWpfBrush()));
			}
			/*
				Triggers =
				{
					new sw.Trigger
					{
						Property = swd.Hyperlink.IsMouseOverProperty,
						Value = true,
						Setters =
						{
							new sw.Setter(swd.Hyperlink.ForegroundProperty, Colors.Green.ToWpfBrush()) 
						}
					}
				}
			};*/
			Hyperlink.Style = style;
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

		Font font;
		public Font Font
		{
			get { return font ?? (font = new Font(new FontHandler(Control))); }
			set
			{
				font = value;
				FontHandler.Apply(Control, null, font);
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
	}
}
