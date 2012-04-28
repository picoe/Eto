using System;
using System.Linq;
using swf = System.Windows.Forms;
using sd = System.Drawing;
using Eto.Forms;
using System.Collections.Generic;

namespace Eto.Platform.Windows.Forms.Controls
{
	public class ComboBoxCellHandler : CellHandler<swf.DataGridViewComboBoxCell, ComboBoxCell>, IComboBoxCell
	{
		CollectionHandler collection;

		class EtoCell : swf.DataGridViewComboBoxCell
		{
			public ComboBoxCellHandler Handler { get; set; }

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


		public ComboBoxCellHandler ()
		{
			Control = new EtoCell {
				Handler = this,
				ValueMember = "Key",
				DisplayMember = "Text"
			};
		}
		
		class CollectionHandler : DataStoreChangedHandler<IListItem, IListStore>
		{
			public ComboBoxCellHandler Handler { get; set; }
			
			public override int IndexOf (IListItem item)
			{
				return Handler.Control.Items.IndexOf (item);
			}
			
			public override void AddRange (IEnumerable<IListItem> items)
			{
				Handler.Control.Items.AddRange (items.ToArray ());
			}
			
			public override void AddItem (IListItem item)
			{
				Handler.Control.Items.Add (item);
			}

			public override void InsertItem (int index, IListItem item)
			{
				Handler.Control.Items.Insert (index, item);
			}

			public override void RemoveItem (int index)
			{
				Handler.Control.Items.RemoveAt (index);
			}

			public override void RemoveAllItems ()
			{
				Handler.Control.Items.Clear ();
			}
		}

		public IListStore DataStore {
			get { return collection != null ? collection.DataStore : null; }
			set {
				if (collection != null)
					collection.Unregister ();
				collection = new CollectionHandler { Handler = this };
				collection.Register (value); 
			}
		}

		public override object GetCellValue (object dataItem)
		{
			if (Widget.Binding != null) {
				return Widget.Binding.GetValue (dataItem);
			}
			return null;
		}

		public override void SetCellValue (object dataItem, object value)
		{
			if (Widget.Binding != null) {
				Widget.Binding.SetValue (dataItem, value);
			}
		}
	}
}

