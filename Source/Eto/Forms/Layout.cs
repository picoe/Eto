using System;

namespace Eto.Forms
{
	public interface ILayout : IInstanceWidget
	{
		void OnLoad();
	}

	public interface IPositionalLayout : ILayout
	{
		void Add(Control child, int x, int y);
		void Move(Control child, int x, int y);
		void Remove(Control child);
	}
	
	public abstract class Layout : InstanceWidget
	{
		public bool Loaded { get; private set; }
		
		public event EventHandler<EventArgs> Load;

		public virtual void OnLoad(EventArgs e)
		{
			if (Load != null) Load(this, e);
			((ILayout)Handler).OnLoad();
			Loaded = true;
		}
		
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
