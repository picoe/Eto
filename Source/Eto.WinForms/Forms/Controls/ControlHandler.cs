using SWF = System.Windows.Forms;
using Eto.Forms;

namespace Eto.WinForms.Forms.Controls
{
	public class ControlHandler : WindowsControl<SWF.Control, Control, Control.ICallback>
	{
		public ControlHandler()
		{
			Control = new SWF.Control();
		}
	}
}
