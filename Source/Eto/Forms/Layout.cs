using System;

namespace Eto.Forms
{
	public interface ILayout : IInstanceWidget
	{
	}

	public interface IPositionalLayout : ILayout
	{
		void Add(Control child, int x, int y);
		void Move(Control child, int x, int y);
		void Remove(Control child);
	}
	
	public abstract class Layout : InstanceWidget
	{
		
		public Layout(Generator g, Container container, Type type, bool initialize = true)
			: base(g, type, false)
		{
			this.Container = container;
			if (initialize) {
				Initialize();
				this.Container.SetLayout(this);
			}
		}
		
		public Container Container
		{
			get; private set;
		}
		
	}
}
