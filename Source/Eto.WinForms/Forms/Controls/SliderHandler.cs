using System;
using swf = System.Windows.Forms;
using sd = System.Drawing;
using Eto.Forms;

namespace Eto.WinForms.Forms.Controls
{
	public class SliderHandler : WindowsControl<swf.TrackBar, Slider>, ISlider
	{
        int? lastValue;

		class EtoTrackBar : swf.TrackBar
		{
			protected override void OnCreateControl ()
			{
				SetStyle (swf.ControlStyles.SupportsTransparentBackColor, true);
				if (Parent != null)
					BackColor = Parent.BackColor;

				base.OnCreateControl ();
			}
		}

		public SliderHandler ()
		{
			this.Control = new EtoTrackBar {
                TickStyle = System.Windows.Forms.TickStyle.BottomRight,
                Maximum = 100,
                AutoSize = true
            };
		}

        public override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Control.ValueChanged += HandleScaleValueChanged;
        }

        void HandleScaleValueChanged(object sender, EventArgs e)
        {
            var value = Control.Value;
            var tick = Control.TickFrequency;
            var offset = value % tick;
            if (SnapToTick && offset != 0)
            {
                if (offset > tick / 2)
                    Control.Value = value - offset + tick;
                else
                    Control.Value -= offset;
            }
            else if (lastValue == null || lastValue.Value != value)
            {
                Widget.OnValueChanged(EventArgs.Empty);
                lastValue = value;
            }
        }

		public int MaxValue {
			get { return Control.Maximum; }
			set { Control.Maximum = value; }
		}

		public int MinValue {
			get { return Control.Minimum; }
			set { Control.Minimum = value; }
		}

		public int Value {
			get { return Control.Value; }
			set { Control.Value = value; }
		}

        public bool SnapToTick { get; set; }

		public int TickFrequency {
			get { return Control.TickFrequency; }
			set {
				Control.TickFrequency = value; 
			}
		}

		public SliderOrientation Orientation {
			get { return Control.Orientation == swf.Orientation.Horizontal ? SliderOrientation.Horizontal : SliderOrientation.Vertical; }
			set { Control.Orientation = value == SliderOrientation.Horizontal ? swf.Orientation.Horizontal : swf.Orientation.Vertical; }
		}
	}
}

