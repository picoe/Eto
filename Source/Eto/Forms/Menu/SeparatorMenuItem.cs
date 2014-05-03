using System;

namespace Eto.Forms
{
	public interface ISeparatorMenuItem : IMenuItem
	{
	}

	[Handler(typeof(ISeparatorMenuItem))]
	public class SeparatorMenuItem : MenuItem
	{
		public SeparatorMenuItem()
		{
		}

		[Obsolete("Use default constructor instead")]
		public SeparatorMenuItem(Generator generator) : this(generator, typeof(ISeparatorMenuItem))
		{
		}

		[Obsolete("Use default constructor and HandlerAttribute instead")]
		protected SeparatorMenuItem(Generator generator, Type type, bool initialize = true)
			: base(generator, type, initialize)
		{
		}
	}
}