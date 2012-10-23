using System;
using MonoTouch.UIKit;
using Eto.Forms;

namespace Eto.Platform.iOS.Forms.Controls
{
	public class SliderHandler : iosControl<UISlider, Slider>, ISlider
	{
		public override UISlider CreateControl ()
		{
			return new UISlider ();
		}

		public override void Initialize ()
		{
			base.Initialize ();
			MaxValue = 100;
		}

		public int MaxValue {
			get { return (int)Control.MaxValue; }
			set { Control.MaxValue = value; }
		}

		public int MinValue {
			get { return (int)Control.MinValue; }
			set { Control.MinValue = value; }
		}

		public int Value {
			get { return (int)Control.Value; }
			set { Control.Value = value; }
		}

		public int TickFrequency {
			get;
			set;
		}

		public bool SnapToTick {
			get;
			set;
		}

		public SliderOrientation Orientation {
			get;
			set;
		}
	}
}

