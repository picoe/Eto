using System;
using Eto.Drawing;
using System.Globalization;
using System.Windows.Input;

namespace Eto.Forms
{
	/// <summary>
	/// Command to relay execution and execute state to delegates
	/// </summary>
	public class RelayCommand : RelayCommand<object>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.RelayCommand"/> class.
		/// </summary>
		/// <remarks>
		/// The <see cref="RelayCommand{T}.CanExecute"/> will always return true.
		/// </remarks>
		/// <param name="execute">Delegate to execute the command.</param>
		public RelayCommand(Action execute)
			: base((obj) => execute())
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.RelayCommand"/> class.
		/// </summary>
		/// <remarks>
		/// This constructor allows you to specify whether the command can be executed.
		/// If the state of the <paramref name="canExecute"/> delegate changes, you can call <see cref="RelayCommand{T}.UpdateCanExecute"/>
		/// to tell the control that is bound to this command to call the delegate again.
		/// </remarks>
		/// <param name="execute">Delegate to execute the command.</param>
		/// <param name="canExecute">Delegate to determine the state of whether the command can be executed.</param>
		public RelayCommand(Action execute, Func<bool> canExecute)
			: base(
				obj => execute(), 
				canExecute != null ? (Predicate<object>)(obj => canExecute()) : null
			)
		{
		}
	}

	/// <summary>
	/// Command to relay execution and execute state to delegates
	/// </summary>
	public class RelayCommand<T> : ICommand
	{
		readonly Action<T> execute;
		readonly Predicate<T> canExecute;

		/// <summary>
		/// Occurs when the return value of <see cref="CanExecute"/> is changed.
		/// </summary>
		public event EventHandler CanExecuteChanged;

		/// <summary>
		/// Raises the <see cref="CanExecuteChanged"/> event.
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected virtual void OnCanExecuteChanged(EventArgs e)
		{
			CanExecuteChanged?.Invoke(this, e);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.RelayCommand{T}"/> class.
		/// </summary>
		/// <remarks>
		/// The <see cref="CanExecute"/> will always return true.
		/// </remarks>
		/// <param name="execute">Delegate to execute the command.</param>
		public RelayCommand(Action<T> execute)
			: this(execute, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.RelayCommand{T}"/> class.
		/// </summary>
		/// <remarks>
		/// This constructor allows you to specify whether the command can be executed.
		/// If the state of the <paramref name="canExecute"/> delegate changes, you can call <see cref="UpdateCanExecute"/>
		/// to tell the control that is bound to this command to call the delegate again.
		/// </remarks>
		/// <param name="execute">Delegate to execute the command.</param>
		/// <param name="canExecute">Delegate to determine the state of whether the command can be executed.</param>
		public RelayCommand(Action<T> execute, Predicate<T> canExecute)
		{
			this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
			this.canExecute = canExecute;
		}

		/// <summary>
		/// Determines whether this command can execute with the specified parameter.
		/// </summary>
		/// <returns><c>true</c> if this command can execute; otherwise, <c>false</c>.</returns>
		/// <param name="parameter">Command argument from the control.</param>
		public bool CanExecute(object parameter)
		{
			return canExecute == null || canExecute(parameter is T tparam ? tparam : default(T));
		}

		/// <summary>
		/// Executes the command with the specified parameter.
		/// </summary>
		/// <param name="parameter">Command argument from the control.</param>
		public void Execute(object parameter)
		{
			execute?.Invoke(parameter is T tparam ? tparam : default(T));
		}

		/// <summary>
		/// Tells consumers of this command that the <see cref="CanExecute"/> state has changed and should be queried again.
		/// </summary>
		public void UpdateCanExecute()
		{
			OnCanExecuteChanged(EventArgs.Empty);
		}
	}
}
