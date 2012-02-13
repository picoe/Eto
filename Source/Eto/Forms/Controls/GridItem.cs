using System;

namespace Eto.Forms
{
	public interface IGridItem
	{
		object GetValue (int column);

		void SetValue (int column, object value);
	}
	
	public class GridItem : IGridItem
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
		
		public object GetValue (int column)
		{
			if (Values == null || Values.Length <= column)
				return null;
			return Values[column];
		}

		public void SetValue (int column, object value)
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

