using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Eto.Drawing;
using Eto.Forms;
using Eto.GtkSharp.Drawing;
using Eto.GtkSharp.Forms.Menu;

namespace Eto.GtkSharp.Forms
{
	public class LinuxTrayIndicatorHandler : WidgetHandler<GLib.Object, TrayIndicator, TrayIndicator.ICallback>, TrayIndicator.IHandler
	{
		const string libappindicator = "libappindicator3.so.1";
		Image image;
		string imagePath;
		ContextMenu menu;

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

		static uint Id = 0;

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

			Id++;
		}

		void RemoveTempImage()
		{
			if (!string.IsNullOrEmpty(imagePath))
			{
				ApplicationHandler.TempFiles.Remove(imagePath);
				File.Delete(imagePath);
				imagePath = null;
			}
		}

		public Image Image
		{
			get { return image; }
			set
			{
				RemoveTempImage();
				imagePath = Path.GetTempFileName();

				image = value;
				image.ToGdk()?.Save(imagePath, "png");

				app_indicator_set_icon(Control.Handle, imagePath);
				ApplicationHandler.TempFiles.Add(imagePath);
			}
		}

		public ContextMenu Menu
		{
			get { return menu; }
			set
			{
				if (menu != null)
				{
					var handler = menu.Handler as ContextMenuHandler;
					if (handler != null)
					{
						handler.Changed -= ContextMenu_Changed;
					}
				}
				menu = value;
				if (menu == null)
					app_indicator_set_menu(Control.Handle, (new Gtk.Menu()).Handle);
				else
				{
					app_indicator_set_menu(Control.Handle, menu.ToGtk().Handle);
					var handler = menu.Handler as ContextMenuHandler;
					if (handler != null)
					{
						// need to re-set the when it has changed.. I guess.
						handler.Changed += ContextMenu_Changed;
					}
				}
			}
		}

		void ContextMenu_Changed(object sender, EventArgs e)
		{
			app_indicator_set_menu(Control.Handle, menu.ToGtk().Handle);
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

		protected override void Dispose(bool disposing)
		{
			if (Control != null)
			{
				Visible = false;
				Control.Dispose();
				Control = null;
			}
			RemoveTempImage();
			base.Dispose(disposing);
		}
	}
}
