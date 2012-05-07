using System;
using swf = System.Windows.Forms;
using sd = System.Drawing;
using Eto.Forms;
using Eto.Drawing;
using Eto.Platform.Windows.Drawing;

namespace Eto.Platform.Windows.Forms.Controls
{
	public class ImageViewCellHandler : CellHandler<swf.DataGridViewImageCell, ImageViewCell>, IImageViewCell
	{
		static sd.Bitmap transparent;

		class EtoCell : swf.DataGridViewImageCell
		{
			public ImageViewCellHandler Handler { get; set; }

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
				if (!Handler.MouseClick (e, RowIndex))
					base.OnMouseClick (e);
			}

			public override object Clone ()
			{
				var val = base.Clone () as EtoCell;
				val.Handler = this.Handler;
				return val;
			}
		}


		public ImageViewCellHandler ()
		{
			Control = new EtoCell { Handler = this };
			Control.ImageLayout = swf.DataGridViewImageCellLayout.Zoom;
		}

		static ImageViewCellHandler ()
		{
			transparent = new sd.Bitmap (1, 1);
			using (var g = sd.Graphics.FromImage (transparent)) {
				g.FillRectangle (sd.Brushes.Transparent, 0, 0, 1, 1);
			}
		}

		public override object GetCellValue (object dataItem)
		{
			if (Widget.Binding != null) {
				var image = Widget.Binding.GetValue (dataItem) as Image;
				if (image != null) {
					var imageHandler = image.Handler as IWindowsImage;
					if (imageHandler != null) {
						return imageHandler.GetImageWithSize (Math.Max (32, this.Control.PreferredSize.Height));
					}
				}
			}
			return transparent;
		}

		public override void SetCellValue (object dataItem, object value)
		{
			if (Widget.Binding != null) {
				Widget.Binding.SetValue (dataItem, value);
			}
		}
	}
}

