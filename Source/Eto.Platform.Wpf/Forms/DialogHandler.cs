using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;
using sw = System.Windows;
using swc = System.Windows.Controls;
using Eto.Platform.Wpf.Forms.Controls;

namespace Eto.Platform.Wpf.Forms
{
	public class DialogHandler : WpfWindow<sw.Window, Dialog>, IDialog
	{
		Button defaultButton;
		Button abortButton;

		public DialogHandler ()
		{
			Control = new sw.Window { ShowInTaskbar = false };
			Setup ();
		}

		public DialogResult ShowDialog (Control parent)
		{
			Control.WindowStartupLocation = sw.WindowStartupLocation.CenterOwner;
			var parentWindow = parent.ParentWindow;
			if (parentWindow != null)
				Control.Owner = ((IWpfWindow)parentWindow.Handler).Control;
			var result = Control.ShowDialog ();
			return Widget.DialogResult;
		}

		public Button DefaultButton
		{
			get { return defaultButton; }
			set
			{
				if (defaultButton != null) {
					var handler = defaultButton.Handler as ButtonHandler;
					handler.Control.IsDefault = false;
				}
				defaultButton = value;
				if (defaultButton != null) {
					var handler = defaultButton.Handler as ButtonHandler;
					handler.Control.IsDefault = true;
				}
			}
		}

		public Button AbortButton
		{
			get { return abortButton; }
			set
			{
				if (abortButton != null) {
					var handler = abortButton.Handler as ButtonHandler;
					handler.Control.IsCancel = false;
				}
				abortButton = value;
				if (abortButton != null) {
					var handler = abortButton.Handler as ButtonHandler;
					handler.Control.IsCancel = true;
				}
			}
		}
	}
}
