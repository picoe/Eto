using System;
using sd = System.Drawing;
using swf = System.Windows.Forms;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.Platform.Windows
{
	public class FormHandler : WindowHandler<swf.Form, Form>, IForm
	{
		public FormHandler()
		{
			Control = new swf.Form {
				StartPosition = swf.FormStartPosition.CenterParent,
				AutoSize = true,
				Size = sd.Size.Empty,
				MinimumSize = sd.Size.Empty
			};
		}

		public void Show()
		{
			Control.Show();
		}
	}
}
