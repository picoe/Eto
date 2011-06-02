using System;
using System.ComponentModel;

namespace Eto
{
	public interface IInstanceWidget : IWidget
	{
		object ControlObject { get; }
		void HandleEvent(string handler);
	}
	

	public abstract class InstanceWidget : Widget, IWidget
	{
		IInstanceWidget inner;
		
		protected InstanceWidget(Generator generator, IWidget handler, bool initialize = true)
			: base(generator, handler, initialize)
		{
			inner = (IInstanceWidget)Handler;
		}

		protected InstanceWidget(Generator generator, Type type, bool initialize = true)
			: base(generator, type, initialize)
		{
			inner = (IInstanceWidget)Handler;
		}
		
		public object ControlObject
		{
			get { return inner.ControlObject; }
		}

		public void HandleEvent(string id)
		{
			inner.HandleEvent(id);
		}
	}
}
