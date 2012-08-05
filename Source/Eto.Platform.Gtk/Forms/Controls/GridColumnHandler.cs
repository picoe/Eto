using System;
using Eto.Forms;
using Eto.Platform.GtkSharp.Forms.Cells;

namespace Eto.Platform.GtkSharp.Forms.Controls
{
	public interface IGridHandler
	{
		bool IsEventHandled(string handler);
		
		void ColumnClicked(GridColumnHandler column);
	}
	
	public interface IGridColumnHandler
	{
		GLib.Value GetValue(object dataItem, int dataColumn, int row);
		void BindCell (IGridHandler grid, ICellDataSource source, int columnIndex, ref int dataIndex);
	}
	
	public class GridColumnHandler : WidgetHandler<Gtk.TreeViewColumn, GridColumn>, IGridColumn, IGridColumnHandler
	{
		Cell dataCell;
		bool autoSize;
		bool editable;
		bool cellsAdded;
		IGridHandler grid;
		
		public GridColumnHandler ()
		{
			Control = new Gtk.TreeViewColumn ();
			AutoSize = true;
			Resizable = true;
		}
		
		public override void Initialize ()
		{
			base.Initialize ();
			this.DataCell = new TextBoxCell (Widget.Generator);
		}

		public string HeaderText {
			get { return Control.Title; }
			set { Control.Title = value; }
		}

		public bool Resizable {
			get { return Control.Resizable; }
			set { Control.Resizable = value; }
		}

		public bool Sortable {
			get { return Control.Clickable; }
			set { Control.Clickable = value; }
		}
		
		public bool AutoSize {
			get {
				return autoSize;
			}
			set {
				autoSize = value;
				if (value)
					Control.Sizing = Gtk.TreeViewColumnSizing.GrowOnly;
				else
					Control.Sizing = Gtk.TreeViewColumnSizing.Fixed;
			}
		}
		
		void SetCellAttributes()
		{
			if (dataCell != null) {
				((ICellHandler)dataCell.Handler).SetEditable (Control, editable);
				SetupEvents ();
			}
		}
		
		public bool Editable {
			get {
				return editable;
			}
			set {
				editable = value;
				SetCellAttributes ();
			}
		}
		
		public int Width {
			get { return Control.Width; }
			set { Control.FixedWidth = value; }
		}
		
		public Cell DataCell {
			get {
				return dataCell;
			}
			set {
				dataCell = value;
			}
		}
		
		public bool Visible {
			get { return Control.Visible; }
			set { Control.Visible = value; }
		}
		
		public void BindCell (IGridHandler grid, ICellDataSource source, int columnIndex, ref int dataIndex)
		{
			this.grid = grid;
			if (dataCell != null) {
				var cellhandler = (ICellHandler)dataCell.Handler;
				if (!cellsAdded) {
					cellhandler.AddCells (Control);
					cellsAdded = true;
				}
				SetCellAttributes ();
				cellhandler.BindCell (source, this, columnIndex, ref dataIndex);
			}
			SetupEvents ();
		}

		public void SetupEvents ()
		{
			if (grid == null) return;
			if (grid.IsEventHandled (GridView.BeginCellEditEvent))
				HandleEvent (GridView.BeginCellEditEvent);
			if (grid.IsEventHandled (GridView.EndCellEditEvent))
				HandleEvent (GridView.EndCellEditEvent);
			if (grid.IsEventHandled (Grid.ColumnHeaderClickEvent))
				HandleEvent (Grid.ColumnHeaderClickEvent);
			if (grid.IsEventHandled (Grid.CellFormattingEvent))
				HandleEvent (Grid.CellFormattingEvent);
		}

		public override void AttachEvent (string handler)
		{
			switch (handler) {
			case Grid.ColumnHeaderClickEvent:
				Control.Clicked += (sender, e) => {
					if (grid != null)
						grid.ColumnClicked (this);
				};
				break;
			default:
				((ICellHandler)dataCell.Handler).HandleEvent(handler);
				break;
			}
		}
		
		public GLib.Value GetValue (object dataItem, int dataColumn, int row)
		{
			if (dataCell != null) {
				return ((ICellHandler)dataCell.Handler).GetValue(dataItem, dataColumn, row);
			}
			else return new GLib.Value((string)null);
		}
		
	}
}

