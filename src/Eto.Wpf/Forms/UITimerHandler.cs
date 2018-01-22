using System;
using Eto.Forms;

namespace Eto.Wpf.Forms
{

	public class UITimerHandler : WidgetHandler<System.Windows.Threading.DispatcherTimer, UITimer, UITimer.ICallback>, UITimer.IHandler
	{

		public UITimerHandler()
		{
			Control = new System.Windows.Threading.DispatcherTimer();
			Control.Tick += (sender, e) => Callback.OnElapsed(Widget, EventArgs.Empty);
		}

		public void Start()
		{
			if (Control.IsEnabled)
				Interval = Interval;
			else
				Control.Start();
		}

		public void Stop()
		{
			if (Control.IsEnabled)
				Control.Stop();
		}

		public double Interval
		{
			get
			{
				return Control.Interval.TotalSeconds;
			}
			set
			{
				Control.Interval = TimeSpan.FromSeconds(value);
			}
		}
	}
}

