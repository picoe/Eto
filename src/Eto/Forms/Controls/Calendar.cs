using System;

namespace Eto.Forms;

/// <summary>
/// Possible modes for the <see cref="Calendar"/> control.
/// </summary>
public enum CalendarMode
{
	/// <summary>
	/// Calendar allows only a single date to be selected.
	/// </summary>
	Single,

	/// <summary>
	/// Can select a range of dates. In some platforms, two calendars will be shown to select the start and end dates of the range.
	/// </summary>
	Range
}

/// <summary>
/// Control to show a Calendar, where the user can select either a single date or a range of dates.
/// </summary>
/// <example>
/// Here is an example for creating a single date calendar and a calendar with a range of dates.
/// <code>
/// var singleDateCalendar = new Calendar();
/// var rangeDateCalendar = new Calendar() { Mode = CalendarMode.Range };
/// </code>
/// </example>
[Handler(typeof(IHandler))]
public class Calendar : Control
{
	new IHandler Handler { get { return (IHandler)base.Handler; } }

	#region Events

	/// <summary>
	/// Event identifier for handlers when attaching the <see cref="SelectedDateChanged"/> event.
	/// </summary>
	public const string SelectedDateChangedEvent = "Calendar.SelectedDateChanged";

	/// <summary>
	/// Occurs when the <see cref="SelectedDate"/> is changed.
	/// </summary>
	public event EventHandler<EventArgs> SelectedDateChanged
	{
		add { Properties.AddHandlerEvent(SelectedDateChangedEvent, value); }
		remove { Properties.RemoveEvent(SelectedDateChangedEvent, value); }
	}

	/// <summary>
	/// Raises the <see cref="SelectedDateChanged"/> event.
	/// </summary>
	/// <param name="e">Event arguments.</param>
	protected virtual void OnSelectedDateChanged(EventArgs e)
	{
		Properties.TriggerEvent(SelectedDateChangedEvent, this, e);
	}

	/// <summary>
	/// Event identifier for handlers when attaching the <see cref="SelectedRangeChanged"/> event.
	/// </summary>
	public const string SelectedRangeChangedEvent = "Calendar.SelectedRangeChanged";

	/// <summary>
	/// Occurs when the <see cref="SelectedRange"/> is changed.
	/// </summary>
	public event EventHandler<EventArgs> SelectedRangeChanged
	{
		add { Properties.AddHandlerEvent(SelectedRangeChangedEvent, value); }
		remove { Properties.RemoveEvent(SelectedRangeChangedEvent, value); }
	}

	/// <summary>
	/// Raises the <see cref="SelectedRangeChanged"/> event.
	/// </summary>
	/// <param name="e">Event arguments.</param>
	protected virtual void OnSelectedRangeChanged(EventArgs e)
	{
		Properties.TriggerEvent(SelectedRangeChangedEvent, this, e);
	}

	#endregion

	/// <summary>
	/// Gets or sets the minimum date entered.
	/// </summary>
	/// <value>The minimum date.</value>
	public DateTime MinDate
	{
		get { return Handler.MinDate; }
		set { Handler.MinDate = value; }
	}

	/// <summary>
	/// Gets or sets the maximum date entered.
	/// </summary>
	/// <value>The maximum date.</value>
	public DateTime MaxDate
	{
		get { return Handler.MaxDate; }
		set { Handler.MaxDate = value; }
	}

	/// <summary>
	/// Gets or sets the selected date.
	/// </summary>
	/// <remarks>If <see cref="Mode"/> is <see cref="CalendarMode.Range"/>, then this will be most recent date the user selected.
	/// To get the selected range, use <see cref="SelectedRange"/> instead.</remarks>
	/// <value>The selected date.</value>
	public DateTime SelectedDate
	{
		get { return Handler.SelectedDate; }
		set { Handler.SelectedDate = value; }
	}

	/// <summary>
	/// Gets or sets the selected range.
	/// </summary>
	/// <remarks>
	/// The <see cref="SelectedRange"/> will have the same start/end dates when <see cref="Mode"/> is <see cref="CalendarMode.Single"/>.
	/// </remarks>
	/// <value>The selected range.</value>
	public Range<DateTime> SelectedRange
	{
		get { return Handler.SelectedRange; }
		set { Handler.SelectedRange = value; }
	}

	/// <summary>
	/// Gets or sets the mode of the <see cref="Calendar"/>.
	/// </summary>
	/// <value>The mode of the <see cref="Calendar"/>.</value>
	public CalendarMode Mode
	{
		get { return Handler.Mode; }
		set { Handler.Mode = value; }
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="Eto.Forms.Calendar"/> class.
	/// </summary>
	public Calendar()
	{
	}

	/// <inheritdoc cref="Calendar()"/>
	/// <param name="handler">Handler implementation to wrap.</param>
	protected Calendar(IHandler handler)
		: base(handler)
	{
	}

	static readonly object callback = new Callback();

	/// <inheritdoc/>
	protected override object GetCallback()
	{
		return callback;
	}

	/// <summary>
	/// Callback interface for handlers of the <see cref="Calendar"/>.
	/// </summary>
	public new interface ICallback : Control.ICallback
	{
		/// <summary>
		/// Raises the <see cref="Calendar.SelectedDateChanged"/> event.
		/// </summary>
		// TODO: undocumented params
		void OnSelectedDateChanged(Calendar widget, EventArgs e);

		/// <summary>
		/// Raises the <see cref="Calendar.SelectedRangeChanged"/> event.
		/// </summary>
		// TODO: undocumented params
		void OnSelectedRangeChanged(Calendar widget, EventArgs e);
	}

	/// <summary>
	/// Callback implementation for handlers of the <see cref="Calendar"/>.
	/// </summary>
	protected new class Callback : Control.Callback, ICallback
	{
		/// <inheritdoc cref="ICallback.OnSelectedDateChanged"/>
		public void OnSelectedDateChanged(Calendar widget, EventArgs e)
		{
			using (widget.Platform.Context)
				widget.OnSelectedDateChanged(e);
		}

		/// <inheritdoc cref="ICallback.OnSelectedRangeChanged"/>
		public void OnSelectedRangeChanged(Calendar widget, EventArgs e)
		{
			using (widget.Platform.Context)
				widget.OnSelectedRangeChanged(e);
		}
	}

	/// <summary>
	/// Handler interface for the <see cref="Calendar"/>.
	/// </summary>
	public new interface IHandler : Control.IHandler
	{
		/// <inheritdoc cref="Calendar.MinDate"/>
		DateTime MinDate { get; set; }

		/// <inheritdoc cref="Calendar.MaxDate"/>
		DateTime MaxDate { get; set; }

		/// <inheritdoc cref="Calendar.SelectedRange"/>
		Range<DateTime> SelectedRange { get; set; }

		/// <inheritdoc cref="Calendar.SelectedDate"/>
		DateTime SelectedDate { get; set; }

		/// <inheritdoc cref="Calendar.Mode"/>
		CalendarMode Mode { get; set; }
	}
}