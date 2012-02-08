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
		ISlider inner;
		
		public event EventHandler<EventArgs> ValueChanged;
		
		public virtual void OnValueChanged (EventArgs e)
		{
			if (ValueChanged != null)
				ValueChanged (this, EventArgs.Empty);
		}
		
		public Slider ()
			: this(Generator.Current)
		{
		}
		
		public Slider (Generator generator)
			: base(generator, typeof(ISlider), true)
		{
			inner = (ISlider)Handler;
		}
		
		public int TickFrequency {
			get { return inner.TickFrequency; }
			set { inner.TickFrequency = value; }
		}

        public bool SnapToTick {
            get { return inner.SnapToTick; }
            set { inner.SnapToTick = value; }
        }

		public int MaxValue {
			get { return inner.MaxValue; }
			set { inner.MaxValue = value; }
		}

		public int MinValue {
			get { return inner.MinValue; }
			set { inner.MinValue = value; }
		}

		public int Value {
			get { return inner.Value; }
			set { inner.Value = value; }
		}

		public SliderOrientation Orientation {
			get { return inner.Orientation; }
			set { inner.Orientation = value; }
		}
	}
}

