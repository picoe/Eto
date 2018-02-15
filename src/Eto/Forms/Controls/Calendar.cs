using System;
using System.Collections;
using System.Collections.Generic;

namespace Eto.Forms
{
	/// <summary>
	/// Mode for the <see cref="Calendar"/> control
	/// </summary>
	public enum CalendarMode
	{
		/// <summary>
		/// Calendar allows only a single date to be selected
		/// </summary>
		Single,

		/// <summary>
		/// Can select a range of dates. In some cases two calendars will be shown to select the start and end dates of the range.
		/// </summary>
		Range
	}

	/// <summary>
	/// Control to show a calendar that the user can select either a single date or range of dates.
	/// </summary>
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
		/// Gets or sets the selected date.
		/// </summary>
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
		/// The SelectedRange will have the same start/end dates when <see cref="Mode"/> is <see cref="CalendarMode.Single"/>.
		/// </remarks>
		/// <value>The selected range.</value>
		public Range<DateTime> SelectedRange
		{
			get { return Handler.SelectedRange; }
			set { Handler.SelectedRange = value; }
		}

		/// <summary>
		/// Gets or sets the mode of the calendar.
		/// </summary>
		/// <value>The calendar mode.</value>
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

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.Calendar"/> class.
		/// </summary>
		/// <param name="handler">Handler implementation to wrap.</param>
		protected Calendar(IHandler handler)
			: base(handler)
		{
		}

		static readonly object callback = new Callback();

		/// <summary>
		/// Gets an instance of an object used to perform callbacks to the widget from handler implementations
		/// </summary>
		/// <returns>The callback.</returns>
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
			/// Raises the selected date changed event.
			/// </summary>
			void OnSelectedDateChanged(Calendar widget, EventArgs e);

			/// <summary>
			/// Raises the selected range changed event.
			/// </summary>
			void OnSelectedRangeChanged(Calendar widget, EventArgs e);
		}

		/// <summary>
		/// Callback implementation for handlers of the <see cref="Calendar"/>.
		/// </summary>
		protected new class Callback : Control.Callback, ICallback
		{
			/// <summary>
			/// Raises the selected date changed event.
			/// </summary>
			public void OnSelectedDateChanged(Calendar widget, EventArgs e)
			{
				using (widget.Platform.Context)
					widget.OnSelectedDateChanged(e);
			}

			/// <summary>
			/// Raises the selected range changed event.
			/// </summary>
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
			/// Gets or sets the selected range.
			/// </summary>
			/// <remarks>
			/// The SelectedRange will have the same start/end dates when <see cref="Mode"/> is <see cref="CalendarMode.Single"/>.
			/// </remarks>
			/// <value>The selected range.</value>
			Range<DateTime> SelectedRange { get; set; }

			/// <summary>
			/// Gets or sets the selected date.
			/// </summary>
			/// <value>The selected date.</value>
			DateTime SelectedDate { get; set; }

			/// <summary>
			/// Gets or sets the mode of the calendar.
			/// </summary>
			/// <value>The calendar mode.</value>
			CalendarMode Mode { get; set; }
		}
	}
}