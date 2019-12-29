#if !NETSTANDARD
using System;

namespace System.Windows.Input
{
}

namespace Eto.Forms
{
	/// <summary>
	/// Interface to define a command to execute.
	/// </summary>
	/// <remarks>
	/// This interface is provided only for .NET 4.0 to mimic the System.Windows.Input.ICommand provided in .NET 4.5 and PCL.
	/// It is useful for MVVM scenarios to bind object events to a command in the model.
	/// </remarks>
	public interface ICommand
	{
		/// <summary>
		/// Determines whether this command can be executed.
		/// </summary>
		/// <returns><c>true</c> if the command can be executed; otherwise, <c>false</c>.</returns>
		/// <param name="parameter">Optional data to pass to the command, or null if not required by the command.</param>
		bool CanExecute(object parameter);

		/// <summary>
		/// Executes the command
		/// </summary>
		/// <param name="parameter">Optional data to pass to the command, or null if not required by the command.</param>
		void Execute(object parameter);

		/// <summary>
		/// Event to handle when the state of the <see cref="CanExecute"/> method changes.
		/// </summary>
		event EventHandler CanExecuteChanged;
	}
}
#endif