using System;
using System.Threading;
using GLib;

namespace Eto.GtkSharp.Forms
{
	public class GtkSynchronizationContext : SynchronizationContext
	{
		public override void Post(SendOrPostCallback d, object state)
		{
			Idle.Add(() =>
			{
				d(state);
				return false;
			});
		}

		public override void Send(SendOrPostCallback d, object state)
		{
			var evt = new ManualResetEvent(false);
			Exception exception = null;

			Idle.Add(() =>
			{
				try
				{
					d(state);
				}
				catch (Exception ex)
				{
					exception = ex;
				}
				finally
				{
					evt.Set();
				}
				return false;
			});

			evt.WaitOne();

			if (exception != null)
				throw exception;
		}
	}
}