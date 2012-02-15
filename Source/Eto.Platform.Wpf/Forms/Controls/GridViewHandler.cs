using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using swc = System.Windows.Controls;
using Eto.Forms;
using System.Collections;
using System.Collections.ObjectModel;

namespace Eto.Platform.Wpf.Forms.Controls
{
	public class GridViewHandler : WpfControl<swc.DataGrid, GridView>, IGridView
	{
		IGridStore store;

		public GridViewHandler ()
		{
			Control = new swc.DataGrid {
				HeadersVisibility = swc.DataGridHeadersVisibility.Column,
				AutoGenerateColumns = false,
				CanUserAddRows = false
			};
		}

		public bool ShowHeader
		{
			get { return Control.HeadersVisibility.HasFlag(swc.DataGridHeadersVisibility.Column); }
			set
			{
				Control.HeadersVisibility = value ? swc.DataGridHeadersVisibility.Column : swc.DataGridHeadersVisibility.None;
			}
		}

		public bool AllowColumnReordering
		{
			get { return Control.CanUserReorderColumns; }
			set { Control.CanUserReorderColumns = value; }
		}

		public void InsertColumn (int index, GridColumn column)
		{
			var colHandler = (GridColumnHandler)column.Handler;
			if (index >= 0 && Control.Columns.Count > 0)
				Control.Columns.Insert (index, colHandler.Control);
			else
				Control.Columns.Add (colHandler.Control);
			for (int i = index; i < Widget.Columns.Count; i++) {
				var childCol = (GridColumnHandler)Widget.Columns[i].Handler;
				childCol.Bind (i);
			}
		}

		public void RemoveColumn (int index, GridColumn column)
		{
			var colHandler = (GridColumnHandler)column.Handler;
			Control.Columns.Remove (colHandler.Control);
		}

		public void ClearColumns ()
		{
			Control.Columns.Clear ();
		}

		public class GridItem : IList
		{
			public GridViewHandler Handler { get; set; }
			public IGridItem TheItem { get; set; }

			public int Add (object value)
			{
				return 0;
			}

			public void Clear ()
			{
			}

			public bool Contains (object value)
			{
				return false;
			}

			public int IndexOf (object value)
			{
				return 0;
			}

			public void Insert (int index, object value)
			{
			}

			public bool IsFixedSize
			{
				get { return true; }
			}

			public bool IsReadOnly
			{
				get { return false; }
			}

			public void Remove (object value)
			{
			}

			public void RemoveAt (int index)
			{
			}

			public object this[int index]
			{
				get { return TheItem.GetValue (index); }
				set { TheItem.SetValue (index, value); }
			}

			public void CopyTo (Array array, int index)
			{
			}

			public int Count
			{
				get { return Handler.Control.Columns.Count; }
			}

			public bool IsSynchronized
			{
				get { return false; }
			}

			public object SyncRoot
			{
				get { return null; }
			}

			public IEnumerator GetEnumerator ()
			{
				return null;
			}
		}

		public IEnumerable<GridItem> GetChildren (IGridStore store)
		{
			if (store == null)
				yield break;
			for (int i = 0; i < store.Count; i++)
				yield return new GridItem { Handler = this, TheItem = store.GetItem (i) };
		}


		public IGridStore DataStore
		{
			get { return store; }
			set
			{
				store = value;
				// must use observable collection for editing
				Control.ItemsSource = new ObservableCollection<GridItem>(GetChildren (store));
			}
		}
	}
}
