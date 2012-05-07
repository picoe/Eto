using System;

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
		
		public TextBoxCell ()
			: this(Generator.Current)
		{
		}
		
		public TextBoxCell (Generator g)
			: base(g, typeof(ITextBoxCell), true)
		{
		}
	}
}

