

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
		
		protected override void Initialize()
		{
			// don't call any initialize routines as we are hosting a native control
			//base.Initialize();
		}
		
		protected override swf.Control CreateControl()
		{
			if (Widget is NativeControlHost host && Callback is NativeControlHost.ICallback callback)
			{
				var args = new CreateNativeControlArgs();
				callback.OnCreateNativeControl(host, args);
				return CreateHost(args.NativeControl);
			}
			return base.CreateControl();
		}
		

		public void Create(object nativeControl)
		{
			Control = CreateHost(nativeControl);
		}
		swf.Control CreateHost(object nativeControl)
		{
			if (nativeControl == null)
			{
				return new swf.UserControl();
			}
			else if (nativeControl is swf.Control control)
			{
				return control;
			}
			else if (nativeControl is IntPtr handle)
			{
				return CreateWithHandle(handle);
			}
			else if (nativeControl is swf.IWin32Window win32Window)
			{
				// keep a reference so it doesn't get GC'd
				_win32Window = win32Window;
				return CreateWithHandle(win32Window.Handle);
			}
			else
				throw new NotSupportedException($"Native control of type {nativeControl.GetType()} is not supported by this platform");
		}

		private swf.Control CreateWithHandle(IntPtr handle)
		{
			var control = new swf.Control();
			Win32.GetWindowRect(handle, out var rect);
			Win32.SetParent(handle, control.Handle);
			control.Size = rect.ToSD().Size;
			Widget.SizeChanged += (sender, e) =>
			{
				var size = control.Size;
				Win32.SetWindowPos(handle, IntPtr.Zero, 0, 0, size.Width, size.Height, Win32.SWP.NOZORDER);
			};
			return control;
		}
	}
}

