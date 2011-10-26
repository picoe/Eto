using System;
using SWF = System.Windows.Forms;
using Eto.Forms;

namespace Eto.Platform.Windows.Forms.Controls
{
	public class SliderHandler : WindowsControl<SWF.TrackBar, Slider>, ISlider
	{
		public SliderHandler ()
		{
			this.Control = new SWF.TrackBar ();
			this.Control.TickStyle = System.Windows.Forms.TickStyle.Both;
			this.Control.ValueChanged += delegate {
				Widget.OnValueChanged (EventArgs.Empty);
			};
		}

		public int MaxValue {
			get { return this.Control.Maximum; }
			set { this.Control.Maximum = value; }
		}

		public int MinValue {
			get { return this.Control.Minimum; }
			set { this.Control.Minimum = value; }
		}

		public int Value {
			get { return this.Control.Value; }
			set { this.Control.Value = value; }
		}

		public int TickFrequency {
			get { return this.Control.TickFrequency; }
			set {
				this.Control.TickFrequency = value; 
			}
		}

		public SliderOrientation Orientation {
			get { return this.Control.Orientation == SWF.Orientation.Horizontal ? SliderOrientation.Horizontal : SliderOrientation.Vertical; }
			set { this.Control.Orientation = value == SliderOrientation.Horizontal ? SWF.Orientation.Horizontal : SWF.Orientation.Vertical; }
		}
	}
}

