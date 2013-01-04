using System;
using SD = System.Drawing;
using Eto.Drawing;
using Eto.Forms;
using MonoMac.AppKit;
using MonoMac.Foundation;

namespace Eto.Platform.Mac.Forms
{
	public class DialogHandler : MacWindow<MyWindow, Dialog>, IDialog
	{
		Button button;
		MacModal.ModalHelper session;
		
		class DialogWindow : MyWindow {
			public new DialogHandler Handler
			{
				get { return base.Handler as DialogHandler; }
				set { base.Handler = value; }
			}
			
			public DialogWindow()
				: base(new SD.Rectangle(0,0,200,200), NSWindowStyle.Closable | NSWindowStyle.Titled, NSBackingStore.Buffered, false)
			{
			}
			
			[Export("cancelOperation:")]
			public void CancelOperation(IntPtr sender)
			{
				if (Handler.AbortButton != null)
					Handler.AbortButton.OnClick (EventArgs.Empty);
			}
		}

		public DialogDisplayMode DisplayMode { get; set; }

		public Button AbortButton { get; set; }
		
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
		{
			this.DisposeControl = false;
			var dlg = new DialogWindow();
			dlg.Handler = this;
			Control = dlg;
			ConfigureWindow ();
		}
		
		public DialogResult ShowDialog (Control parent)
		{
			if (parent != null) {
				if (parent.ControlObject is NSWindow) Control.ParentWindow = (NSWindow)parent.ControlObject;
				else if (parent.ControlObject is NSView) Control.ParentWindow = ((NSView)parent.ControlObject).Window;
			}
			Control.MakeKeyWindow ();
			
			Widget.Closed += HandleClosed;
			switch (DisplayMode) {
			case DialogDisplayMode.Attached:
				MacModal.RunSheet (Control, out session);
				break;
			default:
			case DialogDisplayMode.Default:
			case DialogDisplayMode.Separate:
				MacModal.Run (Control, out session);
				break;
			}
			return Widget.DialogResult;
		}
		
        public void Invalidate(Rectangle rect)
        {
            throw new NotImplementedException();
        }
		void HandleClosed (object sender, EventArgs e)
		{
			if (session != null)
				session.Stop ();
			Widget.Closed -= HandleClosed;
		}
		
		public override void Close ()
		{
			if (session != null && session.IsSheet) {
				session.Stop ();
			}
			else
				base.Close ();
		}
		
    }
}
