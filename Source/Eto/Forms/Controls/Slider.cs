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
	
	[Handler(typeof(Slider.IHandler))]
	public class Slider : Control
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }
		
		public event EventHandler<EventArgs> ValueChanged;

		protected virtual void OnValueChanged(EventArgs e)
		{
			if (ValueChanged != null)
				ValueChanged(this, EventArgs.Empty);
		}
		
		public Slider()
		{
		}

		[Obsolete("Use default constructor instead")]
		public Slider (Generator generator)
			: this (generator, typeof(IHandler))
		{
		}

		[Obsolete("Use default constructor and HandlerAttribute instead")]
		protected Slider (Generator generator, Type type, bool initialize = true)
			: base (generator, type, initialize)
		{
		}
		
		public int TickFrequency {
			get { return Handler.TickFrequency; }
			set { Handler.TickFrequency = value; }
		}

		public bool SnapToTick {
			get { return Handler.SnapToTick; }
			set { Handler.SnapToTick = value; }
		}

		public int MaxValue {
			get { return Handler.MaxValue; }
			set { Handler.MaxValue = value; }
		}

		public int MinValue {
			get { return Handler.MinValue; }
			set { Handler.MinValue = value; }
		}

		public int Value {
			get { return Handler.Value; }
			set { Handler.Value = value; }
		}

		public SliderOrientation Orientation {
			get { return Handler.Orientation; }
			set { Handler.Orientation = value; }
		}

		static readonly object callback = new Callback();
		/// <summary>
		/// Gets an instance of an object used to perform callbacks to the widget from handler implementations
		/// </summary>
		/// <returns>The callback instance to use for this widget</returns>
		protected override object GetCallback() { return callback; }

		public new interface ICallback : Control.ICallback
		{
			void OnValueChanged(Slider widget, EventArgs e);
		}

		protected new class Callback : Control.Callback, ICallback
		{
			public void OnValueChanged(Slider widget, EventArgs e)
			{
				widget.Platform.Invoke(() => widget.OnValueChanged(e));
			}
		}

		public new interface IHandler : Control.IHandler
		{
			int MaxValue { get; set; }

			int MinValue { get; set; }

			int Value { get; set; }

			int TickFrequency { get; set; }

			bool SnapToTick { get; set; }

			SliderOrientation Orientation { get; set; }
		}
	}
}

