using System;
using Eto.Forms;

namespace Eto.Platform.GtkSharp.Forms.Controls
{
	public interface ICellDataSource
	{
		void SetValue(string path, int column, object value);
		void EndCellEditing (string path, int column);
		void BeginCellEditing (string path, int column);
	}
	
	public interface ICellHandler
	{
		Gtk.CellRenderer Control { get; }
		void BindCell (ICellDataSource source, Gtk.TreeViewColumn column, int index);
		void SetEditable (Gtk.TreeViewColumn column, bool editable);
		void GetNullValue (ref GLib.Value val);
		void HandleEvent (string eventHandler);
	}
	
	public abstract class CellHandler<T, W> : WidgetHandler<T, W>, ICell, ICellHandler
		where T: Gtk.CellRenderer
		where W: Cell
	{
		public Gtk.TreeViewColumn Column { get; private set; }
		public int ColumnIndex { get; private set; }
		public ICellDataSource Source { get; private set; }
		
		Gtk.CellRenderer ICellHandler.Control {
			get { return Control; }
		}
		
		public void BindCell (ICellDataSource source, Gtk.TreeViewColumn column, int index)
		{
			Source = source;
			Column = column;
			ColumnIndex = index;
			BindCell ();
		}
		
		protected abstract void BindCell ();
		
		public abstract void SetEditable (Gtk.TreeViewColumn column, bool editable);
		
		protected void SetValue(string path, object value)
		{
			Source.SetValue (path, ColumnIndex, value);
		}
		
		public abstract void GetNullValue (ref GLib.Value val);

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
}

