using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using swc = System.Windows.Controls;
using Eto.Forms;

namespace Eto.Platform.Wpf.Forms.Controls
{
	public interface ICellHandler
	{
		swc.DataGridColumn Control { get; }
		void Bind (int column);
	}

	public abstract class CellHandler<T,W> : WidgetHandler<T, W>, ICell, ICellHandler
		where T: swc.DataGridColumn
		where W: Cell
	{
		swc.DataGridColumn ICellHandler.Control
		{
			get { return Control; }
		}

		public abstract void Bind (int column);
	}
}
