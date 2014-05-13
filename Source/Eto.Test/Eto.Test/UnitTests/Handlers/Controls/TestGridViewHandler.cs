using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eto;
using Eto.Drawing;
using Eto.Forms;

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
		GridView GridView { get { return Widget as GridView; } }

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

		public IDataStore DataStore
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
		}

		public void SelectRow(int row)
		{
			selectedRows.Add(row);
		}

		public void UnselectRow(int row)
		{
			if (selectedRows.Contains(row))
				selectedRows.Remove(row);
		}

		public void SelectAll()
		{
			selectedRows.Clear();
			for (var i = 0; i < RowCount; ++i)
				selectedRows.Add(i);
		}

		public void UnselectAll()
		{
			selectedRows.Clear();
		}

		void SetRowCount()
		{
			RowCount = collection.Collection != null ? collection.Collection.Count : 0;
		}

		void IncrementRowCountBy(int increment)
		{
			RowCount += increment;
		}

		class CollectionHandler : DataStoreChangedHandler<object, IDataStore>
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
			}

			public override void InsertRange(int index, IEnumerable<object> items)
			{
				Handler.SetRowCount();
			}

			public override void RemoveItem(int index)
			{
				Handler.IncrementRowCountBy(-1);
			}

			public override void RemoveRange(int index, int count)
			{
				Handler.SetRowCount();
			}

			public override void RemoveRange(IEnumerable<object> items)
			{
				Handler.SetRowCount();
			}

			public override void RemoveAllItems()
			{
				Handler.SetRowCount();
			}
		}
	}
}
