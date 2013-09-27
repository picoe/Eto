using System;
using MonoTouch.UIKit;
using Eto.Forms;
using MonoTouch.CoreGraphics;

namespace Eto.Platform.iOS.Forms.Controls
{
	public class SliderHandler : iosControl<UISlider, Slider>, ISlider
	{
		SliderOrientation orientation;
		int? lastValue;

		public override UISlider CreateControl()
		{
			var slider = new UISlider();
			slider.ValueChanged += HandleValueChanged;
			return slider;
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
				Widget.OnValueChanged(EventArgs.Empty);
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

		public SliderOrientation Orientation
		{
			get { return orientation; }
			set
			{
				if (orientation != value)
				{
					orientation = value;
					switch (value)
					{
						case SliderOrientation.Horizontal:
							Control.Transform = new CGAffineTransform();
							break;
						case SliderOrientation.Vertical:
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

