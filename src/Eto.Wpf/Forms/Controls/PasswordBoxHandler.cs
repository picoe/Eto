using System;
using swc = System.Windows.Controls;
using sw = System.Windows;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Wpf.Forms.Controls
{
	public class PasswordBoxHandler : WpfControl<swc.PasswordBox, PasswordBox, PasswordBox.ICallback>, PasswordBox.IHandler
	{
		swc.Border border;

		public override sw.FrameworkElement ContainerControl => border;

		protected override sw.Size DefaultSize => new sw.Size(80, double.NaN);

		protected override bool PreventUserResize { get { return true; } }

		public PasswordBoxHandler()
		{
			Control = new swc.PasswordBox();
			border = new EtoBorder { Handler = this, Child = Control };
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
