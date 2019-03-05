// status icon is obsolete, but we still want to use it.
#pragma warning disable 612, 618
using System;
using Eto.Drawing;
using Eto.Forms;
using Eto.GtkSharp.Drawing;

namespace Eto.GtkSharp.Forms
{
	public class OtherTrayIndicatorHandler : WidgetHandler<Gtk.StatusIcon, TrayIndicator, TrayIndicator.ICallback>, TrayIndicator.IHandler
	{
		Image image;
		string tooltip;

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

        public OtherTrayIndicatorHandler()
        {
            Control = new Gtk.StatusIcon();
			Control.Visible = false;
            Control.PopupMenu += Control_PopupMenu;
        }

		public Image Image
		{
			get { return image; }
			set { Control.Pixbuf = (image = value).ToGdk(); }
		}

		public ContextMenu Menu { get; set; }

        private void Control_PopupMenu(object o, Gtk.PopupMenuArgs args)
        {
			Menu.ToGtk()?.Popup(null, null, null, (uint)args.Args[0], (uint)args.Args[1]);
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
#pragma warning restore 612, 618