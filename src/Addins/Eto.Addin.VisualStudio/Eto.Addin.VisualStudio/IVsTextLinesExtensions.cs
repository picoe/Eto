using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TextManager.Interop;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eto.Addin.VisualStudio
{
	public static class IVsTextLinesExtensions
	{
		public static string GetText(this IVsTextLines buffer)
		{
			int lastLine;
			int lastIndex;
			buffer.GetLastLineIndex(out lastLine, out lastIndex);

			string text;
			var ret = buffer.GetLineText(0, 0, lastLine, lastIndex, out text);
			return text;
		}
	}

	public static class CommandExtensions
	{
		public static OleMenuCommand CreateCommand(Guid menuGroup, int cmdID, Action commandEvent, Action<OleMenuCommand> queryEvent = null)
		{
			// Create the OleMenuCommand from the menu group, command ID, and command event
			var menuCommandID = new CommandID(menuGroup, cmdID);
			var command = new OleMenuCommand((sender, e) => commandEvent(), menuCommandID);

			if (queryEvent == null)
				queryEvent = cmd =>
				{
					cmd.Enabled = true;
					cmd.Visible = true;
				};

			// Add an event handler to BeforeQueryStatus if one was passed in
			if (queryEvent != null)
			{
				command.BeforeQueryStatus += (sender, e) => queryEvent((OleMenuCommand)sender);
			}
			return command;
		}

		public static void AddCommand(this IMenuCommandService mcs, Guid menuGroup, int cmdID, Action commandEvent, Action<OleMenuCommand> queryEvent = null)
		{
			var cmd = mcs.FindCommand(new CommandID(menuGroup, cmdID));
			if (cmd != null)
				mcs.RemoveCommand(cmd);
			var command = CreateCommand(menuGroup, cmdID, commandEvent, queryEvent);
			// Add the command using our IMenuCommandService instance
			mcs.AddCommand(command);
		}

	}
}
