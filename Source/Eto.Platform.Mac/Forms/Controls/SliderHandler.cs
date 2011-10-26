using System;
using MonoMac.AppKit;
using Eto.Forms;

namespace Eto.Platform.Mac.Forms.Controls
{
	public class SliderHandler : MacView<NSSlider, Slider>, ISlider
	{
		public SliderHandler ()
		{
			Control = new NSSlider ();
			Control.Activated += delegate {
				if (Control.DoubleValue != (double)Control.IntValue)
					Control.IntValue = (int)Math.Round (Control.DoubleValue);
				
				Widget.OnValueChanged (EventArgs.Empty);
			};
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
			get { return Control.IntValue; }
			set { Control.IntValue = value; }
		}
		
		public int TickFrequency {
			get { return (int)((MaxValue - MinValue + 1) / Control.TickMarksCount); }
			set { 
				Control.TickMarksCount = (int)((MaxValue - MinValue + 1) / value);
				if (value > 1) Control.AllowsTickMarkValuesOnly = true;
			}
		}

		public SliderOrientation Orientation {
			get {
				return (Control.IsVertical == 1) ? SliderOrientation.Vertical : SliderOrientation.Horizontal;
			}
			set {
				// wha?!?! no way to do this other than change size or sumthun?
				var size = this.Control.Frame.Size;
				if (value == SliderOrientation.Vertical) {
					size.Height = size.Width + 1;
				} else
					size.Width = size.Height + 1;
				this.Control.SetFrameSize (size);
			}
		}
	}
}

