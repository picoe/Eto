using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Eto.Drawing;
using Eto.Forms;
using Eto.GtkSharp.Drawing;

namespace Eto.GtkSharp.Forms
{
    public class LinuxNotificationHandler : WidgetHandler<GLib.Object, Notification, Notification.ICallback>, Notification.IHandler
    {
		private const string libnotify = "libnotify.so.4";

		[DllImport(libnotify, CallingConvention = CallingConvention.Cdecl)]
		protected extern static bool notify_init(string app_name);

		[DllImport(libnotify, CallingConvention = CallingConvention.Cdecl)]
		protected extern static void notify_uninit();

		[DllImport(libnotify, CallingConvention = CallingConvention.Cdecl)]
		protected extern static IntPtr notify_get_server_caps();

		[DllImport(libnotify, CallingConvention = CallingConvention.Cdecl)]
		protected extern static IntPtr notify_notification_new(string summary, string body, string icon);

		[DllImport(libnotify, CallingConvention = CallingConvention.Cdecl)]
		protected extern static IntPtr notify_notification_update(IntPtr notification, string summary, string body, string icon);

		[DllImport(libnotify, CallingConvention = CallingConvention.Cdecl)]
		protected extern static bool notify_notification_show(IntPtr notification, IntPtr error);

		[DllImport(libnotify, CallingConvention = CallingConvention.Cdecl)]
		protected extern static void notify_notification_add_action(IntPtr notification, string action, string label, Delegate callback, IntPtr user_data, IntPtr free_func);

		[DllImport(libnotify, CallingConvention = CallingConvention.Cdecl)]
		protected extern static void notify_notification_clear_actions(IntPtr notification);

		private static bool init;
		private static bool allowactions;

		public static void Init()
		{
			try
			{
				notify_init(Assembly.GetExecutingAssembly().FullName);

				var list = new GLib.List(notify_get_server_caps(), typeof(string));
				foreach (var item in list)
				{
					if (item.ToString() == "actions")
					{
						allowactions = true;
						break;
					}
				}

				init = true;
			}
			catch
			{
				Console.WriteLine("Error, libnotify.so.4 was not found, notifications won't be displayed.");
				init = false;
			}
		}

		public static void DeInit()
		{
			if (init)
				notify_uninit();
		}

		Image _contentImage;

		public string Title { get; set; }

		public bool RequiresTrayIndicator
		{
			get { return false; }
		}

		public string Message { get; set; }

		public string UserData { get; set; }

		string iconPath;
		GCHandle handle;

        public LinuxNotificationHandler()
        {
			if (!init)
				return;
				
			Control = GLib.Object.GetObject(notify_notification_new("", "", ""));

			if (allowactions)
			{
				handle = GCHandle.Alloc(this);
				// Undocumented AFAIK: If action is "default" it will not create a button.
				notify_notification_add_action(Control.Handle, "default", "default", (Action<IntPtr, string, IntPtr>)Activated, GCHandle.ToIntPtr(handle), IntPtr.Zero);
			}

        }

		private static void Activated(IntPtr notification, string action, IntPtr user_data)
		{
			var handler = GCHandle.FromIntPtr(user_data).Target as LinuxNotificationHandler;
			if (handler != null)
			{
				var app = ApplicationHandler.Instance;
				app?.Callback.OnNotificationActivated(app.Widget, new NotificationEventArgs(handler.ID, handler.UserData));
			}
		}

		public Image ContentImage
		{
			get { return _contentImage; }
			set
			{
				_contentImage = value;
				var pb = _contentImage.ToGdk();
				if (pb != null)
				{
					if (iconPath == null)
					{
						iconPath = Path.GetTempFileName();
						ApplicationHandler.TempFiles.Add(iconPath);
					}
					pb.Save(iconPath, "png");
				}
				else if (iconPath != null)
				{
					if (File.Exists(iconPath))
						File.Delete(iconPath);
					ApplicationHandler.TempFiles.Remove(iconPath);
					iconPath = null;
				}
			}
		}

		public void Show(TrayIndicator indicator = null)
		{
			if (!init)
				return;
			
			// Empty string will show the default icon, while an incorrect one will show no icon
			notify_notification_update(Control.Handle, Title, Message, iconPath ?? "???");
			notify_notification_show(Control.Handle, IntPtr.Zero);
		}
	}
}
