using System;

namespace Eto.Forms
{
	public interface IUITimer : IWidget
	{
		double Interval { get; set; }

		void Start ();

		void Stop ();
	}

	[Handler(typeof(IUITimer))]
	public class UITimer : Widget
	{
		new IUITimer Handler { get { return (IUITimer)base.Handler; } }

		public static double DefaultInterval = 1.0; // 1 second
		
		public event EventHandler<EventArgs> Elapsed;
		
		protected virtual void OnElapsed(EventArgs e)
		{
			if (Elapsed != null)
				Elapsed(this, e);
		}

		public UITimer()
		{
		}

		[Obsolete("Use default constructor instead")]
		public UITimer (Generator generator) : this (generator, typeof(IUITimer))
		{
		}

		[Obsolete("Use default constructor and HandlerAttribute instead")]
		protected UITimer (Generator generator, Type type, bool initialize = true)
			: base (generator, type, initialize)
		{
		}
		
		/// <summary>
		/// Gets or sets the interval, in seconds
		/// </summary>
		public double Interval {
			get { return Handler.Interval; }
			set { Handler.Interval = value; }
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="Eto.Forms.UITimer"/> is started.
		/// </summary>
		/// <value><c>true</c> if started; otherwise, <c>false</c>.</value>
		public bool Started { get; private set; }

		public void Start ()
		{
			Started = true;
			Handler.Start ();
		}

		public void Stop ()
		{
			Started = false;
			Handler.Stop ();
		}

		static readonly object callback = new Callback();
		protected override object GetCallback() { return callback; }

		public interface ICallback : Widget.ICallback
		{
			void OnElapsed(UITimer widget, EventArgs e);
		}

		protected class Callback : ICallback
		{
			public void OnElapsed(UITimer widget, EventArgs e)
			{
				widget.OnElapsed(e);
			}
		}
	}
}

