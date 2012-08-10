using System;
using MonoMac.AppKit;
using Eto.Forms;
using MonoMac.Foundation;
using MonoMac.ObjCRuntime;

namespace Eto.Platform.Mac.Forms
{
	internal class MacModal : NSObject
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

		public static void Run (NSWindow theWindow)
		{
			var app = NSApplication.SharedApplication;
			var session = app.BeginModalSession (theWindow);
			int result = (int)NSRun.ContinuesResponse;

			// Loop until some result other than continues:
			while (result == (int)NSRun.ContinuesResponse) {
				// Run the window modally until there are no events to process:
				result = app.RunModalSession (session);

				// Give the main loop some time:
				NSRunLoop.Current.LimitDateForMode (NSRunLoopMode.Default);
			}
			app.EndModalSession (session);
		}

	}
}

