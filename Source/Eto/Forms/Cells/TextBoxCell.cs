
namespace Eto.Forms
{
	public interface ITextBoxCell : ICell
	{
	}
	
	public class TextBoxCell : SingleValueCell
	{
		public TextBoxCell (int column)
			: this()
		{
			Binding = new ColumnBinding (column);
		}
		
		public TextBoxCell (string property)
			: this()
		{
			Binding = new PropertyBinding (property);
		}
		
		public TextBoxCell()
			: this((Generator)null)
		{
		}

		public TextBoxCell (Generator generator)
			: base(generator, typeof(ITextBoxCell), true)
		{
		}
	}
}

