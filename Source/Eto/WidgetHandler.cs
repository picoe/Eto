using System;
using System.Collections;
using System.Collections.Generic;

namespace Eto
{
	public abstract class WidgetHandler<W> : IWidget, IDisposable
		where W: Widget
	{
		HashSet<string> eventHooks; 
		
		~WidgetHandler()
		{
			Dispose(false);
		}
		
		public WidgetHandler()
		{
		}
		
		
		public W Widget { get; private set; }
		
		#region IWidget Members

		public virtual void Initialize()
		{
		}
		
		public void HandleEvent(string handler)
		{
			if (eventHooks == null) eventHooks = new HashSet<string>();
			if (eventHooks.Contains(handler)) return;
			AttachEvent(handler);
			eventHooks.Add(handler);
		}
		
		public virtual void AttachEvent(string handler)
		{
		}
		
		public IWidget Handler
		{
			get { return Widget; }
			set { Widget = (W)value; }
		}

		#endregion
		
		#region IDisposable Members

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		
		#endregion
		
	    protected virtual void Dispose(bool disposing)
	    {
	    }		
	}

	public abstract class WidgetHandler<T, W> : WidgetHandler<W>, IInstanceWidget
		where W: Widget
	{
		public WidgetHandler()
		{
			DisposeControl = true;
		}
		
		protected bool DisposeControl { get; set; }

		public virtual T Control { get; protected set; }
		
		public object ControlObject {
			get {
				return this.Control;
			}
		}
		
		protected override void Dispose (bool disposing)
		{
			if (disposing && DisposeControl) {
				var control = this.Control as IDisposable;
		        if (control != null) control.Dispose();
			}
			this.Control = default(T);
			base.Dispose (disposing);
		}

	}
	
	public abstract class WidgetHandler : WidgetHandler<Widget>
	{


	}
}
