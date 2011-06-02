using System;
using SWF = System.Windows.Forms;
using Eto.Forms;

namespace Eto.Platform.Windows
{
	public class ControlHandler : WindowsControl<SWF.Control, Control>
	{
		public ControlHandler()
		{
			Control = new SWF.Control();
		}

	}
}
