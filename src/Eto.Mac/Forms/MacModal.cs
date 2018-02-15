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

namespace Eto.Mac.Forms
{
	public class ModalEventArgs : EventArgs
	{
		public IntPtr Session { get; private set; }

		public Window EtoWindow { get; private set; }

		public NSWindow NativeWindow { get; private set; }

		public bool Stopped { get; private set; }

		public bool IsModal { get; private set; }

		public bool IsSheet { get; private set; }

		public ModalEventArgs(IntPtr session, Window window, NSWindow nativeWindow, bool isModal = false, bool isSheet = false)
		{
			Session = session;
			EtoWindow = window;
			NativeWindow = nativeWindow;
			IsModal = isModal;
			IsSheet = isSheet;
		}

		public void Stop()
		{
			Stopped = true;
			if (IsSheet)
			{
				NSApplication.SharedApplication.EndSheet(NativeWindow);
				NativeWindow.OrderOut(NativeWindow);
			}
			else if (IsModal)
				NSApplication.SharedApplication.StopModal();
		}
	}

	class MacModal : NSObject
	{
		[Export("alertDidEnd:returnCode:contextInfo:")]
		public void AlertDidEnd(NSAlert alert, int returnCode, IntPtr contextInfo)
		{
			NSApplication.SharedApplication.StopModalWithCode(returnCode);
		}

		public static int Run(NSAlert view, Control parent)
		{
			int ret;
			if (parent != null)
			{
				var window = parent.ControlObject as NSWindow;
				if (window == null && parent.ControlObject is NSView)
					window = ((NSView)parent.ControlObject).Window;
				if (window == null || !view.RespondsToSelector(new Selector("beginSheetModalForWindow:modalDelegate:didEndSelector:contextInfo:")))
					ret = (int)view.RunModal();
				else
				{
					ret = 0;
					NSApplication.SharedApplication.InvokeOnMainThread(delegate
					{
						view.BeginSheet(window, new MacModal(), new Selector("alertDidEnd:returnCode:contextInfo:"), IntPtr.Zero);
						ret = (int)NSApplication.SharedApplication.RunModalForWindow(window);
					});
				}
			}
			else
				ret = (int)view.RunModal();
			return ret;
		}

		public static int Run(NSSavePanel panel, Control parent)
		{
			int ret;
			if (parent != null)
			{
				var window = parent.ControlObject as NSWindow;
				if (window == null && parent.ControlObject is NSView)
					window = ((NSView)parent.ControlObject).Window;
				if (window == null || !panel.RespondsToSelector(new Selector("beginSheetModalForWindow:completionHandler:")))
					ret = (int)panel.RunModal();
				else
				{
					panel.BeginSheet(window, result => NSApplication.SharedApplication.StopModalWithCode(result));
					ret = (int)NSApplication.SharedApplication.RunModalForWindow(window);
				}
			}
			else
				ret = (int)panel.RunModal();
			return ret;
		}

		enum NSRun
		{
			StoppedResponse = (-1000),
			AbortedResponse = (-1001),
			ContinuesResponse = (-1002)
		}

		public static void Run(Window window, NSWindow nativeWindow, out ModalEventArgs helper)
		{
			var app = NSApplication.SharedApplication;
			var session = app.BeginModalSession(nativeWindow);
			helper = new ModalEventArgs(session, window, nativeWindow, isModal: true);
			int result;
			var etoapp = ApplicationHandler.Instance;
			
			// Loop until some result other than continues:
			do
			{
				etoapp.TriggerProcessModalSession(helper);
				// Run the window modally until there are no events to process:
				result = (int)app.RunModalSession(session);

				// Give the main loop some time:
				NSRunLoop.Current.RunUntil(NSRunLoop.NSDefaultRunLoopMode, NSDate.DistantFuture);
			} while (result == (int)NSRun.ContinuesResponse || !helper.Stopped);
			
			app.EndModalSession(session);
		}

		public static void RunSheet(Window window, NSWindow theWindow, NSWindow parent, out ModalEventArgs helper)
		{
			var app = NSApplication.SharedApplication;
			app.BeginSheet(theWindow, parent, delegate
			{
				NSApplication.SharedApplication.StopModal();				
			});
			
			var session = app.BeginModalSession(theWindow);
			helper = new ModalEventArgs(session, window, theWindow, isModal: true, isSheet: true);
			int result;
			var etoapp = ApplicationHandler.Instance;

			// Loop until some result other than continues:
			do
			{
				etoapp.TriggerProcessModalSession(helper);
				// Run the window modally until there are no events to process:
				result = (int)app.RunModalSession(session);
				
				// Give the main loop some time:
				NSRunLoop.Current.RunUntil(NSRunLoop.NSDefaultRunLoopMode, NSDate.DistantFuture);
			} while (result == (int)NSRun.ContinuesResponse || !helper.Stopped);
			
			app.EndModalSession(session);

			/**
			theWindow.OrderOut (null);
			app.EndSheet (theWindow);
			/**/
		}

		public static void BeginSheet(Window window, NSWindow theWindow, NSWindow parent, out ModalEventArgs helper, Action completed)
		{
			var app = NSApplication.SharedApplication;
			app.BeginSheet(theWindow, parent, delegate
			{
				NSApplication.SharedApplication.StopModal();
				if (completed != null)
					completed();
			});
			helper = new ModalEventArgs(IntPtr.Zero, window, theWindow, isSheet: true);
		}
				
	}
}

