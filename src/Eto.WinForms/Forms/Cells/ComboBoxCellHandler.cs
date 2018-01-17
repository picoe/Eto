using System.Linq;
using swf = System.Windows.Forms;
using sd = System.Drawing;
using Eto.Forms;
using System.Collections.Generic;

namespace Eto.WinForms.Forms.Cells
{
	public class ComboBoxCellHandler : CellHandler<swf.DataGridViewComboBoxCell, ComboBoxCell, ComboBoxCell.ICallback>, ComboBoxCell.IHandler
	{
		CollectionHandler collection;

		class EtoCell : swf.DataGridViewComboBoxCell
		{
			public ComboBoxCellHandler Handler { get; set; }

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


		public ComboBoxCellHandler()
		{
			Control = new EtoCell
			{
				Handler = this,
				ValueMember = "Key",
				DisplayMember = "Key",
				FlatStyle = swf.FlatStyle.Flat
			};
		}

		struct Item
		{
			ComboBoxCellHandler handler;
			object value;
			public object Value { get { return value; } }

			public string Key
			{
				get { return handler.Widget.ComboKeyBinding.GetValue(value); }
			}

			public override string ToString()
			{
				return handler.Widget.ComboTextBinding.GetValue(value);
			}

			public Item(ComboBoxCellHandler handler, object value)
			{
				this.handler = handler;
				this.value = value;
			}
		}

		class CollectionHandler : EnumerableChangedHandler<object>
		{
			public ComboBoxCellHandler Handler { get; set; }

			public override void AddRange(IEnumerable<object> items)
			{
				Handler.Control.Items.AddRange(items.Select(r => (object)new Item(Handler, r)).ToArray());
			}

			public override void AddItem(object item)
			{
				Handler.Control.Items.Add(new Item(Handler, item));
			}

			public override void InsertItem(int index, object item)
			{
				Handler.Control.Items.Insert(index, new Item(Handler, item));
			}

			public override void RemoveItem(int index)
			{
				Handler.Control.Items.RemoveAt(index);
			}

			public override void RemoveAllItems()
			{
				Handler.Control.Items.Clear();
			}
		}

		public IEnumerable<object> DataStore
		{
			get { return collection != null ? collection.Collection : null; }
			set
			{
				if (collection != null)
					collection.Unregister();
				collection = new CollectionHandler { Handler = this };
				collection.Register(value);
			}
		}

		public override object GetCellValue(object dataItem)
		{
			return Widget.Binding == null ? null : Widget.Binding.GetValue(dataItem);
		}

		public override void SetCellValue(object dataItem, object value)
		{
			if (Widget.Binding != null)
			{
				Widget.Binding.SetValue(dataItem, value);
			}
		}
	}
}

