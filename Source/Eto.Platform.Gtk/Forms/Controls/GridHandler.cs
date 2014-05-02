using System;
using Eto.Forms;
using System.Linq;
using System.Collections.Generic;
using Eto.Platform.GtkSharp.Forms.Cells;

namespace Eto.Platform.GtkSharp.Forms.Controls
{
	public abstract class GridHandler<TWidget> : GtkControl<Gtk.ScrolledWindow, TWidget>, IGrid, ICellDataSource, IGridHandler
		where TWidget : Grid
	{
		ColumnCollection columns;
		ContextMenu contextMenu;
		readonly Dictionary<int, int> columnMap = new Dictionary<int, int>();

		protected bool SkipSelectedChange { get; set; }

		protected Gtk.TreeView Tree { get; private set; }

		protected Dictionary<int, int> ColumnMap { get { return columnMap; } }

		protected GridHandler()
		{
			Control = new Gtk.ScrolledWindow
			{
				ShadowType = Gtk.ShadowType.In
			};
		}

		protected abstract ITreeModelImplementor CreateModelImplementor();

		protected void UpdateModel()
		{
			Tree.Model = new Gtk.TreeModelAdapter(CreateModelImplementor());
		}

		protected override void Initialize()
		{
			base.Initialize();
			Tree = new Gtk.TreeView();
			UpdateModel();
			Tree.HeadersVisible = true;

			Control.Add(Tree);

			Tree.Events |= Gdk.EventMask.ButtonPressMask;
			Tree.ButtonPressEvent += Connector.HandleTreeButtonPressEvent;

			columns = new ColumnCollection { Handler = this };
			columns.Register(Widget.Columns);
		}

		protected new GridConnector Connector { get { return (GridConnector)base.Connector; } }

		protected override WeakConnector CreateConnector()
		{
			return new GridConnector();
		}

		protected class GridConnector : GtkControlConnector
		{
			public new GridHandler<TWidget> Handler { get { return (GridHandler<TWidget>)base.Handler; } }

			[GLib.ConnectBefore]
			public void HandleTreeButtonPressEvent(object o, Gtk.ButtonPressEventArgs args)
			{
				var handler = Handler;
				if (handler.contextMenu != null && args.Event.Button == 3 && args.Event.Type == Gdk.EventType.ButtonPress)
				{
					var menu = ((ContextMenuHandler)handler.contextMenu.Handler).Control;
					menu.Popup();
					menu.ShowAll();
				}
			}

			public void HandleGridSelectionChanged(object sender, EventArgs e)
			{
				if (!Handler.SkipSelectedChange)
					Handler.Widget.OnSelectionChanged(EventArgs.Empty);
			}
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case Grid.ColumnHeaderClickEvent:
				case Grid.CellEditingEvent:
				case Grid.CellEditedEvent:
				case Grid.CellFormattingEvent:
					SetupColumnEvents();
					break;
				case Grid.SelectionChangedEvent:
					Tree.Selection.Changed += Connector.HandleGridSelectionChanged;
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		public override void OnLoadComplete(EventArgs e)
		{
			base.OnLoadComplete(e);
			Tree.AppendColumn(new Gtk.TreeViewColumn());
		}

		void SetupColumnEvents()
		{
			foreach (var col in Widget.Columns.Select(r => r.Handler).OfType<GridColumnHandler>())
			{
				col.SetupEvents();
			}
		}

		protected virtual void UpdateColumns()
		{
			columnMap.Clear();
			int columnIndex = 0;
			int dataIndex = 0;
			foreach (var col in Widget.Columns.Select(r => r.Handler).OfType<IGridColumnHandler>())
			{
				col.BindCell(this, this, columnIndex++, ref dataIndex);
			}
		}

		class ColumnCollection : EnumerableChangedHandler<GridColumn, GridColumnCollection>
		{
			public GridHandler<TWidget> Handler { get; set; }

			public override void AddItem(GridColumn item)
			{
				var colhandler = (GridColumnHandler)item.Handler;
				Handler.Tree.AppendColumn(colhandler.Control);
				Handler.UpdateColumns();
			}

			public override void InsertItem(int index, GridColumn item)
			{
				var colhandler = (GridColumnHandler)item.Handler;
				if (Handler.Tree.Columns.Length > 0)
					Handler.Tree.InsertColumn(colhandler.Control, index);
				else
					Handler.Tree.AppendColumn(colhandler.Control);
				Handler.UpdateColumns();
			}

			public override void RemoveItem(int index)
			{
				var colhandler = (GridColumnHandler)Handler.Widget.Columns[index].Handler;
				Handler.Tree.RemoveColumn(colhandler.Control);
				Handler.UpdateColumns();
			}

			public override void RemoveAllItems()
			{
				foreach (var col in Handler.Tree.Columns)
				{
					Handler.Tree.RemoveColumn(col);
				}
				Handler.UpdateColumns();
			}

		}

		public bool ShowHeader
		{
			get { return Tree.HeadersVisible; }
			set { Tree.HeadersVisible = value; }
		}

		public bool AllowColumnReordering
		{
			get { return Tree.Reorderable; }
			set { Tree.Reorderable = value; }
		}

		public int NumberOfColumns
		{
			get
			{
				return Widget.Columns.Count;
			}
		}

		public abstract object GetItem(Gtk.TreePath path);

		public abstract Gtk.TreeIter GetIterAtRow(int row);

		public void SetColumnMap(int dataIndex, int column)
		{
			columnMap[dataIndex] = column;
		}

		public ContextMenu ContextMenu
		{
			get { return contextMenu; }
			set { contextMenu = value; }
		}

		public void EndCellEditing(Gtk.TreePath path, int column)
		{
			var row = path.Indices.Length > 0 ? path.Indices[0] : -1;
			var item = GetItem(path);
			Widget.OnCellEdited(new GridViewCellArgs(Widget.Columns[column], row, column, item));
		}

		public void BeginCellEditing(Gtk.TreePath path, int column)
		{
			var row = path.Indices.Length > 0 ? path.Indices[0] : -1;
			var item = GetItem(path);
			Widget.OnCellEditing(new GridViewCellArgs(Widget.Columns[column], row, column, item));
		}

		public void ColumnClicked(GridColumnHandler column)
		{
			Widget.OnColumnHeaderClick(new GridColumnEventArgs(column.Widget));
		}

		public bool AllowMultipleSelection
		{
			get { return Tree.Selection.Mode == Gtk.SelectionMode.Multiple; }
			set { Tree.Selection.Mode = value ? Gtk.SelectionMode.Multiple : Gtk.SelectionMode.Browse; }
		}

		public virtual IEnumerable<int> SelectedRows
		{
			get
			{
				var rows = Tree.Selection.GetSelectedRows();
				foreach (var row in rows)
				{
					yield return row.Indices[0];
				}
			}
		}

		public int RowHeight
		{
			get;
			set;
		}

		public void SelectAll()
		{
			Tree.Selection.SelectAll();
		}

		public void SelectRow(int row)
		{
			Tree.Selection.SelectIter(GetIterAtRow(row));
		}

		public void UnselectRow(int row)
		{
			Tree.Selection.UnselectIter(GetIterAtRow(row));
		}

		public void UnselectAll()
		{
			Tree.Selection.UnselectAll();
		}

		public void OnCellFormatting(GridCellFormatEventArgs args)
		{
			Widget.OnCellFormatting(args);
		}

	}
}

