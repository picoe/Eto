using System;
using System.ComponentModel;

namespace Eto.Forms
{
	/// <summary>
	/// Provides a timer that executes code at a specified interval on the UI thread
	/// </summary>
	/// <remarks>
	/// This provides a way to execute UI code at a specified <see cref="Interval"/>.
	/// Note that this is not a high-resolution timer, and you should avoid setting a small interval
	/// otherwise the UI may become unresponsive depending on the logic in the executed code.
	/// 
	/// This typically executes the code on the UI main loop, thus the accuracy of the timer is dependent on
	/// the other UI code executing.
	/// </remarks>
	[Handler(typeof(UITimer.IHandler))]
	public class UITimer : Widget
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.UITimer"/> class.
		/// </summary>
		public UITimer()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.UITimer"/> class with the specified <paramref name="elapsedHandler"/>.
		/// </summary>
		/// <param name="elapsedHandler">Delegate for handling the <see cref="Elapsed"/> event.</param>
		public UITimer(EventHandler<EventArgs> elapsedHandler)
		{
			Elapsed += elapsedHandler;
		}

		/// <summary>
		/// Occurs each time the <see cref="Interval"/> has elapsed
		/// </summary>
		public event EventHandler<EventArgs> Elapsed;

		/// <summary>
		/// Raises the <see cref="Elapsed"/> event.
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected virtual void OnElapsed(EventArgs e)
		{
			if (Elapsed != null)
				Elapsed(this, e);
		}

		/// <summary>
		/// Gets or sets the interval, in seconds
		/// </summary>
		/// <remarks>
		/// Note that this is not a high-resolution timer, and you should avoid setting a small interval
		/// otherwise the UI may become unresponsive depending on the logic in the executed code.
		/// </remarks>
		[DefaultValue(1.0)]
		public double Interval
		{
			get { return Handler.Interval; }
			set { Handler.Interval = value; }
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="Eto.Forms.UITimer"/> is started.
		/// </summary>
		/// <value><c>true</c> if started; otherwise, <c>false</c>.</value>
		public bool Started { get; private set; }

		/// <summary>
		/// Starts the timer
		/// </summary>
		public void Start()
		{
			Started = true;
			Handler.Start();
		}

		/// <summary>
		/// Stops a running timer
		/// </summary>
		public void Stop()
		{
			Started = false;
			Handler.Stop();
		}

		static readonly object callback = new Callback();

		/// <summary>
		/// Gets an instance of an object used to perform callbacks to the widget from handler implementations
		/// </summary>
		/// <returns>The callback instance to use for this widget</returns>
		protected override object GetCallback()
		{
			return callback;
		}

		/// <summary>
		/// Callback interface for <see cref="UITimer"/>
		/// </summary>
		public new interface ICallback : Widget.ICallback
		{
			/// <summary>
			/// Raises the elapsed event.
			/// </summary>
			void OnElapsed(UITimer widget, EventArgs e);
		}

		/// <summary>
		/// Callback implementation for <see cref="UITimer"/>
		/// </summary>
		protected class Callback : ICallback
		{
			/// <summary>
			/// Raises the elapsed event.
			/// </summary>
			public void OnElapsed(UITimer widget, EventArgs e)
			{
				using (widget.Platform.Context)
					widget.OnElapsed(e);
			}
		}

		/// <summary>
		/// Handler interface for <see cref="UITimer"/>
		/// </summary>
		public new interface IHandler : Widget.IHandler
		{
			/// <summary>
			/// Gets or sets the interval, in seconds to execute <see cref="ICallback.OnElapsed"/>
			/// </summary>
			double Interval { get; set; }

			/// <summary>
			/// Starts the timer
			/// </summary>
			void Start();

			/// <summary>
			/// Stops a running timer
			/// </summary>
			void Stop();
		}
	}
}

