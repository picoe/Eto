using System;
using Eto.Forms;
using Eto.Drawing;
using Eto.Mac.Drawing;
#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
#endif

namespace Eto.Mac.Forms
{
	class FontDialogHelper : NSWindowDelegate
	{
		NSFont font = NSFont.SystemFontOfSize(NSFont.SystemFontSize);

		public static FontDialogHelper Instance { get; set; }

		WeakReference handler;
		public FontDialogHandler Handler
		{
			get => handler?.Target as FontDialogHandler;
			set => handler = new WeakReference(value);
		}

		public NSFont Font
		{
			get => font;
			set => font = value;
		}


		[Export("changeFont:")]
		public void ChangeFont(NSFontManager sender)
		{
			var h = Handler;
			if (h == null)
				return;
			font = sender.ConvertFont(font);
			h.Font = font != null ? new Font(new FontHandler(font)) : null;
			h.Callback.OnFontChanged(h.Widget, EventArgs.Empty);
		}

		public override void WillClose(NSNotification notification)
		{
			var h = Handler;
			if (h == null)
				return;
			h.Manager.Target = null;
			h.Manager.Action = null;
			FontDialogHelper.Instance = null;
		}

		public override void DidResignKey(NSNotification notification)
		{
			Handler?.Control.PerformClose(this);
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
			Handler?.Control.PerformClose(this);
			NSNotificationCenter.DefaultCenter.RemoveObserver(this);
		}
	}

	public class FontDialogHandler : MacObject<NSFontPanel, FontDialog, FontDialog.ICallback>, FontDialog.IHandler
	{
		public NSFontManager Manager
		{
			get { return NSFontManager.SharedFontManager; }
		}

		protected override NSFontPanel CreateControl()
		{
			return NSFontPanel.SharedFontPanel;
		}

		protected override bool DisposeControl { get { return false; } }

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

			NSFont selectedFont = Font.ToNS() ?? NSFont.SystemFontOfSize(NSFont.SystemFontSize);

			FontDialogHelper.Instance = new FontDialogHelper { Handler = this, Font = selectedFont };

			Manager.Target = null;
			Manager.Action = null;
			Manager.SetSelectedFont(selectedFont, false);

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

