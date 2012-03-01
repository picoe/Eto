using System;

namespace Eto.Forms
{
	public interface IComboBoxCell : ICell
	{
		IListStore DataStore { get; set; }
	}
	
	public class ComboBoxCell : Cell
	{
		IComboBoxCell handler;
		
		public ComboBoxCell ()
			: this (Generator.Current)
		{
		}
		
		public ComboBoxCell (Generator generator)
			: base (generator, typeof(IComboBoxCell), true)
		{
			handler = (IComboBoxCell)this.Handler;
		}
		
		public IListStore DataStore
		{
			get { return handler.DataStore; }
			set { handler.DataStore = value; }
		}
	}
}

