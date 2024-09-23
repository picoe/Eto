namespace Eto.Wpf.Forms
{
	public class EtoFormWindow : EtoWindow
	{
		public EtoFormWindow()
		{
			AllowDrop = true;
		}

		protected override void OnActivated(EventArgs e)
		{
			if (!Focusable)
				return;
			base.OnActivated(e);
		}

		protected override void OnPreviewGotKeyboardFocus(swi.KeyboardFocusChangedEventArgs e)
		{
			if (!Focusable)
			{
				e.Handled = true;
				return;
			}
			base.OnPreviewGotKeyboardFocus(e);
		}
	}
	
	public class FormHandler : WpfWindow<sw.Window, Form, Form.ICallback>, Form.IHandler
	{
		public FormHandler(sw.Window window)
		{
			Control = window;
		}

		public FormHandler()
		{
			Control = new EtoFormWindow();
		}

		public virtual void Show()
		{
			Control.WindowStartupLocation = sw.WindowStartupLocation.Manual;
			if (Control.IsLoaded)
			{
				Callback.OnLoadComplete(Widget, EventArgs.Empty);
				FireOnLoadComplete = false;
			}
			else
			{
				// We should trigger during the Control.Loaded event
				FireOnLoadComplete = true;
			}
			
			var _ = NativeHandle; // ensure SourceInitialized is called to get right size based on style flags
			
			if (ApplicationHandler.Instance.IsStarted)
				Control.Show();
			else
				ApplicationHandler.Instance.DelayShownWindows.Add(Control);
			WpfFrameworkElementHelper.ShouldCaptureMouse = false;
		}

		protected override void InternalClosing()
		{
			// Clear owner so WPF doesn't change the z-order of the parent when closing
			SetOwner(null);
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
				Control.Focusable = value;
				SetStyleEx(Win32.WS_EX.NOACTIVATE, !value);
				SetStyle(Win32.WS.CHILD, !value);
			}
		}
		
		public override void Focus()
		{
			if (!Control.Focusable)
				BringToFront();
			else
				base.Focus();
		}

		
	}
}