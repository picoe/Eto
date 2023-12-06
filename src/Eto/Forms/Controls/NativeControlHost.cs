namespace Eto.Forms;


/// <summary>
/// Arguments when creating a native control for the <see cref="NativeControlHost"/>
/// </summary>
public class CreateNativeControlArgs : EventArgs
{
	/// <summary>
	/// Native control or handle to use. See comments for <see cref="NativeControlHost"/> on what types are supported for each platform.
	/// </summary>
	/// 
	public object NativeControl { get; set; }
}

/// <summary>
/// Control to host a native control within Eto
/// </summary>
/// <remarks>
/// This can be used as a cross platform way to convert a native control to an eto control without having to reference
/// Eto's platform-specific assemblies and using its ToNatve() method or creating custom handlers.
/// 
/// The type of the supplied controlObject supported depends on the platform you are running on.
/// - Wpf:         FrameworkElement, HWND (IntPtr), System.Windows.Forms.Control, or IWin32Window
/// - WinForms:    HWND, System.Windows.Forms.Control, or IWin32Window
/// - Mac64/macOS: AppKit.NSView, handle to NSView (IntPtr)
/// - Gtk:         Gtk.Widget, or handle to Gtk.Widget (IntPtr)
/// </remarks>
[Handler(typeof(IHandler))]
public class NativeControlHost : Control
{
	new IHandler Handler => (IHandler)base.Handler;

	/// <summary>
	/// Initializes a new instance of the native control host with the specified native control
	/// </summary>
	/// <param name="nativeControl">Native contro to host, of null to create a native hosting control that the caller can use.</param>
	public NativeControlHost(object nativeControl)
	{
		Handler.Create(nativeControl);
		Initialize();
	}

	/// <summary>
	/// Initializes a new instance of the native control host with a native hosting control that the caller can use directly.
	/// </summary>
	public NativeControlHost()
	{
		// derived classes should override OnCreateNativeControl to provide the native control
		if (!IsSubclass)
			Handler.Create(null);
		Initialize();
	}

	bool IsSubclass => GetType() != typeof(NativeControlHost);
	
	/// <summary>
	/// Called to create the native control.
	/// </summary>
	/// <remarks>
	/// When subclassing, override this method to create your native control and set <see cref="CreateNativeControlArgs.NativeControl"/>
	/// to one of the supported native control(s) for the platform you are on.
	/// </remarks>
	/// <param name="e"></param>
	protected virtual void OnCreateNativeControl(CreateNativeControlArgs e)
	{
	}
	
	/// <summary>
	/// Callback interface for the <see cref="NativeControlHost"/>.
	/// </summary>
	public new interface ICallback : Control.ICallback
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="widget"></param>
		/// <param name="e"></param>
		void OnCreateNativeControl(NativeControlHost widget, CreateNativeControlArgs e);
	}

	/// <summary>
	/// Callback implementation for the <see cref="NativeControlHost"/>
	/// </summary>
	protected new class Callback : Control.Callback, ICallback
	{
		/// <inheritdoc/>
		public void OnCreateNativeControl(NativeControlHost widget, CreateNativeControlArgs e)
		{
			using (widget.Platform.Context)
				widget.OnCreateNativeControl(e);
		}
	}

	static readonly object callback = new Callback();

	/// <inheritdoc/>
	protected override object GetCallback() => callback;

	/// <summary>
	/// Handler interface for the <see cref="NativeControlHost"/>
	/// </summary>
	[AutoInitialize(false)]
	public new interface IHandler : Control.IHandler
	{
		/// <summary>
		/// Initializes a new instance of the native control host with the specified native control
		/// </summary>
		/// <param name="nativeControl">Native contro to host, of null to create a native hosting control that the caller can use</param>
		void Create(object nativeControl);
	}
}
