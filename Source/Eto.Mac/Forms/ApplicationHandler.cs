using System;
using Eto.Forms;
using Eto.Mac.Forms.Actions;
using System.Collections.Generic;
using System.Linq;

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
using CGSize = MonoMac.Foundation.NSSize;
using CGRect = MonoMac.Foundation.NSRect;
using CGPoint = MonoMac.Foundation.NSPoint;
using nfloat = System.Double;
using nint = System.Int64;
using nuint = System.UInt64;
#else
using CGSize = System.Drawing.SizeF;
using CGRect = System.Drawing.RectangleF;
using CGPoint = System.Drawing.PointF;
using nfloat = System.Single;
using nint = System.Int32;
using nuint = System.UInt32;
#endif
#endif

namespace Eto.Mac.Forms
{
	public class ApplicationHandler : WidgetHandler<NSApplication, Application, Application.ICallback>, Application.IHandler
	{
		bool attached;

		internal static bool QueueResizing { get; set; }

		public NSApplicationDelegate AppDelegate { get; set; }

		public bool AddFullScreenMenuItem { get; set; }

		public bool AllowClosingMainForm { get; set; }

		public static ApplicationHandler Instance
		{
			get { return Application.Instance == null ? null : Application.Instance.Handler as ApplicationHandler; }
		}

		/// <summary>
		/// Event to inject custom functionality during the event loop of a modal session for an eto dialog.
		/// </summary>
		public event EventHandler<ModalEventArgs> ProcessModalSession;

		/// <summary>
		/// Raises the <see cref="ProcessModalSession"/> event.
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected virtual void OnProcessModalSession(ModalEventArgs e)
		{
			if (ProcessModalSession != null)
				ProcessModalSession(this, e);
		}

		internal void TriggerProcessModalSession(ModalEventArgs e)
		{
			OnProcessModalSession(e);
		}

		public string BadgeLabel
		{
			get
			{
				var badgeLabel = Control.DockTile.BadgeLabel;
				return string.IsNullOrEmpty(badgeLabel) ? null : badgeLabel;
			}
			set
			{
				Control.DockTile.BadgeLabel = value ?? string.Empty;
			}
		}

		public bool ShouldCloseForm(Window window, bool wasClosed)
		{
			if (ReferenceEquals(window, Widget.MainForm))
			{
				if (AllowClosingMainForm && wasClosed)
					Widget.MainForm = null;
				return AllowClosingMainForm;
			}

			return true;
		}

		public ApplicationHandler()
		{
			Control = NSApplication.SharedApplication;
		}

		static void restart_WillTerminate(object sender, EventArgs e)
		{
			// re-open after we terminate
			var args = new string[]
			{
				"-c",
				"open \"$1\"", 
				string.Empty,
				NSBundle.MainBundle.BundlePath
			};
			NSTask.LaunchFromPath("/bin/sh", args);
		}

		public void Invoke(Action action)
		{
			var thread = NSThread.Current;
			if (thread != null && thread.IsMainThread)
				action();
			else
			{
				Control.InvokeOnMainThread(() => action());
			}
		}

		public void AsyncInvoke(Action action)
		{
			Control.BeginInvokeOnMainThread(() => action());
		}

		public void Restart()
		{
			NSApplication.SharedApplication.WillTerminate += restart_WillTerminate;
			NSApplication.SharedApplication.Terminate(AppDelegate);

			// only get here if cancelled, remove event to restart
			NSApplication.SharedApplication.WillTerminate -= restart_WillTerminate;
		}

		public void RunIteration()
		{
			NSApplication.SharedApplication.NextEvent(NSEventMask.AnyEvent, NSDate.DistantFuture, NSRunLoop.NSDefaultRunLoopMode, true);
		}

		public void Attach(object context)
		{
			attached = true;
		}

		public void OnMainFormChanged()
		{
		}

		public void Run()
		{
			if (!attached)
			{
				EtoBundle.Init();

				if (Control.Delegate == null)
					Control.Delegate = AppDelegate ?? new AppDelegate();
				NSApplication.Main(new string[0]);
			}
			else
				Initialize(Control.Delegate as NSApplicationDelegate);
		}

		public void Initialize(NSApplicationDelegate appdelegate)
		{
			AppDelegate = appdelegate;
			Callback.OnInitialized(Widget, EventArgs.Empty);
		}

		public void Quit()
		{
			NSApplication.SharedApplication.Terminate((NSObject)AppDelegate ?? NSApplication.SharedApplication);
		}

		public bool QuitIsSupported { get { return true; } }

		public void Open(string url)
		{
			NSWorkspace.SharedWorkspace.OpenUrl(new NSUrl(url));
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case Application.TerminatingEvent:
				// handled by app delegate
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		public void EnableFullScreen()
		{
			if (Control.RespondsToSelector(new Selector("setPresentationOptions:")))
			{
				AddFullScreenMenuItem = true;
			}
		}

		public Keys CommonModifier
		{
			get
			{
				return Keys.Application;
			}
		}

		public Keys AlternateModifier
		{
			get
			{
				return Keys.Alt;
			}
		}
	}
}
