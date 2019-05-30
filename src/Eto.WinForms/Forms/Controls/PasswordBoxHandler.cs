using SD = System.Drawing;
using swf = System.Windows.Forms;
using System.Linq;
using Eto.Forms;

namespace Eto.WinForms.Forms.Controls
{
	public class PasswordBoxHandler : WindowsControl<swf.TextBox, PasswordBox, PasswordBox.ICallback>, PasswordBox.IHandler
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

		static readonly Win32.WM[] intrinsicEvents = {
														 Win32.WM.LBUTTONDOWN, Win32.WM.LBUTTONUP, Win32.WM.LBUTTONDBLCLK,
														 Win32.WM.RBUTTONDOWN, Win32.WM.RBUTTONUP, Win32.WM.RBUTTONDBLCLK
													 };
		public override bool ShouldBubbleEvent(swf.Message msg)
		{
			return !intrinsicEvents.Contains((Win32.WM)msg.Msg) && base.ShouldBubbleEvent(msg);
		}
	}
}
