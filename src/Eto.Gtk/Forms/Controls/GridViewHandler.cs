using Eto.GtkSharp.Forms.Cells;
namespace Eto.GtkSharp.Forms.Controls
{
	public class GridViewHandler : GridHandler<GridView, GridView.ICallback>, GridView.IHandler, ICellDataSource, IGtkEnumerableModelHandler<object>
	{
		GtkEnumerableModel<object> model;
		CollectionHandler collection;
		protected override ITreeModelImplementor CreateModelImplementor()
		{
			model = new GtkEnumerableModel<object> { Handler = this, Count = collection != null ? collection.Count : 0 };
			return model;
		}

		public class CollectionHandler : EnumerableChangedHandler<object>
		{
			public GridViewHandler Handler { get; set; }

			public override void AddRange(IEnumerable<object> items)
			{
				Handler.UpdateModel();
			}

			public override void AddItem(object item)
			{
				var count = Count;
				var iter = Handler.model.GetIterAtRow(count);
				var path = Handler.model.GetPathAtRow(count);
				Handler.model.Count++;
				Handler.Control.Model.EmitRowInserted(path, iter);
			}

			public override void InsertItem(int index, object item)
			{
				var iter = Handler.model.GetIterAtRow(index);
				var path = Handler.model.GetPathAtRow(index);
				Handler.model.Count++;
				Handler.Control.Model.EmitRowInserted(path, iter);
			}

			public override void RemoveItem(int index)
			{
				var path = Handler.model.GetPathAtRow(index);
				Handler.model.Count--;
				Handler.Control.Model.EmitRowDeleted(path);
			}

			public override void RemoveAllItems()
			{
				Handler.UpdateModel();
			}
		}

		public IEnumerable<object> DataStore
		{
			get => collection?.Collection;
			set
			{
				if (collection != null)
					collection.Unregister();
				UnselectAll();
				collection = new CollectionHandler { Handler = this };
				collection.Register(value);
				EnsureSelection();
			}
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				default:
					base.AttachEvent(id);
					break;
			}
		}

		public IEnumerable<object> SelectedItems
		{
			get
			{
				if (collection != null)
				{
					foreach (var row in SelectedRows)
						yield return collection.ElementAt(row);
				}
			}
		}

		public override Gtk.TreeIter GetIterAtRow(int row)
		{
			return model.GetIterAtRow(row);
		}

		public override Gtk.TreePath GetPathAtRow(int row)
		{
			return model.GetPathAtRow(row);
		}

		protected override void SetSelectedRows(IEnumerable<int> value)
		{
			Control.Selection.UnselectAll();
			if (value != null && collection != null)
			{
				int start = -1;
				int end = -1;
				var count = collection.Count;

				foreach (var row in value.Where(r => r < count).OrderBy(r => r))
				{
					if (start == -1)
						start = end = row;
					else if (row == end + 1)
						end = row;
					else
					{
						if (start == end)
							Control.Selection.SelectIter(GetIterAtRow(start));
						else
							Control.Selection.SelectRange(GetPathAtRow(start), GetPathAtRow(end));
						start = end = row;
					}
				}
				if (start != -1)
				{
					if (start == end)
						Control.Selection.SelectIter(GetIterAtRow(start));
					else
						Control.Selection.SelectRange(GetPathAtRow(start), GetPathAtRow(end));
				}
			}
		}

		public override object GetItem(Gtk.TreePath path)
		{
			return model.GetItemAtPath(path);
		}

		public GLib.Value GetColumnValue(object item, int dataColumn, int row)
		{
			if (dataColumn == RowDataColumn)
				return new GLib.Value(row);
			if (dataColumn == ItemDataColumn)
				return new GLib.Value(item);
			int column;
			if (ColumnMap.TryGetValue(dataColumn, out column))
			{
				var colHandler = (GridColumnHandler)Widget.Columns[column].Handler;
				return colHandler.GetValue(item, dataColumn, row);
			}
			return new GLib.Value((string)null);
		}

		public int GetRowOfItem(object item)
		{
			return collection != null ? collection.IndexOf(item) : -1;
		}

