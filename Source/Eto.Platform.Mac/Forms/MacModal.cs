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
        
        public static int Run(NSAlert view, Control parent)
        {
            int ret;
            if (parent != null) {
                var window = parent.ControlObject as NSWindow;
                if (window == null && parent.ControlObject is NSView)
                    window = ((NSView)parent.ControlObject).Window;
                if (window == null || !view.RespondsToSelector(new Selector("beginSheetModalForWindow:modalDelegate:didEndSelector:contextInfo:")))
                    ret = view.RunModal ();
                else {
					ret = 0;
					NSApplication.SharedApplication.InvokeOnMainThread(delegate {
	                    view.BeginSheet (window, new MacModal (), new Selector ("alertDidEnd:returnCode:contextInfo:"), IntPtr.Zero);
	                    ret = NSApplication.SharedApplication.RunModalForWindow (window);
					});
                }
            } else
                ret = view.RunModal ();
            return ret;
        }
        
        public static int Run(NSSavePanel panel, Control parent)
        {
            int ret;
            if (parent != null) {
                var window = parent.ControlObject as NSWindow;
                if (window == null && parent.ControlObject is NSView)
                    window = ((NSView)parent.ControlObject).Window;
                if (window == null || !panel.RespondsToSelector(new Selector("beginSheetModalForWindow:completionHandler:")))
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

	}
}

