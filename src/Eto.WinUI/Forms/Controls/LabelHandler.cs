namespace Eto.WinUI.Forms.Controls;

public class LabelHandler : WinUIFrameworkElement<muc.TextBlock, Label, Label.ICallback>, Label.IHandler
{
	public override mux.FrameworkElement ContainerControl => Control;
	protected override muc.TextBlock CreateControl() => new muc.TextBlock();

	public TextAlignment TextAlignment
	{
		get => Control.TextAlignment.ToEto();
		set => Control.TextAlignment = value.ToWinUI();
	}

	public VerticalAlignment VerticalAlignment
	{
		get => Control.VerticalAlignment.ToEto();
		set => Control.VerticalAlignment = value.ToWinUI();
	}

	public WrapMode Wrap
	{
		get => Control.TextWrapping.ToEto();
		set => Control.TextWrapping = value.ToWinUI();
	}
	public string Text
	{
		get => Control.Text;
		set => Control.Text = value;
	}
	public Color TextColor
	{
		get => Control.Foreground.ToEtoColor();
		set => Control.Foreground = value.ToWinUIBrush();
	}

	public Font Font { get; set; }

	public override void AttachEvent(string id)
	{
		switch (id)
		{
			case TextControl.TextChangedEvent:
				// do nothing, label doesn't get updated by the user
				break;

			default:
				base.AttachEvent(id);
				break;
		}
	}
}
