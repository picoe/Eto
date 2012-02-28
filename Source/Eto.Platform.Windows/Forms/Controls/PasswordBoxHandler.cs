using System;
using SD = System.Drawing;
using SWF = System.Windows.Forms;
using Eto.Forms;

namespace Eto.Platform.Windows
{
	public class PasswordBoxHandler : WindowsControl<SWF.TextBox, PasswordBox>, IPasswordBox
	{
		public PasswordBoxHandler()
		{
			Control = new SWF.TextBox ();
			Control.UseSystemPasswordChar = true;
		}

		public bool ReadOnly {
			get { return Control.ReadOnly; }
			set { Control.ReadOnly = value; }
		}
		
		public int MaxLength {
			get { return this.Control.MaxLength; }
			set { this.Control.MaxLength = value; }
		}

		public char PasswordChar
		{
			get { return Control.PasswordChar; }
			set
			{
				if (value == '\0')
					Control.UseSystemPasswordChar = true;
				else
				{
					Control.UseSystemPasswordChar = false;
					Control.PasswordChar = value;
				}
			}
		}
	}
}
