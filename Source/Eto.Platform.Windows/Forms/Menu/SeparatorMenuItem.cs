using SD = System.Drawing;
using SWF = System.Windows.Forms;
using Eto.Forms;

namespace Eto.Platform.Windows
{
	/// <summary>
	/// Summary description for MenuBarHandler.
	/// </summary>
	public class SeparatorMenuItemHandler : MenuHandler<SWF.ToolStripSeparator, SeparatorMenuItem>, ISeparatorMenuItem
	{
		
		public SeparatorMenuItemHandler()
		{
			Control = new SWF.ToolStripSeparator();
		}
	}
}
