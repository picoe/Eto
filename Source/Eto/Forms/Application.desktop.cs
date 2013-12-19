#if DESKTOP
using System.Collections.Generic;

namespace Eto.Forms
{
	public partial interface IApplication
	{
		void Restart();

		void RunIteration();

		void CreateStandardMenu(MenuItemCollection menuItems, IEnumerable<Command> commands);
	}

	public partial class Application
	{
		public void RunIteration()
		{
			Handler.RunIteration();
		}

		public void Restart()
		{
			Handler.Restart();
		}

		public void CreateStandardMenu(MenuItemCollection menuItems, IEnumerable<Command> commands = null)
		{
			Handler.CreateStandardMenu(menuItems, commands ?? GetSystemCommands());
		}
	}
}
#endif