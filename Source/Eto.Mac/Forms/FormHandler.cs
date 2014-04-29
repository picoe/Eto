using System;
using Eto.Forms;
using MonoMac.AppKit;
using SD = System.Drawing;

namespace Eto.Mac.Forms
{
	public class FormHandler : MacWindow<MyWindow, Form>, IForm
	{
		protected override bool DisposeControl { get { return false; } }

		public FormHandler()
		{
			Control = new MyWindow(new SD.Rectangle(0, 0, 200, 200), 
			                       NSWindowStyle.Resizable | NSWindowStyle.Closable | NSWindowStyle.Miniaturizable | NSWindowStyle.Titled, 
			                       NSBackingStore.Buffered, false);
			ConfigureWindow();
		}

		public void Show()
		{
			if (WindowState == WindowState.Minimized)
				Control.MakeKeyWindow();
			else
				Control.MakeKeyAndOrderFront(ApplicationHandler.Instance.AppDelegate);
			if (!Control.IsVisible)
				Widget.OnShown(EventArgs.Empty);
		}
	}
}
