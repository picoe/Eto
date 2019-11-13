using System;
using System.Windows.Input;
using Eto.Forms;
using sw = System.Windows;
using swc = System.Windows.Controls;

namespace Eto.Wpf.Forms
{
	public class FormHandler : WpfWindow<sw.Window, Form, Form.ICallback>, Form.IHandler
	{
		public class EtoWindow : sw.Window
		{

			public EtoWindow()
			{
				AllowDrop = true;
			}

			protected override void OnActivated(EventArgs e)
			{
				if (!Focusable)
					return;
				base.OnActivated(e);
			}

			protected override void OnPreviewGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
			{
				if (!Focusable)
				{
					e.Handled = true;
					return;
				}
				base.OnPreviewGotKeyboardFocus(e);
			}
		}

		public FormHandler(sw.Window window)
		{
			Control = window;
		}

		public FormHandler()
		{
			Control = new EtoWindow();
		}

		public void Show()
		{
			Control.WindowStartupLocation = sw.WindowStartupLocation.Manual;
			if (ApplicationHandler.Instance.IsStarted)
				Control.Show();
			else
				ApplicationHandler.Instance.DelayShownWindows.Add(Control);
			WpfFrameworkElementHelper.ShouldCaptureMouse = false;
		}

		public bool ShowActivated
		{
			get { return Control.ShowActivated; }
			set { Control.ShowActivated = value; }
		}

		public bool CanFocus
		{
			get { return Control.Focusable; }
			set
			{
				SetStyle(Win32.WS_EX.NOACTIVATE, !value);
				SetStyle(Win32.WS.CHILD, !value);
				Control.Focusable = value;
			}
		}
	}
}