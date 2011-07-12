using System;
using SD = System.Drawing;
using Eto.Drawing;
using Eto.Forms;
using MonoMac.AppKit;
using MonoMac.Foundation;

namespace Eto.Platform.Mac
{
	public class DialogHandler : MacWindow<NSWindow, Dialog>, IDialog
	{
		Button button;
		public Button DefaultButton
		{
			get { return button; }
			set
			{
				button = value;
				
				if (button != null) {
					var b = button.ControlObject as NSButton;
					if (b != null)
						Control.DefaultButtonCell = b.Cell;
					else
						Control.DefaultButtonCell = null;
				}
				else
					Control.DefaultButtonCell = null;
			}
		}

		public DialogHandler ()
			: base(NSWindowStyle.Closable | NSWindowStyle.Titled)
		{
		}
		
		public DialogResult ShowDialog (Control parent)
		{
			if (parent != null) {
				if (parent.ControlObject is NSWindow) Control.ParentWindow = (NSWindow)parent.ControlObject;
				else if (parent.ControlObject is NSView) Control.ParentWindow = ((NSView)parent.ControlObject).Window;
			}
			//Control.MakeKeyAndOrderFront (ApplicationHandler.Instance.AppDelegate);
			
			Control.WillClose += delegate {
				NSApplication.SharedApplication.StopModal();
			};
			NSApplication.SharedApplication.RunModalForWindow(Control);
			return Widget.DialogResult;
		}

	}
}
