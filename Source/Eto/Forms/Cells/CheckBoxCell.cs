using System;

namespace Eto.Forms
{
	public interface ICheckBoxCell : ICell
	{
	}
	
	public class CheckBoxCell : SingleValueCell
	{
		public CheckBoxCell (int column)
			: this()
		{
			Binding = new ColumnBinding (column);
		}
		
		public CheckBoxCell (string property)
			: this()
		{
			Binding = new PropertyBinding (property);
		}

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

