using System;
using Eto.Drawing;
using Eto.Forms;
using MonoMac.AppKit;
using SD = System.Drawing;

namespace Eto.Platform.Mac.Forms
{
	public class FormHandler : MacWindow<MyWindow, Form>, IDisposable, IForm
	{
		public FormHandler()
		{
			DisposeControl = false;
			Control = new MyWindow(new SD.Rectangle(0,0,200,200), 
				NSWindowStyle.Resizable | NSWindowStyle.Closable | NSWindowStyle.Miniaturizable | NSWindowStyle.Titled, 
				NSBackingStore.Buffered, false);
			ConfigureWindow ();
		}
		

		public void Show ()
		{
			if (!Control.IsVisible)
				Widget.OnShown (EventArgs.Empty);
			if (this.WindowState == WindowState.Minimized)
				Control.MakeKeyWindow ();
			else
				Control.MakeKeyAndOrderFront (ApplicationHandler.Instance.AppDelegate);
		}
	}
}
