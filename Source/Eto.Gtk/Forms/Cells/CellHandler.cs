using System;
using Eto.Forms;
using Eto.GtkSharp.Forms.Controls;
using System.Runtime.InteropServices;

namespace Eto.GtkSharp.Forms.Cells
{
	public interface ICellDataSource
	{
		object GetItem(Gtk.TreePath path);

		void EndCellEditing(Gtk.TreePath path, int column);

		void BeginCellEditing(Gtk.TreePath path, int column);

		void SetColumnMap(int dataIndex, int column);

		int RowHeight { get; set; }

		void OnCellFormatting(GridCellFormatEventArgs args);
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

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case Grid.CellEditingEvent:
					Control.EditingStarted += Connector.HandleEditingStarted;
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		protected new SingleCellConnector Connector { get { return (SingleCellConnector)base.Connector; } }

		protected override WeakConnector CreateConnector()
		{
			return new SingleCellConnector();
		}

		protected class SingleCellConnector : WeakConnector
		{
			public new SingleCellHandler<TControl, TWidget, TCallback> Handler { get { return (SingleCellHandler<TControl, TWidget, TCallback>)base.Handler; } }

			public void HandleEditingStarted(object o, Gtk.EditingStartedArgs args)
			{
				Handler.Source.BeginCellEditing(new Gtk.TreePath(args.Path), Handler.ColumnIndex);
			}
		}
	}

	public abstract class CellHandler<TControl, TWidget, TCallback> : WidgetHandler<TControl, TWidget, TCallback>, Cell.IHandler, ICellHandler
		where TWidget: Cell
		where TControl: Gtk.CellRenderer
	{
		int? dataIndex;
		int itemCol;
		int rowCol;

		public GridColumnHandler Column { get; private set; }

		public int ColumnIndex { get; private set; }

		public ICellDataSource Source { get; private set; }

		public bool FormattingEnabled { get; private set; }

		public abstract void AddCells(Gtk.TreeViewColumn column);

		public void BindCell(ICellDataSource source, GridColumnHandler column, int columnIndex, ref int dataIndex)
		{
			Source = source;
			Column = column;
			ColumnIndex = columnIndex;
			this.dataIndex = dataIndex;
			BindCell(ref dataIndex);
			BindBase(ref dataIndex);
		}

		protected void ReBind()
		{
			if (dataIndex != null)
			{
				var dataIndexValue = dataIndex.Value;
				BindCell(ref dataIndexValue);
				BindBase(ref dataIndexValue);
			}
		}

		protected void BindBase(ref int dataIndex)
		{
			if (FormattingEnabled)
			{
				itemCol = SetColumnMap(dataIndex);
				Column.Control.AddAttribute(Control, "item", dataIndex++);
				rowCol = SetColumnMap(dataIndex);
				Column.Control.AddAttribute(Control, "row", dataIndex++);
			}
		}

		public void Format(GridCellFormatEventArgs args)
		{
			Source.OnCellFormatting(args);
		}

		protected abstract void BindCell(ref int dataIndex);

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
			if (FormattingEnabled)
			{
				if (dataColumn == itemCol)
					return new GLib.Value(dataItem);
				if (dataColumn == rowCol)
					return new GLib.Value(row);
			}
			return GetValueInternal(dataItem, dataColumn, row);
		}

		protected abstract GLib.Value GetValueInternal(object dataItem, int dataColumn, int row);

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case Grid.CellFormattingEvent:
					FormattingEnabled = true;
					ReBind();
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}
	}
}

