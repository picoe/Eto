using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;
using sw = System.Windows;

namespace Eto.Platform.Wpf.Forms
{
	public class FormHandler : WpfWindow<sw.Window, Form>, IForm
	{
		public FormHandler()
		{
			Control = new sw.Window ();
			Setup ();
		}

		public void Show()
		{
			Control.WindowStartupLocation = sw.WindowStartupLocation.Manual;
			Control.Show();
		}
	}
}
