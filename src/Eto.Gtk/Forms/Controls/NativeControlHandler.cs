
namespace Eto.GtkSharp.Forms.Controls
{
	public class NativeControlHandler : GtkControl<Gtk.Widget, Control, Control.ICallback>, NativeControlHost.IHandler
	{

		// there's nothing to measure when we host our own (empty) event box, so use a default size
		// so the control still reports a sane preferred size like other Eto controls.
		static readonly Size s_defaultSize = new Size(100, 100);

		EtoNativeHostBox _eventBox = new EtoNativeHostBox();
		public override Gtk.Widget ContainerControl => _eventBox;

		class EtoNativeHostBox : Gtk.EventBox
		{
			// only when we're hosting our own placeholder - anything actually hosted measures itself.
			public bool UseDefaultSize { get; set; } = true;

#if GTKCORE
			protected override void OnGetPreferredWidth(out int minimum_width, out int natural_width)
			{
				base.OnGetPreferredWidth(out minimum_width, out natural_width);
				if (UseDefaultSize && natural_width <= 0)
					natural_width = s_defaultSize.Width;
			}

			protected override void OnGetPreferredHeight(out int minimum_height, out int natural_height)
			{
				base.OnGetPreferredHeight(out minimum_height, out natural_height);
				if (UseDefaultSize && natural_height <= 0)
					natural_height = s_defaultSize.Height;
			}
#endif
		}

		public NativeControlHandler(Gtk.Widget nativeControl)
		{
			Control = nativeControl;
			_eventBox.UseDefaultSize = false;
		}
		
		public NativeControlHandler()
		{
		}

		protected override void Initialize()
		{
			// don't call any initialize routines as we are hosting a native control
			// base.Initialize();
		}

		protected override Gtk.Widget CreateControl()
		{
			if (Widget is NativeControlHost host && Callback is NativeControlHost.ICallback callback)
			{
				var args = new CreateNativeControlArgs();
				callback.OnCreateNativeControl(host, args);
				return CreateHost(args.NativeControl);
			}
			return base.CreateControl();
		}

		public void Create(object nativeControl) => CreateHost(nativeControl);

		Gtk.Widget CreateHost(object nativeControl)
		{
			if (nativeControl == null)
			{
				return _eventBox;
			}
			else if (nativeControl is Gtk.Widget widget)
			{
				_eventBox.Child = widget;
				_eventBox.UseDefaultSize = false;
				return _eventBox;
			}
			else if (nativeControl is IntPtr handle)
			{
				widget = GLib.Object.GetObject(handle) as Gtk.Widget;
				if (widget == null)
					throw new InvalidOperationException("Could not convert handle to Gtk.Widget");
				_eventBox.Child = widget;
				_eventBox.UseDefaultSize = false;
				return _eventBox;
			}
			else
				throw new NotSupportedException($"Native control of type {nativeControl.GetType()} is not supported by this platform");
		}
	}
}

