using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using swc = System.Windows.Controls;
using sw = System.Windows;
using Eto.Forms;

namespace Eto.Platform.Wpf.Forms.Controls
{
	public class PasswordBoxHandler : WpfControl<swc.PasswordBox, PasswordBox>, IPasswordBox
	{
		public PasswordBoxHandler()
		{
			Control = new swc.PasswordBox { Width = 80 };
		}

		public override void AttachEvent (string handler)
		{
			switch (handler) {
				case TextBox.TextChangedEvent:
					Control.PasswordChanged += delegate {
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
			get { return false; }
			set { }
		}

		public char PasswordChar
		{
			get { return Control.PasswordChar; }
			set { Control.PasswordChar = value; }
		}

		public int MaxLength
		{
			get { return Control.MaxLength; }
			set { Control.MaxLength = value; }
		}

		public string Text
		{
			get { return Control.Password; }
			set { Control.Password = value; }
		}
	}
}
