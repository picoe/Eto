using System;


namespace Eto.Forms
{
	[Handler(typeof(CheckBoxCell.IHandler))]
	public class CheckBoxCell : SingleValueCell<bool?>
	{
		public CheckBoxCell (int column)
			: this()
		{
			Binding = new ColumnBinding<bool?> (column);
		}
		
		public CheckBoxCell (string property)
			: this()
		{
			Binding = new PropertyBinding<bool?> (property);
		}

		public CheckBoxCell()
		{
		}

		[Obsolete("Use default constructor instead")]
		public CheckBoxCell (Generator generator)
			: base(generator, typeof(IHandler), true)
		{
		}

		public new interface IHandler : SingleValueCell<bool?>.IHandler
		{
		}
	}
}

