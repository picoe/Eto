using System;
using swf = System.Windows.Forms;
using Eto.Forms;
using System.Linq;
using System.Collections.Generic;

namespace Eto.Platform.Windows.Forms.Controls
{
	public class GridViewHandler : GridHandler<GridView>, IGridView
	{
		CollectionHandler collection;
		
		public GridViewHandler ()
		{
		}

		protected override IGridItem GetItemAtRow (int row)
		{
			if (collection == null) return null;
			return collection.Collection[row];
		}

		class CollectionHandler : DataStoreChangedHandler<IGridItem, IGridStore>
		{
			public GridViewHandler Handler { get; set; }
			
			public override void AddRange (IEnumerable<IGridItem> items)
			{
				Handler.Control.Refresh ();
				Handler.Control.RowCount = Collection.Count;
			}
			
			public override void AddItem (IGridItem item)
			{
				Handler.Control.RowCount ++;
				Handler.Control.Refresh ();
			}

			public override void InsertItem (int index, IGridItem item)
			{
				Handler.Control.RowCount ++;
				Handler.Control.Refresh ();
			}

			public override void RemoveItem (int index)
			{
				Handler.Control.RowCount --;
				Handler.Control.Refresh ();
			}

			public override void RemoveAllItems ()
			{
				Handler.Control.RowCount = 0;
				Handler.Control.Refresh ();
			}
		}

		public IGridStore DataStore {
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

