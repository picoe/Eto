using System;

namespace Eto.Forms
{
	public interface IUITimer : IInstanceWidget
	{
		double Interval { get; set; }

		void Start ();

		void Stop ();
	}
	
	public class UITimer : InstanceWidget
	{
		IUITimer handler;
		public static double DefaultInterval = 1.0; // 1 second
		
		public event EventHandler<EventArgs> Elapsed;
		
		public virtual void OnElapsed (EventArgs e)
		{
			if (Elapsed != null)
				Elapsed (this, e);
		}

		public UITimer () : this (Generator.Current)
		{
		}
		
		public UITimer (Generator generator) : this (generator, typeof(IUITimer))
		{
		}
		
		protected UITimer (Generator g, Type type, bool initialize = true)
			: base (g, type, initialize)
		{
			handler = (IUITimer)Handler;
		}
		
		/// <summary>
		/// Gets or sets the interval, in seconds
		/// </summary>
		public double Interval {
			get { return handler.Interval; }
			set { handler.Interval = value; }
		}
		
		public void Start ()
		{
			handler.Start ();
		}

		public void Stop ()
		{
			handler.Stop ();
		}
	}
}

