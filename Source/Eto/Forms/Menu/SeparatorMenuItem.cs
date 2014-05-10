using System;

namespace Eto.Forms
{
	[Handler(typeof(SeparatorMenuItem.IHandler))]
	public class SeparatorMenuItem : MenuItem
	{
		public SeparatorMenuItem()
		{
		}

		[Obsolete("Use default constructor instead")]
		public SeparatorMenuItem(Generator generator) : this(generator, typeof(SeparatorMenuItem.IHandler))
		{
		}

		[Obsolete("Use default constructor and HandlerAttribute instead")]
		protected SeparatorMenuItem(Generator generator, Type type, bool initialize = true)
			: base(generator, type, initialize)
		{
		}

		public new interface IHandler : MenuItem.IHandler
		{
		}
	}
}