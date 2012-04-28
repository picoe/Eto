using System;
using Eto.Forms;

namespace Eto.Platform.GtkSharp.Forms.Controls
{
	public interface IGridHandler
	{
		bool IsEventHandled(string handler);
	}
	
	public interface IGridColumnHandler
	{
		GLib.Value GetValue(object dataItem, int dataColumn);
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
				cellhandler.BindCell (source, Control, columnIndex, ref dataIndex);
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
		}

		public override void AttachEvent (string handler)
		{
			((ICellHandler)dataCell.Handler).HandleEvent(handler);
		}
		
		public GLib.Value GetValue (object dataItem, int dataColumn)
		{
			if (dataCell != null) {
				return ((ICellHandler)dataCell.Handler).GetValue(dataItem, dataColumn);
			}
			else return new GLib.Value((string)null);
		}
	}
}

