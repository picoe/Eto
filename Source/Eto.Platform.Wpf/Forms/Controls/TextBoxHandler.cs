using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using swc = System.Windows.Controls;
using sw = System.Windows;
using mwc = Microsoft.Windows.Controls;
using Eto.Forms;

namespace Eto.Platform.Wpf.Forms.Controls
{
	public class TextBoxHandler : WpfControl<mwc.WatermarkTextBox, TextBox>, ITextBox
	{
		bool textChanging;

		public TextBoxHandler ()
		{
			Control = new mwc.WatermarkTextBox { Width = 80 };
		}

		public override void AttachEvent (string handler)
		{
			switch (handler) {
				case TextBox.TextChangedEvent:
					Control.TextChanged += delegate {
						if (!textChanging)
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

		public int MaxLength
		{
			get { return Control.MaxLength; }
			set { Control.MaxLength = value; }
		}

		public string Text
		{
			get { return Control.Text; }
			set {
				textChanging = true;
				Control.Text = value;
				if (value != null)
					Control.CaretIndex = value.Length;
				textChanging = false;
			}
		}

		public string PlaceholderText
		{
			get { return Control.Watermark as string; }
			set { Control.Watermark = value; }
		}
	}
}
