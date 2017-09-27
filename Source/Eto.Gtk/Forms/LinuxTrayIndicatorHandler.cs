using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Eto.Drawing;
using Eto.Forms;
using Eto.GtkSharp.Drawing;

namespace Eto.GtkSharp.Forms
{
	public class LinuxTrayIndicatorHandler : WidgetHandler<GLib.Object, TrayIndicator, TrayIndicator.ICallback>, TrayIndicator.IHandler
	{
		const string libappindicator = "libappindicator3.so.1";

		[DllImport(libappindicator, CallingConvention = CallingConvention.Cdecl)]
		public extern static IntPtr app_indicator_new(string id, string icon_name, int category);

		[DllImport(libappindicator, CallingConvention = CallingConvention.Cdecl)]
		public extern static void app_indicator_set_icon(IntPtr self, string icon_name);

		[DllImport(libappindicator, CallingConvention = CallingConvention.Cdecl)]
		public extern static void app_indicator_set_menu(IntPtr self, IntPtr menu);

		[DllImport(libappindicator, CallingConvention = CallingConvention.Cdecl)]
		public extern static string app_indicator_get_title(IntPtr self);

		[DllImport(libappindicator, CallingConvention = CallingConvention.Cdecl)]
		public extern static void app_indicator_set_title(IntPtr self, string title);

		[DllImport(libappindicator, CallingConvention = CallingConvention.Cdecl)]
		public extern static int app_indicator_get_status(IntPtr self);

		[DllImport(libappindicator, CallingConvention = CallingConvention.Cdecl)]
		public extern static void app_indicator_set_status(IntPtr self, int status);

		[DllImport(libappindicator, CallingConvention = CallingConvention.Cdecl)]
		public extern static void app_indicator_dispose(IntPtr gobject);

		private static uint Id = 0;

		public string Title
		{
			get { return app_indicator_get_title(Control.Handle); }
			set { app_indicator_set_title(Control.Handle, value); }
		}

		public bool Visible
		{
			get { return app_indicator_get_status(Control.Handle) == 1; }
			set { app_indicator_set_status(Control.Handle, value ? 1 : 0); }
		}

		public LinuxTrayIndicatorHandler()
		{
			Control = GLib.Object.GetObject(app_indicator_new(Assembly.GetExecutingAssembly().FullName + Id, "", 0));
			app_indicator_set_menu(Control.Handle, (new Gtk.Menu()).Handle);
			app_indicator_set_status(Control.Handle, 0);

			Id++;
		}

		public void SetIcon(Icon icon)
		{
			var path = Path.GetTempFileName();

			if (icon != null)
			{
				var handler = icon.Handler as IconHandler;
				handler?.Pixbuf?.Save(path, "png");
			}

			app_indicator_set_icon(Control.Handle, path);
			ApplicationHandler.TempFiles.Add(path);
		}

		public void SetMenu(ContextMenu menu)
		{
			if (menu == null)
				app_indicator_set_menu(Control.Handle, (new Gtk.Menu()).Handle);
			else
				app_indicator_set_menu(Control.Handle, (menu.ControlObject as Gtk.Menu).Handle);
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case TrayIndicator.ActivatedEvent:
					// Appindicator only has a context menu.
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}
	}
}
