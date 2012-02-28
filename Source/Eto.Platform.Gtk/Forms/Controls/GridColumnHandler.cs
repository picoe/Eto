using System;
using Eto.Forms;

namespace Eto.Platform.GtkSharp.Forms.Controls
{
	public class GridColumnHandler : WidgetHandler<Gtk.TreeViewColumn, GridColumn>, IGridColumn
	{
		Cell dataCell;
		bool autoSize;
		bool editable;
		int column;
		GridViewHandler grid;
		
		public GridColumnHandler ()
		{
			Control = new Gtk.TreeViewColumn ();
			AutoSize = true;
			Resizable = true;
		}
		
		public override void Initialize ()
		{
			base.Initialize ();
			this.DataCell = new TextCell (Widget.Generator);
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
		
		public void BindCell (GridViewHandler grid, ICellDataSource source, int index)
		{
			this.grid = grid;
			this.column = index;
			if (dataCell != null) {
				Control.PackStart (((ICellHandler)dataCell.Handler).Control, true);
				SetCellAttributes ();
				((ICellHandler)dataCell.Handler).BindCell (source, Control, index);
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
		
		public GLib.Value GetValue (IGridItem item)
		{
			if (dataCell != null) {
				return ((ICellHandler)dataCell.Handler).GetValue(item, this.column);
			}
			else return new GLib.Value((string)null);
		}
	}
}

