using System;
using System.Collections.Generic;

namespace Eto.Forms
{
	/// <summary>
	/// Helper for an item in a <see cref="GridView"/> to store values in an array. 
	/// </summary>
	/// <remarks>
	/// This should only be used when you don't have your own class to represent each row.
	/// You can use the <see cref="ColumnBinding{T}"/> to bind to an indexed value in this item.
	/// </remarks>
	public class GridItem : IColumnItem
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.GridItem"/> class.
		/// </summary>
		public GridItem()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.GridItem"/> class.
		/// </summary>
		/// <param name="values">Values.</param>
		public GridItem(params object[] values)
		{
			this.Values = values;
		}

		/// <summary>
		/// Gets or sets a custom value to associate with this item.
		/// </summary>
		/// <value>The custom tag value.</value>
		public object Tag { get; set; }

		/// <summary>
		/// Gets or sets the values of the row.
		/// </summary>
		/// <value>The values of the row.</value>
		public object[] Values { get; set; }

		/// <summary>
		/// Gets the value from this item for the specified column/index
		/// </summary>
		/// <param name="column">column/index to get the value</param>
		/// <returns>value of the object with the specified column/index</returns>
		public virtual object GetValue(int column)
		{
			if (Values == null || Values.Length <= column)
				return null;
			return Values[column];
		}

		/// <summary>
		/// Sets the value of this object for the specified column/index
		/// </summary>
		/// <param name="column">column/index to set the value</param>
		/// <param name="value">value to set at the specified column/index</param>
		public virtual void SetValue(int column, object value)
		{
			if (Values == null)
			{
				Values = new object[column + 1];
			}
			else if (column >= Values.Length)
			{
				var oldvalues = Values;
				Values = new object[column + 1];
				Array.Copy(oldvalues, Values, oldvalues.Length);
			}
			Values[column] = value;
		}
	}
}