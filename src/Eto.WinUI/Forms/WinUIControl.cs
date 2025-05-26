
using Microsoft.UI.Xaml;

namespace Eto.WinUI.Forms;

public abstract class WinUIControl<TControl, TWidget, TCallback> : WinUIFrameworkElement<TControl, TWidget, TCallback>, Control.IHandler
	where TControl : muc.Control
	where TWidget : Control
	where TCallback : Control.ICallback
{
	public override FrameworkElement ContainerControl => Control;

	public virtual Font Font { get; set; }

	public override bool Enabled
	{
		get => Control.IsEnabled;
		set => Control.IsEnabled = value;
	}

	public override Color BackgroundColor
	{
		get => Control.Background.ToEtoColor();
		set => Control.Background = value.ToWinUIBrush();
	}
	public virtual Color TextColor
	{
		get => Control.Foreground.ToEtoColor();
		set => Control.Foreground = value.ToWinUIBrush();
	}
}
