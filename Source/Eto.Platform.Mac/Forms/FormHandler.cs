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
			DisposeControl = false;
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

		protected override void PositionWindow ()
		{
			base.PositionWindow ();
			if (!centered) { Control.Center(); centered = true; }
		}

		public void Show ()
		{
			if (this.State == WindowState.Minimized)
				Control.MakeKeyWindow();
			else
				Control.MakeKeyAndOrderFront(ApplicationHandler.Instance.AppDelegate);
		}

		
	}
}
