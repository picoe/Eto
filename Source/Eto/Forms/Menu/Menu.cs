using System;

namespace Eto.Forms
{
	public interface IMenu : IInstanceWidget
	{
	}

	public abstract class Menu : InstanceWidget
	{
		//IMenu inner;

		protected Menu()
		{
		}

		[Obsolete("Use default constructor and HandlerAttribute instead")]
		protected Menu (Generator g, Type type, bool initialize = true)
			: base (g, type, initialize)
		{
			//inner = (IMenu)base.Handler;
		}

		protected internal virtual void OnLoad(EventArgs e)
		{
		}

		protected internal virtual void OnUnLoad(EventArgs e)
		{
		}

	}
}