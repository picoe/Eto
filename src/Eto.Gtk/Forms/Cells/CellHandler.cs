using System;
using Eto.Forms;
using Eto.GtkSharp.Forms.Controls;
using System.Runtime.InteropServices;

namespace Eto.GtkSharp.Forms.Cells
{
	public interface ICellDataSource
	{
		object GetItem(int row);

		object GetItem(Gtk.TreePath path);

		void EndCellEditing(Gtk.TreePath path, int column);

		void BeginCellEditing(Gtk.TreePath path, int column);

		void SetColumnMap(int dataIndex, int column);

		int RowHeight { get; set; }

		void OnCellFormatting(GridCellFormatEventArgs args);

		int RowDataColumn { get; }
	}

	public interface IEtoCellRenderer
	{
		bool Editing { get; }
	}

	public interface ICellHandler
	{
		void BindCell(ICellDataSource source, GridColumnHandler column, int columnIndex, ref int dataIndex);

		void SetEditable(Gtk.TreeViewColumn column, bool editable);

		GLib.Value GetValue(object dataItem, int column, int row);

		void HandleEvent(string eventHandler, bool defaultEvent = true);

		void AddCells(Gtk.TreeViewColumn column);
	}

	static class GtkCell
	{
		[DllImport("gtksharpglue-2", CallingConvention = CallingConvention.Cdecl)]
		internal static extern void gtksharp_cellrenderer_invoke_render(IntPtr gtype, IntPtr handle, IntPtr window, IntPtr widget, ref Gdk.Rectangle backgroundArea, ref Gdk.Rectangle cellArea, ref Gdk.Rectangle exposeArea, Gtk.CellRendererState flags);
	}

	public abstract class SingleCellHandler<TControl, TWidget, TCallback> : CellHandler<TControl, TWidget, TCallback>
		where TControl: Gtk.CellRenderer
		where TWidget: Cell
	{
		public override void AddCells(Gtk.TreeViewColumn column)
		{
			column.PackStart(Control, true);
		}
	}

	public abstract class CellHandler<TControl, TWidget, TCallback> : WidgetHandler<TControl, TWidget, TCallback>, Cell.IHandler, ICellHandler
		where TWidget: Cell
		where TControl: Gtk.CellRenderer
	{
		int? dataIndex;

		public GridColumnHandler Column { get; private set; }

		public int ColumnIndex { get; private set; }

		public ICellDataSource Source { get; private set; }

		public bool FormattingEnabled { get; protected set; }

		public abstract void AddCells(Gtk.TreeViewColumn column);

		public void BindCell(ICellDataSource source, GridColumnHandler column, int columnIndex, ref int dataIndex)
		{
			Source = source;
			Column = column;
			ColumnIndex = columnIndex;
			this.dataIndex = dataIndex;
			BindCell(ref dataIndex);
		}

		protected void ReBind()
		{
			if (dataIndex != null)
			{
				var dataIndexValue = dataIndex.Value;
				BindCell(ref dataIndexValue);
			}
		}

		public void Format(GridCellFormatEventArgs args)
		{
			Source.OnCellFormatting(args);
		}

		protected virtual void BindCell(ref int dataIndex)
		{
			//if (FormattingEnabled)
			{
				Column.Control.AddAttribute(Control, "row", Source.RowDataColumn);
			}
		}

		protected int SetColumnMap(int dataIndex)
		{
			Source.SetColumnMap(dataIndex, ColumnIndex);
			return dataIndex;
		}

		public abstract void SetEditable(Gtk.TreeViewColumn column, bool editable);

		protected void SetValue(string path, object value)
		{
			SetValue(new Gtk.TreePath(path), value);
		}

		protected void SetValue(Gtk.TreePath path, object value)
		{
			var item = Source.GetItem(path);
			SetValue(item, value);
		}

		public abstract void SetValue(object dataItem, object value);

		public GLib.Value GetValue(object dataItem, int dataColumn, int row)
		{
			return GetValueInternal(dataItem, dataColumn, row);
		}

		protected abstract GLib.Value GetValueInternal(object dataItem, int dataColumn, int row);

		protected new CellConnector Connector { get { return (CellConnector)base.Connector; } }

		protected override WeakConnector CreateConnector()
		{
			return new CellConnector();
		}

		protected class CellConnector : WeakConnector
		{
			public new CellHandler<TControl, TWidget, TCallback> Handler { get { return (CellHandler<TControl, TWidget, TCallback>)base.Handler; } }

			public void HandleEditingStarted(object o, Gtk.EditingStartedArgs args)
			{
				Handler.Source.BeginCellEditing(new Gtk.TreePath(args.Path), Handler.ColumnIndex);
			}
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case Grid.CellFormattingEvent:
					FormattingEnabled = true;
					ReBind();
					break;
				case Grid.CellEditingEvent:
					Control.EditingStarted += Connector.HandleEditingStarted;
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}
	}
}

