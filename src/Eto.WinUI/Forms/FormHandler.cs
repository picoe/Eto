using mui = Microsoft.UI.Xaml;

namespace Eto.WinUI.Forms;

public class FormHandler : WinUIWindow<mui.Window, Form, Form.ICallback>, Form.IHandler
{
	protected override mui.Window CreateControl() => new mui.Window();

	public bool ShowActivated { get; set; }
	public bool CanFocus { get; set; }

	public void Show()
	{
		//Control.CoreWindow.Di = Windows.UI.Core.CoreWindowActivationMode.ActivatedNotForeground;
		Control.Activate();
	}
}
