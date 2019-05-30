using System;
using swc = Windows.UI.Xaml.Controls;
using Eto.Forms;
using Windows.UI.Xaml.Controls.Primitives;

namespace Eto.WinRT.Forms.Controls
{
	/// <summary>
	/// Slider handler.
	/// </summary>
	/// <copyright>(c) 2014 by Vivek Jhaveri</copyright>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class SliderHandler : WpfControl<swc.Slider, Slider, Slider.ICallback>, Slider.IHandler
	{
		public SliderHandler ()
		{
			Control = new swc.Slider {
				Minimum = 0,
				Maximum = 100,
				TickPlacement = swc.Primitives.TickPlacement.BottomRight
			};
			Control.ValueChanged += delegate {
				Callback.OnValueChanged(Widget, EventArgs.Empty);
			};
		}

		public override bool UseMousePreview { get { return true; } }

		public override bool UseKeyPreview { get { return true; } }

		public int MaxValue
		{
			get { return (int)Control.Maximum; }
			set { Control.Maximum = value; }
		}

		public int MinValue
		{
			get { return (int)Control.Minimum; }
			set { Control.Minimum = value; }
		}

		public int Value
		{
			get { return (int)Control.Value; }
			set { Control.Value = value; }
		}

        public bool SnapToTick
        {
            get { return Control.SnapsTo == SliderSnapsTo.Ticks; }
            set { Control.SnapsTo = SliderSnapsTo.Ticks; }
        }

		public int TickFrequency
		{
			get { return (int)Control.TickFrequency; }
			set { Control.TickFrequency = value; }
		}

		public Orientation Orientation
		{
			get
			{
				switch (Control.Orientation) {
					case swc.Orientation.Vertical:
						return Orientation.Vertical;
					case swc.Orientation.Horizontal:
						return Orientation.Horizontal;
					default:
						throw new NotSupportedException ();
				}
			}
			set
			{
				switch (value) {
					case Orientation.Vertical:
						Control.Orientation = swc.Orientation.Vertical;
						break;
					case Orientation.Horizontal:
						Control.Orientation = swc.Orientation.Horizontal;
						break;
					default:
						throw new NotSupportedException ();
				}
			}
		}
	}
}
