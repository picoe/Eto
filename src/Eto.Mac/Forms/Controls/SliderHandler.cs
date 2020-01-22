using System;
using Eto.Forms;
using Eto.Drawing;

using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
using CoreImage;

namespace Eto.Mac.Forms.Controls
{
	public class SliderHandler : MacControl<NSSlider, Slider, Slider.ICallback>, Slider.IHandler
	{
		public class EtoSlider : NSSlider, IMacControl
		{
			public WeakReference WeakHandler { get; set; }

			public SliderHandler Handler
			{ 
				get { return (SliderHandler)WeakHandler?.Target; }
				set { WeakHandler = new WeakReference(value); } 
			}

			public override nint IsVertical => Handler?.Orientation == Orientation.Vertical ? 1 : 0;
		}

		protected override bool DefaultUseAlignmentFrame => true;

		protected override NSSlider CreateControl() => new EtoSlider();

		protected override void Initialize()
		{
			Control.Activated += HandleActivated;
			MinValue = 0;
			MaxValue = 100;
			base.Initialize();
		}

		static void HandleActivated(object sender, EventArgs e)
		{
			var handler = GetHandler(sender) as SliderHandler;
			if (handler != null)
			{
				handler.TriggerMouseCallback();

				var newval = (int)Math.Round(handler.Control.DoubleValue);
				if (newval != handler.Control.IntValue)
					handler.Control.IntValue = newval;

				handler.Callback.OnValueChanged(handler.Widget, EventArgs.Empty);
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
				return 0;
			}
			set
			{ 
				Control.TickMarksCount = value > 0 ? ((MaxValue - MinValue) / value) + 1 : 0;
			}
		}

		public Orientation Orientation { get; set; }
	}
}

