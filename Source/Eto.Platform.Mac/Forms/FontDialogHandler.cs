using System;
using MonoMac.AppKit;
using Eto.Forms;
using MonoMac.ObjCRuntime;
using Eto.Drawing;
using MonoMac.Foundation;
using Eto.Platform.Mac.Drawing;

namespace Eto.Platform.Mac.Forms
{

	class FontDialogHelper : NSWindowDelegate
	{
		public static FontDialogHelper Instance { get; set; }

		public FontDialogHandler Handler { get; set; }
		
		[Export("changeFont:")]
		public void changeFont (NSFontManager sender)
		{
			var font = sender.ConvertFont (NSFont.SystemFontOfSize (NSFont.SystemFontSize));
			if (font != null) {
				var traits = sender.TraitsOfFont (font);
				Handler.Font = new Font (Handler.Widget.Generator, new FontHandler (font, traits));
			} else {
				Handler.Font = null;
			}
			Handler.Widget.OnFontChanged (EventArgs.Empty);
		}
		
		public override void WillClose (NSNotification notification)
		{
			Handler.Manager.Target = null;
			Handler.Manager.Action = null;
			FontDialogHelper.Instance = null;
		}

		public override void DidResignKey (NSNotification notification)
		{
			Handler.Control.PerformClose(this);
		}

		[Export("changeAttributes:")]
		void ChangeAttributes(NSObject sender)
		{
		}

		[Export("validModesForFontPanel:")]
		NSFontPanelMode ValidModesForFontPanel(NSFontPanel fontPanel)
		{
			return NSFontPanelMode.SizeMask | NSFontPanelMode.FaceMask | NSFontPanelMode.CollectionMask;
		}
		
		[Export("modalClosed:")]
		public void modalClosed(NSNotification notification)
		{
			Handler.Control.PerformClose (this);
			Console.WriteLine ("Closing!");
			NSNotificationCenter.DefaultCenter.RemoveObserver (this);
		}
	}

	public class FontDialogHandler : MacObject<NSFontPanel, FontDialog>, IFontDialog
	{
		public NSFontManager Manager
		{
			get { return NSFontManager.SharedFontManager; }
		}

		public FontDialogHandler ()
		{
			Control = NSFontPanel.SharedFontPanel;
		}

		public override void AttachEvent (string id)
		{
			switch (id) {
			case FontDialog.FontChangedEvent:
				// handled by helper
				break;
			default:
				base.AttachEvent (id);
				break;
			}
		}

		public DialogResult ShowDialog (Window parent)
		{
			NSWindow parentWindow;
			if (parent != null) {
				if (parent.ControlObject is NSWindow)
					parentWindow = (NSWindow)parent.ControlObject;
				else if (parent.ControlObject is NSView)
					parentWindow = ((NSView)parent.ControlObject).Window;
				else
					parentWindow = NSApplication.SharedApplication.KeyWindow;
			} else
				parentWindow = NSApplication.SharedApplication.KeyWindow;

			FontDialogHelper.Instance = new FontDialogHelper{ Handler = this };

			Manager.Target = null;
			Manager.Action = null;
			if (Font != null) {
				var fontHandler = this.Font.Handler as FontHandler;
				Manager.SetSelectedFont (fontHandler.Control, false);
			}
			else
				Manager.SetSelectedFont (NSFont.SystemFontOfSize (NSFont.SystemFontSize), false);

			Control.Delegate = FontDialogHelper.Instance;
			Manager.Target = FontDialogHelper.Instance;
			Manager.Action = new Selector ("changeFont:");

			if (parentWindow != null) {
				if (parentWindow == NSApplication.SharedApplication.ModalWindow)
				{
					NSNotificationCenter.DefaultCenter.AddObserver(FontDialogHelper.Instance, new Selector("modalClosed:"), new NSString("NSWindowWillCloseNotification"), parentWindow);
				}
			}
			
			Manager.OrderFrontFontPanel (parentWindow);
			//if (isModal) Control.MakeKeyWindow();
			Control.MakeKeyAndOrderFront(parentWindow);

			return DialogResult.None; // signal that we are returning right away!
		}

		public Font Font
		{
			get; set;
		}
	}
}

