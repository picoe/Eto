using System;
using swf = System.Windows.Forms;
using Eto.Forms;
using System.Collections.Generic;
using System.Linq;
using Eto.Drawing;

namespace Eto.WinForms.Forms.Controls
{
	public class GridViewHandler : GridHandler<GridView, GridView.ICallback>, GridView.IHandler
	{
		CollectionHandler collection;

		protected override object GetItemAtRow(int row)
		{
			if (row >= 0 && collection != null && collection.Collection != null && collection.Count > row)
				return collection.ElementAt(row);
			return null;
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				default:
					base.AttachEvent(id);
					break;
			}
		}

		class CollectionHandler : EnumerableChangedHandler<object>
		{
			public GridViewHandler Handler { get; set; }

			public override void AddRange(IEnumerable<object> items)
			{
				Handler.SetRowCount();
			}

			public override void AddItem(object item)
			{
				Handler.IncrementRowCountBy(1);
			}

			public override void InsertItem(int index, object item)
			{
				Handler.SupressSelectionChanged++;
				var rows = Handler.SelectedRows.Select(r => r >= index ? r + 1 : r).ToArray();
				Handler.IncrementRowCountBy(1);
				Handler.SelectedRows = rows;
				Handler.SupressSelectionChanged--;
			}

			public override void InsertRange(int index, IEnumerable<object> items)
			{
				Handler.SupressSelectionChanged++;
				var last = index + items.Count();
				var rows = Handler.SelectedRows.Select(r => r >= last ? r - 1 : r).ToArray();
				Handler.SetRowCount();
				Handler.SelectedRows = rows;
				Handler.SupressSelectionChanged--;
			}

			public override void RemoveItem(int index)
			{
				bool isSelected = false;
				Handler.SupressSelectionChanged++;
				var rows = Handler.SelectedRows.Where(r =>
				{
					if (r != index)
						return true;
					isSelected = true;
					return false;
				}).Select(r => r > index ? r - 1 : r).ToArray();
				Handler.IncrementRowCountBy(-1);
				Handler.SelectedRows = rows;
				Handler.SupressSelectionChanged--;
				if (isSelected)
					Handler.Callback.OnSelectionChanged(Handler.Widget, EventArgs.Empty);
			}

			public override void RemoveRange(int index, int count)
			{
				var last = index + count;
				bool isSelected = false;
				Handler.SupressSelectionChanged++;
				var rows = Handler.SelectedRows.Where(r =>
				{
					if (r < index || r >= last)
						return true;
					isSelected = true;
					return false;
				}).Select(r => r >= last ? r - count : r).ToArray();
				Handler.SetRowCount();
				Handler.SelectedRows = rows;
				Handler.SupressSelectionChanged--;
				if (isSelected)
					Handler.Callback.OnSelectionChanged(Handler.Widget, EventArgs.Empty);
			}

			public override void RemoveAllItems()
			{
				Handler.SetRowCount();
			}
		}

		void SetRowCount()
		{
			var columns = Control.Columns.Count;
			if (collection != null)
			{
				var count = collection.Count;
				var oldCount = Control.RowCount;
				// if we shrink by more than 50 items, we set count to 0 first to avoid performance issues with winforms
				const int MinDifference = 50;
				if (count < oldCount - MinDifference) 
				{
					// When going from a large dataset to a small one, DataGridView removes the rows one by one going very slow.
					// We fix this by setting the count to zero first, then setting the count.
					// However, we need to save/restore the selection in that case.
					SupressSelectionChanged++;
					var selectedRows = SelectedRows;
					Control.RowCount = 0;
					if (count > 0)
					{
						ResetSelection();
						Control.RowCount = count;
					}
					SelectedRows = selectedRows.Where(r => r < count);
					SupressSelectionChanged--;
				}
				else
				{
					// when we go from nothing to something, the DataGridView automatically selects the first item.
					// this prevents that
					if (oldCount == 0 && count > 0)
						ResetSelection();
					Control.RowCount = count;
				}
			}
			else
				Control.RowCount = 0;

			// winforms will add a column when setting the count, so when we add a column later, clear it out.
			if (columns == 0 && Control.Columns.Count > 0)
				clearColumns = true;
			Control.Refresh(); // Need to refresh rather than invalidate owing to WinForms DataGridView bugs.
		}

		void IncrementRowCountBy(int increment)
		{
			Control.RowCount += increment;
			Control.Refresh(); // Need to refresh rather than invalidate owing to WinForms DataGridView bugs.
		}

		public object GetCellAt(PointF location, out int column, out int row)
		{
			var result = Control.HitTest((int)location.X, (int)location.Y);
			column = result.ColumnIndex;
			row = result.RowIndex;
			if (row == -1)
				return null;
			return GetItemAtRow(row);
		}

		public GridViewDragInfo GetDragInfo(DragEventArgs args) => args.ControlObject as GridViewDragInfo;

		public IEnumerable<object> DataStore
		{
			get { return collection != null ? collection.Collection : null; }
			set
			{
				if (collection != null)
					collection.Unregister();
				UnselectAll();
				collection = new CollectionHandler { Handler = this };
				collection.Register(value);
				EnsureSelection();
			}
		}

		public IEnumerable<object> SelectedItems
		{
			get
			{
				if (collection != null)
				{
					foreach (var row in SelectedRows)
					{
						yield return collection.ElementAt(row);
					}
				}
			}
		}
	}
}

