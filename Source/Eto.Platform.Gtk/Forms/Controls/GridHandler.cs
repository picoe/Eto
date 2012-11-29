using System;
using Eto.Forms;
using System.Runtime.InteropServices;
using Eto.Drawing;
using System.Linq;
using System.Collections.Generic;
using Eto.Platform.GtkSharp.Forms.Cells;

namespace Eto.Platform.GtkSharp.Forms.Controls
{
	public abstract class GridHandler<W> : GtkControl<Gtk.ScrolledWindow, W>, IGrid, ICellDataSource, IGridHandler
		where W: Grid
	{
		ColumnCollection columns;
		ContextMenu contextMenu;
		Dictionary<int, int> columnMap = new Dictionary<int, int> ();

		protected bool SkipSelectedChange { get; set; }

		protected Gtk.TreeView Tree { get; private set; }

		protected Dictionary<int, int> ColumnMap { get { return columnMap; } }
		
		public GridHandler ()
		{
			Control = new Gtk.ScrolledWindow {
				ShadowType = Gtk.ShadowType.In
			};
		}

		protected abstract Gtk.TreeModelImplementor CreateModelImplementor ();

		protected void UpdateModel ()
		{
			Tree.Model = new Gtk.TreeModelAdapter (CreateModelImplementor ());
		}
		
		public override void Initialize ()
		{
			base.Initialize ();
			Tree = new Gtk.TreeView ();
			UpdateModel ();
			Tree.HeadersVisible = true;

			Control.Add (Tree);

			Tree.Events |= Gdk.EventMask.ButtonPressMask;
			Tree.ButtonPressEvent += HandleTreeButtonPressEvent;

			columns = new ColumnCollection{ Handler = this };
			columns.Register (Widget.Columns);
		}
		
		[GLib.ConnectBefore]
		void HandleTreeButtonPressEvent (object o, Gtk.ButtonPressEventArgs args)
		{
			if (contextMenu != null && args.Event.Button == 3 && args.Event.Type == Gdk.EventType.ButtonPress) {
				var menu = ((ContextMenuHandler)contextMenu.Handler).Control;
				menu.Popup ();
				menu.ShowAll ();
			}
		}
		
		public override void AttachEvent (string handler)
		{
			switch (handler) {
			case Grid.ColumnHeaderClickEvent:
			case Grid.BeginCellEditEvent:
			case Grid.EndCellEditEvent:
			case Grid.CellFormattingEvent:
				SetupColumnEvents ();
				break;
			case Grid.SelectionChangedEvent:
				Tree.Selection.Changed += delegate {
					if (!SkipSelectedChange)
						Widget.OnSelectionChanged (EventArgs.Empty);
				};
				break;
			default:
				base.AttachEvent (handler);
				break;
			}
		}
		
		public override void OnLoadComplete (EventArgs e)
		{
			base.OnLoadComplete (e);
			Tree.AppendColumn (new Gtk.TreeViewColumn ());
		}

		void SetupColumnEvents ()
		{
			foreach (var col in Widget.Columns.Select (r => r.Handler).OfType<GridColumnHandler> ()) {
				col.SetupEvents ();
			}
		}
		
		class ColumnCollection : EnumerableChangedHandler<GridColumn, GridColumnCollection>
		{
			public GridHandler<W> Handler { get; set; }

			void RebindColumns ()
			{
				Handler.columnMap.Clear ();
				int columnIndex = 0;
				int dataIndex = 0;
				foreach (var col in Handler.Widget.Columns.Select (r => r.Handler).OfType<IGridColumnHandler> ()) {
					col.BindCell (Handler, Handler, columnIndex++, ref dataIndex);
				}
			}

			public override void AddItem (GridColumn item)
			{
				var colhandler = (GridColumnHandler)item.Handler;
				Handler.Tree.AppendColumn (colhandler.Control);
				RebindColumns ();
			}

			public override void InsertItem (int index, GridColumn item)
			{
				var colhandler = (GridColumnHandler)item.Handler;
				if (Handler.Tree.Columns.Length > 0)
					Handler.Tree.InsertColumn (colhandler.Control, index);
				else
					Handler.Tree.AppendColumn (colhandler.Control);
				RebindColumns ();
			}

			public override void RemoveItem (int index)
			{
				var colhandler = (GridColumnHandler)Handler.Widget.Columns [index].Handler;
				Handler.Tree.RemoveColumn (colhandler.Control);
				RebindColumns ();
			}

			public override void RemoveAllItems ()
			{
				foreach (var col in Handler.Tree.Columns) {
					Handler.Tree.RemoveColumn (col);
				}
				RebindColumns ();
			}

		}

		public bool ShowHeader {
			get { return Tree.HeadersVisible; }
			set { Tree.HeadersVisible = value; }
		}
		
		public bool AllowColumnReordering {
			get { return Tree.Reorderable; }
			set { Tree.Reorderable = value; }
		}
		
		public int NumberOfColumns {
			get {
				return Widget.Columns.Count;
			}
		}

		public abstract object GetItem (Gtk.TreePath path);

		public abstract Gtk.TreeIter GetIterAtRow (int row);
		
		public void SetColumnMap (int dataIndex, int column)
		{
			columnMap [dataIndex] = column;
		}
		
		public ContextMenu ContextMenu {
			get { return contextMenu; }
			set { contextMenu = value; }
		}

		public void EndCellEditing (Gtk.TreePath path, int column)
		{
			var row = path.Indices.Length > 0 ? path.Indices [0] : -1;
			var item = GetItem (path) as IGridItem;
			Widget.OnEndCellEdit (new GridViewCellArgs (Widget.Columns [column], row, column, item));
		}

		public void BeginCellEditing (Gtk.TreePath path, int column)
		{
			var row = path.Indices.Length > 0 ? path.Indices [0] : -1;
			var item = GetItem (path) as IGridItem;
			Widget.OnBeginCellEdit (new GridViewCellArgs (Widget.Columns [column], row, column, item));
		}
		
		public void ColumnClicked (GridColumnHandler column)
		{
			Widget.OnColumnHeaderClick (new GridColumnEventArgs (column.Widget));
		}

		public bool AllowMultipleSelection {
			get { return Tree.Selection.Mode == Gtk.SelectionMode.Multiple; }
			set { Tree.Selection.Mode = value ? Gtk.SelectionMode.Multiple : Gtk.SelectionMode.Browse; }
		}

		public virtual IEnumerable<int> SelectedRows {
			get {
				var rows = Tree.Selection.GetSelectedRows ();
				foreach (var row in rows) {
					yield return row.Indices[0];
				}
			}
		}

		public int RowHeight
		{
			get; set;
		}

		public void SelectAll ()
		{
			Tree.Selection.SelectAll ();
		}

		public void SelectRow (int row)
		{
			Tree.Selection.SelectIter (GetIterAtRow (row));
		}

		public void UnselectRow (int row)
		{
			Tree.Selection.UnselectIter (GetIterAtRow (row));
		}

		public void UnselectAll ()
		{
			Tree.Selection.UnselectAll ();
		}

		public void OnCellFormatting (GridCellFormatEventArgs args)
		{
			Widget.OnCellFormatting (args);
		}

	}
}

