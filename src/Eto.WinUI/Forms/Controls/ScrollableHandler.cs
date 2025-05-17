

using Microsoft.UI.Xaml;

namespace Eto.WinUI.Forms.Controls;


public class ScrollableHandler : WinUIPanel<muc.ScrollView, Scrollable, Scrollable.ICallback>, Scrollable.IHandler
{
	EtoDockPanel _container;

	public ScrollableHandler()
	{
		_container = new EtoDockPanel { Handler = this };
	}

	protected override void Initialize()
	{
		base.Initialize();
		_container.Child = Control;
	}

	protected override muc.ScrollView CreateControl() => new();

	public void UpdateScrollSizes()
	{
		
	}

	public override void SetContainerContent(FrameworkElement content)
	{
		Control.Content = content;
	}

	public override mux.FrameworkElement ContainerControl => _container;

	public Point ScrollPosition { get; set; }
	public Size ScrollSize { get; set; }
	public BorderType Border { get; set; }
	public Rectangle VisibleRect { get; }
	public bool ExpandContentWidth { get; set; }
	public bool ExpandContentHeight { get; set; }
	public float MinimumZoom { get; set; }
	public float MaximumZoom { get; set; }
	public float Zoom { get; set; }
	public override Color BackgroundColor
	{
		get => Control.Background.ToEtoColor();
		set => Control.Background = value.ToWinUIBrush();
	}
}
