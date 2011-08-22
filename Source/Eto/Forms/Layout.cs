using System;
using System.Collections.Generic;

namespace Eto.Forms
{
	public interface ILayout : IInstanceWidget
	{
		void OnLoad();
		void OnLoadComplete();
		void Update();
	}

	public interface IPositionalLayout : ILayout
	{
		void Add(Control child, int x, int y);
		void Move(Control child, int x, int y);
		void Remove(Control child);
	}
	
	public abstract class Layout : InstanceWidget
	{
		ILayout inner;
		
		public bool Loaded { get; private set; }
		
		public abstract IEnumerable<Control> Controls
		{
			get;
		}
		
		
		public event EventHandler<EventArgs> Load;

		public virtual void OnLoad(EventArgs e)
		{
			if (Load != null) Load(this, e);
			inner.OnLoad();
			Loaded = true;
		}

		public event EventHandler<EventArgs> LoadComplete;

		public virtual void OnLoadComplete(EventArgs e)
		{
			if (LoadComplete != null) LoadComplete(this, e);
			inner.OnLoadComplete();
		}
		
		public Layout(Generator g, Container container, Type type, bool initialize = true)
			: base(g, type, false)
		{
			this.Container = container;
			inner = (ILayout)Handler;
			if (initialize) {
				Initialize();
				this.Container.SetLayout(this);
			}
		}
		
		public Container Container
		{
			get; private set;
		}
		
		public void Update()
		{
			inner.Update();
		}
		
		
	}
}
