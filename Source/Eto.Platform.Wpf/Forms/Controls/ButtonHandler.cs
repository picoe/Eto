using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;

namespace Eto.Platform.Wpf.Forms.Controls
{
	public class ButtonHandler : WpfControl<System.Windows.Controls.Button, Button>, IButton
	{
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
	}
}
