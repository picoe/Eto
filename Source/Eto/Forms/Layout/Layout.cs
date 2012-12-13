using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

namespace Eto.Forms
{
	/// <summary>
	/// Platform handler interface for the the <see cref="Layout"/> class
	/// </summary>
	public interface ILayout : IInstanceWidget
	{
		/// <summary>
		/// Called in the pre-load cycle before the container is shown
		/// </summary>
		void OnPreLoad ();
		
		/// <summary>
		/// Called in the load cycle after the <see cref="OnPreLoad"/>, before the container is shown
		/// </summary>
		void OnLoad ();

		/// <summary>
		/// Called after the load cycle, usually before the control is shown, or after if it is added at runtime to an existing control
		/// </summary>
		void OnLoadComplete ();

		/// <summary>
		/// Re-calculates the layout of the controls and re-positions them, if necessary
		/// </summary>
		/// <remarks>
		/// All layouts should theoretically work without having to manually update them, but in certain cases
		/// this may be necessary to be called.
		/// </remarks>
		void Update ();

		/// <summary>
		/// Method to handle when the layout has been attached to a container
		/// </summary>
		/// <remarks>
		/// Used to handle any platform specific logic that requires the container to perform
		/// </remarks>
		void AttachedToContainer();
	}

	/// <summary>
	/// Platform handler interface for positional layouts where controls are placed in an x, y grid
	/// </summary>
	public interface IPositionalLayout : ILayout
	{
		/// <summary>
		/// Adds the control to the layout given the specified co-ordinates
		/// </summary>
		/// <remarks>
		/// Adding a control typically will make it visible to the user immediately, assuming they can see the control
		/// in the current co-ordinates, and that the control's <see cref="Control.Visible"/> property is true
		/// </remarks>
		/// <param name="child">Child control to add to this layout</param>
		/// <param name="x">X co-ordinate</param>
		/// <param name="y">Y co-ordinate</param>
		void Add (Control child, int x, int y);

		/// <summary>
		/// Moves the control to the specified co-ordinates
		/// </summary>
		/// <remarks>
		/// This assumes that the control is already a child of this layout
		/// </remarks>
		/// <param name="child">Child control to move</param>
		/// <param name="x">New X co-ordinate</param>
		/// <param name="y">New Y co-ordinate</param>
		void Move (Control child, int x, int y);

		/// <summary>
		/// Removes the specified child from this layout
		/// </summary>
		/// <remarks>
		/// This assumes that the control is already a child of this layout.  This will make the child control
		/// invisible to the user
		/// </remarks>
		/// <param name="child">Child control to remove</param>
		void Remove (Control child);
	}

	public abstract class Layout : InstanceWidget, ISupportInitialize
	{
		new ILayout Handler { get { return (ILayout)base.Handler; } }
		Container container;

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
			Handler.OnPreLoad ();
		}
		
		
		public event EventHandler<EventArgs> Load;

		public virtual void OnLoad (EventArgs e)
		{
			Loaded = true;
			if (Load != null)
				Load (this, e);
			Handler.OnLoad ();
		}

		public event EventHandler<EventArgs> LoadComplete;

		public virtual void OnLoadComplete (EventArgs e)
		{
			if (LoadComplete != null)
				LoadComplete (this, e);
			Handler.OnLoadComplete ();
		}

		protected Layout (Generator g, Container container, Type type, bool initialize = true)
			: base(g, type, false)
		{
            if (container != null &&
                container.ControlObject is Container)
                container = container.ControlObject as Container;

			this.container = container;
			if (initialize) {
				Initialize ();
				if (this.Container != null)
					this.Container.Layout = this;
			}
		}

		protected Layout (Generator g, Container container, ILayout handler, bool initialize = true)
			: base (g, handler, false)
		{
			this.container = container;
			if (initialize) {
				Initialize ();
				if (this.Container != null)
					this.Container.Layout = this;
			}
		}
		
		public virtual Container Container {
			get { return container; }
			protected internal set {
				container = value;
				Handler.AttachedToContainer ();
			}
		}
		
		public Layout ParentLayout {
			get { return (Container != null) ? Container.ParentLayout : null; }
		}
		
		public virtual void Update ()
		{
			UpdateContainers (this.Container);
			Handler.Update ();
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
		
		protected void SetInnerLayout (bool load)
		{
			if (Container != null)
				Container.SetInnerLayout (load);
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
