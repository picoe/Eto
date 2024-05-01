using ao = Android.OS;

namespace Eto.Android
{
	internal class UITimerHandler : WidgetHandler<System.Threading.Timer, UITimer, UITimer.ICallback>, UITimer.IHandler
	{ 
		private Double interval;
		private Boolean isEnabled;

		public UITimerHandler()
		{
			Control = new System.Threading.Timer(OnTick, null, System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
		}

		private void OnTick(Object state)
		{
			new ao.Handler(ao.Looper.MainLooper).Post(OnTickSafe);
		}

		private void OnTickSafe()
		{
			Callback.OnElapsed(Widget, EventArgs.Empty);
		}

		public Double Interval
		{
			get
			{
				return interval;
			}

			set
			{
				interval = value;

				if(isEnabled)
					Control.Change((int)(Interval * 1000), (int)(Interval * 1000));
			}
		}

		public void Start()
		{
			if (isEnabled)
				return;

			Control.Change((int)(Interval * 1000), (int)(Interval * 1000));

			isEnabled = true;
		}

		public void Stop()
		{
			if (!isEnabled)
				return;

			Control.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);

			isEnabled = false;
		}
	}
}