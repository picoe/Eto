using System;

namespace Eto.Forms
{
	public enum SliderOrientation
	{
		Horizontal,
		Vertical
	}
	
	public interface ISlider : IControl
	{
		int MaxValue { get; set; }

		int MinValue { get; set; }

		int Value { get; set; }

		int TickFrequency { get; set; }

		bool SnapToTick { get; set; }

		SliderOrientation Orientation { get; set; }
	}
	
	public class Slider : Control
	{
		ISlider handler;
		
		public event EventHandler<EventArgs> ValueChanged;
		
		public virtual void OnValueChanged (EventArgs e)
		{
			if (ValueChanged != null)
				ValueChanged (this, EventArgs.Empty);
		}
		
		public Slider ()
			: this (Generator.Current)
		{
		}
		
		public Slider (Generator generator)
			: this (generator, typeof(ISlider))
		{
		}
		
		protected Slider (Generator generator, Type type, bool initialize = true)
			: base (generator, type, initialize)
		{
			handler = (ISlider)Handler;
		}
		
		public int TickFrequency {
			get { return handler.TickFrequency; }
			set { handler.TickFrequency = value; }
		}

		public bool SnapToTick {
			get { return handler.SnapToTick; }
			set { handler.SnapToTick = value; }
		}

		public int MaxValue {
			get { return handler.MaxValue; }
			set { handler.MaxValue = value; }
		}

		public int MinValue {
			get { return handler.MinValue; }
			set { handler.MinValue = value; }
		}

		public int Value {
			get { return handler.Value; }
			set { handler.Value = value; }
		}

		public SliderOrientation Orientation {
			get { return handler.Orientation; }
			set { handler.Orientation = value; }
		}
	}
}

