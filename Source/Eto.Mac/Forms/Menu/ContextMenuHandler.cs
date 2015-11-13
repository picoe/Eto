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
	class ContextHandler : NSMenuDelegate
	{
		public static ContextHandler Instance
		{
			get;
			set;
		}

		WeakReference handler;
		public ContextMenuHandler Handler
		{
			get
			{
				return (ContextMenuHandler)handler.Target;
			}
			set
			{
				handler = new WeakReference(value);
			}
		}

		public override void MenuWillHighlightItem(NSMenu menu, NSMenuItem item)
		{

		}

		[Export("menuWillOpen:")]
		public void HandleMenuWillOpen(NSMenu menu)
		{
			Handler.Callback.OnMenuOpening(Handler.Widget, EventArgs.Empty);
		}
	}

	public class ContextMenuHandler : WidgetHandler<NSMenu, ContextMenu, ContextMenu.ICallback>, ContextMenu.IHandler
	{
		protected override NSMenu CreateControl()
		{
			return new NSMenu();
		}

		protected override void Initialize()
		{
			Control.AutoEnablesItems = false;
			Control.ShowsStateColumn = true;
			ContextHandler.Instance = new ContextHandler()
			{
				Handler = this
			};

			Control.Delegate = ContextHandler.Instance;

			base.Initialize();
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case ContextMenu.MenuOpeningEvent:
					// handled by delegate
					break;

				default:
					base.AttachEvent(id);
					break;
			}
		}

		public void AddMenu (int index, MenuItem item)
		{
			Control.InsertItem ((NSMenuItem)item.ControlObject, index);
		}

		public void RemoveMenu (MenuItem item)
		{
			Control.RemoveItem ((NSMenuItem)item.ControlObject);
		}

		public void Clear ()
		{
			Control.RemoveAllItems ();
		}
		
		public void Show (Control relativeTo)
		{
			NSEvent nsevent = NSApplication.SharedApplication.CurrentEvent;
			if (nsevent == null) {
				var location = NSEvent.CurrentMouseLocation;
				location = NSApplication.SharedApplication.KeyWindow.ConvertScreenToBase (location);
				
				var time = DateTime.Now.ToOADate();
				var windowNumber = NSApplication.SharedApplication.KeyWindow.WindowNumber;
				
				nsevent = NSEvent.MouseEvent(NSEventType.RightMouseDown, location, 0, time, windowNumber, null, 0, 0, 0.1f);
			}
			var view = relativeTo != null ? relativeTo.ControlObject as NSView : null;
			NSMenu.PopUpContextMenu(Control, nsevent, view);
		}

	}
}

