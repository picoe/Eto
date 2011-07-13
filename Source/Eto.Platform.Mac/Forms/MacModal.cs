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
                    view.BeginSheet (window, new MacModal (), new Selector ("alertDidEnd:returnCode:contextInfo:"), IntPtr.Zero);
                    ret = NSApplication.SharedApplication.RunModalForWindow (window);
                }
            } else
                ret = view.RunModal ();
            return ret;
        }
        
        public static int Run(NSSavePanel view, Control parent)
        {
            int ret;
            if (parent != null) {
                var window = parent.ControlObject as NSWindow;
                if (window == null && parent.ControlObject is NSView)
                    window = ((NSView)parent.ControlObject).Window;
                if (window == null || !view.RespondsToSelector(new Selector("beginSheetModalForWindow:completionHandler:")))
                    ret = view.RunModal ();
                else {
                    view.BeginSheet (window, delegate(int returnCode) { NSApplication.SharedApplication.StopModalWithCode (returnCode); });
                    ret = NSApplication.SharedApplication.RunModalForWindow (window);
                }
            } else
                ret = view.RunModal ();
            return ret;
        }

	}
}

