using System;
using MonoMac.AppKit;
using Eto.Forms;
using MonoMac.Foundation;
using MonoMac.ObjCRuntime;

namespace Eto.Mac.Forms
{
	class MacModal : NSObject
	{
		[Export("alertDidEnd:returnCode:contextInfo:")]
		public void AlertDidEnd (NSAlert alert, int returnCode, IntPtr contextInfo)
		{
			NSApplication.SharedApplication.StopModalWithCode (returnCode);
		}

		public static int Run (NSAlert view, Control parent)
		{
			int ret;
			if (parent != null) {
				var window = parent.ControlObject as NSWindow;
				if (window == null && parent.ControlObject is NSView)
					window = ((NSView)parent.ControlObject).Window;
				if (window == null || !view.RespondsToSelector (new Selector ("beginSheetModalForWindow:modalDelegate:didEndSelector:contextInfo:")))
					ret = view.RunModal ();
				else {
					ret = 0;
					NSApplication.SharedApplication.InvokeOnMainThread (delegate {
						view.BeginSheet (window, new MacModal (), new Selector ("alertDidEnd:returnCode:contextInfo:"), IntPtr.Zero);
						ret = NSApplication.SharedApplication.RunModalForWindow (window);
					});
				}
			} else
				ret = view.RunModal ();
			return ret;
		}

		public static int Run (NSSavePanel panel, Control parent)
		{
			int ret;
			if (parent != null) {
				var window = parent.ControlObject as NSWindow;
				if (window == null && parent.ControlObject is NSView)
					window = ((NSView)parent.ControlObject).Window;
				if (window == null || !panel.RespondsToSelector (new Selector ("beginSheetModalForWindow:completionHandler:")))
					ret = panel.RunModal ();
				else {
					panel.BeginSheet (window, delegate(int result) { 
						NSApplication.SharedApplication.StopModalWithCode (result); 
					});
					ret = NSApplication.SharedApplication.RunModalForWindow (window);
				}
			} else
				ret = panel.RunModal ();
			return ret;
		}

		enum NSRun
		{
			StoppedResponse    = (-1000),
			AbortedResponse    = (-1001),
			ContinuesResponse  = (-1002)
		}
		
		public class ModalHelper
		{
			public IntPtr Session { get; set; }
			
			public NSWindow Window { get; set; }
			
			public bool Stopped { get; private set; }
			
			public bool IsModal { get; set; }
			
			public bool IsSheet { get; set; }
			
			public void Stop ()
			{
				Stopped = true;
				if (IsSheet) {
					NSApplication.SharedApplication.EndSheet (Window);
					Window.OrderOut (Window);
				}
				else if (IsModal)
					NSApplication.SharedApplication.StopModal ();
			}
		}
			
		public static void Run (NSWindow theWindow, out ModalHelper helper)
		{
			var app = NSApplication.SharedApplication;
			var session = app.BeginModalSession (theWindow);
			helper = new ModalHelper { Session = session, IsModal = true, Window = theWindow };
			int result;
			
			// Loop until some result other than continues:
			do {
				// Run the window modally until there are no events to process:
				result = app.RunModalSession (session);

				// Give the main loop some time:
				NSRunLoop.Current.RunUntil (NSRunLoop.NSDefaultRunLoopMode, NSDate.DistantFuture);
			}
			while (result == (int)NSRun.ContinuesResponse || !helper.Stopped);
			
			app.EndModalSession (session);
		}

		public static void RunSheet (NSWindow theWindow, out ModalHelper helper)
		{
			var app = NSApplication.SharedApplication;
			var parent = theWindow.ParentWindow;
			app.BeginSheet(theWindow, parent, delegate {
				NSApplication.SharedApplication.StopModal();				
			});
			
			var session = app.BeginModalSession (theWindow);
			helper = new ModalHelper { Session = session, IsModal = true, IsSheet = true, Window = theWindow };
			int result;
			
			// Loop until some result other than continues:
			do {
				// Run the window modally until there are no events to process:
				result = app.RunModalSession (session);
				
				// Give the main loop some time:
				NSRunLoop.Current.RunUntil (NSRunLoop.NSDefaultRunLoopMode, NSDate.DistantFuture);
			}
			while (result == (int)NSRun.ContinuesResponse || !helper.Stopped);
			
			app.EndModalSession (session);

			/**
			theWindow.OrderOut (null);
			app.EndSheet (theWindow);
			/**/
		}
		public static void BeginSheet(NSWindow theWindow, out ModalHelper helper, Action completed)
		{
			var app = NSApplication.SharedApplication;
			var parent = theWindow.ParentWindow;
			app.BeginSheet(theWindow, parent, delegate {
				NSApplication.SharedApplication.StopModal();
				if (completed != null)
					completed();
			});
			helper = new ModalHelper { IsModal = true, IsSheet = true, Window = theWindow };
		}
				
	}
}

