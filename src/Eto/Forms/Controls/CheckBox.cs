using System;

namespace Eto.Forms;

/// <summary>
/// A control that shows a two or three state check box.
/// </summary>
/// <remarks>
/// A two state check box is either checked (<see langword="true"/>) or unchecked (<see langword="false"/>).
/// 
/// A three state check box can also have an additional <see langword="null"/> value.
/// </remarks>
/// <example>
/// An example to create a two and a three state check box:
/// <code>
/// var twoStateCheckBox = new Checkbox() { Text = "Two state checkbox" };
/// var threeStateCheckBox = new Checkbox() { Text = "Three state checkbox", ThreeState = true };
/// </code>
/// </example>
[Handler(typeof(CheckBox.IHandler))]
public class CheckBox : TextControl
{
	new IHandler Handler { get { return (IHandler)base.Handler; } }

	/// <summary>
	/// Occurs when the <see cref="Checked"/> property is changed by the user.
	/// </summary>
	public event EventHandler<EventArgs> CheckedChanged
	{
		add { Properties.AddEvent(CheckedChangedKey, value); }
		remove { Properties.RemoveEvent(CheckedChangedKey, value); }
	}

	static readonly object CheckedChangedKey = new object();

	/// <summary>
	/// Raises the <see cref="CheckedChanged"/> event.
	/// </summary>
	/// <param name="e">Event arguments.</param>
	protected virtual void OnCheckedChanged(EventArgs e)
	{
		Properties.TriggerEvent(CheckedChangedKey, this, e);
	}

	/// <summary>
	/// Gets or sets the checked state.
	/// </summary>
	/// <remarks>
	/// When <see cref="ThreeState"/> is <see langword="true"/>, <see langword="null"/> signifies an indeterminate value.
	/// </remarks>
	/// <value>The checked value.</value>
	public virtual bool? Checked
	{
		get { return Handler.Checked; }
		set { Handler.Checked = value; }
	}

	/// <summary>
	/// Gets or sets a value indicating whether this <see cref="CheckBox"/> allows three states: <see langword="true"/>, <see langword="false"/>, or <see langword="null"/>.
	/// </summary>
	/// <value><see langword="true"/> if this <see cref="CheckBox"/> allows three state; otherwise, <see langword="false"/>.</value>
	public bool ThreeState
	{
		get { return Handler.ThreeState; }
		set { Handler.ThreeState = value; }
	}

	/// <summary>
	/// Gets a binding for the <see cref="Checked"/> property.
	/// </summary>
	/// <value>The binding for the <see cref="Checked"/> property.</value>
	public BindableBinding<CheckBox, bool?> CheckedBinding
	{
		get
		{
			return new BindableBinding<CheckBox, bool?>(
				this, 
				c => c.Checked, 
				(c, v) => c.Checked = v, 
				(c, h) => c.CheckedChanged += h, 
				(c, h) => c.CheckedChanged -= h
			);
		}
	}

	static readonly object callback = new Callback();
		
	/// <inheritdoc/>
	protected override object GetCallback() { return callback; }

	/// <summary>
	/// Callback interface for the <see cref="CheckBox"/>.
	/// </summary>
	public new interface ICallback : TextControl.ICallback
	{
		/// <summary>
		/// Raises the <see cref="CheckBox.CheckedChanged"/> event.
		/// </summary>
		// TODO: undocumented properties
		void OnCheckedChanged(CheckBox widget, EventArgs e);
	}

	/// <summary>
	/// Callback implementation for handlers of <see cref="CheckBox"/>
	/// </summary>
	protected new class Callback : TextControl.Callback, ICallback
	{
		/// <inheritdoc cref="ICallback.OnCheckedChanged"/>
		public void OnCheckedChanged(CheckBox widget, EventArgs e)
		{
			using (widget.Platform.Context)
				widget.OnCheckedChanged(e);
		}
	}

	/// <summary>
	/// Handler interface for the <see cref="CheckBox"/> control.
	/// </summary>
	public new interface IHandler : TextControl.IHandler
	{
		/// <inheritdoc cref="CheckBox.Checked"/>
		bool? Checked { get; set; }

		/// <inheritdoc cref="CheckBox.ThreeState"/>
		bool ThreeState { get; set; }
	}

}