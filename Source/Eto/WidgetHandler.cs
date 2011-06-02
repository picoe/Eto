using System;
using System.Collections;
using System.Collections.Generic;

namespace Eto
{
	public abstract class WidgetHandler<T, W> : IWidget, IDisposable
		where W: IWidget
	{
		HashSet<string> eventHooks; 
		
		~WidgetHandler()
		{
			Dispose(false);
		}
		
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
			if (disposing && DisposeControl) {
				var control = this.Control as IDisposable;
		        if (control != null) control.Dispose();
			}
			this.Control = default(T);
	    }		
	}
	
	public abstract class WidgetHandler : WidgetHandler<object, IWidget>
	{


	}
}
