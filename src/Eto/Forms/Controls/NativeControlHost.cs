namespace Eto.Forms;

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
	/// Initializes a new instance of the native control host with the specified native controlObject
	/// </summary>
	/// <param name="controlObject">ControlObject to host, of null to create a native hosting control that the caller can use</param>
	public NativeControlHost(object controlObject)
	{
		Handler.Create(controlObject);
		Initialize();
	}

	/// <summary>
	/// Initializes a new instance of the native control host with a native hosting control that the caller can use directly.
	/// </summary>
	public NativeControlHost() : this(null)
	{
	}

	/// <summary>
	/// Handler interface for the <see cref="NativeControlHost"/>
	/// </summary>
	[AutoInitialize(false)]
	public new interface IHandler : Control.IHandler
	{
		/// <summary>
		/// Initializes a new instance of the native control host with the specified native controlObject
		/// </summary>
		/// <param name="controlObject">ControlObject to host, of null to create a native hosting control that the caller can use</param>
		void Create(object controlObject);
	}
}
