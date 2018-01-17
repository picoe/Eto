using System;

namespace Eto.Forms
{
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
		public Orientation Orientation
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
				using (widget.Platform.Context)
					widget.OnValueChanged(e);
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
			Orientation Orientation { get; set; }
		}
	}

	/// <summary>
	/// Enumeration of the orientations available for the <see cref="Slider"/> control
	/// </summary>
	[Obsolete("Since 2.1: Use Orientation instead")]
	public struct SliderOrientation
	{
		readonly Orientation orientation;

		SliderOrientation(Orientation orientation)
		{
			this.orientation = orientation;
		}

		/// <summary>
		/// Slider should be shown in a horizontal orientation
		/// </summary>
		public static SliderOrientation Horizontal { get { return Orientation.Horizontal; } }

		/// <summary>
		/// Slider should be shown in a vertical orientation
		/// </summary>
		public static SliderOrientation Vertical { get { return Orientation.Vertical; } }

		/// <summary>Converts to an Orientation</summary>
		public static implicit operator Orientation(SliderOrientation orientation)
		{
			return orientation.orientation;
		}

		/// <summary>Converts an Orientation to a RadioButtonListOrientation</summary>
		public static implicit operator SliderOrientation(Orientation orientation)
		{
			return new SliderOrientation(orientation);
		}

		/// <summary>Compares for equality</summary>
		/// <param name="orientation1">Orientation1.</param>
		/// <param name="orientation2">Orientation2.</param>
		public static bool operator ==(Orientation orientation1, SliderOrientation orientation2)
		{
			return orientation1 == orientation2.orientation;
		}

		/// <summary>Compares for inequality</summary>
		/// <param name="orientation1">Orientation1.</param>
		/// <param name="orientation2">Orientation2.</param>
		public static bool operator !=(Orientation orientation1, SliderOrientation orientation2)
		{
			return orientation1 != orientation2.orientation;
		}

		/// <summary>
		/// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="Eto.Forms.SliderOrientation"/>.
		/// </summary>
		/// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="Eto.Forms.SliderOrientation"/>.</param>
		/// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to the current
		/// <see cref="Eto.Forms.SliderOrientation"/>; otherwise, <c>false</c>.</returns>
		public override bool Equals(object obj)
		{
			return (obj is SliderOrientation && (this == (SliderOrientation)obj))
				|| (obj is Orientation && (this == (Orientation)obj));
		}

		/// <summary>
		/// Serves as a hash function for a <see cref="Eto.Forms.SliderOrientation"/> object.
		/// </summary>
		/// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a hash table.</returns>
		public override int GetHashCode()
		{
			return orientation.GetHashCode();
		}
	}
}

