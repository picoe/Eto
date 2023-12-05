

namespace Eto.WinForms.Forms.Controls
{
	public class NativeControlHandler : WindowsControl<swf.Control, Control, Control.ICallback>, NativeControlHost.IHandler
	{
		swf.IWin32Window _win32Window;
		public NativeControlHandler(swf.Control nativeControl)
		{
			Control = nativeControl;
		}
		
		public NativeControlHandler()
		{
		}
		

		public void Create(object controlObject)
		{
			if (controlObject == null)
			{
				Control = new swf.UserControl();
			}
			else if (controlObject is swf.Control control)
			{
				Control = control;
			}
			else if (controlObject is IntPtr handle)
			{
				CreateWithHandle(handle);
			}
			else if (controlObject is swf.IWin32Window win32Window)
			{
				// keep a reference so it doesn't get GC'd
				_win32Window = win32Window;
				CreateWithHandle(win32Window.Handle);
			}
			else
				throw new NotSupportedException($"controlObject of type {controlObject.GetType()} is not supported by this platform");
		}

		private void CreateWithHandle(IntPtr handle)
		{
			Control = new swf.Control();
			Win32.GetWindowRect(handle, out var rect);
			Win32.SetParent(handle, Control.Handle);
			Control.Size = rect.ToSD().Size;
			Widget.SizeChanged += (sender, e) =>
			{
				var size = Control.Size;
				Win32.SetWindowPos(handle, IntPtr.Zero, 0, 0, size.Width, size.Height, Win32.SWP.NOZORDER);
			};
		}
	}
}

