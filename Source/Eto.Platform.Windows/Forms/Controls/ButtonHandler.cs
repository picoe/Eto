using System;
using System.Reflection;
using SD = System.Drawing;
using SWF = System.Windows.Forms;
using Eto.Drawing;
using Eto.Forms;
using Eto.Platform.Windows.Drawing;

namespace Eto.Platform.Windows
{
	public class ButtonHandler : WindowsControl<System.Windows.Forms.Button, Button>, IButton
	{
        Image image;

		public override SWF.DockStyle DockStyle {
			get {
				return SWF.DockStyle.None;
			}
		}

		public ButtonHandler()
		{
			Control = new SWF.Button();
			Control.MinimumSize = Button.DefaultSize.ToSD ();
			Control.AutoSize = true;
			Control.Click += delegate {
				Widget.OnClick(EventArgs.Empty);
			};
		}
                //this.Control.MaximumSize = Generator.Convert(value);
        public Image Image
        {
            get
            {
                return image;
            }
            set
            {
                image = value;
                var sdimage = image.ControlObject as SD.Image;
                if (sdimage != null) Control.Image = sdimage;
                else
                {
                    var icon = image.ControlObject as IconHandler;
                    Control.Image = icon.GetLargestIcon().ToBitmap();
                }
                if (//!sizeSet &&   TODO
                    Control.Image != null)
                    Control.Size = Control.Image.Size;
            }
        }
	}
}
