using System;
using swf = System.Windows.Forms;
using Eto.Forms;
using System.Collections.Generic;

namespace Eto.Platform.Windows.Forms.Controls
{
	public class GridViewHandler : GridHandler<GridView>, IGridView
	{
		CollectionHandler collection;
		
		public bool ShowCellBorders
		{
			get { return Control.CellBorderStyle != swf.DataGridViewCellBorderStyle.None; }
			set { Control.CellBorderStyle = value ? swf.DataGridViewCellBorderStyle.Single : swf.DataGridViewCellBorderStyle.None; }
		}

		protected override object GetItemAtRow (int row)
		{
			if (collection != null && collection.Collection != null && collection.Collection.Count > row) 			
				return collection.Collection[row];
			return null;
		}

		public override void AttachEvent(string id)
		{
			switch (id) {
			case GridView.CellClickEvent:
				Control.CellClick += (sender, e) => {
					var item = GetItemAtRow (e.RowIndex);
					var column = Widget.Columns [e.ColumnIndex];
					Widget.OnCellClick (new GridViewCellArgs (column, e.RowIndex, e.ColumnIndex, item));
				};
				break;
			default:
				base.AttachEvent(id);
				break;
			}
		}

		class CollectionHandler : DataStoreChangedHandler<object, IDataStore>
		{
			public GridViewHandler Handler { get; set; }
			
			public override void AddRange (IEnumerable<object> items)
			{
				Handler.SetRowCount();
			}
			
			public override void AddItem (object item)
			{
				Handler.IncrementRowCountBy(1);
			}

			public override void InsertItem (int index, object item)
			{
				Handler.IncrementRowCountBy(1);
			}

			public override void InsertRange(int index, IEnumerable<object> items)
			{
				Handler.SetRowCount();
			}

			public override void RemoveItem (int index)
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

			public override void RemoveAllItems ()
			{
				Handler.SetRowCount();
			}
		}

		void SetRowCount()
		{
			Control.RowCount = collection.Collection != null ? collection.Collection.Count : 0;
			Control.Refresh(); // Need to refresh rather than invalidate owing to WinForms DataGridView bugs.
		}

		void IncrementRowCountBy(int increment)
		{
			Control.RowCount += increment;
			Control.Refresh(); // Need to refresh rather than invalidate owing to WinForms DataGridView bugs.
		}

		public IDataStore DataStore {
			get { return collection != null ? collection.Collection : null; }
			set {
				if (collection != null)
					collection.Unregister ();
				collection = new CollectionHandler { Handler = this };
				collection.Register (value);
			}
		}
	}
}

