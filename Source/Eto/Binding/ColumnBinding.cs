using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.ObjectModel;

namespace Eto
{
	public interface IColumnItem
	{
		object GetValue (int column);

		void SetValue (int column, object value);
	}
	
	public class ColumnBinding : SingleBinding
	{
		public int Column { get; set; }
		
		public ColumnBinding ()
		{
		}
		
		public ColumnBinding (int column)
		{
			this.Column = column;
		}
		
		protected override object InternalGetValue (object dataItem)
		{
			var colitem = dataItem as IColumnItem;
			if (colitem != null) {
				return colitem.GetValue (Column);
			}
			var listitem = dataItem as IList;
			if (listitem != null) {
				return listitem [Column];
			}
			return null;
		}
		
		protected override void InternalSetValue (object dataItem, object value)
		{
			var colitem = dataItem as IColumnItem;
			if (colitem != null) {
				colitem.SetValue (Column, value);
			}
			var listitem = dataItem as IList;
			if (listitem != null) {
				listitem [Column] = value;
			}
		}
	}
	
}
