using System;


namespace Eto.Forms
{
	public interface ITextBoxCell : ICell
	{
	}

	[Handler(typeof(ITextBoxCell))]
	public class TextBoxCell : SingleValueCell
	{
		public TextBoxCell (int column)
		{
			Binding = new ColumnBinding (column);
		}
		
		public TextBoxCell (string property)
		{
			Binding = new PropertyBinding (property);
		}
		
		public TextBoxCell()
		{
		}

		[Obsolete("Use default constructor instead")]
		public TextBoxCell (Generator generator)
			: base(generator, typeof(ITextBoxCell), true)
		{
		}
	}
}

