using System;
using Eto.Forms;
using System.Runtime.InteropServices;
using Eto.Drawing;
using System.Linq;
using System.Collections.Generic;
using Eto.Platform.GtkSharp.Forms.Cells;

namespace Eto.Platform.GtkSharp.Forms.Controls
{
	public class GridViewHandler : GridHandler<GridView>, IGridView, ICellDataSource, IGtkListModelHandler<object, IDataStore>, IGridHandler
	{
		GtkListModel<object, IDataStore> model;
		CollectionHandler collection;
		
		public GridViewHandler ()
		{
		}

		protected override ITreeModelImplementor CreateModelImplementor ()
		{
			model = new GtkListModel<object, IDataStore> { Handler = this };
			return model;
		}
		
		public class CollectionHandler : DataStoreChangedHandler<object, IDataStore>
		{
			public GridViewHandler Handler { get; set; }

			public override void AddRange (IEnumerable<object> items)
			{
				Handler.UpdateModel ();
			}

			public override void AddItem (object item)
			{
				var iter = Handler.model.GetIterAtRow (Collection.Count);
				var path = Handler.model.GetPathAtRow (Collection.Count);
				Handler.Tree.Model.EmitRowInserted (path, iter);
			}

			public override void InsertItem (int index, object item)
			{
				var iter = Handler.model.GetIterAtRow (index);
				var path = Handler.model.GetPathAtRow (index);
				Handler.Tree.Model.EmitRowInserted (path, iter);
			}

			public override void RemoveItem (int index)
			{
				var path = Handler.model.GetPathAtRow (index);
				Handler.Tree.Model.EmitRowDeleted (path);
			}

			public override void RemoveAllItems ()
			{
				Handler.UpdateModel ();
			}
		}
		
		public IDataStore DataStore {
			get { return collection != null ? collection.Collection : null; }
			set {
				if (collection != null)
					collection.Unregister ();
				collection = new CollectionHandler{ Handler = this };
				collection.Register (value);
			}
		}

		public override Gtk.TreeIter GetIterAtRow (int row)
		{
			return model.GetIterAtRow (row);
		}

		public override object GetItem (Gtk.TreePath path)
		{
			return model.GetItemAtPath (path);
		}

		public GLib.Value GetColumnValue (object item, int dataColumn, int row)
		{
			int column;
			if (ColumnMap.TryGetValue (dataColumn, out column)) {
				var colHandler = (IGridColumnHandler)Widget.Columns[column].Handler;
				return colHandler.GetValue (item, dataColumn, row);
			}
			return new GLib.Value ((string)null);
		}

		public int GetRowOfItem (object item)
		{
			if (collection == null) return -1;
			return collection.IndexOf (item);
		}
	}
}

