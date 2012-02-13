using System;
using swf = System.Windows.Forms;
using Eto.Forms;

namespace Eto.Platform.Windows.Forms.Controls
{
	public interface ICellHandler
	{
		swf.DataGridViewCell Control { get; }
	}
	
	public class CellHandler<T, W> : WidgetHandler<T, W>, ICell, ICellHandler
		where T: swf.DataGridViewCell
		where W: Cell
	{
		swf.DataGridViewCell ICellHandler.Control {
			get { return Control; }
		}
	}
}

