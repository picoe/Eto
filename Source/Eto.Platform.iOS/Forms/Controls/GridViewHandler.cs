using System;
using MonoTouch.UIKit;
using Eto.Forms;
using System.Linq;
using Eto.Platform.iOS.Forms.Cells;
using MonoTouch.Foundation;

namespace Eto.Platform.iOS.Forms.Controls
{
	public class GridViewHandler : GridHandler<UITableView, GridView>, IGridView
	{
		Collection store;

		public class GridTableDelegate : GridHandler<UITableView, GridView>.TableDelegate
		{
			public GridViewHandler TreeHandler { get; set; }

			public override GridHandler<UITableView, GridView> Handler {
				get { return TreeHandler; }
				set { }
			}
		}


		class Collection : DataStoreChangedHandler<IGridItem, IGridStore>
		{
			public override void AddItem (IGridItem item)
			{
			}
			public override void InsertItem (int index, IGridItem item)
			{
			}
			public override void RemoveItem (int index)
			{
			}
			public override void RemoveAllItems ()
			{
			}
		}

		protected override UITableViewDelegate CreateDelegate ()
		{
			return new GridTableDelegate { TreeHandler  = this };
		}

		public GridViewHandler ()
		{
			store = new Collection();
		}

		public class DataSource : UITableViewDataSource
		{
			public GridViewHandler Handler { get; set; }

			public const string CELL_ID = "GridView_Cell";

			public override int RowsInSection (UITableView tableView, int section)
			{
				if (Handler.store.Collection != null)
					return Handler.store.Collection.Count;
				else
					return 0;
			}

			public override UITableViewCell GetCell (UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
			{
				var cell = tableView.DequeueReusableCell(CELL_ID);
				if (cell == null)
					cell = new UITableViewCell(UITableViewCellStyle.Default, CELL_ID);
				var item = Handler.GetItem (indexPath);
				foreach (var column in Handler.Widget.Columns.Where (r=> r.DataCell != null).Select(r => r.DataCell.Handler).OfType<IiOSCellHandler>())
				{
					column.Configure (item, cell);
				}
				return cell;
			}
		}

		public IGridItem GetItem(NSIndexPath indexPath)
		{
			var section = store.Collection[indexPath.Section] as IDataStore<IGridItem>;
			if (section != null) {
				return section[indexPath.Row];
			}
			return null;
		}

		public override void Initialize ()
		{
			base.Initialize ();
			Control.DataSource = new DataSource { Handler = this };
		}

		public IGridStore DataStore {
			get { return store.Collection; }
			set {
				store.Register(value);
				Control.ReloadData ();
			}
		}
	}
}

