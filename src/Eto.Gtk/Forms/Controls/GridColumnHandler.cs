using Eto.GtkSharp.Forms.Cells;
namespace Eto.GtkSharp.Forms.Controls
{
	public interface IGridHandler
	{
		Gtk.TreeView Tree { get; }

		bool IsEventHandled(string handler);

		void ColumnClicked(GridColumnHandler column);
		int GetColumnDisplayIndex(GridColumnHandler column);
		void SetColumnDisplayIndex(GridColumnHandler column, int index);
		void ColumnWidthChanged(GridColumnHandler h);
	}

	public class GridColumnHandler : WidgetHandler<Gtk.TreeViewColumn, GridColumn>, GridColumn.IHandler
	{
		Cell dataCell;
		bool autoSize;
		bool editable;
		bool cellsAdded;
		IGridHandler grid;

		public IGridHandler GridHandler => grid;

		public GridColumnHandler()
		{
			Control = new Gtk.TreeViewColumn();
			AutoSize = true;
			Resizable = true;
			DataCell = new TextBoxCell();
			Control.Clickable = true;
		}

		public string HeaderText
		{
			get => Control.Title;
			set => Control.Title = value;
		}

		public bool Resizable
		{
			get => Control.Resizable;
			set => Control.Resizable = value;
		}

		static readonly object Sortable_Key = new object();

		public bool Sortable
		{
			get => Widget.Properties.Get<bool>(Sortable_Key);
			set => Widget.Properties.Set(Sortable_Key, value);
		}

		public bool AutoSize
		{
			get => Control.Sizing == Gtk.TreeViewColumnSizing.Fixed ? false : true;
			set
			{
				autoSize = value;
				Control.Sizing = value ? Gtk.TreeViewColumnSizing.Autosize : Gtk.TreeViewColumnSizing.Fixed;
			}
		}

		void SetCellAttributes()
		{
			if (dataCell != null)
			{
				((ICellHandler)dataCell.Handler).SetEditable(Control, editable);
				SetupEvents();
				if (Control.TreeView != null)
					Control.TreeView.QueueDraw();
			}
		}

		public bool Editable
		{
			get
			{
				return editable;
			}
			set
			{
				editable = value;
				SetCellAttributes();
			}
		}

		public int Width
		{
			get => Control.Width;
			set
			{
				autoSize = value == -1;
				Control.FixedWidth = value;
				Control.Sizing = autoSize ? Gtk.TreeViewColumnSizing.Autosize : Gtk.TreeViewColumnSizing.Fixed;
			}
		}

		public Cell DataCell
		{
			get => dataCell;
			set => dataCell = value;
		}

		public bool Visible
		{
			get => Control.Visible;
			set => Control.Visible = value;
		}

		public void SetupCell(IGridHandler grid, ICellDataSource source, int columnIndex, ref int dataIndex)
		{
			this.grid = grid;
			if (dataCell != null)
			{
				var cellhandler = (ICellHandler)dataCell.Handler;
				if (!cellsAdded)
				{
					cellhandler.AddCells(Control);
					cellsAdded = true;
				}
				SetCellAttributes();
				cellhandler.BindCell(source, this, columnIndex, ref dataIndex);
			}
			SetupEvents();
		}

		public void SetupEvents()
		{
			if (grid == null)
				return;
			if (grid.IsEventHandled(Grid.CellEditingEvent))
				HandleEvent(Grid.CellEditingEvent);
			if (grid.IsEventHandled(Grid.CellEditedEvent))
				HandleEvent(Grid.CellEditedEvent);
			if (grid.IsEventHandled(Grid.ColumnHeaderClickEvent))
				HandleEvent(Grid.ColumnHeaderClickEvent);
			if (grid.IsEventHandled(Grid.CellFormattingEvent))
				HandleEvent(Grid.CellFormattingEvent);
			if (grid.IsEventHandled(Grid.RowFormattingEvent))
				HandleEvent(Grid.RowFormattingEvent);
			if (grid.IsEventHandled(Grid.ColumnWidthChangedEvent))
				HandleEvent(Grid.ColumnWidthChangedEvent);
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case Grid.ColumnHeaderClickEvent:
					var handler = new WeakReference(this);
					Control.Clicked += (sender, e) =>
					{
						var h = ((GridColumnHandler)handler.Target);
						if (h != null && h.grid != null && h.Sortable)
							h.grid.ColumnClicked(h);
					};
					break;
				case Grid.ColumnWidthChangedEvent:
					var handler2 = new WeakReference(this);
					var lastWidth = -1;
					Control.AddNotification("width", (o, args) =>
					{
						var h = (GridColumnHandler)handler2.Target;
						if (h == null)
							return;
						if (lastWidth == h.Width)
							return;
						lastWidth = h.Width;
						h.grid?.ColumnWidthChanged(h);
					});
					break;
				default:
					((ICellHandler)dataCell.Handler).HandleEvent(id);
					break;
			}
		}

		public GLib.Value GetValue(object dataItem, int dataColumn, int row)
		{
			if (dataCell != null)
				return ((ICellHandler)dataCell.Handler).GetValue(dataItem, dataColumn, row);
			return new GLib.Value((string)null);
		}

		public bool Expand
		{
			get => Control.Expand;
			set => Control.Expand = value;
		}
		public TextAlignment HeaderTextAlignment
		{
			get => GtkConversions.ToEtoAlignment(Control.Alignment);
			set => Control.Alignment = value.ToAlignment();
		}
		public int MinWidth
		{
			get => Control.MinWidth == -1 ? 0 : Control.MinWidth;
			set
			{
				Control.MinWidth = value;
				GridHandler?.Tree?.ColumnsAutosize();
			}
		}
		public int MaxWidth
		{
			get => Control.MaxWidth == -1 ? int.MaxValue : Control.MaxWidth;
			set
			{
				Control.MaxWidth = value == int.MaxValue ? -1 : value;
				GridHandler?.Tree?.ColumnsAutosize();
			}
		}

		int? displayIndex;

		public int DisplayIndex
		{
			get => GridHandler?.GetColumnDisplayIndex(this) ?? displayIndex ?? -1;
			set
			{
				if (GridHandler != null)
					GridHandler.SetColumnDisplayIndex(this, value);
				else
					displayIndex = value;
			}
		}

#if GTK3
		public string HeaderToolTip
		{
			get => Control.Button.TooltipText;
			set => Control.Button.TooltipText = value;
		}
#else
		public string HeaderToolTip { get; set; }
#endif

		public IIndirectBinding<string> CellToolTipBinding { get; set; }

		internal void SetDisplayIndex()
		{
			if (displayIndex != null && GridHandler != null)
			{
				GridHandler.SetColumnDisplayIndex(this, displayIndex.Value);
				displayIndex = null;
			}
		}
	}
}