		public EnumerableChangedHandler<object> Collection
		{
			get { return collection; }
		}

		public void ReloadData(IEnumerable<int> rows)
		{
			if (rows != null)
			{
				foreach (var row in rows)
				{
					var iter = GetIterAtRow(row);
					var path = GetPathAtRow(row);
					Control.Model.EmitRowChanged(path, iter);
				}
			}
			else
				UpdateModel();
		}


		public GridCell GetCellAt(PointF location)
		{
			int columnIndex;
			int rowIndex;
			object item;
			GridCellType cellType;
			int headerIndex = -1;

#if !GTK2
			if (ShowHeader)
			{
				int headerHeight = 0;
				for (int i = 0; i < Control.Columns.Length; i++)
				{
					Gtk.TreeViewColumn col = Control.Columns[i];
					var header = col.Button?.GetWindow();
					if (header != null)
					{
						var bounds = header.GetBounds();
						if (bounds.Contains(Point.Round(location)))
						{
							headerIndex = GetColumnIndex(col);
						}
						headerHeight = Math.Max(headerHeight, bounds.Height);
					}
				}
				location.Y -= headerHeight;
			}
#endif

			if (headerIndex == -1 && Control.GetPathAtPos((int)location.X, (int)location.Y, out var path, out var dataColumn))
			{
				columnIndex = GetColumnIndex(dataColumn);
				rowIndex = GetRowIndexOfPath(path);
				item = model.GetItemAtPath(path);
				if (columnIndex == -1)
					cellType = GridCellType.None;
				else
					cellType = GridCellType.Data;
			}
			else if (headerIndex != -1)
			{
				cellType = GridCellType.ColumnHeader;
				columnIndex = headerIndex;
				rowIndex = -1;
				item = null;
			}
			else
			{
				columnIndex = -1;
				rowIndex = -1;
				item = null;
				cellType = GridCellType.None;
			}

			var column = columnIndex != -1 ? Widget.Columns[columnIndex] : null;
			return new GridCell(column, columnIndex, rowIndex, cellType, item);
		}


		protected class GridViewConnector : GridConnector
		{
			GridViewDragInfo _dragInfo;

			public new GridViewHandler Handler { get { return (GridViewHandler)base.Handler; } }

			protected override DragEventArgs GetDragEventArgs(Gdk.DragContext context, PointF? location, uint time = 0, object controlObject = null, DataObject data = null)
			{
				var t = Handler?.Control;
				GridViewDragInfo dragInfo = _dragInfo;
				if (dragInfo == null && location != null)
				{
					if (t.GetDestRowAtPos((int)location.Value.X, (int)location.Value.Y, out var path, out var pos))
					{
						var item = Handler.model.GetItemAtPath(path);
						var indecies = path.Indices;
						var index = indecies[indecies.Length - 1];
						dragInfo = new GridViewDragInfo(Handler.Widget, item, index, pos.ToEto());
					}
				}

				return base.GetDragEventArgs(context, location, time, dragInfo, data);
			}

			public override void HandleDragMotion(object o, Gtk.DragMotionArgs args)
			{
				base.HandleDragMotion(o, args);

				var h = Handler;
				if (h == null)
					return;
				var info = h.GetDragInfo(DragArgs);
				if (info == null)
					return;

				if (info.Index >= 0)
				{
					var path = new Gtk.TreePath(new[] { info.Index });
					var pos = info.Position.ToGtk();
					h.Control.SetDragDestRow(path, pos);
				}
			}

			public override void HandleDragDrop(object o, Gtk.DragDropArgs args)
			{
				// use the info from last drag if it was set
				var info = Handler?.GetDragInfo(DragArgs);
				if (info?.IsChanged == true)
					_dragInfo = info;
				base.HandleDragDrop(o, args);
				_dragInfo = null;
			}
		}

		protected new GridViewConnector Connector => (GridViewConnector)base.Connector;

		protected override WeakConnector CreateConnector() => new GridViewConnector();

		public GridViewDragInfo GetDragInfo(DragEventArgs e) => e.ControlObject as GridViewDragInfo;

		protected override bool HasRows => model.Count > 0;

	}
}

