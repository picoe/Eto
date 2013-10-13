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

		class Collection : DataStoreChangedHandler<object, IDataStore>
		{
			public GridViewHandler Handler { get; set; }


			public override void AddItem(object item)
			{
				Handler.ReloadData();
			}
			public override void InsertItem(int index, object item)
			{
				Handler.ReloadData();
			}
			public override void RemoveItem (int index)
			{
				Handler.ReloadData();
			}
			public override void RemoveAllItems ()
			{
				Handler.ReloadData();
			}
		}

		public bool ShowCellBorders
		{
			set { } // TODO
		}

		protected override UITableViewDelegate CreateDelegate ()
		{
			return new GridTableDelegate(this);
		}

		public GridViewHandler ()
		{
			store = new Collection { Handler = this };
		}

		public class DataSource : UITableViewDataSource
		{
			public GridViewHandler Handler { get; set; }

			public const string CELL_ID = "GridView_Cell";

			public override int RowsInSection (UITableView tableView, int section)
			{
				var result = Handler.store.Collection != null ? Handler.store.Collection.Count : 0;
				return result;
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

			public override void CommitEditingStyle(
				UITableView tableView, 
				UITableViewCellEditingStyle editingStyle, 
				NSIndexPath indexPath)
			{
				switch (editingStyle)
				{
					case UITableViewCellEditingStyle.Delete:
						// remove the item from the underlying data source
						if (Handler.Widget.DeleteItemHandler != null &&
							Handler.Widget.DeleteItemHandler(Handler.GetItem(indexPath)))
						{
							// That succeeded, so delete the row from the table
							tableView.DeleteRows(new NSIndexPath[] { indexPath }, UITableViewRowAnimation.Fade);
						}
						break;
					case UITableViewCellEditingStyle.None:
					case UITableViewCellEditingStyle.Insert: // TODO
						break;					
				}
			}

			public override bool CanEditRow(UITableView tableView, NSIndexPath indexPath)
			{
				// If the widget returns true, allow the row to be deleted
				return Handler.Widget.CanDeleteItem != null &&
					Handler.Widget.CanDeleteItem(Handler.GetItem(indexPath)); 
			}
					
		}

		public object GetItem(NSIndexPath indexPath)
		{
			return store.Collection[indexPath.Row];
		}

		protected override void Initialize ()
		{
			base.Initialize ();
			Control.DataSource = new DataSource { Handler = this };
		}

		public IDataStore DataStore {
			get { return store.Collection; }
			set {
				store.Register(value);
				ReloadData();
			}
		}

		public void ReloadData()
		{
			Control.ReloadData();
		}
	}

	public class GridTableDelegate : GridHandlerTableDelegate
	{
		private GridViewHandler GridViewHandler { get { return this.Handler as GridViewHandler; } }

		public GridTableDelegate(IGrid handler)
			: base(handler)
		{
		}

		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			base.RowSelected(tableView, indexPath);

			// CellClick event
			GridViewHandler.Widget.OnCellClick(new GridViewCellArgs(
				null, // TODO
				indexPath.Row,
				0, // Is this correct?
				GridViewHandler.GetItem(indexPath)));
		}

		public override string TitleForDeleteConfirmation(UITableView tableView, NSIndexPath indexPath)
		{
			var result = GridViewHandler.Widget.DeleteConfirmationTitle;
			if (string.IsNullOrEmpty(result))
				result = base.TitleForDeleteConfirmation(tableView, indexPath);
			return result;
		}

		public override UITableViewCellEditingStyle EditingStyleForRow(UITableView tableView, NSIndexPath indexPath)
		{
			return GridViewHandler.Widget.CanDeleteItem != null &&
				GridViewHandler.Widget.CanDeleteItem(GridViewHandler.GetItem(indexPath))
				? UITableViewCellEditingStyle.Delete
				: UITableViewCellEditingStyle.None;
		}
	}
}

