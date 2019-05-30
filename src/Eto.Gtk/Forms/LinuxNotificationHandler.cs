using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Eto.Drawing;
using Eto.Forms;
using Eto.GtkSharp.Drawing;

namespace Eto.GtkSharp.Forms
{
	class NotificationWrapper : GLib.Object
	{
#if !GTK2
		public delegate void ClosedHandler(object o, EventArgs args);
        public event ClosedHandler Closed
        {
            add
            {
                this.AddSignalHandler("closed", value, typeof(EventArgs));
            }
            remove
            {
                this.RemoveSignalHandler("closed", value);
            }
        }
#endif

		public NotificationWrapper(IntPtr handle) : base(handle)
		{
			
		}


	}

    public class LinuxNotificationHandler : WidgetHandler<IntPtr, Notification, Notification.ICallback>, Notification.IHandler
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
		private static MethodInfo activatedmethod;

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

				var methods = typeof(LinuxNotificationHandler).GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic);
				foreach(var m in methods)
				{
					if (m.Name == "Activated")
					{
						activatedmethod = m;
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
		string iconPath;

		public string Title { get; set; }

		public bool RequiresTrayIndicator
		{
			get { return false; }
		}

		public string Message { get; set; }

		public string UserData { get; set; }

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
			var notification = new NotificationWrapper(notify_notification_new(Title, Message, iconPath ?? "???"));
			var data = Marshal.StringToHGlobalUni(ID + (char)1 + UserData);
#if !GTK2
			notification.Closed += (sender, e) =>
			{
				Marshal.FreeHGlobal(data);
				notification.Dispose();
			};
#endif

			if (allowactions)
			{
				// Undocumented AFAIK: If action is "default" it will not create a button.
				notify_notification_add_action(
					notification.Handle,
					"default",
					"default",
					Delegate.CreateDelegate(typeof(ActivatedDelegate), activatedmethod),
					data,
					IntPtr.Zero
				);
			}

			notify_notification_show(notification.Handle, IntPtr.Zero);
		}
		
		private delegate void ActivatedDelegate(IntPtr notification, IntPtr action, IntPtr user_data);

		private static void Activated(IntPtr notification, IntPtr action, IntPtr user_data)
		{
			var data = Marshal.PtrToStringUni(user_data).Split((char)1);
			var app = ApplicationHandler.Instance;

			app?.Callback.OnNotificationActivated(app.Widget, new NotificationEventArgs(data[0], data[1]));
		}
	}
}
