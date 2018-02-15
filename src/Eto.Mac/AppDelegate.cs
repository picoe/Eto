using Eto.Forms;
using System.ComponentModel;
using Eto.Mac.Forms;

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

namespace Eto.Mac
{
	[Register("EtoAppDelegate")]
	public class AppDelegate : NSApplicationDelegate
	{
		public override bool ApplicationShouldHandleReopen(NSApplication sender, bool hasVisibleWindows)
		{
			if (!hasVisibleWindows)
			{
				var form = Application.Instance.MainForm;
				if (form != null)
					form.Show();
			}
			return true;
		}

		public override void DidBecomeActive(NSNotification notification)
		{
			var form = Application.Instance.MainForm;
			if (form != null && !form.Visible)
				form.Show();
		}

		public override void DidFinishLaunching(NSNotification notification)
		{
			var handler = Application.Instance.Handler as ApplicationHandler;
			if (handler != null)
				handler.Initialize(this);
		}

		public override NSApplicationTerminateReply ApplicationShouldTerminate(NSApplication sender)
		{
			var args = new CancelEventArgs();
			var app = ((ApplicationHandler)Application.Instance.Handler);
			var form = Application.Instance.MainForm == null ? null : Application.Instance.MainForm.Handler as IMacWindow;
			if (form != null)
				args.Cancel = !form.CloseWindow(ce => app.Callback.OnTerminating(app.Widget, ce));
			else
			{
				app.Callback.OnTerminating(app.Widget, args);
			}
			return args.Cancel ? NSApplicationTerminateReply.Cancel : NSApplicationTerminateReply.Now;
		}
	}
}

