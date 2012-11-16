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
        IImage image;

		public override SWF.DockStyle DockStyle {
			get {
				return SWF.DockStyle.None;
			}
		}

        static ButtonHandler()
        {
            Style.Add<Button, SWF.Button>(
                "flatbutton",
                (widget,
                    control) =>
                {
                    control.FlatStyle = SWF.FlatStyle.Flat;
                    control.FlatAppearance.BorderSize = 0;
                    control.UseVisualStyleBackColor = true;
                });

            Style.Add<Button, SWF.Button>(
                "right",
                (widget,
                    control) =>
                {
                    control.Dock = SWF.DockStyle.Right;
                });

            Style.Add<Button, SWF.Button>(
                "fixed_size",
                (widget,
                    control) =>
                {
                    control.AutoSize = false;
                });

            Style.Add<Button, SWF.Button>(
                "tabbutton",
                (widget,
                    control) =>
                {
                    control.FlatStyle = 
                        SWF.FlatStyle.Flat;

                    control.AutoSize = true;

                    control.Dock =
                        SWF.DockStyle.Left;

                    control.Image =
                        null;//DockingWindows.Properties.Resources.Debugger,

                    control.BackColor =
                        SD.SystemColors.ButtonFace;

                    control.ImageAlign =
                        SD.ContentAlignment.MiddleLeft;

                    control.TextImageRelation = 
                        SWF.TextImageRelation.ImageBeforeText;

                    control.FlatAppearance.BorderSize = 0;
                });
        }

		public ButtonHandler()
		{
			Control = new SWF.Button();
			Control.MinimumSize = Generator.Convert(Button.DefaultSize);
			Control.AutoSize = true;
			Control.Click += delegate {
				Widget.OnClick(EventArgs.Empty);
			};
		}
                //this.Control.MaximumSize = Generator.Convert(value);
        public IImage Image
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
