using System;
namespace Eto
{
	public interface IWidget
	{
		IWidget Handler { get; set; }
		void Initialize();
	}
	
	public abstract class Widget : IWidget, IDisposable
	{
		public Generator Generator { get; private set; }
		public object Tag { get; set; }
		public IWidget Handler { get; set; }
		
		~Widget()
		{
			Dispose(false);
		}
		
		protected Widget (Generator generator, IWidget handler, bool initialize = true)
		{
			this.Generator = generator;
			this.Handler = handler;
			this.Handler.Handler = this; // tell the handler who we are
			if (initialize) Initialize();
		}

		protected Widget (Generator generator, Type type, bool initialize = true)
		{
			this.Generator = generator;
			this.Handler = (IWidget)generator.CreateControl(type);
			this.Handler.Handler = this; // tell the handler who we are
			if (initialize) Initialize();
		}
		
		public void Initialize()
		{
			Handler.Initialize();
		}
		
		#region IDisposable Members

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		
		#endregion
		
	    protected virtual void Dispose(bool disposing)
	    {
			if (disposing)
			{
				var handler = this.Handler as IDisposable;
		        if (handler != null) handler.Dispose();
				Handler = null;
			}
	    }		
		
	}
}

