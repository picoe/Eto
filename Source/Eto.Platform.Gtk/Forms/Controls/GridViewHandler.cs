using System;
using Eto.Forms;
using System.Runtime.InteropServices;
using Eto.Drawing;
using System.Linq;
using System.Collections.Generic;

namespace Eto.Platform.GtkSharp.Forms.Controls
{
	public class GridViewHandler : GridHandler<GridView>, IGridView, ICellDataSource, IGtkListModelHandler<IGridItem, IGridStore>, IGridHandler
	{
		GtkListModel<IGridItem, IGridStore> model;
		CollectionHandler collection;
		
		public GridViewHandler ()
		{
		}

		protected override Gtk.TreeModelImplementor CreateModelImplementor ()
		{
			model = new GtkListModel<IGridItem, IGridStore> { Handler = this };
			return model;
		}
		
		public class CollectionHandler : DataStoreChangedHandler<IGridItem, IGridStore>
		{
			public GridViewHandler Handler { get; set; }

			public override void AddRange (IEnumerable<IGridItem> items)
			{
				Handler.UpdateModel ();
			}

			public override void AddItem (IGridItem item)
			{
				var iter = Handler.model.GetIterAtRow (DataStore.Count);
				var path = Handler.model.GetPathAtRow (DataStore.Count);
				Handler.Tree.Model.EmitRowInserted (path, iter);
			}

			public override void InsertItem (int index, IGridItem item)
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
		
		public IGridStore DataStore {
			get { return collection != null ? collection.DataStore : null; }
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

		public GLib.Value GetColumnValue (IGridItem item, int dataColumn)
		{
			int column;
			if (ColumnMap.TryGetValue (dataColumn, out column)) {
				var colHandler = (IGridColumnHandler)Widget.Columns[column].Handler;
				return colHandler.GetValue (item, dataColumn);
			}
			return new GLib.Value ((string)null);
		}

	}
}

