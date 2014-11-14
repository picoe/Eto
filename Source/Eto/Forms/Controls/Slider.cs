using System;

namespace Eto.Forms
{
	/// <summary>
	/// Enumeration of the orientations available for the <see cref="Slider"/> control
	/// </summary>
	public enum SliderOrientation
	{
		/// <summary>
		/// Slider should be shown in a horizontal orientation
		/// </summary>
		Horizontal,

		/// <summary>
		/// Slider should be shown in a vertical orientation
		/// </summary>
		Vertical
	}

	/// <summary>
	/// Control with a knob the user can slide up/down or left/right to select a numeric range.
	/// </summary>
	[Handler(typeof(Slider.IHandler))]
	public class Slider : Control
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		/// <summary>
		/// Occurs when the <see cref="Value"/> property is changed.
		/// </summary>
		public event EventHandler<EventArgs> ValueChanged;

		/// <summary>
		/// Raises the <see cref="ValueChanged"/> event.
		/// </summary>
		/// <param name="e">Event arguments.</param>
		protected virtual void OnValueChanged(EventArgs e)
		{
			if (ValueChanged != null)
				ValueChanged(this, EventArgs.Empty);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.Slider"/> class.
		/// </summary>
		public Slider()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.Slider"/> class.
		/// </summary>
		/// <param name="generator">Generator.</param>
		[Obsolete("Use default constructor instead")]
		public Slider(Generator generator)
			: this(generator, typeof(IHandler))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.Slider"/> class.
		/// </summary>
		/// <param name="generator">Generator.</param>
		/// <param name="type">Type.</param>
		/// <param name="initialize">If set to <c>true</c> initialize.</param>
		[Obsolete("Use default constructor and HandlerAttribute instead")]
		protected Slider(Generator generator, Type type, bool initialize = true)
			: base(generator, type, initialize)
		{
		}

		/// <summary>
		/// Gets or sets the hint for numeric value between each visual tick.
		/// </summary>
		/// <remarks>
		/// This is for visual representation only, unless the <see cref="SnapToTick"/> is set to true.
		/// </remarks>
		/// <value>The tick frequency.</value>
		public int TickFrequency
		{
			get { return Handler.TickFrequency; }
			set { Handler.TickFrequency = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether the slider will snap to each tick.
		/// </summary>
		/// <remarks>
		/// This will restrict the user's input to only values at each <see cref="TickFrequency"/> interval
		/// between the <see cref="MinValue"/> and <see cref="MaxValue"/>.
		/// </remarks>
		/// <seealso cref="TickFrequency"/>
		/// <value><c>true</c> if the slider will snap to each tick; otherwise, <c>false</c>.</value>
		public bool SnapToTick
		{
			get { return Handler.SnapToTick; }
			set { Handler.SnapToTick = value; }
		}

		/// <summary>
		/// Gets or sets the maximum value that can be set by the user.
		/// </summary>
		/// <value>The maximum value.</value>
		public int MaxValue
		{
			get { return Handler.MaxValue; }
			set { Handler.MaxValue = value; }
		}

		/// <summary>
		/// Gets or sets the minimum value that can be set by the user.
		/// </summary>
		/// <value>The minimum value.</value>
		public int MinValue
		{
			get { return Handler.MinValue; }
			set { Handler.MinValue = value; }
		}

		/// <summary>
		/// Gets or sets the current slider value.
		/// </summary>
		/// <value>The value.</value>
		public int Value
		{
			get { return Handler.Value; }
			set { Handler.Value = value; }
		}

		/// <summary>
		/// Gets or sets the orientation of the slider.
		/// </summary>
		/// <value>The slider orientation.</value>
		public SliderOrientation Orientation
		{
			get { return Handler.Orientation; }
			set { Handler.Orientation = value; }
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
		/// Callback interface for the <see cref="Slider"/>
		/// </summary>
		public new interface ICallback : Control.ICallback
		{
			/// <summary>
			/// Raises the value changed event.
			/// </summary>
			void OnValueChanged(Slider widget, EventArgs e);
		}

		/// <summary>
		/// Callback implementation for handlers of <see cref="Slider"/>
		/// </summary>
		protected new class Callback : Control.Callback, ICallback
		{
			/// <summary>
			/// Raises the value changed event.
			/// </summary>
			public void OnValueChanged(Slider widget, EventArgs e)
			{
				widget.Platform.Invoke(() => widget.OnValueChanged(e));
			}
		}

		/// <summary>
		/// Handler interface for the <see cref="Slider"/>
		/// </summary>
		public new interface IHandler : Control.IHandler
		{
			/// <summary>
			/// Gets or sets the maximum value that can be set by the user.
			/// </summary>
			/// <value>The maximum value.</value>
			int MaxValue { get; set; }

			/// <summary>
			/// Gets or sets the minimum value that can be set by the user.
			/// </summary>
			/// <value>The minimum value.</value>
			int MinValue { get; set; }

			/// <summary>
			/// Gets or sets the current slider value.
			/// </summary>
			/// <value>The value.</value>
			int Value { get; set; }

			/// <summary>
			/// Gets or sets the hint for numeric value between each visual tick.
			/// </summary>
			/// <remarks>
			/// This is for visual representation only, unless the <see cref="SnapToTick"/> is set to true.
			/// </remarks>
			/// <value>The tick frequency.</value>
			int TickFrequency { get; set; }

			/// <summary>
			/// Gets or sets a value indicating whether the slider will snap to each tick.
			/// </summary>
			/// <remarks>
			/// This will restrict the user's input to only values at each <see cref="TickFrequency"/> interval
			/// between the <see cref="MinValue"/> and <see cref="MaxValue"/>.
			/// </remarks>
			/// <seealso cref="TickFrequency"/>
			/// <value><c>true</c> if the slider will snap to each tick; otherwise, <c>false</c>.</value>
			bool SnapToTick { get; set; }

			/// <summary>
			/// Gets or sets the orientation of the slider.
			/// </summary>
			/// <value>The slider orientation.</value>
			SliderOrientation Orientation { get; set; }
		}
	}
}

