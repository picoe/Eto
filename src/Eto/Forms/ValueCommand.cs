using System;
using System.Windows.Input;

namespace Eto.Forms
{
	/// <summary>
	/// Interface for a command that can provide a value
	/// </summary>
	/// <remarks>
	/// Some controls can provide a value, which can be difficult to get via the command
	/// when the value changes.
	/// This inteface allows you to extend your command to allow getting and setting a value of any
	/// type and have an event when the value changes programatically.
	/// </remarks>
	/// <seealso cref="ValueCommand{T}"/>
	public interface IValueCommand<T> : ICommand
	{
		/// <summary>
		/// Gets the current value in the command.
		/// </summary>
		/// <remarks>
		/// This is typically called by the control when the <see cref="ValueChanged"/> event is raised.
		/// </remarks>
		/// <returns>The value.</returns>
		/// <param name="parameter">CommandParameter from the control/widget.</param>
		T GetValue(object parameter);

		/// <summary>
		/// Sets the value in the command from the control.
		/// </summary>
		/// <remarks>
		/// This is typically invoked when the control has updated its value.
		/// </remarks>
		/// <param name="parameter">CommandParameter from the control/widget.</param>
		/// <param name="value">Value to set to.</param>
		void SetValue(object parameter, T value);

		/// <summary>
		/// Event to signal to the control/widget that the value has been changed.
		/// </summary>
		/// <remarks>
		/// Controls should call <see cref="GetValue(object)"/> when this is raised
		/// so that they can set the state of the control to match the value provided.
		/// </remarks>
		event EventHandler<EventArgs> ValueChanged;
	}

	/// <summary>
	/// Command that provides a specific value.
	/// </summary>
	public class ValueCommand<T> : Command, IValueCommand<T>
	{
		T _value;

		/// <summary>
		/// Gets or sets the value for the command.
		/// </summary>
		/// <value>The value.</value>
		public T Value
		{
			get => _value;
			set
			{
				if (!Equals(_value, value))
				{
					_value = value;
					OnValueChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Event for when the <see cref="Value"/> is changed.
		/// </summary>
		public event EventHandler<EventArgs> ValueChanged;

		/// <summary>
		/// Raises the <see cref="ValueChanged"/> event.
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected virtual void OnValueChanged(EventArgs e) => ValueChanged?.Invoke(this, e);

		T IValueCommand<T>.GetValue(object parameter) => _value;

		void IValueCommand<T>.SetValue(object parameter, T value) => Value = value;
	}
}
