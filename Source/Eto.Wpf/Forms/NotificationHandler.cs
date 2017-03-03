using Eto.Drawing;
using Eto.Forms;
using System;
using Swc = System.Windows.Controls;
using Swf = System.Windows.Forms;

namespace Eto.Wpf.Forms
{
	public class NotificationHandler : WidgetHandler<Swc.Control, Notification, Notification.ICallback>, Notification.IHandler
	{
		public string Message { get; set; }

		public bool RequiresTrayIndicator
		{
			get { return true; }
		}

		public string Title { get; set; }

		EventHandler activated;

		public void SetIcon(Icon icon)
		{

		}

		public void Show(TrayIndicator indicator = null)
		{
			if (indicator != null)
			{
				var tray = indicator.ControlObject as Swf.NotifyIcon;
				tray.ShowBalloonTip(3000, Title, Message, Swf.ToolTipIcon.None);
				tray.BalloonTipClicked += Tray_BalloonTipClicked;
			}
			else
			{
				// TODO: Check for Windows 10 and implement the nightmarish Notification API
			}
		}

		private void Tray_BalloonTipClicked(object sender, EventArgs e)
		{
			var tray = sender as Swf.NotifyIcon;
			tray.BalloonTipClicked -= Tray_BalloonTipClicked;

			activated?.Invoke(this, EventArgs.Empty);
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case Notification.ActivatedEvent:
					activated += (sender, e) => Callback.OnActivated(Widget, EventArgs.Empty);
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}
	}
}
