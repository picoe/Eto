

namespace Eto.WinUI.Forms.Controls;

public class ScrollableHandler : WinUIContainer<muc.ScrollView, Scrollable, Scrollable.ICallback>, Scrollable.IHandler
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
				Control.Content = value.ToNative();
			}
		}
	}

	public Padding Padding { get; set; }
	public Size MinimumSize { get; set; }
	public ContextMenu ContextMenu { get; set; }

	protected override muc.ScrollView CreateControl() => new();

	public void UpdateScrollSizes()
	{
		throw new NotImplementedException();
	}

	public override mux.FrameworkElement ContainerControl => Control;

	public Point ScrollPosition { get; set; }
	public Size ScrollSize { get; set; }
	public BorderType Border { get; set; }
	public Rectangle VisibleRect { get; }
	public bool ExpandContentWidth { get; set; }
	public bool ExpandContentHeight { get; set; }
	public float MinimumZoom { get; set; }
	public float MaximumZoom { get; set; }
	public float Zoom { get; set; }
}
