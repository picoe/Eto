using System;
using System.ComponentModel;

namespace Eto
{
	public interface IInstanceWidget : IWidget
	{
		object ControlObject { get; }

		void HandleEvent (string handler);
	}
	
	public abstract class InstanceWidget : Widget, IWidget
	{
		IInstanceWidget inner;
		
		public virtual string Style
		{
			get { return null; }
		}
		
		protected InstanceWidget (Generator generator, IWidget handler, bool initialize = true)
			: base(generator, handler, initialize)
		{
			inner = (IInstanceWidget)Handler;
			Eto.Style.OnStyleWidget(this);
		}

		protected InstanceWidget (Generator generator, Type type, bool initialize = true)
			: base(generator, type, initialize)
		{
			inner = (IInstanceWidget)Handler;
			Eto.Style.OnStyleWidget(this);
		}
		
		public object ControlObject {
			get { return inner.ControlObject; }
		}

		public void HandleEvent (string id)
		{
			inner.HandleEvent (id);
		}
		
		public void HandleEvent (params string[] ids)
		{
			foreach (var id in ids) {
				HandleEvent (id);
			}
		}
	}
}
