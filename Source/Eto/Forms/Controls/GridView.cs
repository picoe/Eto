using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace Eto.Forms
{
	public interface IGridStore : IDataStore<object>
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

		public object Item { get; private set; }

		public GridViewCellArgs (GridColumn gridColumn, int row, int column, object item)
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

#if MOBILE
		/// <summary>
		/// A delegate method to delete an item in response to a user's
		/// request. The method should return true after deleting the
		/// item, or false to indicate the item could not be deleted.
		/// </summary>
		public Func<object, bool> DeleteItemHandler { get; set; }

		/// <summary>
		/// A delegate that returns true if an item can be edited
		/// </summary>
		public Func<object, bool> CanDeleteItem { get; set; }

		/// <summary>
		/// The text to display in a Delete item button.
		/// </summary>
		public string DeleteConfirmationTitle { get; set; }
#endif

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

		public override IEnumerable<object> SelectedItems
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

