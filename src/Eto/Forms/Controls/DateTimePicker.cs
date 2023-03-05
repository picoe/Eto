using System;
using System.ComponentModel;
using Eto.Drawing;

namespace Eto.Forms;

/// <summary>
/// Available modes for how a <see cref="DateTimePicker"/> can be displayed.
/// </summary>
[Flags]
public enum DateTimePickerMode
{
	/// <summary>
	/// Only the date component will be shown.
	/// </summary>
	Date = 1,
	/// <summary>
	/// Only the time component will be shown.
	/// </summary>
	Time = 2,
	/// <summary>
	/// Both the date and time components will be shown.
	/// </summary>
	DateTime = Date | Time
}

/// <summary>
/// A control that allows the user to select a date and/or a time.
/// </summary>
/// <example>
/// Here is an example that creates a date/time picker, where the user will only be able to choose between two dates:
/// <code>
/// var dateTimePicker = new DateTimePicker () { MinDate = DateTime.Now, MaxDate = DateTime.Now.AddYears(2) }; 
/// </code>
/// </example>
[Handler(typeof(DateTimePicker.IHandler))]
public class DateTimePicker : CommonControl
{
	new IHandler Handler { get { return (IHandler)base.Handler; } }

	/// <summary>
	/// Occurs when the <see cref="DateTimePicker.Value"/> property has changed by the user.
	/// </summary>
	public event EventHandler<EventArgs> ValueChanged;

	/// <summary>
	/// Raises the <see cref="ValueChanged"/> event.
	/// </summary>
	/// <param name="e">Event arguments.</param>
	protected virtual void OnValueChanged(EventArgs e)
	{
		if (ValueChanged != null)
			ValueChanged(this, e);
	}

	/// <summary>
	/// Gets or sets the minimum date that is allowed to be entered.
	/// </summary>
	/// <value>The minimum date.</value>
	public DateTime MinDate
	{
		get { return Handler.MinDate; }
		set { Handler.MinDate = value; }
	}

	/// <summary>
	/// Gets or sets the maximum date that is allowed to be entered.
	/// </summary>
	/// <value>The maximum date.</value>
	public DateTime MaxDate
	{
		get { return Handler.MaxDate; }
		set { Handler.MaxDate = value; }
	}

	/// <summary>
	/// Gets or sets the value of the date/time picker. <see langword="null"/> to display blank or with a unchecked checkbox.
	/// </summary>
	/// <value>The current value.</value>
	public DateTime? Value
	{
		get { return Handler.Value; }
		set { Handler.Value = value; }
	}

	/// <summary>
	/// Gets a binding to the <see cref="Value"/> property.
	/// </summary>
	/// <value>The value binding.</value>
	public BindableBinding<DateTimePicker, DateTime?> ValueBinding
	{
		get
		{
			return new BindableBinding<DateTimePicker,DateTime?>(
				this,
				r => r.Value,
				(r,val) => r.Value = val,
				(r, ev) => r.ValueChanged += ev,
				(r, ev) => r.ValueChanged -= ev
			);
		}
	}

	/// <summary>
	/// Gets or sets the mode of how the date/time picker will be displayed.
	/// </summary>
	/// <value>The picker mode.</value>
	[DefaultValue(DateTimePickerMode.Date)]
	public DateTimePickerMode Mode
	{
		get { return Handler.Mode; }
		set { Handler.Mode = value; }
	}

	/// <inheritdoc cref="TextControl.TextColor"/>
	public Color TextColor
	{
		get { return Handler.TextColor; }
		set { Handler.TextColor = value; }
	}

	/// <summary>
	/// Gets or sets a value indicating whether to show the control's border.
	/// </summary>
	/// <remarks>
	/// This is a hint to omit the border of the control and show it as plainly as possible.
	/// Typically used when you want to show the control within a cell of the <see cref="GridView"/>.
	/// </remarks>
	/// <value><see langword="true"/> to show the control border; otherwise, <see langword="false"/>.</value>
	[DefaultValue(true)]
	public bool ShowBorder
	{
		get { return Handler.ShowBorder; }
		set { Handler.ShowBorder = value; }
	}

	static readonly object callback = new Callback();
	
	/// <inheritdoc/>
	protected override object GetCallback() { return callback; }

	/// <summary>
	/// Callback interface for the <see cref="DateTimePicker"/>.
	/// </summary>
	public new interface ICallback : CommonControl.ICallback
	{
		/// <summary><inheritdoc cref="DateTimePicker.OnValueChanged"/></summary>
		// TODO: parameters
		void OnValueChanged(DateTimePicker widget, EventArgs e);
	}

	/// <summary>
	/// Callback implementation for handlers of the <see cref="DateTimePicker"/>.
	/// </summary>
	protected new class Callback : CommonControl.Callback, ICallback
	{
		/// <inheritdoc cref="ICallback.OnValueChanged"/>
		public void OnValueChanged(DateTimePicker widget, EventArgs e)
		{
			using (widget.Platform.Context)
				widget.OnValueChanged(e);
		}
	}

	#region Handler

	/// <summary>
	/// Handler interface for the <see cref="DateTimePicker"/> control
	/// </summary>
	public new interface IHandler : CommonControl.IHandler
	{
		/// <inheritdoc cref="DateTimePicker.Value"/>
		DateTime? Value { get; set; }

		/// <inheritdoc cref="DateTimePicker.MinDate"/>
		DateTime MinDate { get; set; }

		/// <inheritdoc cref="DateTimePicker.MaxDate"/>
		DateTime MaxDate { get; set; }

		/// <inheritdoc cref="DateTimePicker.Mode"/>
		DateTimePickerMode Mode { get; set; }

		/// <inheritdoc cref="DateTimePicker.TextColor"/>
		Color TextColor { get; set; }

		/// <inheritdoc cref="DateTimePicker.ShowBorder"/>
		bool ShowBorder { get; set; }
	}

	#endregion
}