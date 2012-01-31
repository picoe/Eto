using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using swc = System.Windows.Controls;
using Eto.Forms;

namespace Eto.Platform.Wpf.Forms.Controls
{
	public class TextBoxHandler : WpfControl<swc.TextBox, TextBox>, ITextBox
	{
		public TextBoxHandler ()
		{
			Control = new swc.TextBox ();
		}

		public override void AttachEvent (string handler)
		{
			switch (handler) {
				case TextBox.TextChangedEvent:
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

		public int MaxLength
		{
			get { return Control.MaxLength; }
			set { Control.MaxLength = value; }
		}

		public string Text
		{
			get { return Control.Text; }
			set { Control.Text = value; }
		}
	}
}
