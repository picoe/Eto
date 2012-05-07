using System;
using System.Collections.Generic;

namespace Eto.Forms
{
	public interface IGridItem
	{
	}

	public class GridItemCollection : DataStoreCollection<IGridItem>, IGridStore
	{
		public GridItemCollection ()
		{
		}

		public GridItemCollection (IEnumerable<IGridItem> items)
			: base (items)
		{
		}
	}

	public class GridItem : IGridItem, IColumnItem
	{
		public GridItem()
		{
		}
		
		public GridItem(params object[] values)
		{
			this.Values = values;
		}
		
		public object[] Values
		{
			get; set;
		}
		
		public virtual object GetValue (int column)
		{
			if (Values == null || Values.Length <= column)
				return null;
			return Values[column];
		}

		public virtual void SetValue (int column, object value)
		{
			if (Values == null) {
				Values = new object[column + 1];
			}
			else if (column >= Values.Length) {
				var oldvalues = Values;
				Values = new object[column + 1];
				Array.Copy (oldvalues, Values, oldvalues.Length);
			}
			Values[column] = value;
		}
	}

}

