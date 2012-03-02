using System;
using System.ComponentModel;
using System.Collections;

namespace Eto.Forms
{
	public abstract class Binding
	{
		public abstract object GetValue(object dataItem);
		public abstract void SetValue(object dataItem, object value);
	}
	
	public interface IColumnItem
	{
		object GetValue (int column);

		void SetValue (int column, object value);
	}
	
	public class ColumnBinding : Binding
	{
		public int Column { get; set; }
		
		public ColumnBinding()
		{
		}
		
		public ColumnBinding(int column)
		{
			this.Column = column;
		}
		
		public override object GetValue (object dataItem)
		{
			var colitem = dataItem as IColumnItem;
			if (colitem != null) {
				return colitem.GetValue (Column);
			}
			var listitem = dataItem as IList;
			if (listitem != null) {
				return listitem[Column];
			}
			return null;
		}
		
		public override void SetValue (object dataItem, object value)
		{
			var colitem = dataItem as IColumnItem;
			if (colitem != null) {
				colitem.SetValue (Column, value);
			}
			var listitem = dataItem as IList;
			if (listitem != null) {
				listitem[Column] = value;
			}
		}
	}
	
	public class PropertyBinding : Binding
	{
		PropertyDescriptor property;
		
		public string Property { get; set; }
		
		public PropertyBinding ()
		{
		}
		
		public PropertyBinding (string property)
		{
			this.Property = property;
		}
		
		void EnsureProperty(object dataItem)
		{
			if (property == null && dataItem != null) {
				property = TypeDescriptor.GetProperties(dataItem).Find (Property, true);
			}
		}
		
		public override object GetValue (object dataItem)
		{
			EnsureProperty (dataItem);
			if (property != null) {
				return property.GetValue (dataItem);
			}
			return null;
		}
		
		public override void SetValue (object dataItem, object value)
		{
			EnsureProperty (dataItem);
			if (property != null) {
				property.SetValue (dataItem, value);
			}
		}
	}
}

