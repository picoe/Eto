
using CommunityToolkit.WinUI.Controls;
using Windows.Security.Authentication.OnlineId;

namespace Eto.WinUI.Forms.Controls;

public class EtoDockPanel : CommunityToolkit.WinUI.Controls.DockPanel
{
	public IWinUIFrameworkElement Handler { get; set; }
	protected override wf.Size MeasureOverride(wf.Size availableSize)
	{
		return Handler?.MeasureOverride(availableSize, base.MeasureOverride) ?? base.MeasureOverride(availableSize);
	}

	public mux.UIElement Child
	{
		get => Children.Count > 0 ? Children[0] : null;
		set
		{
			if (Children.Count > 0)
				Children.RemoveAt(0);
			if (value != null)
				Children.Insert(0, value);
		}
	}
}

public class PanelHandler : WinUIContainer<EtoDockPanel, Panel, Panel.ICallback>, Panel.IHandler
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

	public Padding Padding
	{
		get => Control.Padding.ToEto();
		set => Control.Padding = value.ToWinUI();
	}
	public Size MinimumSize
	{
		get => Control.GetMinSize().ToEtoSize();
		set => Control.SetMinSize(value);
	}
	public ContextMenu ContextMenu { get; set; }
	public override mux.FrameworkElement ContainerControl => Control;

	public override Color BackgroundColor
	{
		get => Control.Background.ToEtoColor();
		set => Control.Background = value.ToWinUIBrush();
	}

	protected override EtoDockPanel CreateControl() => new EtoDockPanel();
}
