using System.Collections;
using System.Collections.Generic;
using System;
using System.Globalization;

namespace Eto.Forms
{
	/// <summary>
	/// Interface to provide a source for the <see cref="ColumnBinding{T}"/>
	/// </summary>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public interface IColumnItem
	{
		/// <summary>
		/// Gets the value from this item for the specified column/index
		/// </summary>
		/// <param name="column">column/index to get the value</param>
		/// <returns>value of the object with the specified column/index</returns>
		object GetValue(int column);

		/// <summary>
		/// Sets the value of this object for the specified column/index
		/// </summary>
		/// <param name="column">column/index to set the value</param>
		/// <param name="value">value to set at the specified column/index</param>
		void SetValue(int column, object value);
	}

	/// <summary>
	/// Column/Index binding for objects implementing <see cref="IColumnItem"/> or <see cref="IList"/>
	/// </summary>
	/// <remarks>
	/// This binding is an indirect binding on a particular column/index of each object.
	/// This is used to get/set values of a passed-in object to the <see cref="IndirectBinding{T}.GetValue"/> and
	/// <see cref="IndirectBinding{T}.SetValue"/>.
	/// </remarks>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class ColumnBinding<T> : IndirectBinding<T>
	{
		/// <summary>
		/// Gets or sets the column the binding will get/set the value
		/// </summary>
		public int Column { get; set; }

		/// <summary>
		/// Initializes a new instance of the ColumnBinding class
		/// </summary>
		public ColumnBinding()
		{
		}

		/// <summary>
		/// Initializes a new instance of the ColumnBinding class with the specified column
		/// </summary>
		/// <param name="column">column/index to get/set the value from each object</param>
		public ColumnBinding(int column)
		{
			this.Column = column;
		}

		/// <summary>
		/// Implements the logic to get the value from the specified object
		/// </summary>
		/// <param name="dataItem">object to get the value from</param>
		/// <returns>value at the <see cref="Column"/> of the specified object</returns>
		protected override T InternalGetValue(object dataItem)
		{
			var list = dataItem as IList;
			if (list != null)
				return Convert(list[Column]);

			var colitem = dataItem as IColumnItem;
			if (colitem != null)
				return Convert(colitem.GetValue(Column));

			var listitem = dataItem as IList<T>;
			if (listitem != null)
				return listitem[Column];

			return default(T);
		}

		static T Convert(object val)
		{
			var propertyType = typeof(T);
			if (val != null && !propertyType.IsInstanceOfType(val))
			{
				propertyType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;
				val = System.Convert.ChangeType(val, propertyType, CultureInfo.InvariantCulture);
			}
			return (T)val;
		}

		/// <summary>
		/// Implements the logic to set the value to the specified object
		/// </summary>
		/// <param name="dataItem">object to set the value</param>
		/// <param name="value">value to set at the <see cref="Column"/> of the specified object</param>
		protected override void InternalSetValue(object dataItem, T value)
		{
			var list = dataItem as IList;
			if (list != null)
				list[Column] = value;

			var colitem = dataItem as IColumnItem;
			if (colitem != null)
				colitem.SetValue(Column, value);

			var listitem = dataItem as IList<T>;
			if (listitem != null)
				listitem[Column] = value;
		}
	}
}
