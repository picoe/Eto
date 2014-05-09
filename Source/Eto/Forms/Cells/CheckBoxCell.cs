using System;


namespace Eto.Forms
{
	[Handler(typeof(CheckBoxCell.IHandler))]
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

		public CheckBoxCell()
		{
		}

		[Obsolete("Use default constructor instead")]
		public CheckBoxCell (Generator generator)
			: base(generator, typeof(IHandler), true)
		{
		}

		public interface IHandler : SingleValueCell.IHandler
		{
		}
	}
}

