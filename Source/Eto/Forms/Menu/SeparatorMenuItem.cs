#if DESKTOP
using System;

namespace Eto.Forms
{
	public interface ISeparatorMenuItem : IMenuItem
	{
	}
	
	public class SeparatorMenuItem : MenuItem
	{
		public SeparatorMenuItem()
			: this((Generator)null)
		{
		}

		public SeparatorMenuItem (Generator generator) : this (generator, typeof(ISeparatorMenuItem))
		{
		}
		
		protected SeparatorMenuItem (Generator generator, Type type, bool initialize = true)
			: base (generator, type, initialize)
		{
			
		}

	}
}
#endif