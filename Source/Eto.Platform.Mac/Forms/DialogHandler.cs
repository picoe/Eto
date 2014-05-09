using System;
using SD = System.Drawing;
using Eto.Forms;
using MonoMac.AppKit;
using MonoMac.Foundation;
using System.Threading.Tasks;

namespace Eto.Platform.Mac.Forms
{
	public class DialogHandler : MacWindow<MyWindow, Dialog>, IDialog
	{
		Button button;
		MacModal.ModalHelper session;

		protected override bool DisposeControl { get { return false; } }

		class DialogWindow : MyWindow
		{
			public new DialogHandler Handler
			{
				get { return base.Handler as DialogHandler; }
				set { base.Handler = value; }
			}

			public DialogWindow()
				: base(new SD.Rectangle(0, 0, 200, 200), NSWindowStyle.Closable | NSWindowStyle.Titled, NSBackingStore.Buffered, false)
			{
			}

			[Export("cancelOperation:")]
			public void CancelOperation(IntPtr sender)
			{
				if (Handler.AbortButton != null)
					Handler.AbortButton.OnClick(EventArgs.Empty);
			}
		}

		public DialogDisplayMode DisplayMode { get; set; }

		public Button AbortButton { get; set; }

		public Button DefaultButton
		{
			get { return button; }
			set
			{
				button = value;
				
				if (button != null)
				{
					var b = button.ControlObject as NSButton;
					Control.DefaultButtonCell = b == null ? null : b.Cell;
				}
				else
					Control.DefaultButtonCell = null;
			}
		}

		public DialogHandler()
		{
			var dlg = new DialogWindow();
			dlg.Handler = this;
			Control = dlg;
			ConfigureWindow();
		}

		public void ShowModal(Control parent)
		{
			session = null;
			if (parent != null && parent.ParentWindow != null)
			{
				var nswindow = parent.ParentWindow.ControlObject as NSWindow;
				if (nswindow != null)
					Control.ParentWindow = nswindow;
			}
			Widget.OnShown(EventArgs.Empty);

			Widget.Closed += HandleClosed;
			if (DisplayMode.HasFlag(DialogDisplayMode.Attached))
				MacModal.RunSheet(Control, out session);
			else
			{
				Control.MakeKeyWindow();
				MacModal.Run(Control, out session);
			}
		}

		public Task ShowModalAsync(Control parent)
		{
			var tcs = new TaskCompletionSource<bool>();
			session = null;
			if (parent != null && parent.ParentWindow != null)
			{
				var nswindow = parent.ParentWindow.ControlObject as NSWindow;
				if (nswindow != null)
					Control.ParentWindow = nswindow;
			}
			Widget.OnShown(EventArgs.Empty);

			Widget.Closed += HandleClosed;
			if (DisplayMode.HasFlag(DialogDisplayMode.Attached))
			{
				MacModal.BeginSheet(Control, out session, () => tcs.SetResult(true));
			}
			else
			{
				Control.MakeKeyWindow();
				Application.Instance.AsyncInvoke(() =>
				{
					MacModal.Run(Control, out session);
					tcs.SetResult(true);
				});

			}
			return tcs.Task;
		}

		void HandleClosed(object sender, EventArgs e)
		{
			if (session != null)
				session.Stop();
			Widget.Closed -= HandleClosed;
		}

		public override void Close()
		{
			if (session != null && session.IsSheet)
				session.Stop();
			else
				base.Close();
		}
		
	}
}
