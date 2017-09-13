using System;
using System.IO;
using System.Reflection;
using Eto.Drawing;
using Eto.Forms;
using Eto.GtkSharp.Drawing;

namespace Eto.GtkSharp.Forms
{
    public class TrayIndicatorHandler : WidgetHandler<IntPtr, TrayIndicator, TrayIndicator.ICallback>, TrayIndicator.IHandler
    {
        private static uint s_id = 0;
        private IntPtr _menuEmpty;

        public TrayIndicatorHandler()
        {
            _menuEmpty = (new Gtk.Menu()).Handle;

			Control = AppIndicator.app_indicator_new(Assembly.GetExecutingAssembly().FullName + s_id, "", 0);
			AppIndicator.app_indicator_set_menu(Control, _menuEmpty);
            AppIndicator.app_indicator_set_status(Control, 0);

            s_id++;
        }

        public string Title
        {
            get => AppIndicator.app_indicator_get_title(Control);
            set => AppIndicator.app_indicator_set_title(Control, value);
        }

        public bool Visible
        {
            get => AppIndicator.app_indicator_get_status(Control) == 1;
            set => AppIndicator.app_indicator_set_status(Control, value ? 1 : 0);
        }

        public void SetIcon(Icon icon)
        {
            var path = Path.GetTempFileName();

            if (icon != null)
            {
                (icon.Handler as IconHandler)?.Pixbuf?.Save(path, "png");
                ApplicationHandler.TempFiles.Add(path);
            }

            AppIndicator.app_indicator_set_icon(Control, path);
        }

        public void SetMenu(ContextMenu menu)
        {
			//app_indicator_set_menu(Control.Handle, (menu.ControlObject as Gtk.Menu).Handle);

			AppIndicator.app_indicator_set_menu(Control, menu?.NativeHandle ?? _menuEmpty);
			

            //AppIndicator.app_indicator_set_menu(Control, menu.NativeHandle);
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
