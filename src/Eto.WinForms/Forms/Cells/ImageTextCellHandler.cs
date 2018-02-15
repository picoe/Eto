using System;
using swf = System.Windows.Forms;
using sd = System.Drawing;
using Eto.Forms;
using Eto.Drawing;
using Eto.WinForms.Drawing;

namespace Eto.WinForms.Forms.Cells
{
	public class ImageTextCellHandler : CellHandler<ImageTextCellHandler.EtoCell, ImageTextCell, ImageTextCell.ICallback>, ImageTextCell.IHandler, ITextCellHandler
	{
		public static int IconSize = 16;
		public static int IconPadding = 2;

		public class EtoCell : swf.DataGridViewTextBoxCell
		{
			bool wasClicked;
			public ImageTextCellHandler Handler { get; set; }

			public override Type EditType => typeof(EtoDataGridViewTextBoxEditingControl);

			public sd.Drawing2D.InterpolationMode InterpolationMode { get; set; }

			EtoDataGridViewTextBoxEditingControl EditingTextBox => DataGridView?.EditingControl as EtoDataGridViewTextBoxEditingControl;

			public override void DetachEditingControl()
			{
				base.DetachEditingControl();
				var editingControl = EditingTextBox;
				if (editingControl != null)
				{
					editingControl.Handler = null;
					editingControl.IsMouseDown = false;
				}
				wasClicked = false;
			}

			protected override void OnClick(swf.DataGridViewCellEventArgs e)
			{
				base.OnClick(e);
				wasClicked = true;
			}

			protected override void OnMouseUp(swf.DataGridViewCellMouseEventArgs e)
			{
				base.OnMouseUp(e);
				wasClicked = false;
			}

			public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, swf.DataGridViewCellStyle dataGridViewCellStyle)
			{
				base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);

				var editingControl = EditingTextBox;
				if (editingControl != null)
				{
					editingControl.Handler = Handler;
					editingControl.IsMouseDown = wasClicked;
				}
			}

			public EtoCell()
			{
				InterpolationMode = sd.Drawing2D.InterpolationMode.HighQualityBicubic;
			}

			public override void PositionEditingControl(bool setLocation, bool setSize, sd.Rectangle cellBounds, sd.Rectangle cellClip, swf.DataGridViewCellStyle cellStyle, bool singleVerticalBorderAdded, bool singleHorizontalBorderAdded, bool isFirstDisplayedColumn, bool isFirstDisplayedRow)
			{
				Handler.PositionEditingControl(RowIndex, ref cellClip, ref cellBounds, IconSize + IconPadding * 2);
				base.PositionEditingControl(setLocation, setSize, cellBounds, cellClip, cellStyle, singleVerticalBorderAdded, singleHorizontalBorderAdded, isFirstDisplayedColumn, isFirstDisplayedRow);
			}

			protected override object GetFormattedValue(object value, int rowIndex, ref swf.DataGridViewCellStyle cellStyle, System.ComponentModel.TypeConverter valueTypeConverter, System.ComponentModel.TypeConverter formattedValueTypeConverter, swf.DataGridViewDataErrorContexts context)
			{
				var val = value as object[];
				return base.GetFormattedValue((val != null) ? val[1] : null, rowIndex, ref cellStyle, valueTypeConverter, formattedValueTypeConverter, context);
			}

			protected override sd.Size GetPreferredSize(sd.Graphics graphics, swf.DataGridViewCellStyle cellStyle, int rowIndex, sd.Size constraintSize)
			{
				var size = base.GetPreferredSize(graphics, cellStyle, rowIndex, constraintSize);
				var val = GetValue(rowIndex) as object[];
				var img = val[0] as sd.Image;
				if (img != null) size.Width += IconSize + IconPadding * 2;
				size.Width += Handler.GetRowOffset(rowIndex);
				return size;
			}

