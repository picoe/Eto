using Eto.Mac.Forms;



namespace Eto.Mac
{
	[Register("EtoAppDelegate")]
	public class AppDelegate : NSApplicationDelegate
	{
		// a main form that was really closed can't be shown or closed again
		static Form OpenMainForm
		{
			get
			{
				var form = Application.Instance.MainForm;
				return form != null && Application.Instance.Windows.Contains(form) ? form : null;
			}
		}

		public override bool ApplicationShouldHandleReopen(NSApplication sender, bool hasVisibleWindows)
		{
			if (!hasVisibleWindows)
			{
				var form = OpenMainForm;
				if (form != null)
					form.Show();
			}
			return true;
		}

		public override void DidBecomeActive(NSNotification notification)
		{
			var form = OpenMainForm;
			if (form != null && !form.Visible)
				form.Show();
		}

		public override void DidFinishLaunching(NSNotification notification)
		{
			var handler = Application.Instance.Handler as ApplicationHandler;
			if (handler != null)
			{
				handler.InitialMenu = Messaging.IntPtr_objc_msgSend(NSApplication.SharedApplication.Handle, MacWindow.selMainMenu);
				handler.Initialize(this);
			}
		}

		public override NSApplicationTerminateReply ApplicationShouldTerminate(NSApplication sender)
		{
			var args = new CancelEventArgs();
			var app = ((ApplicationHandler)Application.Instance.Handler);
			var form = OpenMainForm?.Handler as IMacWindow;
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

