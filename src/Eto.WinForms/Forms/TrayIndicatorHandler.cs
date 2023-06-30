using Eto.WinForms.Forms.Menu;

namespace Eto.WinForms.Forms
{
    public class TrayIndicatorHandler : WidgetHandler<swf.NotifyIcon, TrayIndicator, TrayIndicator.ICallback>, TrayIndicator.IHandler
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
					var bitmap = new sd.Bitmap(_image.ToSD());
					Control.Icon = sd.Icon.FromHandle(bitmap.GetHicon());
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
            Control = new swf.NotifyIcon();
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
