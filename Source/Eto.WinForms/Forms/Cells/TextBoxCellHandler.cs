using swf = System.Windows.Forms;
using sd = System.Drawing;
using Eto.Forms;
using System;

namespace Eto.WinForms.Forms.Cells
{
	public class TextBoxCellHandler : CellHandler<swf.DataGridViewTextBoxCell, TextBoxCell, TextBoxCell.ICallback>, TextBoxCell.IHandler
	{
		class EtoCell : swf.DataGridViewTextBoxCell
		{
			public TextBoxCellHandler Handler { get; set; }

			public override void PositionEditingControl (bool setLocation, bool setSize, sd.Rectangle cellBounds, sd.Rectangle cellClip, swf.DataGridViewCellStyle cellStyle, bool singleVerticalBorderAdded, bool singleHorizontalBorderAdded, bool isFirstDisplayedColumn, bool isFirstDisplayedRow)
			{
				Handler.PositionEditingControl (RowIndex, ref cellClip, ref cellBounds);
				base.PositionEditingControl (setLocation, setSize, cellBounds, cellClip, cellStyle, singleVerticalBorderAdded, singleHorizontalBorderAdded, isFirstDisplayedColumn, isFirstDisplayedRow);
			}

			protected override sd.Size GetPreferredSize (sd.Graphics graphics, swf.DataGridViewCellStyle cellStyle, int rowIndex, sd.Size constraintSize)
			{
				var size = base.GetPreferredSize (graphics, cellStyle, rowIndex, constraintSize);
				size.Width += Handler.GetRowOffset (rowIndex);
				return size;
			}

			protected override void Paint (System.Drawing.Graphics graphics, System.Drawing.Rectangle clipBounds, System.Drawing.Rectangle cellBounds, int rowIndex, swf.DataGridViewElementStates cellState, object value, object formattedValue, string errorText, swf.DataGridViewCellStyle cellStyle, swf.DataGridViewAdvancedBorderStyle advancedBorderStyle, swf.DataGridViewPaintParts paintParts)
			{
				Handler.Paint (graphics, clipBounds, ref cellBounds, rowIndex, cellState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, ref paintParts);
				base.Paint (graphics, clipBounds, cellBounds, rowIndex, cellState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);
			}

			protected override void OnMouseClick (swf.DataGridViewCellMouseEventArgs e)
			{
				if (!Handler.MouseClick (e, e.RowIndex))
					base.OnMouseClick (e);
			}

			public override object Clone ()
			{
				var val = (EtoCell)base.Clone();
				val.Handler = Handler;
				return val;
			}
		}


		public TextBoxCellHandler ()
		{
			Control = new EtoCell { Handler = this };
		}

		public override void SetCellValue (object dataItem, object value)
		{
			if (Widget.Binding != null) {
				Widget.Binding.SetValue (dataItem, Convert.ToString(value));
			}
		}

		public override object GetCellValue (object dataItem)
		{
			return Widget.Binding == null ? null : Widget.Binding.GetValue(dataItem);
		}

	}
}

