using System;
using Eto.Forms;

namespace Eto.Platform.GtkSharp.Forms.Controls
{
	public class GridColumnHandler : WidgetHandler<Gtk.TreeViewColumn, GridColumn>, IGridColumn
	{
		Cell dataCell;
		bool autoSize;
		bool editable;
		
		public GridColumnHandler ()
		{
			Control = new Gtk.TreeViewColumn ();
			Resizable = true;
			AutoSize = true;
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
					Control.Sizing = Gtk.TreeViewColumnSizing.Autosize;
				else
					Control.Sizing = Gtk.TreeViewColumnSizing.Fixed;
			}
		}
		
		void SetCellAttributes()
		{
			if (dataCell != null) {
				((ICellHandler)dataCell.Handler).SetEditable (Control, editable);
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
				Control.PackStart (((ICellHandler)dataCell.Handler).Control, true);
				SetCellAttributes ();
			}
		}
		
		public bool Visible {
			get { return Control.Visible; }
			set { Control.Visible = value; }
		}
		
		public void BindCell (ICellDataSource source, int index)
		{
			if (dataCell != null) {
				((ICellHandler)dataCell.Handler).BindCell (source, Control, index);
			}
		}
		
		public void GetNullValue (ref GLib.Value val)
		{
			if (dataCell != null) {
				((ICellHandler)dataCell.Handler).GetNullValue(ref val);
			}
		}
	}
}

