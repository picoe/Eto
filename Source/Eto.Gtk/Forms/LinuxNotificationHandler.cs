using System;
using System.IO;
using System.Runtime.InteropServices;
using Eto.Drawing;
using Eto.Forms;
using Eto.GtkSharp.Drawing;

namespace Eto.GtkSharp.Forms
{
    public class LinuxNotificationHandler : WidgetHandler<GLib.Object, Notification, Notification.ICallback>, Notification.IHandler
    {
		private const string libnotify = "libnotify.so";

		[DllImport(libnotify, CallingConvention = CallingConvention.Cdecl)]
		public extern static bool notify_init(string app_name);

		[DllImport(libnotify, CallingConvention = CallingConvention.Cdecl)]
		public extern static void notify_uninit();

		[DllImport(libnotify, CallingConvention = CallingConvention.Cdecl)]
		public extern static IntPtr notify_notification_new(string summary, string body, string icon);

		[DllImport(libnotify, CallingConvention = CallingConvention.Cdecl)]
		public extern static IntPtr notify_notification_update(IntPtr notification, string summary, string body, string icon);

		[DllImport(libnotify, CallingConvention = CallingConvention.Cdecl)]
		public extern static bool notify_notification_show(IntPtr notification, IntPtr error);

		[DllImport(libnotify, CallingConvention = CallingConvention.Cdecl)]
		public extern static void notify_notification_add_action(IntPtr notification, string action, string label, Delegate callback, IntPtr user_data, IntPtr free_func);

		[DllImport(libnotify, CallingConvention = CallingConvention.Cdecl)]
		public extern static void notify_notification_clear_actions(IntPtr notification);

		public string Title { get; set; }

		public bool RequiresTrayIndicator
		{
			get { return false; }
		}

		public string Message { get; set; }

		EventHandler activated;
		string iconPath;

        public LinuxNotificationHandler()
        {
			Control = GLib.Object.GetObject(notify_notification_new("", "", ""));

			// Undocumented AFAIK: If action is "default" it will not create a button.
			notify_notification_add_action(Control.Handle, "default", "default", (Action)Activated, IntPtr.Zero, IntPtr.Zero);

			// Empty string will show the default icon, while an incorrect one will show no icon
			iconPath = "???";
        }

		private void Activated()
		{
			activated?.Invoke(this, EventArgs.Empty);
		}

		public void SetIcon(Icon icon)
		{
			iconPath = Path.GetTempFileName();
			if (icon != null)
				(icon.Handler as IconHandler)?.Pixbuf?.Save(iconPath, "png");
			ApplicationHandler.TempFiles.Add(iconPath);
		}

		public void Show(TrayIndicator indicator = null)
		{
			notify_notification_update(Control.Handle, Title, Message, iconPath);
			notify_notification_show(Control.Handle, IntPtr.Zero);
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
