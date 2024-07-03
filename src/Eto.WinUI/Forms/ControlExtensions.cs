namespace Eto.WinUI.Forms;

public static class ControlExtensions
{
	public static IWinUIFrameworkElement? GetFrameworkElement(this Control control)
	{
		if (control == null)
			return null;
		var handler = control.Handler as IWinUIFrameworkElement;
		if (handler != null)
			return handler;
		var controlObject = control.ControlObject as Control;
		return controlObject?.GetFrameworkElement() ?? null;
	}

	//public static IWpfContainer GetWpfContainer(this Container control)
	//{
	//	if (control == null)
	//		return null;
	//	var handler = control.Handler as IWpfContainer;
	//	if (handler != null)
	//		return handler;
	//	var controlObject = control.ControlObject as Container;
	//	return controlObject != null ? controlObject.GetWpfContainer() : null;
	//}

	public static mux.FrameworkElement? GetContainerControl(this Control control)
	{
		if (control == null)
			return null;
		var handler = control.Handler as IWinUIFrameworkElement;
		if (handler != null)
			return handler.ContainerControl;

		var controlObject = control.ControlObject as Control;
		if (controlObject != null)
			return controlObject.GetContainerControl();

		return control.ControlObject as mux.FrameworkElement;
	}
}
