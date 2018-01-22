using System;
using Eto.Forms;

namespace Eto.GtkSharp.Forms
{
	public class UITimerHandler : WidgetHandler<object, UITimer, UITimer.ICallback>, UITimer.IHandler
	{
		uint? handle;
		double interval = 1;

		public void Start()
		{
			Stop();
			handle = GLib.Timeout.Add((uint)(Interval * 1000), delegate
			{
				Callback.OnElapsed(Widget, EventArgs.Empty);
				return true;
			});
		}

		public void Stop()
		{
			if (handle != null)
			{
				GLib.Source.Remove(handle.Value);
				handle = null;
			}
		}

		public double Interval
		{
			get { return interval; }
			set
			{
				if (Math.Abs(interval - value) > double.Epsilon)
				{
					interval = value;
					if (handle != null)
						Start();
				}
			}
		}

		protected override void Dispose(bool disposing)
		{
			Stop();
			base.Dispose(disposing);
		}
	}
}

