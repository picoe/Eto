using System;
using Eto.Forms;

namespace Eto.Platform.GtkSharp.Forms.Controls
{
	public interface ICellDataSource
	{
		object GetItem (string path);

		void EndCellEditing (string path, int column);

		void BeginCellEditing (string path, int column);
		
		void SetColumnMap(int dataIndex, int column);
	}
	
	public interface ICellHandler
	{
		void BindCell (ICellDataSource source, Gtk.TreeViewColumn column, int columnIndex, ref int dataIndex);

		void SetEditable (Gtk.TreeViewColumn column, bool editable);

		GLib.Value GetValue (object dataItem, int column);

		void HandleEvent (string eventHandler);

		void AddCells (Gtk.TreeViewColumn column);
	}
	
	public abstract class SingleCellHandler<T, W> : CellHandler<T, W>
		where T: Gtk.CellRenderer
		where W: Cell
	{
		public override void AddCells (Gtk.TreeViewColumn column)
		{
			column.PackStart (this.Control, true);
		}
		
		public override void AttachEvent (string eventHandler)
		{
			switch (eventHandler) {
			case GridView.BeginCellEditEvent:
				Control.EditingStarted += (sender, e) => {
					Source.BeginCellEditing (e.Path, ColumnIndex);
				};
				break;
			default:
				base.AttachEvent (eventHandler);
				break;
			}
		}

	}
	
	public abstract class CellHandler<T, W> : WidgetHandler<T, W>, ICell, ICellHandler
		where W: Cell
	{
		public Gtk.TreeViewColumn Column { get; private set; }

		public int ColumnIndex { get; private set; }

		public ICellDataSource Source { get; private set; }
		
		public abstract void AddCells (Gtk.TreeViewColumn column);
		
		public void BindCell (ICellDataSource source, Gtk.TreeViewColumn column, int columnIndex, ref int dataIndex)
		{
			Source = source;
			Column = column;
			ColumnIndex = columnIndex;
			BindCell (ref dataIndex);
		}
		
		protected abstract void BindCell (ref int dataIndex);
		
		protected void SetColumnMap(int dataIndex)
		{
			Source.SetColumnMap (dataIndex, ColumnIndex);
		}
		
		public abstract void SetEditable (Gtk.TreeViewColumn column, bool editable);
		
		protected void SetValue (string path, object value)
		{
			var item = Source.GetItem (path);
			SetValue (item, value);
		}
		
		public abstract void SetValue (object dataItem, object value);
		
		public abstract GLib.Value GetValue (object dataItem, int dataColumn);

	}
}

