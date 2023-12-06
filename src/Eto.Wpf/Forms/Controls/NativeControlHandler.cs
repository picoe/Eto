using Eto.Forms;
using System.Windows.Interop;
using IWin32WindowWinForms = System.Windows.Forms.IWin32Window;
using IWin32WindowInterop = System.Windows.Interop.IWin32Window;

namespace Eto.Wpf.Forms.Controls;
public class NativeControlHandler : WpfFrameworkElement<sw.FrameworkElement, Control, Control.ICallback>, NativeControlHost.IHandler
{
	public override IntPtr NativeHandle
	{
		get
		{
			if (Control is EtoHwndHost host)
				return host.Handle;
			return base.NativeHandle;
		}
	}

	public NativeControlHandler()
	{
	}

	public NativeControlHandler(sw.FrameworkElement nativeControl)
	{
		Control = nativeControl;
	}

	protected override void Initialize()
	{
		// don't call any initialize routines as we are hosting a native control
		// base.Initialize();
	}

	protected override sw.FrameworkElement CreateControl()
	{
		if (Widget is NativeControlHost host && Callback is NativeControlHost.ICallback callback)
		{
			var args = new CreateNativeControlArgs();
			callback.OnCreateNativeControl(host, args);
			return CreateHost(args.NativeControl);
		}
		return base.CreateControl();
	}

	public override Color BackgroundColor
	{
		get => throw new NotSupportedException("You cannot get this property for native controls");
		set => throw new NotSupportedException("You cannot set this property for native controls");
	}

	public void Create(object nativeControl)
	{
		Control = CreateHost(nativeControl);
	}
	
	public sw.FrameworkElement CreateHost(object nativeControl)
	{
		if (nativeControl == null)
		{
			var host = new EtoHwndHost(null);
			host.GetPreferredSize += () => Size.Round(UserPreferredSize.ToEto() * (Widget.ParentWindow?.Screen?.LogicalPixelSize ?? 1));
			return host;
		}
		else if (nativeControl is sw.FrameworkElement element)
		{
			return element;
		}
		else if (nativeControl is IntPtr handle)
		{
			return new EtoHwndHost(new HandleRef(this, handle));
		}
		else if (nativeControl is IWin32WindowWinForms win32Window)
		{
			// keep a reference to the win32window object
			var host = new EtoHwndHost(new HandleRef(win32Window, win32Window.Handle));
			host.GetPreferredSize += () => Size.Round(UserPreferredSize.ToEto() * (Widget.ParentWindow?.Screen?.LogicalPixelSize ?? 1));
			return host;
		}
		else if (nativeControl is IWin32WindowInterop win32WindowWpf)
		{
			// keep a reference to the win32window object
			var host = new EtoHwndHost(new HandleRef(win32WindowWpf, win32WindowWpf.Handle));
			host.GetPreferredSize += () => Size.Round(UserPreferredSize.ToEto() * (Widget.ParentWindow?.Screen?.LogicalPixelSize ?? 1));
			return host;
		}
		else
			throw new NotSupportedException($"Native control of type {nativeControl.GetType()} is not supported by this platform");
	}
}

sealed class EtoHwndHost : HwndHost
{
	HandleRef? _hwnd;
	swc.ScrollViewer _parentScrollViewer;
	sd.Rectangle _regionRect = sd.Rectangle.Empty;

	public Func<Size> GetPreferredSize { get; set; }

	public EtoHwndHost(HandleRef? hwnd)
	{
		_hwnd = hwnd;
	}

	protected override HandleRef BuildWindowCore(HandleRef hwndParent)
	{
		if (_hwnd == null)
		{
			// create a new WinForms Usercontrol to host whatever we want
			var size = GetPreferredSize?.Invoke() ?? new Size(100, 100);
			var ctl = new swf.UserControl();
			ctl.Size = size.ToSD();
			_hwnd = new HandleRef(ctl, ctl.Handle);
		}
		Win32.SetParent(_hwnd.Value.Handle, hwndParent.Handle);
		HookParentScrollViewer();
		return _hwnd.Value;
	}

	protected override void OnVisualParentChanged(sw.DependencyObject oldParent)
	{
		base.OnVisualParentChanged(oldParent);
		HookParentScrollViewer();
	}

	void HookParentScrollViewer()
	{
		if (_parentScrollViewer != null)
			_parentScrollViewer.ScrollChanged -= ParentScrollViewerScrollChanged;
		_regionRect = sd.Rectangle.Empty;
		_parentScrollViewer = this.GetVisualParent<swc.ScrollViewer>();
		if (_parentScrollViewer != null)
			_parentScrollViewer.ScrollChanged += ParentScrollViewerScrollChanged;
	}

	protected override void DestroyWindowCore(HandleRef hwnd)
	{
	}

	private void ParentScrollViewerScrollChanged(object sender, swc.ScrollChangedEventArgs e)
	{
		UpdateRegion();
	}

	double LogicalPixelSize => sw.PresentationSource.FromVisual(this)?.CompositionTarget.TransformToDevice.M11 ?? 1.0;

	sd.Size ScaledSize(double w, double h)
	{
		var pixelSize = LogicalPixelSize;
		return new sd.Size((int)Math.Round(w * pixelSize), (int)Math.Round(h * pixelSize));
	}

	void UpdateRegion()
	{
		if (_parentScrollViewer == null || !IsVisible)
			return;

		if (_parentScrollViewer.Content is not sw.FrameworkElement content)
			return;

		var transform = content.TransformToDescendant(this);
		var offset = transform.Transform(new sw.Point(_parentScrollViewer.HorizontalOffset, _parentScrollViewer.VerticalOffset));

		
		var loc = ScaledSize(offset.X, offset.Y);
		var size = ScaledSize(_parentScrollViewer.ViewportWidth, _parentScrollViewer.ViewportHeight);

		var rect = new sd.Rectangle(new sd.Point(loc), size);
		if (rect != _regionRect)
		{
			SetRegion(rect);
			_regionRect = rect;
		}
	}

	private void SetRegion(sd.Rectangle rect)
	{
		using (var graphics = sd.Graphics.FromHwnd(Handle))
			Win32.SetWindowRgn(Handle, new sd.Region(rect).GetHrgn(graphics), true);
	}



}
