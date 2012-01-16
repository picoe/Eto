using System;
using System.Collections.Generic;
using System.Linq;

namespace Eto.Forms
{
	public interface ILayout : IInstanceWidget
	{
		void OnPreLoad ();
		
		void OnLoad ();

		void OnLoadComplete ();

		void Update ();
	}

	public interface IPositionalLayout : ILayout
	{
		void Add (Control child, int x, int y);

		void Move (Control child, int x, int y);

		void Remove (Control child);
	}
	
	public abstract class Layout : InstanceWidget
	{
		ILayout inner;
		
		public bool Loaded { get; private set; }
		
		public abstract IEnumerable<Control> Controls {
			get;
		}
		
		public event EventHandler<EventArgs> PreLoad;

		public virtual void OnPreLoad (EventArgs e)
		{
			if (PreLoad != null)
				PreLoad (this, e);
			inner.OnPreLoad ();
		}
		
		
		public event EventHandler<EventArgs> Load;

		public virtual void OnLoad (EventArgs e)
		{
			Loaded = true;
			if (Load != null)
				Load (this, e);
			inner.OnLoad ();
		}

		public event EventHandler<EventArgs> LoadComplete;

		public virtual void OnLoadComplete (EventArgs e)
		{
			if (LoadComplete != null)
				LoadComplete (this, e);
			inner.OnLoadComplete ();
		}
		
		public Layout (Generator g, Container container, Type type, bool initialize = true)
			: base(g, type, false)
		{
			this.Container = container;
			inner = (ILayout)Handler;
			if (initialize) {
				Initialize ();
				if (this.Container != null)
					this.Container.Layout = this;
			}
		}
		
		public Container Container {
			get;
			internal set;
		}
		
		public void Update ()
		{
			UpdateContainers (this.Container);
			inner.Update ();
		}
		
		void UpdateContainers (Container container)
		{
			foreach (var c in container.Controls.OfType<Container>()) {
				if (c.Layout != null) {
					UpdateContainers (c);
					c.Layout.Update ();
				}
			}
		}
		
		
	}
}
