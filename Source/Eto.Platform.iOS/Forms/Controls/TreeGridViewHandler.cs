using System;
using MonoTouch.UIKit;
using Eto.Forms;
using System.Linq;
using Eto.Platform.iOS.Forms.Cells;
using MonoTouch.Foundation;

namespace Eto.Platform.iOS.Forms.Controls
{
	public class TreeGridViewHandler : GridHandler<UITableView, TreeGridView>, ITreeGridView
	{
		Collection store;

		public class TreeGridTableDelegate : GridHandler<UITableView, TreeGridView>.TableDelegate
		{
			public TreeGridViewHandler TreeHandler { get; set; }

			public override GridHandler<UITableView, TreeGridView> Handler {
				get { return TreeHandler; }
				set { }
			}

			public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
			{
				base.RowSelected (tableView, indexPath);

				TreeHandler.Widget.OnSelectedItemChanged(EventArgs.Empty);
			}
		}


		class Collection : DataStoreChangedHandler<ITreeGridItem, ITreeGridStore<ITreeGridItem>>
		{
			public override void AddItem (ITreeGridItem item)
			{
			}
			public override void InsertItem (int index, ITreeGridItem item)
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
			return new TreeGridTableDelegate { TreeHandler  = this };
		}

		public TreeGridViewHandler ()
		{
			store = new Collection();
		}

		public class DataSource : UITableViewDataSource
		{
			public TreeGridViewHandler Handler { get; set; }

			public override int NumberOfSections (UITableView tableView)
			{
				return Handler.store.Collection != null ? Handler.store.Collection.Count : 0;
			}

			public override int RowsInSection (UITableView tableView, int section)
			{
				var item = Handler.store.Collection[section] as IDataStore<ITreeGridItem>;
				if (item != null)
					return item.Count;
				else
					return 0;
			}

			public override string TitleForHeader (UITableView tableView, int section)
			{
				var coll = Handler.store.Collection;
				var dataItem = coll[section];
				foreach (var column in Handler.Widget.Columns.Where (r=> r.DataCell != null).Select(r => r.DataCell.Handler).OfType<IiOSCellHandler>())
				{
					return column.TitleForSection (dataItem);
				}
				return string.Empty;
			}

			public const string CELL_ID = "GridView_Cell";

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

		public ITreeGridItem GetItem(NSIndexPath indexPath)
		{
			var section = store.Collection[indexPath.Section] as IDataStore<ITreeGridItem>;
			if (section != null) {
				return section[indexPath.Row];
			}
			return null;
		}

		protected override void Initialize ()
		{
			base.Initialize ();
			Control.DataSource = new DataSource { Handler = this };
		}

		public ITreeGridStore<ITreeGridItem> DataStore {
			get { return store.Collection; }
			set {
				store.Register(value);
				Control.ReloadData ();
			}
		}

		public ITreeGridItem SelectedItem {
			get {
				var path = Control.IndexPathForSelectedRow;
				if (path != null)
					return GetItem (path);
				else
					return null;
			}
			set {
				// traverse datastore to find item
				//var index = store.IndexOf (value);

			}
		}
	}
}

