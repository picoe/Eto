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
		NSApplication app = NSApplication.SharedApplication;

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

		public ModalEventArgs(Window window, NSWindow nativeWindow, bool isModal = false, bool isSheet = false)
		{
			EtoWindow = window;
			NativeWindow = nativeWindow;
			IsModal = isModal;
			IsSheet = isSheet;
		}

		public virtual void Stop()
		{
			if (Stopped)
				return;
			Stopped = true;
			StopAction?.Invoke(this);
		}

		public virtual void Restart(Action restartAction, bool useAsync = true)
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

		protected virtual bool RunSessionInternal()
		{
			ApplicationHandler.Instance.TriggerProcessModalSession(this);
			// Run the window modally until there are no events to process:
			var result = (int)app.RunModalSession(Session);

			// Give the main loop some time:
			NSRunLoop.Current.RunUntil(NSRunLoop.NSDefaultRunLoopMode, NSDate.DistantFuture);
			var continues = result == (int)NSRun.ContinuesResponse;
			if (Stopped && continues)
			{
				// we were told to continue, but we actually want to stop
				app.StopModal();
			}
			return continues;
		}

		public void RunModal()
		{
			do
			{
				RunSession();
			}
			while (ShouldRestart());
		}

		public void RunSheet(NSWindow parent)
		{
			do
			{
				NSApplication.SharedApplication.BeginSheet(NativeWindow, parent);

				RunSession();

				NSApplication.SharedApplication.EndSheet(NativeWindow);
				NativeWindow.OrderOut(NativeWindow);
			}
			while (ShouldRestart());
		}

		public void RunSession()
		{
			var etoWindow = NativeWindow as EtoWindow;
			if (etoWindow != null && etoWindow.DisableCenterParent)
				etoWindow.DisableSetOrigin = true;

			Session = app.BeginModalSession(NativeWindow);
			bool result;
			if (etoWindow != null)
				etoWindow.DisableSetOrigin = false;

			// Loop until some result other than continues:
			do
			{
				result = RunSessionInternal();
			} while (result || !Stopped);

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
			helper = new ModalEventArgs(window, nativeWindow, isModal: true, isSheet: isSheet);
			helper.CanRestart = true;
			helper.RunModal();
		}

		public static void RunSheet(Window window, NSWindow theWindow, NSWindow parent, out ModalEventArgs helper)
		{
			helper = new ModalEventArgs(window, theWindow, isModal: true, isSheet: true);
			helper.CanRestart = true;
			helper.RunSheet(parent);
		}

		public static void BeginSheet(Window window, NSWindow theWindow, NSWindow parent, out ModalEventArgs helper, Action completed)
		{
			var app = NSApplication.SharedApplication;
			app.BeginSheet(theWindow, parent);
			helper = new ModalEventArgs(window, theWindow, isSheet: true);
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

