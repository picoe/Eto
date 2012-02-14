using System;
using swf = System.Windows.Forms;
using Eto.Forms;

namespace Eto.Platform.Windows.Forms.Controls
{
	public interface ICellHandler
	{
		swf.DataGridViewCell Control { get; }
		object GetItemValue (object cellValue);
		object GetCellValue (object itemValue);
	}
	
	public class CellHandler<T, W> : WidgetHandler<T, W>, ICell, ICellHandler
		where T: swf.DataGridViewCell
		where W: Cell
	{
		swf.DataGridViewCell ICellHandler.Control {
			get { return Control; }
		}

		public virtual object GetItemValue (object cellValue)
		{
			return cellValue;
		}

		public virtual object GetCellValue (object itemValue)
		{
			return itemValue;
		}

	}
}

