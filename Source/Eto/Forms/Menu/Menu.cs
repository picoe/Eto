using System;

namespace Eto.Forms
{
	public abstract class Menu : Widget
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

		public interface ISubmenuHandler
		{
			void AddMenu (int index, MenuItem item);

			void RemoveMenu (MenuItem item);

			void Clear ();
		}
	}
}