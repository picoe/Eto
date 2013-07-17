using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace Eto.Forms
{
	public interface IGridStore : IDataStore<IGridItem>
	{
	}


	public partial interface IGridView : IGrid
	{
		IGridStore DataStore { get; set; }

	}

	public class GridViewCellArgs : EventArgs
	{
		public GridColumn GridColumn { get; private set; }

		public int Row { get; private set; }

		public int Column { get; private set; }

		public IGridItem Item { get; private set; }

		public GridViewCellArgs (GridColumn gridColumn, int row, int column, IGridItem item)
		{
			this.GridColumn = gridColumn;
			this.Row = row;
			this.Column = column;
			this.Item = item;
		}
	}

	public partial class GridView : Grid
	{
		IGridView handler;

		public GridView ()
			: this (Generator.Current)
		{
		}

		public GridView (Generator g)
			: this (g, typeof (IGridView))
		{
		}
		
		protected GridView (Generator generator, Type type, bool initialize = true)
			: base (generator, type, initialize)
		{
			handler = (IGridView)Handler;
		}

		public IGridStore DataStore {
			get { return handler.DataStore; }
			set { handler.DataStore = value; }
		}

		public override IEnumerable<IGridItem> SelectedItems
		{
			get
			{
				if (DataStore == null)
					yield break;
				if (SelectedRows != null)
					foreach (var row in SelectedRows)
						yield return DataStore[row];
			}
		}
	}
}

