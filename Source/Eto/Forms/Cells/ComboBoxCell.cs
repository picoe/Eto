using System;
using System.Collections.Generic;


namespace Eto.Forms
{
	[Handler(typeof(ComboBoxCell.IHandler))]
	public class ComboBoxCell : SingleValueCell<object>
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		public IIndirectBinding<string> ComboTextBinding { get; set; }

		public IIndirectBinding<string> ComboKeyBinding { get; set; }

		public ComboBoxCell(int column)
			: this()
		{
			Binding = new ColumnBinding<object>(column);
		}

		public ComboBoxCell(string property)
			: this()
		{
			Binding = new PropertyBinding(property);
		}

		public ComboBoxCell()
		{
			ComboTextBinding = new ListItemTextBinding();
			ComboKeyBinding = new ListItemKeyBinding();
		}

		[Obsolete("Use default constructor instead")]
		public ComboBoxCell(Generator generator)
			: base(generator, typeof(IHandler), true)
		{
			ComboTextBinding = new ListItemTextBinding();
			ComboKeyBinding = new ListItemKeyBinding();
		}

		public IEnumerable<object> DataStore
		{
			get { return Handler.DataStore; }
			set { Handler.DataStore = value; }
		}

		public new interface IHandler : SingleValueCell<object>.IHandler
		{
			IEnumerable<object> DataStore { get; set; }
		}
	}
}

