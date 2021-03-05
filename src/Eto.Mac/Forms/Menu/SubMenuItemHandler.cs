using System;
using Eto.Forms;
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

namespace Eto.Mac.Forms.Menu
{
	class EtoSubMenuDelegate : NSMenuDelegate
	{
		WeakReference handler;
		public SubMenuItemHandler Handler
		{
			get { return (SubMenuItemHandler)handler.Target; }
			set { handler = new WeakReference(value); }
		}

		public override void MenuWillHighlightItem(NSMenu menu, NSMenuItem item)
		{
		}

		public override void MenuWillOpen(NSMenu menu)
		{
			Handler?.OnOpening();
		}

		public override void MenuDidClose(NSMenu menu)
		{
			var h = Handler;
			if (h == null)
				return;
			h.OnClosing();

			Application.Instance.AsyncInvoke(() => h.OnClosed());
		}
	}

	public class SubMenuItemHandler : ButtonMenuItemHandler<SubMenuItem, SubMenuItem.ICallback>, SubMenuItem.IHandler
	{
		protected override void Initialize()
		{
			base.Initialize();
			EnsureSubMenu();
			Control.Submenu.Delegate = new EtoSubMenuDelegate { Handler = this };
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case SubMenuItem.OpeningEvent:
				case SubMenuItem.ClosingEvent:
				case SubMenuItem.ClosedEvent:
					// handled intrinsically by delegate
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		public virtual void OnClosed()
		{
			Callback.OnClosed(Widget, EventArgs.Empty);
		}

		public virtual void OnClosing()
		{
			Callback.OnClosing(Widget, EventArgs.Empty);
		}

		public virtual void OnOpening()
		{
			Callback.OnOpening(Widget, EventArgs.Empty);
		}

		public override void RemoveMenu(MenuItem item)
		{
			Control.Submenu.RemoveItem((NSMenuItem)item.ControlObject);
		}

		public override void Clear()
		{
			Control.Submenu.RemoveAllItems();
		}
	}
}
