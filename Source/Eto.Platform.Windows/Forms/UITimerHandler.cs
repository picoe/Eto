using System;
using Eto.Forms;

namespace Eto.Platform.Windows.Forms
{
	public class UITimerHandler : WidgetHandler<System.Windows.Forms.Timer, UITimer>, IUITimer
	{
		//bool enabled;
		
		public UITimerHandler ()
		{
			Control = new System.Windows.Forms.Timer ();
			Control.Tick += (sender, e) => {
				//Control.Enabled = enabled;
				Widget.OnElapsed (EventArgs.Empty);
			};
		}

		public void Start ()
		{
			//enabled = true;
			Control.Start ();
		}

		public void Stop ()
		{
			//enabled = false;
			Control.Stop ();
		}

		public double Interval {
			get {
				return ((double)Control.Interval) / 1000.0;
			}
			set {
				Control.Interval = (int)(value * 1000.0);
			}
		}
	}
}

