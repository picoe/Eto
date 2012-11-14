using System;
using Eto.Forms;
using MonoTouch.UIKit;

namespace Eto.Platform.iOS.Forms.Cells
{
	public interface IiOSCellHandler
	{
		void Configure (object dataItem, UITableViewCell cell);

		string TitleForSection (object dataItem);
	}

	public abstract class CellHandler<T, W> : WidgetHandler<T, W>, ICell, IiOSCellHandler
		where W: Cell
	{
		public CellHandler ()
		{
		}

		public abstract void Configure (object dataItem, UITableViewCell cell);

		public abstract string TitleForSection (object dataItem);

	}
}

