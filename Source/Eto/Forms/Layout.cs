using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

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

	public abstract class Layout : InstanceWidget, ISupportInitialize
	{
		ILayout inner;

		public bool Initializing { get; private set; }
		
		public bool Loaded { get; private set; }

		public virtual Layout InnerLayout
		{
			get { return this; }
		}
		
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

		protected Layout (Generator g, Container container, Type type, bool initialize = true)
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

		protected Layout (Generator g, Container container, ILayout handler, bool initialize = true)
			: base (g, handler, false)
		{
			this.Container = container;
			inner = (ILayout)Handler;
			if (initialize) {
				Initialize ();
				if (this.Container != null)
					this.Container.Layout = this;
			}
		}
		
		public virtual Container Container {
			get;
			protected internal set;
		}
		
		public Layout ParentLayout {
			get { return (Container != null) ? Container.ParentLayout : null; }
		}
		
		public virtual void Update ()
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

		public virtual void BeginInit ()
		{
			Initializing = true;
		}

		public virtual void EndInit ()
		{
			Initializing = false;
		}
	}
}
