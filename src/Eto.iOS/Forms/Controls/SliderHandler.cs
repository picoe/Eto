using System;
using UIKit;
using Eto.Forms;
using CoreGraphics;

namespace Eto.iOS.Forms.Controls
{
	public class SliderHandler : IosControl<UISlider, Slider, Slider.ICallback>, Slider.IHandler
	{
		Orientation orientation;
		int? lastValue;

		public SliderHandler()
		{
			Control = new UISlider();
			Control.ValueChanged += HandleValueChanged;
		}

		void HandleValueChanged(object sender, EventArgs e)
		{
			var value = Value;
			var offset = value % TickFrequency;
			if (SnapToTick && offset != 0)
			{
				if (offset > TickFrequency / 2)
					value = value - offset + TickFrequency;
				else
					value -= offset;
				Value = value;
			}

			if (lastValue == null || lastValue.Value != value)
			{
				Callback.OnValueChanged(Widget, EventArgs.Empty);
				lastValue = value;
			}
		}

		protected override void Initialize()
		{
			base.Initialize();
			MaxValue = 100;
			TickFrequency = 1;
		}

		public int MaxValue
		{
			get { return (int)Control.MaxValue; }
			set { Control.MaxValue = value; }
		}

		public int MinValue
		{
			get { return (int)Control.MinValue; }
			set { Control.MinValue = value; }
		}

		public int Value
		{
			get { return (int)Control.Value; }
			set { Control.Value = value; }
		}

		public int TickFrequency { get; set; }

		public bool SnapToTick
		{
			get;
			set;
		}

		public Orientation Orientation
		{
			get { return orientation; }
			set
			{
				if (orientation != value)
				{
					orientation = value;
					switch (value)
					{
						case Orientation.Horizontal:
							Control.Transform = new CGAffineTransform();
							break;
						case Orientation.Vertical:
							Control.Transform = CGAffineTransform.MakeRotation((float)Math.PI * -0.5f);
							break;
						default:
							throw new NotSupportedException();
					}
				}
			}
		}
	}
}

