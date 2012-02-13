using System;

namespace Eto.Forms
{
	public interface ITextCell : ICell
	{
	}
	
	public class TextCell : Cell
	{
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

