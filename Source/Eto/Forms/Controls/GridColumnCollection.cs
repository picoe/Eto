using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Eto.Forms
{
	public class GridColumnCollection : Collection<GridColumn>
	{
		internal IGridView Handler { get; set; }

		protected override void InsertItem (int index, GridColumn item)
		{
			base.InsertItem (index, item);
			Handler.InsertColumn (index, item);
		}

		protected override void RemoveItem (int index)
		{
			var item = this[index];
			base.RemoveItem (index);
			Handler.RemoveColumn (index, item);
		}

		protected override void SetItem (int index, GridColumn item)
		{
			var oldItem = this[index];
			base.SetItem (index, item);
			Handler.RemoveColumn (index, oldItem);
			Handler.InsertColumn (index, item);
		}

		protected override void ClearItems ()
		{
			base.ClearItems ();
			Handler.ClearColumns ();
		}
	}
}
