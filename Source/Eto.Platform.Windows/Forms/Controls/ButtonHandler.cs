using System;
using System.Reflection;
using SD = System.Drawing;
using SWF = System.Windows.Forms;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.Platform.Windows
{
	public class ButtonHandler : WindowsControl<System.Windows.Forms.Button, Button>, IButton
	{

		public ButtonHandler()
		{
			Control = new SWF.Button();
			Control.MinimumSize = Button.DefaultSize.ToSD ();
			Control.AutoSize = true;
			Control.Click += delegate {
				Widget.OnClick(EventArgs.Empty);
			};
		}
	}
}
