using System;
using swf = System.Windows.Forms;
using sd = System.Drawing;
using Eto.Forms;

namespace Eto.WinForms.Forms.Cells
{
	public class ProgressCellHandler : CellHandler<ProgressCellHandler.EtoCell, ProgressCell, ProgressCell.ICallback>, ProgressCell.IHandler
	{
		public class EtoCell : swf.DataGridViewImageCell
		{
			public ProgressCellHandler Handler { get; set; }

			// Used to make custom cell consistent with a DataGridViewImageCell
			static sd.Image emptyImage;
			static EtoCell()
			{
				emptyImage = new sd.Bitmap(1, 1, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			}

			public EtoCell()
			{
				this.ValueType = typeof(int);
			}

			// Method required to make the Progress Cell consistent with the default Image Cell. 
			// The default Image Cell assumes an Image as a value, although the value of the Progress Cell is an int.
			protected override object GetFormattedValue(object value, int rowIndex, ref swf.DataGridViewCellStyle cellStyle, System.ComponentModel.TypeConverter valueTypeConverter, System.ComponentModel.TypeConverter formattedValueTypeConverter, swf.DataGridViewDataErrorContexts context)
			{
				return emptyImage;
			}

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

				int progressVal = (int)value;
				float percentage = ((float)progressVal / 100.0f);
				string progress = progressVal.ToString() + "%";

				sd.Brush backColorBrush = new sd.SolidBrush(cellStyle.BackColor);
				sd.Brush foreColorBrush = new sd.SolidBrush(cellStyle.ForeColor);

				sd.Rectangle bounds = new sd.Rectangle(cellBounds.X + 1, cellBounds.Y, cellBounds.Width - 2, cellBounds.Height);
				sd.Rectangle paintRect = new sd.Rectangle(bounds.X + 1, bounds.Y + 1, bounds.Width - 2, bounds.Height - 3);

				if (paintParts == swf.DataGridViewPaintParts.Border)
					this.PaintBorder(graphics, clipBounds, bounds, cellStyle, advancedBorderStyle);

				sd.Color bkColor;
				bool isSelected = elementState == swf.DataGridViewElementStates.Selected;
				if (isSelected)
					bkColor = cellStyle.SelectionBackColor;
				else
					bkColor = cellStyle.BackColor;

				if (paintParts == swf.DataGridViewPaintParts.Background)
					using (sd.SolidBrush backBrush = new sd.SolidBrush(bkColor))
						graphics.FillRectangle(backBrush, paintRect);

				paintRect.Offset(cellStyle.Padding.Right, cellStyle.Padding.Top);
				paintRect.Width -= cellStyle.Padding.Horizontal;
				paintRect.Height -= cellStyle.Padding.Vertical;

				if (swf.ProgressBarRenderer.IsSupported)
				{
					swf.ProgressBarRenderer.DrawHorizontalBar(graphics, paintRect);

					sd.Rectangle barBounds = new sd.Rectangle( paintRect.X + 2, paintRect.Y + 2, paintRect.Width - 4, paintRect.Height - 4); 
					barBounds.Width = (int)Math.Round(barBounds.Width * percentage);
					swf.ProgressBarRenderer.DrawHorizontalChunks(graphics, barBounds);
				}
				else
				{
					graphics.FillRectangle(sd.Brushes.LightGray, paintRect);
					graphics.DrawRectangle(sd.Pens.Black, paintRect);
					sd.Rectangle barBounds = new sd.Rectangle(paintRect.X + 2, paintRect.Y + 2, paintRect.Width - 4, paintRect.Height - 3);
					barBounds.Width = (int)Math.Round(barBounds.Width * percentage);
					graphics.FillRectangle(new sd.SolidBrush(sd.Color.FromArgb(0, 216, 35)), barBounds);
				}

				swf.TextFormatFlags flags = swf.TextFormatFlags.HorizontalCenter | swf.TextFormatFlags.VerticalCenter;
				sd.Color fColor = cellStyle.ForeColor;
				paintRect.Inflate(-2, -2);
				swf.TextRenderer.DrawText(graphics, progress, cellStyle.Font, paintRect, fColor, flags);
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

		public ProgressCellHandler()
		{
			Control = new EtoCell { Handler = this };
		}

		public override void SetCellValue(object dataItem, object value)
		{
			if (Widget.Binding != null)
				Widget.Binding.SetValue(dataItem, value is int ? (int)value : 0);
		}

		public override object GetCellValue(object dataItem)
		{
			if (Widget.Binding != null)
				return Widget.Binding.GetValue(dataItem);

			return 0;
		}
	}
}
