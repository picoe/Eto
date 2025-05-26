using Eto.WinUI.Forms.Controls;

namespace Eto.WinUI.Forms;

public abstract class WinUIPanel<TControl, TWidget, TCallback> : WinUIContainer<TControl, TWidget, TCallback>, Panel.IHandler
	where TControl : class
	where TWidget : Panel
	where TCallback : Panel.ICallback
{
	Control _content;
	public virtual Control Content
	{
		get => _content;
		set
		{
			if (_content != value)
			{
				_content = value;
				_border.Child = _content.ToNative();
			}
		}
	}
	public virtual Padding Padding
	{
		get => _border.Padding.ToEto();
		set => _border.Padding = value.ToWinUI();
	}
	public virtual Size MinimumSize
	{
		get => ContainerControl.GetMinSize().ToEtoSize();
		set => ContainerControl.SetMinSize(value);
	}

	public ContextMenu ContextMenu { get; set; }

	EtoDockPanel _border;

	public WinUIPanel()
	{
		_border = new EtoDockPanel { Handler = this };
	}
	public override mux.FrameworkElement ContainerControl => _border;

	protected override void Initialize()
	{
		base.Initialize();
		SetContainerContent(_border);
	}

	public abstract void SetContainerContent(mux.FrameworkElement content);

}
