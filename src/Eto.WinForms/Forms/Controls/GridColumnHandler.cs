using swf = System.Windows.Forms;
using sd = System.Drawing;
using Eto.Forms;
using Eto.WinForms.Forms.Cells;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Eto.WinForms.Forms.Controls
{

	public class GridColumnHandler : WidgetHandler<swf.DataGridViewColumn, GridColumn>, GridColumn.IHandler, ICellConfigHandler
	{
		Cell dataCell;

		class EtoDataGridViewColumn : swf.DataGridViewColumn
		{
			public GridColumnHandler Handler { get; set; }

			public override int GetPreferredWidth(DataGridViewAutoSizeColumnMode autoSizeColumnMode, bool fixedHeight)
			{
				return Math.Min(Handler.MaxWidth, base.GetPreferredWidth(autoSizeColumnMode, fixedHeight));
			}

		}

		public IGridHandler GridHandler { get; private set; }

		public GridColumnHandler()
		{
			Control = new EtoDataGridViewColumn { Handler = this };
			DataCell = new TextBoxCell();
			Editable = false;
			Resizable = true;
		}

		protected override void Initialize()
		{
			base.Initialize();
			SetAutoSizeMode();
		}

		public string HeaderText
		{
			get { return Control.HeaderText; }
			set { Control.HeaderText = value; }
		}

		public bool Resizable
		{
			get { return Control.Resizable == swf.DataGridViewTriState.True; }
			set { Control.Resizable = value ? swf.DataGridViewTriState.True : swf.DataGridViewTriState.False; }
		}

		public bool Sortable
		{
			get { return Control.SortMode == swf.DataGridViewColumnSortMode.Programmatic; }
			set { Control.SortMode = (value) ? swf.DataGridViewColumnSortMode.Programmatic : swf.DataGridViewColumnSortMode.NotSortable; }
		}

		static readonly object AutoSize_Key = new object();

		public bool AutoSize
		{
			get => Widget.Properties.Get<bool>(AutoSize_Key, true);
			set
			{
				if (Widget.Properties.TrySet(AutoSize_Key, value, true))
					SetAutoSizeMode();
			}
		}

		public int Width
		{
			get { return Control.Width; }
			set
			{
				AutoSize = value == -1;
				Control.Width = value;
			}
		}

		public Cell DataCell
		{
			get { return dataCell; }
			set
			{
				dataCell = value;
				if (dataCell != null)
				{
					var cellHandler = (ICellHandler)dataCell.Handler;
					cellHandler.CellConfig = this;
					Control.CellTemplate = cellHandler.Control;
				}
				else
					Control.CellTemplate = null;
			}
		}

		public bool Editable
		{
			get { return !Control.ReadOnly; }
			set { Control.ReadOnly = !value; }
		}

		public bool Visible
		{
			get { return Control.Visible; }
			set { Control.Visible = value; }
		}

		public swf.DataGridViewColumn Column => Control;

		static readonly object Expand_Key = new object();

		public bool Expand
		{
			get => Widget.Properties.Get<bool>(Expand_Key);
			set
			{
				if (Widget.Properties.TrySet(Expand_Key, value))
				{
					SetAutoSizeMode();
				}
			}
		}

		void SetAutoSizeMode()
		{
			if (Expand)
				Control.AutoSizeMode = swf.DataGridViewAutoSizeColumnMode.Fill;
			else if (AutoSize)
				Control.AutoSizeMode = swf.DataGridViewAutoSizeColumnMode.DisplayedCells;
			else
				Control.AutoSizeMode = swf.DataGridViewAutoSizeColumnMode.None;
		}

		public TextAlignment HeaderTextAlignment
		{
			get => Control.HeaderCell.Style.Alignment.ToEtoTextAlignment();
			set => Control.HeaderCell.Style.Alignment = value.ToSWFGridViewContentAlignment();
		}
		public int MinWidth { get => Control.MinimumWidth; set => Control.MinimumWidth = value; }

		int maxWidth = int.MaxValue;
		public int MaxWidth
		{
			get => maxWidth;
			set
			{
				maxWidth = value;
				if (AutoSize)
				{
					Control.DataGridView?.AutoResizeColumn(Control.Index);
				}
				else if (Control.Width > maxWidth)
				{
					Control.Width = maxWidth;
				}

			}
		}

		public void SetCellValue(object dataItem, object value)
		{
			if (dataCell != null)
			{
				var cellHandler = (ICellHandler)dataCell.Handler;
				cellHandler.SetCellValue(dataItem, value);
			}
		}

		public object GetCellValue(object dataItem)
		{
			if (dataCell != null)
			{
				var cellHandler = ((ICellHandler)dataCell.Handler);
				return cellHandler.GetCellValue(dataItem);
			}
			return null;
		}

		public virtual void Setup(IGridHandler gridHandler)
		{
			GridHandler = gridHandler;
		}

		public void Paint(sd.Graphics graphics, sd.Rectangle clipBounds, sd.Rectangle cellBounds, int rowIndex, swf.DataGridViewElementStates cellState, object value, object formattedValue, string errorText, swf.DataGridViewCellStyle cellStyle, swf.DataGridViewAdvancedBorderStyle advancedBorderStyle, ref swf.DataGridViewPaintParts paintParts)
		{
			if (GridHandler != null)
				GridHandler.Paint(this, graphics, clipBounds, cellBounds, rowIndex, cellState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, ref paintParts);
		}

		public int GetRowOffset(int rowIndex)
		{
			return GridHandler != null ? GridHandler.GetRowOffset(this, rowIndex) : 0;
		}

		public bool MouseClick(swf.MouseEventArgs e, int rowIndex)
		{
			return GridHandler != null && GridHandler.CellMouseClick(this, e, rowIndex);
		}
	}
}

