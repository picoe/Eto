using System;
using Eto.Forms;

namespace Eto.GtkSharp.Forms
{
	public class UITimerHandler : WidgetHandler<object, UITimer>, IUITimer
	{
		bool enabled;
		bool stopped = true;
		
		public void Start ()
		{
			if (enabled) return;
			enabled = true;
			if (!stopped) return;
			stopped = false;
			GLib.Timeout.Add((uint)(Interval * 1000), delegate {
				if (!enabled) { 
					stopped = true; return false;
				}
				Widget.OnElapsed(EventArgs.Empty);
				return true;
			});
		}

		public void Stop ()
		{
			enabled = false;
		}

		public double Interval { get; set; }

		protected override void Dispose(bool disposing)
		{
			base.Dispose (disposing);
			enabled = false;
		}
	}
}

