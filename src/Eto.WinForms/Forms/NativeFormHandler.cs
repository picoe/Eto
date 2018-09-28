using System;
using Eto.Forms;
using Eto.Drawing;
using swf = System.Windows.Forms;

namespace Eto.WinForms.Forms
{
	public class NativeFormHandler : WindowHandler<swf.Form, Form, Form.ICallback>, Form.IHandler
	{
		public override bool IsAttached => true;

		public bool ShowActivated { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool CanFocus { get => Control.CanFocus; set => throw new NotImplementedException(); }

		public NativeFormHandler(swf.Form form)
		{
			Control = form;
		}
		public void Show() => Control.Show();
	}
}
