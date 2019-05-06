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
		public IntPtr Session { get; set; }

		public Window EtoWindow { get; private set; }

		public NSWindow NativeWindow { get; private set; }

		public bool Stopped { get; private set; }

		public bool IsModal { get; private set; }

		public bool IsSheet { get; private set; }

		public bool CanRestart { get; set; }

		public Action<ModalEventArgs> StopAction { get; set; }

		Action restartAction;
		bool shouldRestart;

		public ModalEventArgs(IntPtr session, Window window, NSWindow nativeWindow, bool isModal = false, bool isSheet = false)
		{
			EtoWindow = window;
			NativeWindow = nativeWindow;
			IsModal = isModal;
			IsSheet = isSheet;
		}

		public void Stop()
		{
			if (Stopped)
				return;
			Stopped = true;
			StopAction?.Invoke(this);
		}

		public void Restart(Action restartAction, bool useAsync = true)
		{
			if (CanRestart)
			{

				if (useAsync)
				{
					this.restartAction = restartAction;
					shouldRestart = true;
					Stop();
				}
				else
                {
                    var app = NSApplication.SharedApplication;

                    // end modal session
                    app.StopModal();
                    app.EndModalSession(Session);

                    restartAction?.Invoke();

                    // restart new session
                    Session = app.BeginModalSession(NativeWindow);
                    Stopped = false;
                }
            }
			else
			{
				restartAction?.Invoke();
			}
		}

		public bool ShouldRestart()
		{
			if (shouldRestart)
			{
				restartAction?.Invoke();
				restartAction = null;
				shouldRestart = false;
				Stopped = false;
				return true;
			}
			return false;
		}

		enum NSRun
		{
			StoppedResponse = (-1000),
			AbortedResponse = (-1001),
			ContinuesResponse = (-1002)
		}

		public void RunSession()
		{
			var app = NSApplication.SharedApplication;
			Session = app.BeginModalSession(NativeWindow);
			int result;
			var etoapp = ApplicationHandler.Instance;

			// Loop until some result other than continues:
			do
			{
				etoapp.TriggerProcessModalSession(this);
				// Run the window modally until there are no events to process:
				result = (int)app.RunModalSession(Session);

				// Give the main loop some time:
				NSRunLoop.Current.RunUntil(NSRunLoop.NSDefaultRunLoopMode, NSDate.DistantFuture);
				if (Stopped && result == (int)NSRun.ContinuesResponse)
				{
					app.StopModal();
				}
			} while (result == (int)NSRun.ContinuesResponse || !Stopped);

			app.EndModalSession(Session);
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

		public static void Run(Window window, NSWindow nativeWindow, out ModalEventArgs helper, bool isSheet = false)
		{
			helper = new ModalEventArgs(IntPtr.Zero, window, nativeWindow, isModal: true, isSheet: isSheet);
			helper.CanRestart = true;
			do
			{
				helper.RunSession();
			}
			while (helper.ShouldRestart());
		}

		public static void RunSheet(Window window, NSWindow theWindow, NSWindow parent, out ModalEventArgs helper)
		{
			helper = new ModalEventArgs(IntPtr.Zero, window, theWindow, isModal: true, isSheet: true);
			helper.CanRestart = true;
			do
			{
				NSApplication.SharedApplication.BeginSheet(theWindow, parent);

				helper.RunSession();

				NSApplication.SharedApplication.EndSheet(theWindow);
				theWindow.OrderOut(theWindow);
			}
			while (helper.ShouldRestart());
		}

		public static void BeginSheet(Window window, NSWindow theWindow, NSWindow parent, out ModalEventArgs helper, Action completed)
		{
			var app = NSApplication.SharedApplication;
			app.BeginSheet(theWindow, parent);
			helper = new ModalEventArgs(IntPtr.Zero, window, theWindow, isSheet: true);
			helper.StopAction = e =>
			{
				NSApplication.SharedApplication.EndSheet(e.NativeWindow);
				e.NativeWindow.OrderOut(e.NativeWindow);
				NSApplication.SharedApplication.StopModal();
				completed?.Invoke();
			};
		}
				
	}
}

