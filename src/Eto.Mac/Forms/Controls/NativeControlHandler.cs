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

		protected override void Initialize()
		{
			// don't call any initialize routines as we are hosting a native control
			base.Initialize();
		}

		protected override NSView CreateControl()
		{
			if (Widget is NativeControlHost host && Callback is NativeControlHost.ICallback callback)
			{
				var args = new CreateNativeControlArgs();
				callback.OnCreateNativeControl(host, args);
				return CreateHost(args.NativeControl);
			}
			return base.CreateControl();
		}

		public override SizeF GetPreferredSize(SizeF availableSize)
		{
			return Control?.FittingSize.ToEto() ?? SizeF.Empty;
		}

		public NativeControlHandler(NSViewController nativeControl)
		{
			controller = nativeControl;
			Control = controller.View;
		}

		public override NSView ContainerControl => Control;
		
		public void Create(object nativeControl)
		{
			Control = CreateHost(nativeControl);
		}

		NSView CreateHost(object nativeControl)
		{
			if (nativeControl == null)
			{
				return new MacPanelView();
			}
			else if (nativeControl is NSView view)
			{
				return view;
			}
			else if (nativeControl is NSViewController viewController)
			{
				controller = viewController;
				return controller.View;
			}
			else if (nativeControl is IntPtr handle)
			{
				view = Runtime.GetNSObject(handle) as NSView;
				if (view == null)
					throw new InvalidOperationException("supplied handle is invalid or does not refer to an object derived from NSView");
				return view;
			}
			else
				throw new NotSupportedException($"Native control of type {nativeControl.GetType()} is not supported by this platform");
		}
	}
}

