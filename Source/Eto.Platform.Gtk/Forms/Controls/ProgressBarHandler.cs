using System;
using Eto.Forms;

namespace Eto.Platform.GtkSharp.Forms.Controls
{
	public class ProgressBarHandler : GtkControl<Gtk.ProgressBar, ProgressBar>, IProgressBar
	{
		int minValue;
		int maxValue = 100;
		bool indeterminate;
		UITimer timer;
		public static double UpdateInterval = 0.2;
		public static double PulseStep = 0.1;

		public ProgressBarHandler ()
		{
			this.Control = new Gtk.ProgressBar {
				Fraction = 0
			};
		}

		public bool Indeterminate {
			get { return indeterminate; }
			set {
				indeterminate = value;
				if (indeterminate) {
					if (timer == null) {
						timer = new UITimer (Widget.Generator);
						timer.Elapsed += delegate {
							Control.Pulse ();
						};
					}
					timer.Interval = UpdateInterval;
					Control.PulseStep = PulseStep;
					timer.Start ();
				} else if (timer != null)
					timer.Stop ();
			}
		}
		
		public int MaxValue {
			get { return maxValue; }
			set {
				var val = Value;
				maxValue = value;
				Value = val;
			}
		}

		public int MinValue {
			get { return minValue; }
			set {
				var val = Value;
				minValue = value;
				Value = val;
			}
		}

		public int Value {
			get { return (int)((Control.Fraction * MaxValue) + MinValue); }
			set {
				Control.Fraction = Math.Max (0, Math.Min (1, ((double)value - MinValue) / (double)MaxValue));
			}
		}
	}
}

