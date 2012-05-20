using System;
using Eto.Drawing;
using Eto.Forms;
using MonoMac.AppKit;
using SD = System.Drawing;

namespace Eto.Platform.Mac.Forms
{
	public class FormHandler : MacWindow<MyWindow, Form>, IDisposable, IForm
	{
		bool centered;

		public FormHandler()
		{
			Control = new MyWindow(new SD.Rectangle(0,0,200,200), 
				NSWindowStyle.Resizable | NSWindowStyle.Closable | NSWindowStyle.Miniaturizable | NSWindowStyle.Titled, 
				NSBackingStore.Buffered, false);
			ConfigureWindow ();
		}
		
		public override Point Location {
			get { return base.Location; }
			set {
				base.Location = value;
				centered = true;
			}
		}

		public void Show()
		{
			if (!centered) { Control.Center(); centered = true; }
			Control.MakeKeyAndOrderFront(ApplicationHandler.Instance.AppDelegate);
		}

		
	}
}
