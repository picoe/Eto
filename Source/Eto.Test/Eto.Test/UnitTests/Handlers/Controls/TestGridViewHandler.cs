using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eto;
using Eto.Drawing;
using Eto.Forms;
using System.Collections;

namespace Eto.Test.UnitTests.Handlers.Controls
{
	/// <summary>
	/// A mock GridViewHandler implementation.
	/// </summary>
	/// <copyright>(c) 2014 by Vivek Jhaveri</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	class TestGridViewHandler : TestControlHandler, GridView.IHandler
	{
		CollectionHandler collection;

		new GridView.ICallback Callback { get { return base.Callback as GridView.ICallback; } }

		new GridView Widget { get { return base.Widget as GridView; } }

		// Boilerplate
		public ContextMenu ContextMenu { get; set; }

		public bool ShowCellBorders { get; set; }

		public bool ShowHeader { get; set; }

		public int RowHeight { get; set; }

		public bool AllowColumnReordering { get; set; }

		public bool AllowMultipleSelection { get; set; }

		/// <summary>
		/// Simulates the UI control's row count.
		/// </summary>
		int RowCount { get; set; }

		public IEnumerable<object> DataStore
		{
			get { return collection != null ? collection.Collection : null; }
			set
			{
				if (collection != null)
					collection.Unregister();
				collection = new CollectionHandler { Handler = this };
				collection.Register(value);
			}
		}

		HashSet<int> selectedRows = new HashSet<int>();

		/// <summary>
		/// Indexes of rows selected in the UI.
		/// </summary>
		public IEnumerable<int> SelectedRows
		{
			get { return selectedRows; }
			set { selectedRows = new HashSet<int>(value); }
		}

		public IEnumerable<object> SelectedItems
		{
			get
			{
				if (collection != null)
				{
					foreach (var row in selectedRows)
					{
						yield return collection.ElementAt(row);
					}
				}
			}
		}

		public void SelectRow(int row)
		{
			if (!selectedRows.Contains(row))
			{
				selectedRows.Add(row);
				Callback.OnSelectionChanged(Widget, EventArgs.Empty);
			}
		}

		public void UnselectRow(int row)
		{
			if (selectedRows.Contains(row))
			{
				selectedRows.Remove(row);
				Callback.OnSelectionChanged(Widget, EventArgs.Empty);
			}
		}

		public void SelectAll()
		{
			if (collection != null && selectedRows.Count != collection.Count)
			{
				selectedRows.Clear();
				for (var i = 0; i < RowCount; ++i)
					selectedRows.Add(i);
				Callback.OnSelectionChanged(Widget, EventArgs.Empty);
			}
		}

		public void UnselectAll()
		{
			if (selectedRows.Count > 0)
			{
				selectedRows.Clear();
				Callback.OnSelectionChanged(Widget, EventArgs.Empty);
			}
		}

		public void BeginEdit(int row, int column)
		{
			if (RowCount >= row)
			{
				Widget.BeginEdit(row, column);
				Callback.OnCellEditing(Widget, (GridViewCellEventArgs)GridViewCellEventArgs.Empty);
			}
		}

		void SetRowCount()
		{
			RowCount = collection.Collection != null ? collection.Count : 0;
		}

		void IncrementRowCountBy(int increment)
		{
			RowCount += increment;
		}

		void RemoveSelected(int index, int count)
		{
			var selectedCount = selectedRows.Count;
			var selected = (IEnumerable<int>)selectedRows;
			int last = index + count - 1;
			selected = selected.Where(r => r < index || r > last);
			selected = selected.Select(r => r > last ? r - count : r);
			selectedRows = new HashSet<int>(selected);
			if (selectedRows.Count != selectedCount)
				Callback.OnSelectionChanged(Widget, EventArgs.Empty);
		}

		void InsertSelected(int index, int count)
		{
			var selected = (IEnumerable<int>)selectedRows;
			int last = index + count - 1;
			selected = selected.Select(r => r >= last ? r + count : r);
			selectedRows = new HashSet<int>(selected);
		}

		class CollectionHandler : EnumerableChangedHandler<object>
		{
			public TestGridViewHandler Handler { get; set; }

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
				Handler.IncrementRowCountBy(1);
				Handler.InsertSelected(index, 1);
			}

			public override void InsertRange(int index, IEnumerable<object> items)
			{
				Handler.SetRowCount();
				Handler.InsertSelected(index, items.Count());
			}

			public override void RemoveItem(int index)
			{
				Handler.RemoveSelected(index, 1);
				Handler.IncrementRowCountBy(-1);
			}

			public override void RemoveRange(int index, int count)
			{
				Handler.RemoveSelected(index, count);
				Handler.SetRowCount();
			}

			public override void RemoveAllItems()
			{
				Handler.SetRowCount();
				Handler.selectedRows.Clear();
			}
		}
	}
}
