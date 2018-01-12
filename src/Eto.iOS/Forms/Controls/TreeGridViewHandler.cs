using System;
using UIKit;
using Eto.Forms;
using System.Linq;
using Eto.iOS.Forms.Cells;
using Foundation;
using Eto.Drawing;
using System.Collections.Generic;

namespace Eto.iOS.Forms.Controls
{
	public class TreeGridViewHandler : GridHandler<UITableView, TreeGridView, TreeGridView.ICallback>, TreeGridView.IHandler
	{
		Collection store;

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
			return new TreeGridTableDelegate(this);
		}

		public TreeGridViewHandler ()
		{
			store = new Collection();
		}

		public class DataSource : UITableViewDataSource
		{
			WeakReference handler;
			public TreeGridViewHandler Handler { get { return (TreeGridViewHandler)handler.Target; } set { handler = new WeakReference(value); } }

			public override nint NumberOfSections (UITableView tableView)
			{
				return Handler.store.Collection != null ? Handler.store.Collection.Count : 0;
			}

			public override nint RowsInSection (UITableView tableView, nint section)
			{
				var item = Handler.store.Collection[(int)section] as IDataStore<ITreeGridItem>;
				if (item != null)
					return item.Count;
				else
					return 0;
			}

			public override string TitleForHeader (UITableView tableView, nint section)
			{
				var coll = Handler.store.Collection;
				var dataItem = coll[(int)section];
				foreach (var column in Handler.Widget.Columns.Where (r=> r.DataCell != null).Select(r => r.DataCell.Handler).OfType<ICellHandler>())
				{
					return column.TitleForSection (dataItem);
				}
				return string.Empty;
			}

			public const string CELL_ID = "GridView_Cell";

			public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			{
				var cell = tableView.DequeueReusableCell(CELL_ID);
				if (cell == null)
					cell = new UITableViewCell(UITableViewCellStyle.Default, CELL_ID);

				var item = Handler.GetItem (indexPath);
				foreach (var column in Handler.Widget.Columns.Where (r=> r.DataCell != null).Select(r => r.DataCell.Handler).OfType<ICellHandler>())
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

		public void ReloadData()
		{
			throw new NotImplementedException();
		}

		public void ReloadItem(ITreeGridItem item)
		{
			throw new NotImplementedException();
		}

		public ITreeGridItem GetCellAt(PointF location, out int column)
		{
			throw new NotImplementedException();
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

		public ContextMenu ContextMenu
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public IEnumerable<object> SelectedItems
		{
			get
			{
				throw new NotImplementedException();
			}
		}
	}

	public class TreeGridTableDelegate : GridHandlerTableDelegate
	{
		TreeGridViewHandler TreeHandler { get { return Handler as TreeGridViewHandler; } }

		public TreeGridTableDelegate(TreeGridViewHandler handler) : base(handler)
		{
		}

		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			base.RowSelected(tableView, indexPath);

			TreeHandler.Callback.OnSelectedItemChanged(TreeHandler.Widget, EventArgs.Empty);
		}
	}
}

