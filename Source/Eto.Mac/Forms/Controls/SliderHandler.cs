using System;
using Eto.Forms;
using Eto.Drawing;
#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
#endif

namespace Eto.Mac.Forms.Controls
{
	public class SliderHandler : MacControl<NSSlider, Slider, Slider.ICallback>, Slider.IHandler
	{
		Orientation orientation;

		public class EtoSlider : NSSlider, IMacControl
		{
			public WeakReference WeakHandler { get; set; }

			public object Handler
			{ 
				get { return WeakHandler.Target; }
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
				var newval = (int)Math.Round(handler.Control.DoubleValue);
				if (newval != handler.Control.IntValue)
					handler.Control.IntValue = newval;

				handler.Callback.OnValueChanged(handler.Widget, EventArgs.Empty);

				var ev = NSApplication.SharedApplication.CurrentEvent;
				if (ev != null)
				{
					// trigger mouse events when value is changed as they are buried by the slider
					if (ev.Type == NSEventType.LeftMouseUp || ev.Type == NSEventType.RightMouseUp)
						handler.Callback.OnMouseUp(handler.Widget, Conversions.GetMouseEvent(handler.Control, ev, false));
					else if (ev.Type == NSEventType.LeftMouseDragged || ev.Type == NSEventType.RightMouseDragged)
						handler.Callback.OnMouseMove(handler.Widget, Conversions.GetMouseEvent(handler.Control, ev, false));
				}
			}
		}

		protected override SizeF GetNaturalSize(SizeF availableSize)
		{
			return Orientation == Orientation.Horizontal ? new Size(80, 30) : new Size(30, 80);
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
				return MaxValue - MinValue;
			}
			set
			{ 
				Control.TickMarksCount = ((MaxValue - MinValue) / value) + 1;
			}
		}

		public Orientation Orientation
		{
			get
			{
				return orientation;
			}
			set
			{
				orientation = value;
				// wha?!?! no way to do this other than change size or sumthun?
				var size = Control.Frame.Size;
				if (value == Orientation.Vertical)
				{
					size.Height = size.Width + 1;
				}
				else
					size.Width = size.Height + 1;
				Control.SetFrameSize(size);
				LayoutIfNeeded();
			}
		}
	}
}

