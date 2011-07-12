using System;
using Eto.Drawing;
using Eto.Forms;
using MonoMac.AppKit;

namespace Eto.Platform.Mac
{
	public class FormHandler : MacWindow<NSWindow, Form>, IDisposable, IForm
	{
		bool centered;

		public FormHandler()
			: base(NSWindowStyle.Resizable | NSWindowStyle.Closable | NSWindowStyle.Miniaturizable | NSWindowStyle.Titled)
		{
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
