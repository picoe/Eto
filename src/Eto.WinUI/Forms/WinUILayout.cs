namespace Eto.WinUI.Forms;

public abstract class WinUILayout<TControl, TWidget, TCallback> : WinUIContainer<TControl, TWidget, TCallback>, Layout.IHandler
	where TControl : mux.UIElement
	where TWidget : Layout
	where TCallback : Layout.ICallback
{

	public virtual void Update()
	{
		Control.UpdateLayout();
	}
}
