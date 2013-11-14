using System;

namespace Eto.Forms
{
	public interface IComboBox : IListControl
	{
	}

	public class ComboBox : ListControl
	{
		public ComboBox()
			: this((Generator)null)
		{
		}

		public ComboBox(Generator generator)
			: this(generator, typeof(IComboBox))
		{
		}

		protected ComboBox(Generator generator, Type type, bool initialize = true)
			: base(generator, type, initialize)
		{
		}
	}

}
