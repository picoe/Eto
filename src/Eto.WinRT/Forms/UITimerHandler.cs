#if TODO_XAML
using System;
using Eto.Forms;

namespace Eto.WinRT.Forms
{

	public class UITimerHandler : WidgetHandler<Windows.UI.Xaml.Threading.DispatcherTimer, UITimer>, IUITimer
	{
		
		public UITimerHandler ()
		{
			Control = new Windows.UI.Xaml.Threading.DispatcherTimer ();
			Control.Tick += (sender, e) => Widget.OnElapsed(EventArgs.Empty);
		}

		public void Start ()
		{
			if (!Control.IsEnabled)
				Control.Start ();
		}

		public void Stop ()
		{
			if (Control.IsEnabled)
			Control.Stop ();
		}

		public double Interval {
			get {
				return Control.Interval.TotalSeconds;
			}
			set {
				Control.Interval = TimeSpan.FromSeconds(value);
			}
		}
	}
}

#endif