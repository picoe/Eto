using System;
using SWF = System.Windows.Forms;
using Eto.Forms;

namespace Eto.Platform.Windows
{
	/// <summary>
	/// Summary description for MenuBarHandler.
	/// </summary>
	public abstract class MenuHandler<T, W> : WidgetHandler<T, W>, IWidget
		where T: SWF.ToolStripItem
		where W: Widget
	{
		
	}
}
