namespace Eto.WinForms.Forms.Controls
{
	public class ControlHandler : WindowsControl<swf.Control, Control, Control.ICallback>
	{
		public ControlHandler()
		{
			Control = new swf.Control();
		}
	}
}
