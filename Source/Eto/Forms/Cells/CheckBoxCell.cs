using System;

namespace Eto.Forms
{
	public interface ICheckBoxCell : ICell
	{
	}
	
	public class CheckBoxCell : Cell
	{
		public CheckBoxCell ()
			: this(Generator.Current)
		{
		}
		
		public CheckBoxCell (Generator g)
			: base(g, typeof(ICheckBoxCell), true)
		{
		}
	}
}

