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

		protected Menu(Generator g, Type type) : base(g, type)
		{
			//inner = (IMenu)base.Handler;
		}

	}
}
