using System;
using Eto.Forms;
using Eto.Drawing;
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
	interface IColorDialogHandler
	{
		Color Color { get; set; }
		ColorDialog Widget { get; }
		NSColorPanel Control { get; }
		ColorDialog.ICallback Callback { get; }

		void OnWillClose(NSNotification notification);
		void OnDidResignKey(NSNotification notification);
	}

	class ColorHandler : NSWindowDelegate
	{
		public static ColorHandler Instance { get; set; }
		WeakReference handler;
		public IColorDialogHandler Handler { get { return (IColorDialogHandler)handler.Target; } set { handler = new WeakReference(value); } }

		[Export("changeColor:")]
		public void ChangeColor(NSColorPanel panel)
		{
			var h = Handler;
			if (h != null)
			{
				h.Color = panel.Color.UsingColorSpace(NSColorSpace.DeviceRGB).ToEto(false);
				h.Callback.OnColorChanged(h.Widget, EventArgs.Empty);
			}
			else
			{
				// the ColorDialog was probably GC'd, so unhook gracefully
				Instance = null;
			}
		}

		public override void WillClose(NSNotification notification)
		{
			Handler?.OnWillClose(notification);
			var control = Handler?.Control ?? NSColorPanel.SharedColorPanel;
			control.SetTarget(null);
			control.SetAction(null);
			Instance = null;
		}

		public override void DidResignKey(NSNotification notification)
		{
			Handler?.OnDidResignKey(notification);
		}

		[Export("modalClosed:")]
		public void ModalClosed(NSNotification notification)
		{
			var control = Handler?.Control ?? NSColorPanel.SharedColorPanel;
			control.PerformClose(this);
			NSNotificationCenter.DefaultCenter.RemoveObserver(this);
		}
	}

	public class ColorDialogHandler : MacObject<NSColorPanel, ColorDialog, ColorDialog.ICallback>, ColorDialog.IHandler, IColorDialogHandler
	{
		protected override NSColorPanel CreateControl()
		{
			return NSColorPanel.SharedColorPanel;
		}

		protected override bool DisposeControl { get { return false; } }

		protected override void Initialize()
		{
			Color = Colors.White;
			base.Initialize();
		}

		public Color Color { get; set; }

		public bool AllowAlpha { get; set; }

		public bool SupportsAllowAlpha => true;

		#region IDialog implementation

		public virtual DialogResult ShowDialog(Window parent)
		{
			//Control = new NSColorPanel();
			NSWindow parentWindow;
			//Console.WriteLine ("Parent: {0}. {1}, {2}", parent, parent.ControlObject, NSApplication.SharedApplication.ModalWindow);
			if (parent != null)
			{
				parentWindow = parent.ParentWindow.ControlObject as NSWindow ?? NSApplication.SharedApplication.KeyWindow;
				if (parentWindow != null)
					Control.ParentWindow = parentWindow;
			}
			else parentWindow = NSApplication.SharedApplication.KeyWindow;

			ColorHandler.Instance = new ColorHandler { Handler = this };
			Control.Delegate = ColorHandler.Instance;
			Control.SetTarget(null);
			Control.SetAction(null);
			Control.Color = Color.ToNSUI();

			Control.SetTarget(ColorHandler.Instance);
			Control.SetAction(new Selector("changeColor:"));
			Control.ShowsAlpha = AllowAlpha;

			//Control.Continuous = false;
			bool isModal = false;
			if (parentWindow != null)
			{
				if (parentWindow == NSApplication.SharedApplication.ModalWindow)
				{
					//Control.WorksWhenModal = true;
					//Control.ParentWindow = parentWindow;
					NSNotificationCenter.DefaultCenter.AddObserver(ColorHandler.Instance, new Selector("modalClosed:"), new NSString("NSWindowWillCloseNotification"), parentWindow);
					isModal = true;
				}
			}


			// work around for modal dialogs wanting to show the color panel.. only works when the panel is key

			//if (isModal) Control.MakeKeyAndOrderFront (parentWindow);
			//else Control.OrderFront (parentWindow);
			if (Control == NSColorPanel.SharedColorPanel)
				NSApplication.SharedApplication.OrderFrontColorPanel(parentWindow);
			else
				Control.OrderFront(parentWindow);

			if (isModal) Control.MakeKeyWindow();
			//Control.OrderFront (parentWindow);


			return DialogResult.None; // signal that we are returning right away!
		}

		protected virtual void OnWillClose(NSNotification notification)
		{
			// extension to do custom stuff when the panel is closed
		}

		void IColorDialogHandler.OnWillClose(NSNotification notification) => OnWillClose(notification);

		protected virtual void OnDidResignKey(NSNotification notification)
		{
			// extension to do custom stuff when the panel loses key focus
		}

		void IColorDialogHandler.OnDidResignKey(NSNotification notification) => OnDidResignKey(notification);

		#endregion
	}
}

