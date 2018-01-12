using swf = System.Windows.Forms;
using sd = System.Drawing;
using Eto.Forms;
using System;

namespace Eto.WinForms.Forms.Cells
{
	interface ITextCellHandler
	{
		AutoSelectMode AutoSelectMode { get; }
	}

	class EtoDataGridViewTextBoxEditingControl : swf.DataGridViewTextBoxEditingControl
	{
		public ITextCellHandler Handler { get; set; }
		public bool IsMouseDown { get; set; }
		public override void PrepareEditingControlForEdit(bool selectAll)
		{
			var handler = Handler;
			if (handler != null)
			{
				if (handler.AutoSelectMode == AutoSelectMode.Never)
				{
					base.PrepareEditingControlForEdit(false);
					if (!IsMouseDown)
					{
						// no selection, place cursor at end
						SelectionStart = TextLength;
						SelectionLength = 0;
					}
					else
						PlaceCursorAtMousePosition();
				}
				else if (handler.AutoSelectMode == AutoSelectMode.OnFocus)
				{
					if (!IsMouseDown)
						base.PrepareEditingControlForEdit(true);
					else
					{
						base.PrepareEditingControlForEdit(false);
						PlaceCursorAtMousePosition();
					}
				}
			}
			else
				base.PrepareEditingControlForEdit(selectAll);
		}

		void PlaceCursorAtMousePosition()
		{
			// place text cursor where the user clicked
			var clientPt = PointToClient(MousePosition);
			var idx = GetCharIndexFromPosition(clientPt);
			if (idx >= 0)
			{
				SelectionStart = idx;
				SelectionLength = 0;
			}
		}
	}

	public class TextBoxCellHandler : CellHandler<swf.DataGridViewTextBoxCell, TextBoxCell, TextBoxCell.ICallback>, TextBoxCell.IHandler, ITextCellHandler
	{
		class EtoCell : swf.DataGridViewTextBoxCell
		{
			bool wasClicked;
			public TextBoxCellHandler Handler { get; set; }

			public override Type EditType => typeof(EtoDataGridViewTextBoxEditingControl);

			public override void PositionEditingControl(bool setLocation, bool setSize, sd.Rectangle cellBounds, sd.Rectangle cellClip, swf.DataGridViewCellStyle cellStyle, bool singleVerticalBorderAdded, bool singleHorizontalBorderAdded, bool isFirstDisplayedColumn, bool isFirstDisplayedRow)
			{
				Handler.PositionEditingControl(RowIndex, ref cellClip, ref cellBounds);
				base.PositionEditingControl(setLocation, setSize, cellBounds, cellClip, cellStyle, singleVerticalBorderAdded, singleHorizontalBorderAdded, isFirstDisplayedColumn, isFirstDisplayedRow);
			}

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

			protected override sd.Size GetPreferredSize(sd.Graphics graphics, swf.DataGridViewCellStyle cellStyle, int rowIndex, sd.Size constraintSize)
			{
				var size = base.GetPreferredSize(graphics, cellStyle, rowIndex, constraintSize);
				size.Width += Handler.GetRowOffset(rowIndex);
				return size;
			}

			protected override void Paint(System.Drawing.Graphics graphics, System.Drawing.Rectangle clipBounds, System.Drawing.Rectangle cellBounds, int rowIndex, swf.DataGridViewElementStates cellState, object value, object formattedValue, string errorText, swf.DataGridViewCellStyle cellStyle, swf.DataGridViewAdvancedBorderStyle advancedBorderStyle, swf.DataGridViewPaintParts paintParts)
			{
				Handler.Paint(graphics, clipBounds, ref cellBounds, rowIndex, cellState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, ref paintParts);
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
				return val;
			}
		}


		public TextBoxCellHandler()
		{
			Control = new EtoCell { Handler = this };
		}

		public override void SetCellValue(object dataItem, object value)
		{
			if (Widget.Binding != null)
			{
				Widget.Binding.SetValue(dataItem, Convert.ToString(value));
			}
		}

		public override object GetCellValue(object dataItem)
		{
			return Widget.Binding == null ? null : Widget.Binding.GetValue(dataItem);
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