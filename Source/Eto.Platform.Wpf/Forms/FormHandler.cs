using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;
using sw = System.Windows;
using swc = System.Windows.Controls;

namespace Eto.Platform.Wpf.Forms
{
	public class FormHandler : WpfWindow<sw.Window, Form>, IForm
	{
		public override sw.Window CreateControl ()
		{
			return new sw.Window ();
		}

		public void Show()
		{
			Control.WindowStartupLocation = sw.WindowStartupLocation.Manual;
			if (ApplicationHandler.Instance.IsStarted)
				Control.Show ();
			else
				ApplicationHandler.Instance.DelayShownWindows.Add (Control);
		}
	}
}
