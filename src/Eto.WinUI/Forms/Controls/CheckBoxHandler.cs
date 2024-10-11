namespace Eto.WinUI.Forms.Controls;

public class CheckBoxHandler : WinUIControl<muc.CheckBox, CheckBox, CheckBox.ICallback>, CheckBox.IHandler
{
	public string Text
	{
		get => Control.Content as string;
		set => Control.Content = value;
	}
	public bool? Checked
	{
		get => Control.IsChecked;
		set => Control.IsChecked = value;
	}
	public bool ThreeState
	{
		get => Control.IsThreeState;
		set => Control.IsThreeState = value;
	}

	protected override muc.CheckBox CreateControl() => new();

	protected override void Initialize()
	{
		base.Initialize();
		Control.Checked += Control_Checked;
	}

	private void Control_Checked(object sender, mux.RoutedEventArgs e)
	{
		Callback.OnCheckedChanged(Widget, EventArgs.Empty);
	}

}
