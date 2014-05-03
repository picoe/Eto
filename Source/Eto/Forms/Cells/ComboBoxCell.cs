using System;


namespace Eto.Forms
{
	public interface IComboBoxCell : ICell
	{
		IListStore DataStore { get; set; }
	}

	[Handler(typeof(IComboBoxCell))]
	public class ComboBoxCell : SingleValueCell
	{
		new IComboBoxCell Handler { get { return (IComboBoxCell)base.Handler; } }
		
		public ComboBoxCell (int column)
		{
			Binding = new ColumnBinding (column);
		}
		
		public ComboBoxCell (string property)
		{
			Binding = new PropertyBinding (property);
		}

		public ComboBoxCell()
		{
		}

		[Obsolete("Use default constructor instead")]
		public ComboBoxCell (Generator generator)
			: base (generator, typeof(IComboBoxCell), true)
		{
		}
		
		public IListStore DataStore {
			get { return Handler.DataStore; }
			set { Handler.DataStore = value; }
		}
	}
}

