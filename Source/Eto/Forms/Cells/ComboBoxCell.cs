using System;

namespace Eto.Forms
{
	public interface IComboBoxCell : ICell
	{
		IListStore DataStore { get; set; }
	}
	
	public class ComboBoxCell : SingleValueCell
	{
		IComboBoxCell handler;
		
		public ComboBoxCell (int column)
			: this()
		{
			Binding = new ColumnBinding (column);
		}
		
		public ComboBoxCell (string property)
			: this()
		{
			Binding = new PropertyBinding (property);
		}
		
		public ComboBoxCell ()
			: this (Generator.Current)
		{
		}
		
		public ComboBoxCell (Generator generator)
			: base (generator, typeof(IComboBoxCell), true)
		{
			handler = (IComboBoxCell)this.Handler;
		}
		
		public IListStore DataStore {
			get { return handler.DataStore; }
			set { handler.DataStore = value; }
		}
	}
}

