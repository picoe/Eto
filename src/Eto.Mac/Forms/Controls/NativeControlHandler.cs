namespace Eto.Mac.Forms.Controls
{
	public class NativeControlHandler : MacView<NSView, Control, Control.ICallback>, NativeControlHost.IHandler
	{
		NSViewController controller;
		bool createdOwnView;

		// MacBase.AddMethod adds to the native CLASS, so hosting our own view must not use MacPanelView
		// itself - every other view deriving from it (notably the window's content view) would inherit
		// the methods added for this control's events.
		class EtoNativeControlView : MacPanelView
		{
		}

		// there's nothing to measure when we host our own (empty) view, so use a default size
		// so the control still reports a sane preferred size like other Eto controls.
		static readonly SizeF DefaultSize = new SizeF(100, 100);

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
			var size = Control?.FittingSize.ToEto() ?? SizeF.Empty;
			if (createdOwnView)
			{
				// there is nothing to measure in the placeholder view we created, so fall back to any
				// explicitly set size and then to a default so it still reports a sane preferred size.
				var userSize = UserPreferredSize;
				if (size.Width <= 0)
					size.Width = userSize.Width >= 0 ? userSize.Width : Math.Min(DefaultSize.Width, availableSize.Width);
				if (size.Height <= 0)
					size.Height = userSize.Height >= 0 ? userSize.Height : Math.Min(DefaultSize.Height, availableSize.Height);
			}
			return size;
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
				createdOwnView = true;
				return new EtoNativeControlView();
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

