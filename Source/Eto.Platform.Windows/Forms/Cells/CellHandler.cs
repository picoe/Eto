using System;
using swf = System.Windows.Forms;
using Eto.Forms;

namespace Eto.Platform.Windows.Forms.Controls
{
	public interface ICellHandler
	{
		swf.DataGridViewCell Control { get; }
		void SetCellValue (object dataItem, object value);
		object GetCellValue (object dataItem);
	}
	
	public abstract class CellHandler<T, W> : WidgetHandler<T, W>, ICell, ICellHandler
		where T: swf.DataGridViewCell
		where W: Cell
	{
		swf.DataGridViewCell ICellHandler.Control {
			get { return Control; }
		}

		public abstract void SetCellValue (object dataItem, object value);

		public abstract object GetCellValue (object dataItem);

	}
}

