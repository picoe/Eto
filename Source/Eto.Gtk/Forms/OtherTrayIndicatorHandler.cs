using System;
using Eto.Drawing;
using Eto.Forms;
using Eto.GtkSharp.Drawing;

namespace Eto.GtkSharp.Forms
{
	public class OtherTrayIndicatorHandler : WidgetHandler<Gtk.StatusIcon, TrayIndicator, TrayIndicator.ICallback>, TrayIndicator.IHandler
	{
		public string Title
		{
			get { return tooltip; }
			set
			{
#if GTK3
				Control.TooltipText = tooltip = value;
#else
				Control.Tooltip = tooltip = value;
#endif
			}
        }

        public bool Visible
        {
            get { return Control.Visible; }
            set { Control.Visible = value; }
        }

        private string tooltip;
        private Gtk.Menu menu;

        public OtherTrayIndicatorHandler()
        {
            Control = new Gtk.StatusIcon();
            Control.PopupMenu += Control_PopupMenu;

            tooltip = "";
        }

        public void SetIcon(Icon icon)
        {
            Gdk.Pixbuf pixbuf = null;

            if (icon != null)
                pixbuf = (icon.Handler as IconHandler)?.Pixbuf;

            Control.Pixbuf = pixbuf;
        }

        public void SetMenu(ContextMenu menu)
        {
            this.menu = menu?.ControlObject as Gtk.Menu;
        }

        private void Control_PopupMenu(object o, Gtk.PopupMenuArgs args)
        {
            menu?.Popup(null, null, null, (uint)args.Args[0], (uint)args.Args[1]);
        }

        public override void AttachEvent(string id)
        {
            switch (id)
            {
                case TrayIndicator.ActivatedEvent:
                    Control.Activate += (o, e) => Callback.OnActivated(Widget, EventArgs.Empty);
                    break;
                default:
                    base.AttachEvent(id);
                    break;
            }
        }
    }
}
