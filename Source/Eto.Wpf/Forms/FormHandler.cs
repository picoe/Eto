using Eto.Forms;
using Eto.Drawing;
using sw = System.Windows;
using swc = System.Windows.Controls;

namespace Eto.Wpf.Forms
{
	public class FormHandler : WpfWindow<sw.Window, Form, Form.ICallback>, Form.IHandler
	{
		public FormHandler(sw.Window window)
		{
			Control = window;
		}

		public FormHandler()
		{
			Control = new sw.Window();
		}

		public void Show()
		{
			Control.WindowStartupLocation = sw.WindowStartupLocation.Manual;
			var size = Widget.Content.GetPreferredSize(Size.MaxValue); //.Round(Widget.Screen.Bounds.Size));
			//InnerContent.MinWidth = size.Width;
			//InnerContent.MinHeight = size.Height;

			if (ApplicationHandler.Instance.IsStarted)
				Control.Show();
			else
				ApplicationHandler.Instance.DelayShownWindows.Add(Control);
			WpfFrameworkElementHelper.ShouldCaptureMouse = false;
		}
	}
}
