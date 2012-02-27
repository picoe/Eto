using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;

namespace Eto.Platform.Wpf.Forms
{

	public class UITimerHandler : WidgetHandler<System.Windows.Threading.DispatcherTimer, UITimer>, IUITimer
	{
		
		public UITimerHandler ()
		{
			Control = new System.Windows.Threading.DispatcherTimer ();
			Control.Tick += (sender, e) => {
				Widget.OnElapsed (EventArgs.Empty);
			};
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

