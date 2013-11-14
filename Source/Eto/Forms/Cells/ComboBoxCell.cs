
namespace Eto.Forms
{
	public interface IComboBoxCell : ICell
	{
		IListStore DataStore { get; set; }
	}
	
	public class ComboBoxCell : SingleValueCell
	{
		new IComboBoxCell Handler { get { return (IComboBoxCell)base.Handler; } }
		
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

		public ComboBoxCell()
			: this((Generator)null)
		{
		}
		
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

