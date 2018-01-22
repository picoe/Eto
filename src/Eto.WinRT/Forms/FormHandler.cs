#if TODO_XAML
using Eto.Forms;
using sw = Windows.UI.Xaml;
using swc = Windows.UI.Xaml.Controls;

namespace Eto.WinRT.Forms
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
            WpfFrameworkElementHelper.ShouldCaptureMouse = false;
        }
	}
}
#endif