using MonoMac.AppKit;
using MonoMac.Foundation;
using Eto.Forms;
using System.ComponentModel;
using Eto.Mac.Forms;

namespace Eto.Mac
{
	[MonoMac.Foundation.Register("EtoAppDelegate")]
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
			var form = Application.Instance.MainForm == null ? null : Application.Instance.MainForm.Handler as IMacWindow;
			if (form != null)
				args.Cancel = !form.CloseWindow(Application.Instance.OnTerminating);
			else
				Application.Instance.OnTerminating(args);
			return args.Cancel ? NSApplicationTerminateReply.Cancel : NSApplicationTerminateReply.Now;
		}
	}
}

