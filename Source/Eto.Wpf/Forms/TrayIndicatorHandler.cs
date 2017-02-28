using Eto.Drawing;
using Eto.Forms;
using System;
using System.IO;
using Sd = System.Drawing;
using Swf = System.Windows.Forms;
using Swmi = System.Windows.Media.Imaging;
using Swc = System.Windows.Controls;

namespace Eto.Wpf.Forms
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

        private Swc.ContextMenu menu;

        public TrayIndicatorHandler()
        {
            Control = new Swf.NotifyIcon();
            Control.MouseClick += Control_MouseClick;
        }

        public void SetIcon(Icon icon)
        {
            if (icon == null)
                Control.Icon = null;
            else
            {
                using (var stream = new MemoryStream())
                {
                    var enc = new Swmi.BmpBitmapEncoder();
                    enc.Frames.Add(Swmi.BitmapFrame.Create(icon.ToWpf()));
                    enc.Save(stream);
                    stream.Position = 0;

                    var bitmap = new Sd.Bitmap(stream);
                    bitmap.MakeTransparent();
                    Control.Icon = Sd.Icon.FromHandle(bitmap.GetHicon());
                }
            }
        }

        public void SetMenu(ContextMenu menu)
        {
            this.menu = menu?.ControlObject as Swc.ContextMenu;
        }

        private void Control_MouseClick(object sender, Swf.MouseEventArgs e)
        {
            if (menu != null && e.Button.HasFlag(Swf.MouseButtons.Right))
            {
                // TODO: Close tray menu when clicked outside
                // https://weblogs.asp.net/marianor/a-wpf-wrapper-around-windows-form-notifyicon

                menu.IsOpen = true;
            }
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
