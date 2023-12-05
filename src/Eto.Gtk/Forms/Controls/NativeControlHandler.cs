
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

		public void Create(object controlObject)
		{
			if (controlObject == null)
			{
				Control = _eventBox;
			}
			else if (controlObject is Gtk.Widget widget)
			{
				Control = widget;
				_eventBox.Child = widget;
			}
			else if (controlObject is IntPtr handle)
			{
				widget = GLib.Object.GetObject(handle) as Gtk.Widget;
				if (widget == null)
					throw new InvalidOperationException("Could not convert handle to Gtk.Widget");
				Control = widget;
				_eventBox.Child = widget;
			}
			else
				throw new NotSupportedException($"controlObject of type {controlObject.GetType()} is not supported by this platform");
		}
	}
}

