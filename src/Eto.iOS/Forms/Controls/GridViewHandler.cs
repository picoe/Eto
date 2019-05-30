using System;
using UIKit;
using Eto.Forms;
using System.Linq;
using Eto.iOS.Forms.Cells;
using Foundation;
using Eto.Drawing;
using Eto.iOS.Drawing;
using NSCell = UIKit.UITableViewCell;
using System.Collections.Generic;

namespace Eto.iOS.Forms.Controls
{
	public class GridViewHandler : GridHandler<UITableView, GridView, GridView.ICallback>, GridView.IHandler
	{
		Collection store;

		class Collection : EnumerableChangedHandler<object>
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

			public override nint RowsInSection (UITableView tableView, nint section)
			{
				var result = Handler.store.Count;
				return result;
			}

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

			public override void CommitEditingStyle(
				UITableView tableView, 
				UITableViewCellEditingStyle editingStyle, 
				NSIndexPath indexPath)
			{
				switch (editingStyle)
				{
					case UITableViewCellEditingStyle.Delete:
						// Invoke the handler
						if (Handler.Widget.DeleteItemHandler != null)
							Handler.Widget.DeleteItemHandler(Handler.GetItem(indexPath));
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
			return store.ElementAt(indexPath.Row);
		}

		protected override void Initialize ()
		{
			base.Initialize ();
			Control.DataSource = new DataSource { Handler = this };
			Control.SeparatorStyle = UITableViewCellSeparatorStyle.None;
		}

		public IEnumerable<object> DataStore {
			get { return store.Collection; }
			set {
				store.Register(value);
				ReloadData();
			}
		}

		public IEnumerable<object> SelectedItems
		{
			get
			{
				foreach (var row in SelectedRows)
				{
					yield return store.ElementAt(row);
				}
			}
		}

		public void ReloadData()
		{
			Control.ReloadData();
		}

		public void OnCellFormatting(GridColumn column, object item, int row, NSCell cell)
		{
			Callback.OnCellFormatting(Widget, new IosCellFormatArgs(column, item, row, cell));
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

		public void ReloadData(IEnumerable<int> rows)
		{
			Control.ReloadData();
		}
	}

	class IosCellFormatArgs : GridCellFormatEventArgs
	{
		public ICellHandler CellHandler { get { return Column.DataCell.Handler as ICellHandler; } }

		public NSCell Cell { get; private set; }

		public IosCellFormatArgs(GridColumn column, object item, int row, NSCell cell)
			: base(column, item, row)
		{
			this.Cell = cell;
		}

		public override Font Font
		{
			get { return CellHandler.GetFont(Cell); }
			set { CellHandler.SetFont(Cell, value); }
		}

		public override Color BackgroundColor
		{
			get { return CellHandler.GetBackgroundColor(Cell); }
			set { CellHandler.SetBackgroundColor(Cell, value); }
		}

		public override Color ForegroundColor
		{
			get { return CellHandler.GetForegroundColor(Cell); }
			set { CellHandler.SetForegroundColor(Cell, value); }
		}
	}

	public class GridTableDelegate : GridHandlerTableDelegate
	{
		new GridViewHandler Handler { get { return base.Handler as GridViewHandler; } }

		public GridTableDelegate(GridViewHandler handler)
			: base(handler)
		{
		}

		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			base.RowSelected(tableView, indexPath);

			// CellClick event
			Handler.Callback.OnCellClick(Handler.Widget, new GridViewCellEventArgs(
				null, // TODO
				indexPath.Row,
				0, // Is this correct?
				Handler.GetItem(indexPath)));
		}

		public override string TitleForDeleteConfirmation(UITableView tableView, NSIndexPath indexPath)
		{
			var result = Handler.Widget.DeleteConfirmationTitle != null
				? Handler.Widget.DeleteConfirmationTitle(Handler.GetItem(indexPath)) : "";
			if (string.IsNullOrEmpty(result))
				result = base.TitleForDeleteConfirmation(tableView, indexPath);
			return result;
		}

		public override UITableViewCellEditingStyle EditingStyleForRow(UITableView tableView, NSIndexPath indexPath)
		{
			return Handler.Widget.CanDeleteItem != null &&
				Handler.Widget.CanDeleteItem(Handler.GetItem(indexPath))
				? UITableViewCellEditingStyle.Delete
				: UITableViewCellEditingStyle.None;
		}

		public override void WillDisplay(UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
		{
			var item = Handler.GetItem(indexPath);
			var column = Handler.Widget.Columns[0]; // can there be more than one column?
			Handler.OnCellFormatting(column, item, -1 /*row*/, cell as NSCell);
		}
	}
}

