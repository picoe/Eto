namespace Eto.GtkSharp.Forms.Controls
{
	public class ProgressBarHandler : GtkControl<Gtk.ProgressBar, ProgressBar, ProgressBar.ICallback>, ProgressBar.IHandler
	{
		int _value;
		int _minValue;
		int _maxValue = 100;
		bool _indeterminate;
		UITimer _timer;
		public static double UpdateInterval = 0.2;
		public static double PulseStep = 0.1;

		public ProgressBarHandler()
		{
			this.Control = new Gtk.ProgressBar
			{
				Fraction = 0
			};
		}

		protected new ProgressBarConnector Connector { get { return (ProgressBarConnector)base.Connector; } }

		protected override WeakConnector CreateConnector()
		{
			return new ProgressBarConnector();
		}

		protected class ProgressBarConnector : GtkControlConnector
		{
			public new ProgressBarHandler Handler { get { return (ProgressBarHandler)base.Handler; } }

			public void TimerElapsed(object sender, EventArgs e)
			{
				var timer = (UITimer)sender;
				var handler = Handler;
				if (handler != null)
					handler.Control.Pulse();
				else
					timer.Stop();
			}
		}

		public bool Indeterminate
		{
			get { return _indeterminate; }
			set
			{
				_indeterminate = value;
				if (_indeterminate)
				{
					if (_timer == null)
					{
						_timer = new UITimer();
						_timer.Elapsed += Connector.TimerElapsed;
					}
					_timer.Interval = UpdateInterval;
					Control.PulseStep = PulseStep;
					_timer.Start();
				}
				else if (_timer != null)
					_timer.Stop();
			}
		}

		public int MaxValue
		{
			get { return _maxValue; }
			set
			{
				var val = Value;
				_maxValue = value;
				Value = val;
			}
		}

		public int MinValue
		{
			get { return _minValue; }
			set
			{
				var val = Value;
				_minValue = value;
				Value = val;
			}
		}

		public int Value
		{
			get => _value;
			set
			{
				_value = value;
				Control.Fraction = Math.Max(0, Math.Min(1, ((double)value - MinValue) / (double)MaxValue));
			}
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (_timer != null)
				_timer.Stop();
		}
	}
}

