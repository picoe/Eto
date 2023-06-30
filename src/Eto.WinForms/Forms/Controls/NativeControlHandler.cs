namespace Eto.WinForms.Forms.Controls
{
	public class NativeControlHandler : WindowsControl<swf.Control, Control, Control.ICallback>
	{
		public NativeControlHandler(swf.Control nativeControl)
		{
			Control = nativeControl;
		}
	}
}

