namespace Eto.Mac.Forms.Controls
{
	public class NativeControlHandler : MacView<NSView, Control, Control.ICallback>, NativeControlHost.IHandler
	{
		NSViewController controller;

		public NativeControlHandler(NSView nativeControl)
		{
			Control = nativeControl;
		}
		
		public NativeControlHandler()
		{
		}

		public override SizeF GetPreferredSize(SizeF availableSize)
		{
			return Control.FittingSize.ToEto();
		}

		public NativeControlHandler(NSViewController nativeControl)
		{
			controller = nativeControl;
			Control = controller.View;
		}

		public override NSView ContainerControl { get { return Control; } }
		
		public void Create(object controlObject)
		{
			if (controlObject == null)
			{
				Control = new NSView();
			}
			else if (controlObject is NSView view)
			{
				Control = view;
			}
			else if (controlObject is IntPtr handle)
			{
				view = Runtime.GetNSObject(handle) as NSView;
				if (view == null)
					throw new InvalidOperationException("supplied handle is invalid or does not refer to an object derived from NSView");
				Control = view;
			}
			else
				throw new NotSupportedException($"controlObject of type {controlObject.GetType()} is not supported by this platform");
		}
	}
}

