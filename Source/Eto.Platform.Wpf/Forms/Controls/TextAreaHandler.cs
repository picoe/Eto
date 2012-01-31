using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using swc = System.Windows.Controls;
using sw = System.Windows;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Platform.Wpf.Forms.Controls
{
	public class TextAreaHandler : WpfControl<swc.TextBox, TextArea>, ITextArea
	{
		public TextAreaHandler ()
		{
			Control = new swc.TextBox {
				AcceptsReturn = true,
				AcceptsTab = true,
				HorizontalScrollBarVisibility = swc.ScrollBarVisibility.Auto,
				VerticalScrollBarVisibility = swc.ScrollBarVisibility.Auto
			};
		}

		public override void AttachEvent (string handler)
		{
			switch (handler) {
				case TextArea.TextChangedEvent:
					Control.TextChanged += delegate {
						Widget.OnTextChanged (EventArgs.Empty);
					};
					break;
				default:
					base.AttachEvent (handler);
					break;
			}
		}

		public bool ReadOnly
		{
			get { return Control.IsReadOnly; }
			set { Control.IsReadOnly = value; }
		}

		public override Size Size
		{
			get
			{
				return new Size ((int)Control.MinWidth, (int)Control.Height);
			}
			set
			{
				Control.MinWidth = value.Width; Control.Height = value.Height;
			}
		}

		public void Append (string text, bool scrollToCursor)
		{
			Control.AppendText (text);
			if (scrollToCursor) Control.ScrollToEnd ();
		}

		public string Text
		{
			get	{ return Control.Text; }
			set	{ Control.Text = value;	}
		}

		public bool Wrap
		{
			get { return Control.TextWrapping == sw.TextWrapping.Wrap; }
			set	{
				Control.TextWrapping = value ? sw.TextWrapping.Wrap : sw.TextWrapping.NoWrap;
			}
		}
	}
}