			protected override void Paint(sd.Graphics graphics, sd.Rectangle clipBounds, sd.Rectangle cellBounds, int rowIndex, swf.DataGridViewElementStates cellState, object value, object formattedValue, string errorText, swf.DataGridViewCellStyle cellStyle, swf.DataGridViewAdvancedBorderStyle advancedBorderStyle, swf.DataGridViewPaintParts paintParts)
			{
				Handler.Paint(graphics, clipBounds, ref cellBounds, rowIndex, cellState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, ref paintParts);

				var val = value as object[];
				var img = val[0] as sd.Image;
				if (img != null)
				{
					if (paintParts.HasFlag(swf.DataGridViewPaintParts.Background))
						using (var b = new sd.SolidBrush(cellState.HasFlag(swf.DataGridViewElementStates.Selected) ? cellStyle.SelectionBackColor : cellStyle.BackColor))
						{
							graphics.FillRectangle(b, new sd.Rectangle(cellBounds.X, cellBounds.Y, IconSize + IconPadding * 2, cellBounds.Height));
						}
					
					var container = graphics.BeginContainer();
					graphics.SetClip(cellBounds);
					if (paintParts.HasFlag(swf.DataGridViewPaintParts.Background))
						using (var background = new sd.SolidBrush(cellState.HasFlag(swf.DataGridViewElementStates.Selected) ? cellStyle.SelectionBackColor : cellStyle.BackColor))
							graphics.FillRectangle(background, cellBounds);
					graphics.InterpolationMode = InterpolationMode;
					graphics.DrawImage(img, new sd.Rectangle(cellBounds.X + IconPadding, cellBounds.Y + (cellBounds.Height - Math.Min(img.Height, cellBounds.Height)) / 2, IconSize, IconSize));
					graphics.EndContainer(container);
					cellBounds.X += IconSize + IconPadding * 2;
					cellBounds.Width -= IconSize + IconPadding * 2;
				}
				base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);
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
				val.InterpolationMode = InterpolationMode;
				return val;
			}
		}

		public ImageTextCellHandler()
		{
			Control = new EtoCell { Handler = this };
		}

		public override void SetCellValue(object dataItem, object value)
		{
			if (Widget.TextBinding != null)
			{
				Widget.TextBinding.SetValue(dataItem, Convert.ToString(value));
			}
		}

		public override object GetCellValue(object dataItem)
		{
			var obj = new object[2];
			if (Widget.ImageBinding != null)
			{
				var image = Widget.ImageBinding.GetValue(dataItem) as Image;
				if (image != null)
				{
					var imageHandler = image.Handler as IWindowsImageSource;
					if (imageHandler != null)
					{
						obj[0] = imageHandler.GetImageWithSize(Math.Max(32, Control.PreferredSize.Height));
					}
				}
			}
			if (Widget.TextBinding != null)
			{
				obj[1] = Convert.ToString(Widget.TextBinding.GetValue(dataItem));
			}
			return obj;
		}

		public ImageInterpolation ImageInterpolation
		{
			get { return Control.InterpolationMode.ToEto(); }
			set { Control.InterpolationMode = value.ToSD(); }
		}

		TextAlignment _textAlignment;
		public TextAlignment TextAlignment
		{
			get { return _textAlignment; }
			set
			{
				_textAlignment = value;
				SetAlignment();
			}
		}

		VerticalAlignment _verticalAlignment = VerticalAlignment.Center;
		public VerticalAlignment VerticalAlignment
		{
			get { return _verticalAlignment; }
			set
			{
				_verticalAlignment = value;
				SetAlignment();
			}
		}

		public AutoSelectMode AutoSelectMode { get; set; }

		void SetAlignment()
		{
			if (Column == null)
				return;
			Column.DefaultCellStyle.Alignment = WinConversions.ToSWF(TextAlignment, VerticalAlignment);
		}

		protected override void InitializeColumn()
		{
			base.InitializeColumn();
			SetAlignment();
		}
	}
}

