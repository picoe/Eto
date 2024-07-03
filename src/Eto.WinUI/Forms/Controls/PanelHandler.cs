
namespace Eto.WinUI.Forms.Controls;

public class PanelHandler : WinUIContainer<muc.Border, Panel, Panel.ICallback>, Panel.IHandler
{
	Control _content;
	public Control Content
	{
		get => _content;
		set
		{
			if (_content != value)
			{
				_content = value;
				Control.Child = value.ToNative();
			}
		}
	}

	public Padding Padding { get; set; }
	public Size MinimumSize { get; set; }
	public ContextMenu ContextMenu { get; set; }
	public override mux.FrameworkElement ContainerControl => Control;

	protected override muc.Border CreateControl() => new();
}
