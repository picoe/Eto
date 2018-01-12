using Eto.Drawing;
using Eto.Forms;
using System;
using Sd = System.Drawing;
using Swf = System.Windows.Forms;

namespace Eto.WinForms.Forms
{
    public class TrayIndicatorHandler : WidgetHandler<Swf.NotifyIcon, TrayIndicator, TrayIndicator.ICallback>, TrayIndicator.IHandler
    {
        public string Title
        {
            get { return Control.Text; }
            set { Control.Text = value; }
        }

        public bool Visible
        {
            get { return Control.Visible; }
            set { Control.Visible = value; }
        }

        public TrayIndicatorHandler()
        {
            Control = new Swf.NotifyIcon();
        }

        public void SetIcon(Icon icon)
        {
            if (icon == null)
                Control.Icon = null;
            else
            {
                var bitmap = new Sd.Bitmap(icon.ToSD());
                Control.Icon = Sd.Icon.FromHandle(bitmap.GetHicon());
            }
        }

        public void SetMenu(ContextMenu menu)
        {
            Control.ContextMenuStrip = menu.ControlObject as Swf.ContextMenuStrip;
        }

        public override void AttachEvent(string id)
        {
            switch (id)
            {
                case TrayIndicator.ActivatedEvent:
                    Control.MouseClick += (sender, e) =>
                    {
                        if (e.Button.HasFlag(Swf.MouseButtons.Left))
                            Callback.OnActivated(Widget, EventArgs.Empty);
                    };
                    break;
                default:
                    base.AttachEvent(id);
                    break;
            }
        }
    }
}
