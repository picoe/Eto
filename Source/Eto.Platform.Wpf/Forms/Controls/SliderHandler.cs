using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using swc = System.Windows.Controls;
using Eto.Forms;

namespace Eto.Platform.Wpf.Forms.Controls
{
	public class SliderHandler : WpfControl<swc.Slider, Slider>, ISlider
	{
		public SliderHandler ()
		{
			Control = new swc.Slider {
				Minimum = 0,
				Maximum = 100,
				TickPlacement = swc.Primitives.TickPlacement.BottomRight
			};
			Control.ValueChanged += delegate {
				Widget.OnValueChanged (EventArgs.Empty);
			};
		}

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
            get { return Control.IsSnapToTickEnabled; }
            set { Control.IsSnapToTickEnabled = value; }
        }

		public int TickFrequency
		{
			get { return (int)Control.TickFrequency; }
			set { Control.TickFrequency = value; }
		}

		public SliderOrientation Orientation
		{
			get
			{
				switch (Control.Orientation) {
					case swc.Orientation.Vertical:
						return SliderOrientation.Vertical;
					case swc.Orientation.Horizontal:
						return SliderOrientation.Horizontal;
					default:
						throw new NotSupportedException ();
				}
			}
			set
			{
				switch (value) {
					case SliderOrientation.Vertical:
						Control.Orientation = swc.Orientation.Vertical;
						break;
					case SliderOrientation.Horizontal:
						Control.Orientation = swc.Orientation.Horizontal;
						break;
					default:
						throw new NotSupportedException ();
				}
			}
		}
	}
}
