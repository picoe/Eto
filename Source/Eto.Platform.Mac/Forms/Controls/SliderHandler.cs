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
			MinValue = 0;
			MaxValue = 100;
		}

		public int MaxValue {
			get { return (int)Control.MaxValue; }
			set { 
				var old = TickFrequency;
				Control.MaxValue = value;
				TickFrequency = old;
			}
		}

		public int MinValue {
			get { return (int)Control.MinValue; }
			set {
				var old = TickFrequency;
				Control.MinValue = value;
				TickFrequency = old;
			}
		}

		public int Value {
			get { return Control.IntValue; }
			set { Control.IntValue = value; }
		}
		
		public int TickFrequency {
			get { 
				if (Control.TickMarksCount > 1)
					return (int)((MaxValue - MinValue) / (Control.TickMarksCount - 1));
				else
					return MaxValue - MinValue;
			}
			set { 
				Control.TickMarksCount = (int)((MaxValue - MinValue) / value) + 1;
				if (value > 1) Control.AllowsTickMarkValuesOnly = true;
			}
		}
		
		public override bool Enabled {
			get { return Control.Enabled; }
			set { Control.Enabled = value; }
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

