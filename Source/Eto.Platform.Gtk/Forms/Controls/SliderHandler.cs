using System;
using Eto.Forms;

namespace Eto.Platform.GtkSharp.Forms.Controls
{
	public class SliderHandler : GtkControl<Gtk.EventBox, Slider>, ISlider
	{
		int min = 0;
		int max = 100;
		int tick = 1;
		Gtk.Scale scale;
		
		public SliderHandler ()
		{
			this.Control = new Gtk.EventBox ();
			scale = new Gtk.HScale (min, max, 1);
			scale.ValueChanged += HandleScaleValueChanged;
			this.Control.Child = scale;
		}

		void HandleScaleValueChanged (object sender, EventArgs e)
		{
			Widget.OnValueChanged (EventArgs.Empty);
		}
		
		public int MaxValue {
			get { return max; }
			set {
				max = value;
				scale.SetRange (min, max);
			}
		}

		public int MinValue {
			get { return min; }
			set {
				min = value;
				scale.SetRange (min, max);
			}
		}

		public int Value {
			get { return (int)scale.Value; }
			set { scale.Value = value; }
		}

		public int TickFrequency {
			get {
				return tick;
			}
			set {
				tick = value;
				// TODO: Only supported from GTK 2.16
			}
		}

		public SliderOrientation Orientation {
			get {
				return (scale is Gtk.HScale) ? SliderOrientation.Horizontal : SliderOrientation.Vertical;
			}
			set {
				if (Orientation != value) {
					scale.ValueChanged -= HandleScaleValueChanged;
					this.Control.Remove (scale);
					if (value == SliderOrientation.Horizontal)
						scale = new Gtk.HScale (min, max, 1);
					else
						scale = new Gtk.VScale (min, max, 1);
					scale.ValueChanged += HandleScaleValueChanged;
					this.Control.Child = scale;
					scale.ShowAll ();
				}
			}
		}
	}
}

