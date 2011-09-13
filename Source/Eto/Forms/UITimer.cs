using System;

namespace Eto.Forms
{
	public interface IUITimer : IWidget
	{
		double Interval { get; set; }
		void Start();
		void Stop();
	}
	
	public class UITimer : Widget, IUITimer
	{
		IUITimer inner;
		
		public static double DefaultInterval = 1.0; // 1 second
		
		public event EventHandler<EventArgs> Elapsed;
		
		public virtual void OnElapsed(EventArgs e)
		{
			if (Elapsed != null) Elapsed(this, e);
		}

		public UITimer ()
			: this(Generator.Current)
		{
		}
		
		public UITimer (Generator generator)
			: base(generator, typeof(IUITimer))
		{
			inner = (IUITimer)Handler;
		}
		
		/// <summary>
		/// Gets or sets the interval, in seconds
		/// </summary>
		public double Interval
		{
			get { return inner.Interval; }
			set { inner.Interval = value; }
		}
		
		public void Start()
		{
			inner.Start();
		}
		public void Stop()
		{
			inner.Stop();
		}
	}
}

