using System.Collections.Generic;
using System;

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

		internal void InternalCreateStandardMenu(MenuItemCollection menuItems, IEnumerable<Command> commands = null)
		{
			Handler.CreateStandardMenu(menuItems, commands ?? GetSystemCommands());
		}

		[Obsolete("Use MenuBar.CreateStandardMenu() instead")]
		public void CreateStandardMenu(MenuItemCollection menuItems, IEnumerable<Command> commands = null)
		{
			Handler.CreateStandardMenu(menuItems, commands ?? GetSystemCommands());
		}
	}
}
