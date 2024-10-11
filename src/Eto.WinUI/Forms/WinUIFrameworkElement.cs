namespace Eto.WinUI.Forms;

public static class ControlExtensions
{
	public static IWinUIFrameworkElement GetFrameworkElement(this Control control)
	{
		if (control == null)
			return null;
		var handler = control.Handler as IWinUIFrameworkElement;
		if (handler != null)
			return handler;
		var controlObject = control.ControlObject as Control;
		return controlObject != null ? controlObject.GetFrameworkElement() : null;
	}

	//public static IWpfContainer GetWpfContainer(this Container control)
	//{
	//	if (control == null)
	//		return null;
	//	var handler = control.Handler as IWpfContainer;
	//	if (handler != null)
	//		return handler;
	//	var controlObject = control.ControlObject as Container;
	//	return controlObject != null ? controlObject.GetWpfContainer() : null;
	//}

	public static mux.FrameworkElement GetContainerControl(this Control control)
	{
		if (control == null)
			return null;
		var handler = control.Handler as IWinUIFrameworkElement;
		if (handler != null)
			return handler.ContainerControl;

		var controlObject = control.ControlObject as Control;
		if (controlObject != null)
			return controlObject.GetContainerControl();

		return control.ControlObject as mux.FrameworkElement;
	}
}

public interface IWinUIFrameworkElement
{
	mux.FrameworkElement ContainerControl { get; }
}

public abstract partial class WinUIFrameworkElement<TControl, TWidget, TCallback> : WidgetHandler<TControl, TWidget, TCallback>, Control.IHandler, IWinUIFrameworkElement
	where TControl : class
	where TWidget : Control
	where TCallback : Control.ICallback
{

	public abstract mux.FrameworkElement ContainerControl { get; }
	public virtual mux.FrameworkElement FocusControl => ContainerControl;


	public virtual Color BackgroundColor { get; set; }

	public Size Size
	{
		get => new Size(Width, Height);
		set
		{
			Width = value.Width;
			Height = value.Height;
		}
	}

	public int Width
	{
		get => (int)ContainerControl.Width; 
		set => ContainerControl.Width = value;
	}
	public int Height
	{
		get => (int)ContainerControl.Height;
		set => ContainerControl.Height = value;
	}
	public virtual bool Enabled { get; set; }
	public virtual bool HasFocus => ContainerControl.FocusState != mux.FocusState.Unfocused;
	public virtual bool Visible
	{
		get => ContainerControl.Visibility == mux.Visibility.Visible;
		set => ContainerControl.Visibility = value ? mux.Visibility.Visible : mux.Visibility.Collapsed;
	}
	public IEnumerable<string> SupportedPlatformCommands { get; }
	public Point Location { get; }
	public string ToolTip { get; set; }
	public Cursor Cursor { get; set; }
	public int TabIndex { get; set; }
	public virtual IEnumerable<Control> VisualControls { get; }
	public virtual bool AllowDrop
	{
		get => ContainerControl.AllowDrop;
		set => ContainerControl.AllowDrop = value;
	}
	public bool IsMouseCaptured { get; }

	public bool CaptureMouse()
	{
		throw new NotImplementedException();
	}

	public virtual void DoDragDrop(DataObject data, DragEffects allowedEffects, Image image, PointF cursorOffset)
	{
		throw new NotImplementedException();
	}

	public virtual void Focus()
	{
		ContainerControl.Focus(mux.FocusState.Programmatic);
	}

	public Window GetNativeParentWindow()
	{
		throw new NotImplementedException();
	}

	public virtual SizeF GetPreferredSize(SizeF availableSize)
	{
		throw new NotImplementedException();
	}

	public virtual void Invalidate(bool invalidateChildren)
	{
	}

	public virtual void Invalidate(Rectangle rect, bool invalidateChildren)
	{
	}

	public virtual void MapPlatformCommand(string systemCommand, Command command)
	{
	}

	public virtual void OnLoad(EventArgs e)
	{
	}

	public virtual void OnLoadComplete(EventArgs e)
	{
	}

	public virtual void OnPreLoad(EventArgs e)
	{
	}

	public virtual void OnUnLoad(EventArgs e)
	{
	}

	public virtual PointF PointFromScreen(PointF point)
	{
		throw new NotImplementedException();
	}

	public virtual PointF PointToScreen(PointF point)
	{
		throw new NotImplementedException();
		//ContainerControl.TransformToVisual()
	}

	public void ReleaseMouseCapture()
	{
		throw new NotImplementedException();
	}

	public virtual void ResumeLayout()
	{
	}

	public virtual void SetParent(Container oldParent, Container newParent)
	{
	}

	public virtual void SuspendLayout()
	{
	}

	public virtual void UpdateLayout() => ContainerControl.InvalidateMeasure();
}
