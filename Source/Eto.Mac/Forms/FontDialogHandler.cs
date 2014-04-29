using System;
using MonoMac.AppKit;
using Eto.Forms;
using MonoMac.ObjCRuntime;
using Eto.Drawing;
using MonoMac.Foundation;
using Eto.Mac.Drawing;

namespace Eto.Mac.Forms
{
	class FontDialogHelper : NSWindowDelegate
	{
		public static FontDialogHelper Instance { get; set; }

		public FontDialogHandler Handler { get; set; }

		[Export("changeFont:")]
		public void ChangeFont(NSFontManager sender)
		{
			var font = sender.ConvertFont(NSFont.SystemFontOfSize(NSFont.SystemFontSize));
			Handler.Font = font != null ? new Font(Handler.Widget.Platform, new FontHandler(font)) : null;
			Handler.Widget.OnFontChanged(EventArgs.Empty);
		}

		public override void WillClose(NSNotification notification)
		{
			Handler.Manager.Target = null;
			Handler.Manager.Action = null;
			FontDialogHelper.Instance = null;
		}

		public override void DidResignKey(NSNotification notification)
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
		public void ModalClosed(NSNotification notification)
		{
			Handler.Control.PerformClose(this);
			NSNotificationCenter.DefaultCenter.RemoveObserver(this);
		}
	}

	public class FontDialogHandler : MacObject<NSFontPanel, FontDialog>, IFontDialog
	{
		public NSFontManager Manager
		{
			get { return NSFontManager.SharedFontManager; }
		}

		public FontDialogHandler()
		{
			Control = NSFontPanel.SharedFontPanel;
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case FontDialog.FontChangedEvent:
				// handled by helper
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		public DialogResult ShowDialog(Window parent)
		{
			NSWindow parentWindow;
			if (parent != null)
			{
				parentWindow = parent.ParentWindow.ControlObject as NSWindow ?? NSApplication.SharedApplication.KeyWindow;
				if (parentWindow != null)
					Control.ParentWindow = parentWindow;
			}
			else
				parentWindow = NSApplication.SharedApplication.KeyWindow;

			FontDialogHelper.Instance = new FontDialogHelper { Handler = this };

			Manager.Target = null;
			Manager.Action = null;
			if (Font != null)
			{
				var fontHandler = (FontHandler)Font.Handler;
				Manager.SetSelectedFont(fontHandler.Control, false);
			}
			else
				Manager.SetSelectedFont(NSFont.SystemFontOfSize(NSFont.SystemFontSize), false);

			Control.Delegate = FontDialogHelper.Instance;
			Manager.Target = FontDialogHelper.Instance;
			Manager.Action = new Selector("changeFont:");

			if (parentWindow != null)
			{
				if (parentWindow == NSApplication.SharedApplication.ModalWindow)
				{
					NSNotificationCenter.DefaultCenter.AddObserver(FontDialogHelper.Instance, new Selector("modalClosed:"), new NSString("NSWindowWillCloseNotification"), parentWindow);
				}
			}
			
			Manager.OrderFrontFontPanel(parentWindow);
			//if (isModal) Control.MakeKeyWindow();
			Control.MakeKeyAndOrderFront(parentWindow);

			return DialogResult.None; // signal that we are returning right away!
		}

		public Font Font
		{
			get;
			set;
		}
	}
}

