using swf = System.Windows.Forms;
using sd = System.Drawing;
using Eto.Forms;

namespace Eto.WinForms.Forms.Cells
{
	public class CheckBoxCellHandler : CellHandler<swf.DataGridViewCheckBoxCell, CheckBoxCell, CheckBoxCell.ICallback>, CheckBoxCell.IHandler
	{

		class EtoCell : swf.DataGridViewCheckBoxCell
		{
			public CheckBoxCellHandler Handler { get; set; }

			public override void PositionEditingControl(bool setLocation, bool setSize, sd.Rectangle cellBounds, sd.Rectangle cellClip, swf.DataGridViewCellStyle cellStyle, bool singleVerticalBorderAdded, bool singleHorizontalBorderAdded, bool isFirstDisplayedColumn, bool isFirstDisplayedRow)
			{
				Handler.PositionEditingControl(RowIndex, ref cellClip, ref cellBounds);
				base.PositionEditingControl(setLocation, setSize, cellBounds, cellClip, cellStyle, singleVerticalBorderAdded, singleHorizontalBorderAdded, isFirstDisplayedColumn, isFirstDisplayedRow);
			}

			protected override sd.Size GetPreferredSize(sd.Graphics graphics, swf.DataGridViewCellStyle cellStyle, int rowIndex, sd.Size constraintSize)
			{
				var size = base.GetPreferredSize(graphics, cellStyle, rowIndex, constraintSize);
				size.Width += Handler.GetRowOffset(rowIndex);
				return size;
			}

			protected override void Paint(sd.Graphics graphics, sd.Rectangle clipBounds, sd.Rectangle cellBounds, int rowIndex, swf.DataGridViewElementStates elementState, object value, object formattedValue, string errorText, swf.DataGridViewCellStyle cellStyle, swf.DataGridViewAdvancedBorderStyle advancedBorderStyle, swf.DataGridViewPaintParts paintParts)
			{
				Handler.Paint(graphics, clipBounds, ref cellBounds, rowIndex, elementState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, ref paintParts);
				base.Paint(graphics, clipBounds, cellBounds, rowIndex, elementState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);
			}

			protected override void OnMouseClick(swf.DataGridViewCellMouseEventArgs e)
			{
				if (!Handler.MouseClick(e, e.RowIndex))
					base.OnMouseClick(e);
			}

			public override object Clone()
			{
				var val = (EtoCell)base.Clone();
				val.Handler = Handler;
				return val;
			}
		}

		public CheckBoxCellHandler()
		{
			Control = new EtoCell { Handler = this };
		}

		public override void SetCellValue(object dataItem, object value)
		{
			if (Widget.Binding != null)
			{
				Widget.Binding.SetValue(dataItem, (bool?)value);
			}
		}

		public override object GetCellValue(object dataItem)
		{
			if (Widget.Binding != null)
			{
				if (Control.ThreeState)
					return Widget.Binding.GetValue(dataItem);
				return Widget.Binding.GetValue(dataItem) ?? false;
			}
			return Control.ThreeState ? null : (bool?)false;
		}
	}
}

