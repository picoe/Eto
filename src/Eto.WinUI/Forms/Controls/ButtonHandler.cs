namespace Eto.WinUI.Forms.Controls;

public class ButtonHandler : WinUIControl<muc.Button, Button, Button.ICallback>, Button.IHandler
{
	muc.TextBlock _textBlock;
	public Image Image { get; set; }
	public ButtonImagePosition ImagePosition { get; set; }
	public Size MinimumSize { get; set; }

	public string Text
	{
		get => _textBlock.Text;
		set => _textBlock.Text = value;
	}

	protected override muc.Button CreateControl() => new muc.Button();

	protected override void Initialize()
	{
		base.Initialize();
		_textBlock = new muc.TextBlock();
		Control.Content = _textBlock;
		Control.Click += Control_Click;
	}

	private void Control_Click(object sender, mux.RoutedEventArgs e)
	{
		Callback.OnClick(Widget, EventArgs.Empty);
	}

	public override void AttachEvent(string id)
	{
		switch (id)
		{
			default:
				base.AttachEvent(id);
				break;
		}
	}
}
