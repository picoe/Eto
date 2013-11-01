using SD = System.Drawing;
using swf = System.Windows.Forms;
using Eto.Forms;

namespace Eto.Platform.Windows
{
	public class PasswordBoxHandler : WindowsControl<swf.TextBox, PasswordBox>, IPasswordBox
	{
		public PasswordBoxHandler()
		{
			Control = new swf.TextBox();
			Control.UseSystemPasswordChar = true;
		}

		public bool ReadOnly
		{
			get { return Control.ReadOnly; }
			set { Control.ReadOnly = value; }
		}

		public int MaxLength
		{
			get { return Control.MaxLength; }
			set { Control.MaxLength = value; }
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
