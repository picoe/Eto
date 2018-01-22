using Eto.Drawing;
using Eto.Forms;
using System;
using System.IO;
using sd = System.Drawing;
using swf = System.Windows.Forms;
using swmi = System.Windows.Media.Imaging;
using swc = System.Windows.Controls;
using Eto.Wpf.Forms.Menu;

namespace Eto.Wpf.Forms
{
    public class TrayIndicatorHandler : WidgetHandler<swf.NotifyIcon, TrayIndicator, TrayIndicator.ICallback>, TrayIndicator.IHandler
    {
		Image _image;

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
            Control = new swf.NotifyIcon();
            Control.MouseClick += Control_MouseClick;
        }

        public Image Image
		{
			get { return _image; }
			set
			{
				_image = value;
				Control.Icon = _image.ToSDIcon();
			}
		}

        public ContextMenu Menu { get; set; }

        private void Control_MouseClick(object sender, swf.MouseEventArgs e)
        {
			var menu = ContextMenuHandler.GetControl(Menu);

			if (menu != null && e.Button.HasFlag(swf.MouseButtons.Right))
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
                        if (e.Button.HasFlag(swf.MouseButtons.Left))
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
