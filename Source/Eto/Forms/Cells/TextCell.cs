using System;

namespace Eto.Forms
{
	public interface ITextCell : ICell
	{
	}
	
	public class TextCell : SingleValueCell
	{
		public TextCell (int column)
			: this()
		{
			Binding = new ColumnBinding (column);
		}
		
		public TextCell (string property)
			: this()
		{
			Binding = new PropertyBinding (property);
		}
		
		public TextCell ()
			: this(Generator.Current)
		{
		}
		
		public TextCell (Generator g)
			: base(g, typeof(ITextCell), true)
		{
		}
	}
}

