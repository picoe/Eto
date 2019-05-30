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
		public static FontDialogHelper Instance { get; set; }

		WeakReference handler;
		public FontDialogHandler Handler
		{
			get => handler?.Target as FontDialogHandler;
			set => handler = new WeakReference(value);
		}

		public NSFont Font { get; set; } = NSFont.UserFontOfSize(NSFont.SystemFontSize);

		[Export("changeFont:")]
		public void ChangeFont(NSFontManager sender)
		{
			var h = Handler;
			if (h == null)
			{
				Cleanup();
				return;
			}
			Font = sender.ConvertFont(Font);
			h.Font = Font != null ? new Font(new FontHandler(Font)) : null;
			h.Callback.OnFontChanged(h.Widget, EventArgs.Empty);
		}

		public override void WillClose(NSNotification notification) => Cleanup();

		void Cleanup()
		{
			NSFontManager.SharedFontManager.WeakDelegate = null;
			NSFontPanel.SharedFontPanel.Delegate = null;
			FontDialogHelper.Instance = null;
		}

		public override void DidResignKey(NSNotification notification)
		{
			Handler?.ClosePanel();
			Cleanup();
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
			NSNotificationCenter.DefaultCenter.RemoveObserver(this);
			Handler?.ClosePanel();
			Cleanup();
		}
	}

	public class FontDialogHandler : MacObject<NSFontPanel, FontDialog, FontDialog.ICallback>, FontDialog.IHandler
	{
		protected override NSFontPanel CreateControl() => NSFontPanel.SharedFontPanel;

		public bool ClosePanelWhenFinished { get; set; }

		protected override bool DisposeControl => false;

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
			var parentWindow = parent?.ParentWindow.ControlObject as NSWindow ?? NSApplication.SharedApplication.KeyWindow;
			if (parentWindow != null)
				Control.ParentWindow = parentWindow;

			var selectedFont = Font.ToNS() ?? NSFont.UserFontOfSize(NSFont.SystemFontSize);
			var helper = FontDialogHelper.Instance = new FontDialogHelper { Handler = this, Font = selectedFont };

			if (parentWindow != null && parentWindow == NSApplication.SharedApplication.ModalWindow)
			{
				NSNotificationCenter.DefaultCenter.AddObserver(helper, new Selector("modalClosed:"), new NSString("NSWindowWillCloseNotification"), parentWindow);
			}

			var manager = NSFontManager.SharedFontManager;

			manager.Target = null;
			manager.Action = new Selector("changeFont:"); // in case it was set to something else
			Control.Delegate = helper;
			manager.WeakDelegate = helper; // using the delegate makes it work with modal dialogs, see: https://stackoverflow.com/a/9506984/981187
			manager.SetSelectedFont(selectedFont, false);

			manager.OrderFrontFontPanel(parentWindow);
			Control.MakeKeyWindow(); // make key so when it loses key we can reset the delegate

			return DialogResult.None; // signal that we are returning right away!
		}

		internal void ClosePanel()
		{
			if (ClosePanelWhenFinished)
				Control.PerformClose(Control);
		}

		public Font Font { get; set; }
	}
}

