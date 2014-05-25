using System;


namespace Eto.Forms
{
	[Handler(typeof(IHandler))]
	public class TextBoxCell : SingleValueCell<string>
	{
		public TextBoxCell (int column)
		{
			Binding = new ColumnBinding<string> (column);
		}
		
		public TextBoxCell (string property)
		{
			Binding = new PropertyBinding<string> (property);
		}
		
		public TextBoxCell()
		{
		}

		[Obsolete("Use default constructor instead")]
		public TextBoxCell (Generator generator)
			: base(generator, typeof(IHandler), true)
		{
		}

		public new interface IHandler : SingleValueCell<string>.IHandler
		{
		}
	}
}

