using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;

namespace Eto.Platform.Wpf.Forms
{
	public class FormHandler : WpfWindow<System.Windows.Window, Form>, IForm
	{
		public FormHandler()
		{
			Control = new System.Windows.Window();
			Setup ();
		}

		public void Show()
		{
			Control.Show();
		}
	}
}
