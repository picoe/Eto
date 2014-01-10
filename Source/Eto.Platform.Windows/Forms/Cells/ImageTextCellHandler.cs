using System;
using swf = System.Windows.Forms;
using sd = System.Drawing;
using Eto.Forms;
using Eto.Drawing;
using Eto.Platform.Windows.Drawing;

namespace Eto.Platform.Windows.Forms.Controls
{
	public class ImageTextCellHandler : CellHandler<swf.DataGridViewTextBoxCell, ImageTextCell>, IImageTextCell
	{
		public static int IconSize = 16;
		public static int IconPadding = 2;

		class EtoCell : swf.DataGridViewTextBoxCell
		{
			public ImageTextCellHandler Handler { get; set; }

			public override void PositionEditingControl (bool setLocation, bool setSize, sd.Rectangle cellBounds, sd.Rectangle cellClip, swf.DataGridViewCellStyle cellStyle, bool singleVerticalBorderAdded, bool singleHorizontalBorderAdded, bool isFirstDisplayedColumn, bool isFirstDisplayedRow)
			{
				Handler.PositionEditingControl (RowIndex, ref cellClip, ref cellBounds, IconSize + IconPadding * 2);
				base.PositionEditingControl (setLocation, setSize, cellBounds, cellClip, cellStyle, singleVerticalBorderAdded, singleHorizontalBorderAdded, isFirstDisplayedColumn, isFirstDisplayedRow);
			}

			protected override object GetFormattedValue (object value, int rowIndex, ref swf.DataGridViewCellStyle cellStyle, System.ComponentModel.TypeConverter valueTypeConverter, System.ComponentModel.TypeConverter formattedValueTypeConverter, swf.DataGridViewDataErrorContexts context)
			{
				var val = value as object[];
				return base.GetFormattedValue ((val != null) ? val[1] : null, rowIndex, ref cellStyle, valueTypeConverter, formattedValueTypeConverter, context);
			}

			protected override sd.Size GetPreferredSize (sd.Graphics graphics, swf.DataGridViewCellStyle cellStyle, int rowIndex, sd.Size constraintSize)
			{
				var size = base.GetPreferredSize (graphics, cellStyle, rowIndex, constraintSize);
				var val = GetValue (rowIndex) as object[];
				var img = val[0] as sd.Image;
				if (img != null) size.Width += IconSize + IconPadding * 2;
				size.Width += Handler.GetRowOffset (rowIndex);
				return size;
			}

			protected override void Paint (System.Drawing.Graphics graphics, System.Drawing.Rectangle clipBounds, System.Drawing.Rectangle cellBounds, int rowIndex, swf.DataGridViewElementStates cellState, object value, object formattedValue, string errorText, swf.DataGridViewCellStyle cellStyle, swf.DataGridViewAdvancedBorderStyle advancedBorderStyle, swf.DataGridViewPaintParts paintParts)
			{
				Handler.Paint (graphics, clipBounds, ref cellBounds, rowIndex, cellState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, ref paintParts);

				var val = value as object[];
				var img = val[0] as sd.Image;
				if (img != null) {
					var container = graphics.BeginContainer ();
					graphics.SetClip (cellBounds);

					graphics.DrawImage (img, new sd.Rectangle (cellBounds.X + IconPadding, cellBounds.Y + (cellBounds.Height - Math.Min(img.Height, cellBounds.Height)) / 2, IconSize, IconSize));
					graphics.EndContainer (container);
					cellBounds.X += IconSize + IconPadding * 2;
					cellBounds.Width -= IconSize + IconPadding * 2;
				}
				base.Paint (graphics, clipBounds, cellBounds, rowIndex, cellState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);
			}

			protected override void OnMouseClick (swf.DataGridViewCellMouseEventArgs e)
			{
				if (!Handler.MouseClick (e, e.RowIndex))
					base.OnMouseClick (e);
			}

			public override object Clone ()
			{
				var val = (EtoCell)base.Clone ();
				val.Handler = Handler;
				return val;
			}
		}

		public ImageTextCellHandler ()
		{
			Control = new EtoCell { Handler = this };
		}

		public override void SetCellValue (object dataItem, object value)
		{
			if (Widget.TextBinding != null) {
				Widget.TextBinding.SetValue (dataItem, value);
			}
		}

		public override object GetCellValue (object dataItem)
		{
			var obj = new object[2];
			if (Widget.ImageBinding != null) {
				var image = Widget.ImageBinding.GetValue (dataItem) as Image;
				if (image != null) {
					var imageHandler = image.Handler as IWindowsImageSource;
					if (imageHandler != null) {
						obj[0] = imageHandler.GetImageWithSize (Math.Max (32, Control.PreferredSize.Height));
					}
				}
			}
			if (Widget.TextBinding != null) {
				obj[1] = Convert.ToString (Widget.TextBinding.GetValue (dataItem));
			}
			return obj;
		}

	}
}

