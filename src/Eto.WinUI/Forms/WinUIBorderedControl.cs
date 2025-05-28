using Eto.WinUI.Forms.Controls;

namespace Eto.WinUI.Forms;

public class WinUIBorderedControl<TControl, TWidget, TCallback> : WinUIFrameworkElement<TControl, TWidget, TCallback>
	where TControl : mux.FrameworkElement
	where TWidget : Control
	where TCallback : Control.ICallback
{
	readonly EtoDockPanel _border = new();

	public Padding Padding
	{
		get => _border.Padding.ToEto();
		set => _border.Padding = value.ToWinUI();
	}
	public Size MinimumSize
	{
		get => _border.GetMinSize().ToEtoSize();
		set => _border.SetMinSize(value);

	}
	public ContextMenu ContextMenu { get; set; }
	public sealed override mux.FrameworkElement ContainerControl => _border;

	public override Color BackgroundColor
	{
		get => _border.Background.ToEtoColor();
		set => _border.Background = value.ToWinUIBrush();
	}

	protected override void Initialize()
	{
		base.Initialize();
		_border.Handler = this;
		_border.Children.Add(Control);
	}

}
