using System;
using System.ComponentModel;
using Eto.Drawing;

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
	/// Date/time picker control to enter a date and/or time value
	/// </summary>
	[Handler(typeof(DateTimePicker.IHandler))]
	public class DateTimePicker : CommonControl
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		/// <summary>
		/// The default mode for all new date/time pickers.
		/// </summary>
		[Obsolete("Set the mode of your picker directly or use styles")]
		public static DateTimePickerMode DefaultMode = DateTimePickerMode.Date;

		/// <summary>
		/// Occurs when the <see cref="DateTimePicker.Value"/> property has changed by the user
		/// </summary>
		public event EventHandler<EventArgs> ValueChanged;

		/// <summary>
		/// Raises the <see cref="ValueChanged"/> event.
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected virtual void OnValueChanged(EventArgs e)
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
			: this(generator, typeof(IHandler))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.DateTimePicker"/> class.
		/// </summary>
		/// <param name="generator">Generator to create the handler</param>
		/// <param name="type">Type of the handler interface to create, must implement <see cref="IHandler"/></param>
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
		[DefaultValue(DateTimePickerMode.Date)]
		public DateTimePickerMode Mode
		{
			get { return Handler.Mode; }
			set { Handler.Mode = value; }
		}

		/// <summary>
		/// Gets or sets the color of the text.
		/// </summary>
		/// <remarks>
		/// By default, the text will get a color based on the user's theme. However, this is usually black.
		/// </remarks>
		/// <value>The color of the text.</value>
		public Color TextColor
		{
			get { return Handler.TextColor; }
			set { Handler.TextColor = value; }
		}

		static readonly object callback = new Callback();
		/// <summary>
		/// Gets an instance of an object used to perform callbacks to the widget from handler implementations
		/// </summary>
		/// <returns>The callback instance to use for this widget</returns>
		protected override object GetCallback() { return callback; }

		/// <summary>
		/// Callback interface for the <see cref="DateTimePicker"/>.
		/// </summary>
		public new interface ICallback : CommonControl.ICallback
		{
			/// <summary>
			/// Raises the value changed event.
			/// </summary>
			void OnValueChanged(DateTimePicker widget, EventArgs e);
		}

		/// <summary>
		/// Callback implementation for handlers of the <see cref="DateTimePicker"/>.
		/// </summary>
		protected new class Callback : CommonControl.Callback, ICallback
		{
			/// <summary>
			/// Raises the value changed event.
			/// </summary>
			public void OnValueChanged(DateTimePicker widget, EventArgs e)
			{
				widget.Platform.Invoke(() => widget.OnValueChanged(e));
			}
		}

		#region Handler

		/// <summary>
		/// Handler interface for the <see cref="DateTimePicker"/> control
		/// </summary>
		public new interface IHandler : CommonControl.IHandler
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

			/// <summary>
			/// Gets or sets the color of the text.
			/// </summary>
			/// <remarks>
			/// By default, the text will get a color based on the user's theme. However, this is usually black.
			/// </remarks>
			/// <value>The color of the text.</value>
			Color TextColor { get; set; }
		}

		#endregion
	}
}

