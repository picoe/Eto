using System;

namespace Eto.Forms
{
	/// <summary>
	/// Modes for the <see cref="DateTimePicker"/>
	/// </summary>
	[Flags]
	public enum DateTimePickerMode
	{
		/// <summary>
		/// Show only the date component
		/// </summary>
		Date = 1,
		/// <summary>
		/// Show only the time component
		/// </summary>
		Time = 2,
		/// <summary>
		/// Show both the date and time components
		/// </summary>
		DateTime = Date | Time
	}

	/// <summary>
	/// Handler interface for the <see cref="DateTimePicker"/> control
	/// </summary>
	public interface IDateTimePicker : ICommonControl
	{
		/// <summary>
		/// Gets or sets the value of the date/time picker
		/// </summary>
		/// <value>The current value.</value>
		DateTime? Value { get; set; }

		/// <summary>
		/// Gets or sets the minimum date entered
		/// </summary>
		/// <value>The minimum date.</value>
		DateTime MinDate { get; set; }

		/// <summary>
		/// Gets or sets the maximum date entered
		/// </summary>
		/// <value>The maximum date.</value>
		DateTime MaxDate { get; set; }

		/// <summary>
		/// Gets or sets the mode of the date/time picker.
		/// </summary>
		/// <value>The picker mode.</value>
		DateTimePickerMode Mode { get; set; }
	}

	/// <summary>
	/// Date/time picker control to enter a date and/or time value
	/// </summary>
	[Handler(typeof(IDateTimePicker))]
	public class DateTimePicker : CommonControl
	{
		new IDateTimePicker Handler { get { return (IDateTimePicker)base.Handler; } }

		public static DateTimePickerMode DefaultMode = DateTimePickerMode.Date;

		/// <summary>
		/// Occurs when the <see cref="DateTimePicker.Value"/> property has changed by the user
		/// </summary>
		public event EventHandler<EventArgs> ValueChanged;

		/// <summary>
		/// Raises the <see cref="ValueChanged"/> event.
		/// </summary>
		/// <param name="e">Event arguments</param>
		public virtual void OnValueChanged(EventArgs e)
		{
			if (ValueChanged != null)
				ValueChanged(this, e);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.DateTimePicker"/> class.
		/// </summary>
		public DateTimePicker()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.DateTimePicker"/> class.
		/// </summary>
		/// <param name="generator">Generator to create the handler</param>
		[Obsolete("Use default constructor instead")]
		public DateTimePicker(Generator generator)
			: this(generator, typeof(IDateTimePicker))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.DateTimePicker"/> class.
		/// </summary>
		/// <param name="generator">Generator to create the handler</param>
		/// <param name="type">Type of the handler interface to create, must implement <see cref="IDateTimePicker"/></param>
		/// <param name="initialize">If set to <c>true</c>, initialize after created, otherwise the subclass should call Initialize.</param>
		[Obsolete("Use default constructor and HandlerAttribute instead")]
		protected DateTimePicker(Generator generator, Type type, bool initialize = true)
			: base(generator, type, initialize)
		{
		}

		/// <summary>
		/// Gets or sets the minimum date entered
		/// </summary>
		/// <value>The minimum date.</value>
		public DateTime MinDate
		{
			get { return Handler.MinDate; }
			set { Handler.MinDate = value; }
		}

		/// <summary>
		/// Gets or sets the maximum date entered
		/// </summary>
		/// <value>The maximum date.</value>
		public DateTime MaxDate
		{
			get { return Handler.MaxDate; }
			set { Handler.MaxDate = value; }
		}

		/// <summary>
		/// Gets or sets the value of the date/time picker. Null to display blank or with a unchecked checkbox.
		/// </summary>
		/// <value>The current value.</value>
		public DateTime? Value
		{
			get { return Handler.Value; }
			set { Handler.Value = value; }
		}

		/// <summary>
		/// Gets or sets the mode of the date/time picker.
		/// </summary>
		/// <value>The picker mode.</value>
		public DateTimePickerMode Mode
		{
			get { return Handler.Mode; }
			set { Handler.Mode = value; }
		}
	}
}

