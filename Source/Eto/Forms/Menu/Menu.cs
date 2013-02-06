#if DESKTOP
using System;
using System.Collections;

namespace Eto.Forms
{
	public interface IMenu : IInstanceWidget
	{
	}

	public abstract class Menu : InstanceWidget
	{
		//IMenu inner;

		protected Menu (Generator g, Type type, bool initialize = true)
			: base (g, type, initialize)
		{
			//inner = (IMenu)base.Handler;
		}

	}
}
#endif