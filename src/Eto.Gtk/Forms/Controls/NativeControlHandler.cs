
namespace Eto.GtkSharp.Forms.Controls
{
	public class NativeControlHandler : GtkControl<Gtk.Widget, Control, Control.ICallback>, NativeControlHost.IHandler
	{

		Gtk.EventBox _eventBox = new Gtk.EventBox();
		public override Gtk.Widget ContainerControl => _eventBox;
				
		public NativeControlHandler(Gtk.Widget nativeControl)
		{
			Control = nativeControl;
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
				return _eventBox;
			}
			else if (nativeControl is IntPtr handle)
			{
				widget = GLib.Object.GetObject(handle) as Gtk.Widget;
				if (widget == null)
					throw new InvalidOperationException("Could not convert handle to Gtk.Widget");
				_eventBox.Child = widget;
				return _eventBox;
			}
			else
				throw new NotSupportedException($"Native control of type {nativeControl.GetType()} is not supported by this platform");
		}
	}
}

