using System;
using Eto.Forms;
using Eto.Drawing;
#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
using CoreImage;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
using MonoMac.CoreImage;
#if Mac64
using nfloat = System.Double;
using nint = System.Int64;
using nuint = System.UInt64;
#else
using nfloat = System.Single;
using nint = System.Int32;
using nuint = System.UInt32;
#endif
#if SDCOMPAT
using CGSize = System.Drawing.SizeF;
using CGRect = System.Drawing.RectangleF;
using CGPoint = System.Drawing.PointF;
#endif
#endif

namespace Eto.Mac.Forms.Menu
{
	class ContextHandler : NSMenuDelegate
	{
		WeakReference handler;
		public ContextMenuHandler Handler
		{
			get { return (ContextMenuHandler)handler.Target; }
			set { handler = new WeakReference(value); }
		}

		public override void MenuWillHighlightItem(NSMenu menu, NSMenuItem item)
		{
		}

		public override void MenuWillOpen(NSMenu menu)
		{
			var h = Handler;
			if (h == null)
				return;
			h.Callback.OnOpening(h.Widget, EventArgs.Empty);
		}

		public override void MenuDidClose(NSMenu menu)
		{
			var h = Handler;
			if (h == null)
				return;
			h.Callback.OnClosing(h.Widget, EventArgs.Empty);

			Application.Instance.AsyncInvoke(() => h.Callback.OnClosed(h.Widget, EventArgs.Empty));
		}
	}

	public class ContextMenuHandler : WidgetHandler<EtoMenu, ContextMenu, ContextMenu.ICallback>, ContextMenu.IHandler
	{
		protected override EtoMenu CreateControl() => new EtoMenu();

		protected override void Initialize()
		{
			Control.WorksWhenModal = true;
			Control.AutoEnablesItems = false;
			Control.ShowsStateColumn = true;
			Control.Delegate = new ContextHandler() { Handler = this };

			base.Initialize();
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case ContextMenu.OpeningEvent:
				case ContextMenu.ClosedEvent:
				case ContextMenu.ClosingEvent:
					// handled by delegate
					break;

				default:
					base.AttachEvent(id);
					break;
			}
		}

		public void AddMenu(int index, MenuItem item)
		{
			Control.InsertItem((NSMenuItem)item.ControlObject, index);
		}

		public void RemoveMenu(MenuItem item)
		{
			Control.RemoveItem((NSMenuItem)item.ControlObject);
		}

		public void Clear()
		{
			Control.RemoveAllItems();
		}

		public void Show(Control relativeTo, PointF? location)
		{
			var view = relativeTo?.GetContainerView();

			if (location != null || view == null)
			{
				CGPoint cglocation;
				if (view != null && location != null)
				{
					cglocation = location.Value.ToNS();
					if (!view.IsFlipped)
						cglocation.Y = view.Frame.Height - cglocation.Y;
				}
				else
				{
					cglocation = (location ?? Mouse.Position).ToNS();
					var origin = NSScreen.Screens[0].Frame.Bottom;
					cglocation.Y = origin - cglocation.Y;
				}

				Control.PopUpMenu(null, cglocation, view);
			}
			else
			{
				NSEvent nsevent = NSApplication.SharedApplication.CurrentEvent;
				if (nsevent == null)
				{
					var keyWindow = NSApplication.SharedApplication.KeyWindow;
					var mouseLocation = NSEvent.CurrentMouseLocation;
					mouseLocation = keyWindow.ConvertScreenToBase(mouseLocation);

					var time = DateTime.Now.ToOADate();
					var windowNumber = keyWindow.WindowNumber;

					nsevent = NSEvent.MouseEvent(NSEventType.RightMouseDown, mouseLocation, 0, time, windowNumber, null, 0, 0, 0.1f);
				}

				NSMenu.PopUpContextMenu(Control, nsevent, view);
			}
		}
	}
}