using Eto.Drawing;
using Eto.Forms;
using System;
using Sd = System.Drawing;
using Swf = System.Windows.Forms;
using Eto.WinForms.Forms.Menu;

namespace Eto.WinForms.Forms
{
    public class TrayIndicatorHandler : WidgetHandler<Swf.NotifyIcon, TrayIndicator, TrayIndicator.ICallback>, TrayIndicator.IHandler
    {
		Image _image;
		ContextMenu _menu;
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

		public Image Image
		{
			get { return _image; }
			set
			{
				_image = value;
				if (_image == null)
					Control.Icon = null;
				else
				{
					var bitmap = new Sd.Bitmap(_image.ToSD());
					Control.Icon = Sd.Icon.FromHandle(bitmap.GetHicon());
				}
			}
		}

		public ContextMenu Menu
		{
			get { return _menu; }
			set
			{
				_menu = value;
				Control.ContextMenuStrip = ContextMenuHandler.GetControl(_menu);
			}
		}

		public TrayIndicatorHandler()
        {
            Control = new Swf.NotifyIcon();
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
