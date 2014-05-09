using System;
using swc = System.Windows.Controls;
using sw = System.Windows;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Wpf.Forms.Controls
{
	public class PasswordBoxHandler : WpfControl<swc.PasswordBox, PasswordBox, PasswordBox.ICallback>, PasswordBox.IHandler
	{
		protected override Size DefaultSize { get { return new Size(80, -1); } }

		public PasswordBoxHandler()
		{
			Control = new swc.PasswordBox();
		}

		public override sw.Size GetPreferredSize(sw.Size constraint)
		{
			return base.GetPreferredSize(Conversions.ZeroSize);
		}

		public override bool UseMousePreview { get { return true; } }

		public override bool UseKeyPreview { get { return true; } }

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case TextControl.TextChangedEvent:
					Control.PasswordChanged += delegate
					{
						Callback.OnTextChanged(Widget, EventArgs.Empty);
					};
					break;
				default:
					base.AttachEvent(id);
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
