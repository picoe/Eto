using System;
using MonoMac.AppKit;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Platform.Mac.Forms.Controls
{
	public class SliderHandler : MacControl<NSSlider, Slider>, ISlider
	{
		SliderOrientation orientation;

		public class EtoSlider : NSSlider, IMacControl
		{
			public WeakReference WeakHandler { get; set; }

			public object Handler
			{ 
				get { return (object)WeakHandler.Target; }
				set { WeakHandler = new WeakReference(value); } 
			}
		}

		public SliderHandler()
		{
			Control = new EtoSlider { Handler = this };
			Control.Activated += HandleActivated;
			MinValue = 0;
			MaxValue = 100;
		}

		static void HandleActivated(object sender, EventArgs e)
		{
			var handler = GetHandler(sender) as SliderHandler;
			if (handler != null)
			{
				if (handler.Control.DoubleValue != (double)handler.Control.IntValue)
					handler.Control.IntValue = (int)Math.Round(handler.Control.DoubleValue);

				handler.Widget.OnValueChanged(EventArgs.Empty);
			}
		}

		protected override Size GetNaturalSize(Size availableSize)
		{
			if (Orientation == SliderOrientation.Horizontal)
				return new Size(80, 30);
			else
				return new Size(30, 80);
		}

		public int MaxValue
		{
			get { return (int)Control.MaxValue; }
			set
			{ 
				var old = TickFrequency;
				Control.MaxValue = value;
				TickFrequency = old;
			}
		}

		public int MinValue
		{
			get { return (int)Control.MinValue; }
			set
			{
				var old = TickFrequency;
				Control.MinValue = value;
				TickFrequency = old;
			}
		}

		public int Value
		{
			get { return Control.IntValue; }
			set { Control.IntValue = value; }
		}

		public bool SnapToTick
		{
			get { return Control.AllowsTickMarkValuesOnly; }
			set { Control.AllowsTickMarkValuesOnly = value; }
		}

		public int TickFrequency
		{
			get
			{ 
				if (Control.TickMarksCount > 1)
					return (int)((MaxValue - MinValue) / (Control.TickMarksCount - 1));
				else
					return MaxValue - MinValue;
			}
			set
			{ 
				Control.TickMarksCount = (int)((MaxValue - MinValue) / value) + 1;
			}
		}

		public SliderOrientation Orientation
		{
			get
			{
				return orientation;
			}
			set
			{
				orientation = value;
				// wha?!?! no way to do this other than change size or sumthun?
				var size = this.Control.Frame.Size;
				if (value == SliderOrientation.Vertical)
				{
					size.Height = size.Width + 1;
				}
				else
					size.Width = size.Height + 1;
				this.Control.SetFrameSize(size);
				LayoutIfNeeded();
			}
		}
	}
}

