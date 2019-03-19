using System;

namespace Eto.Forms
{
	/// <summary>
	/// A <see cref="IValueCommand{T}"/> class that uses delegates for getting and setting the value of the command
	/// </summary>
	/// <remarks>
	/// This can be used to delegate the getting/setting of the value to separate methods.
	/// Call <see cref="RelayValueCommand{TParameter, TValue}.UpdateValue"/> to signal that the value has changed.
	/// </remarks>
	/// <seealso cref="ValueCommand{T}"/>
	public class RelayValueCommand<TValue> : RelayValueCommand<object, TValue>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="T:Eto.Forms.RelayValueCommand`1"/> class.
		/// </summary>
		/// <param name="getValue">Delegate to get the value.</param>
		/// <param name="setValue">Delegate to set value.</param>
		/// <param name="execute">Delegate to call when the command is executed (which usually changes the value).</param>
		/// <param name="canExecute">Delegate to call to determine if the command can be executed.</param>
		public RelayValueCommand(Func<TValue> getValue, Action<TValue> setValue, Action execute = null, Func<bool> canExecute = null)
			: base(
				obj => getValue(),
				(obj, value) => setValue(value),
				execute != null ? (Action<object>)(obj => execute()) : null,
				canExecute != null ? (Predicate<object>)(obj => canExecute()) : null
			)
		{
		}
	}

	/// <summary>
	/// A <see cref="IValueCommand{T}"/> class that uses delegates for getting and setting the value of the command with the command parameter.
	/// </summary>
	/// <remarks>
	/// This can be used to delegate the getting/setting of the value to separate methods.
	/// Call <see cref="UpdateValue"/> to signal that the value has changed.
	/// </remarks>
	/// <seealso cref="ValueCommand{T}"/>
    public class RelayValueCommand<TParameter, TValue> : RelayCommand<TParameter>, IValueCommand<TValue>
    {
        readonly Func<TParameter, TValue> _getValue;
        readonly Action<TParameter, TValue> _setValue;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Eto.Forms.RelayValueCommand`2"/> class.
		/// </summary>
		/// <param name="getValue">Delegate to get the value.</param>
		/// <param name="setValue">Delegate to set value.</param>
		/// <param name="execute">Delegate to call when the command is executed, which usually changes the value. (optional).</param>
		/// <param name="canExecute">Delegate to call to determine if the command can be executed (optional).</param>
        public RelayValueCommand(Func<TParameter, TValue> getValue, Action<TParameter, TValue> setValue, Action<TParameter> execute = null, Predicate<TParameter> canExecute = null)
            : base(execute, canExecute)
        {
			_getValue = getValue ?? throw new ArgumentNullException(nameof(getValue));
			_setValue = setValue ?? throw new ArgumentNullException(nameof(setValue));
		}

		/// <summary>
		/// Event to signal to the control/widget that the value has been changed.
		/// </summary>
		/// <remarks>
		/// Controls should call <see cref="GetValue(object)"/> when this is raised
		/// so that they can set the state of the control to match the value provided.
		/// </remarks>
		public event EventHandler<EventArgs> ValueChanged;

		/// <summary>
		/// Raises the <see cref="ValueChanged"/> event.
		/// </summary>
		/// <param name="e">Event arguments</param>
        protected virtual void OnValueChanged(EventArgs e) => ValueChanged?.Invoke(this, e);

		/// <summary>
		/// Signals that the value has been updated and the delegate to get the value should be called.
		/// </summary>
        public void UpdateValue() => OnValueChanged(EventArgs.Empty);

		/// <summary>
		/// Gets the value.
		/// </summary>
		/// <returns>The value.</returns>
		/// <param name="parameter">Command Parameter.</param>
        public TValue GetValue(object parameter)
        {
            TParameter param = parameter is TParameter tparam ? tparam : default(TParameter);
            return _getValue != null ? _getValue(param) : default(TValue);
        }

		/// <summary>
		/// Sets the value.
		/// </summary>
		/// <param name="parameter">Command Parameter.</param>
		/// <param name="value">Value to set to.</param>
        public void SetValue(object parameter, TValue value)
        {
            TParameter param = parameter is TParameter tparam ? tparam : default(TParameter);
            _setValue?.Invoke(param, value);
        }
    }
}
