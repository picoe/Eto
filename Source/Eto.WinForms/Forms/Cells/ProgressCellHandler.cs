using System;
using swf = System.Windows.Forms;
using sd = System.Drawing;
using Eto.Forms;

namespace Eto.WinForms.Forms.Cells
{
    public class ProgressCellHandler : CellHandler<ProgressCellHandler.EtoCell, ProgressCell, ProgressCell.ICallback>, ProgressCell.IHandler
    {
        public ProgressCellHandler()
        {
            Control = new EtoCell { Handler = this };
        }

        public override void SetCellValue(object dataItem, object value)
        {
            if (Widget.Binding != null)
            {
                // Get the progress value
                float? progressValue = value as float?;

                if (progressValue.HasValue)
                    progressValue = progressValue < 0f ? 0f : progressValue > 1f ? 1f : progressValue;

                Widget.Binding.SetValue(dataItem, progressValue);
            }
        }

        public override object GetCellValue(object dataItem)
        {
            if (Widget.Binding != null)
            {
                float? progress = Widget.Binding.GetValue(dataItem);
                if (progress.HasValue)
                    progress = progress < 0f ? 0f : progress > 1f ? 1f : progress;
                return progress;
            }
            return (float?)null;
        }


        // The progress cell
        public class EtoCell : swf.DataGridViewTextBoxCell
        {
            public ProgressCellHandler Handler { get; set; }

            public EtoCell()
            {
                ValueType = typeof(int);
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
                formattedValue = "";
                Handler.Paint(graphics, clipBounds, ref cellBounds, rowIndex, elementState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, ref paintParts);
                base.Paint(graphics, clipBounds, cellBounds, rowIndex, elementState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);

                float? progressVal;
                if (!(progressVal = value as float?).HasValue)
                    return;

                float percentage = (float)progressVal;
                string progress = (int)(progressVal * 100f) + "%";

                sd.Rectangle paintRect = new sd.Rectangle(cellBounds.X + 1, cellBounds.Y + 2, cellBounds.Width - 2, cellBounds.Height - 4);

                PaintBorder(graphics, clipBounds, paintRect, cellStyle, advancedBorderStyle);

                sd.Color bkColor;
                if (elementState == swf.DataGridViewElementStates.Selected)
                    bkColor = cellStyle.SelectionBackColor;
                else
                    bkColor = cellStyle.BackColor;

                using (sd.SolidBrush backBrush = new sd.SolidBrush(bkColor))
                    graphics.FillRectangle(backBrush, paintRect);

                if (swf.ProgressBarRenderer.IsSupported)
                {
                    swf.ProgressBarRenderer.DrawHorizontalBar(graphics, paintRect);

                    sd.Rectangle barBounds = new sd.Rectangle(paintRect.X + 2, paintRect.Y + 2, paintRect.Width - 4, paintRect.Height - 4);
                    barBounds.Width = (int)Math.Round(barBounds.Width * percentage);
                    swf.ProgressBarRenderer.DrawHorizontalChunks(graphics, barBounds);
                }
                else
                {
                    sd.Rectangle barBounds = new sd.Rectangle(paintRect.X + 2, paintRect.Y + 2, paintRect.Width - 4, paintRect.Height - 3);
                    barBounds.Width = (int)Math.Round(barBounds.Width * percentage);

                    graphics.FillRectangle(sd.Brushes.LightGray, paintRect);
                    graphics.DrawRectangle(sd.Pens.Black, paintRect);
                    graphics.FillRectangle(new sd.SolidBrush(sd.Color.FromArgb(0, 216, 35)), barBounds);
                }

                swf.TextRenderer.DrawText(graphics, progress, cellStyle.Font, paintRect, cellStyle.ForeColor, swf.TextFormatFlags.HorizontalCenter | swf.TextFormatFlags.VerticalCenter);
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
    }
}
