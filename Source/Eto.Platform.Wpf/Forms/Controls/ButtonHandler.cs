using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Platform.Wpf.Forms.Controls
{
	public class ButtonHandler : WpfControl<System.Windows.Controls.Button, Button>, IButton
	{
        IImage image;

		public ButtonHandler ()
		{
			Control = new System.Windows.Controls.Button ();
			Control.Click += new System.Windows.RoutedEventHandler (Control_Click);
			Control.MinWidth = Button.DefaultSize.Width;
			Control.MinHeight = Button.DefaultSize.Height;
		}

		void Control_Click (object sender, System.Windows.RoutedEventArgs e)
		{
			Widget.OnClick (EventArgs.Empty);
		}

		public string Text
		{
			get { return Generator.ConvertMneumonicFromWPF(Control.Content); }
			set { Control.Content = Generator.ConvertMneumonicToWPF(value); }
		}

		public IImage Image
		{
			get { return image; }
			set
			{
                image = value;
                
				if (image != null) {
                    Control.Content =
                        image.ControlObject;

					if (false)//if (!setSize) 
                    {
						Control.Width = image.Size.Width;
						Control.Height = image.Size.Height;
					}
				}
				else
					Control.Content = null;
			}
		}
	}
}
