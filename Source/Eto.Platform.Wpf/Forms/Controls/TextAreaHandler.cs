using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WC = System.Windows.Controls;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Platform.Wpf.Forms.Controls
{
	public class TextAreaHandler : WpfControl<WC.TextBox, TextArea>, ITextArea
	{
		public TextAreaHandler ()
		{
			Control = new WC.TextBox ();
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
			get
			{
				return Control.Text;
			}
			set
			{
				Control.Text = value;
			}
		}
	}
}
