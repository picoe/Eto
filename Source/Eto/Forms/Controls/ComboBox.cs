using System;

namespace Eto.Forms
{
	[Handler(typeof(ComboBox.IHandler))]
	public class ComboBox : ListControl
	{
		public ComboBox()
		{
		}

		[Obsolete("Use default constructor instead")]
		public ComboBox(Generator generator)
			: this(generator, typeof(IHandler))
		{
		}

		[Obsolete("Use default constructor and HandlerAttribute instead")]
		protected ComboBox(Generator generator, Type type, bool initialize = true)
			: base(generator, type, initialize)
		{
		}

		public interface IHandler : ListControl.IHandler
		{
		}
	}
}
