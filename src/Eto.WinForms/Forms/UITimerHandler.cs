using System;
using Eto.Forms;

namespace Eto.WinForms.Forms
{
	public class UITimerHandler : WidgetHandler<System.Windows.Forms.Timer, UITimer, UITimer.ICallback>, UITimer.IHandler
	{
		//bool enabled;
		
		public UITimerHandler ()
		{
			Control = new System.Windows.Forms.Timer ();
			Control.Tick += (sender, e) => Callback.OnElapsed(Widget, EventArgs.Empty);
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

