using System;
using System.Collections.Generic;
using Eto.Drawing;
using System.Linq;

namespace Eto.Forms
{
	/// <summary>
	/// Handler interface for the <see cref="Container"/> control
	/// </summary>
	public interface IContainer : IControl
	{
		/// <summary>
		/// Gets or sets the size for the client area of the control
		/// </summary>
		/// <remarks>
		/// The client size differs from the <see cref="IControl.Size"/> in that it excludes the decorations of
		/// the container, such as the title bar and border around a <see cref="Window"/>, or the title and line 
		/// around a <see cref="GroupBox"/>.
		/// </remarks>
		/// <value>The size of the client area</value>
		Size ClientSize { get; set; }

		/// <summary>
		/// Gets a value indicating whether PreLoad/Load/LoadComplete/Unload events are propegated to the children controls
		/// </summary>
		/// <remarks>
		/// This is mainly used when you want to use Eto controls in your handler, such as with the <see cref="ThemedContainerHandler{TContainer,TWidget}"/>
		/// </remarks>
		/// <value><c>true</c> to recurse events to children; otherwise, <c>false</c>.</value>
		bool RecurseToChildren { get; }
	}

	/// <summary>
	/// Base class for controls that contain children controls
	/// </summary>
	public abstract class Container : Control
	{
		new IContainer Handler { get { return (IContainer)base.Handler; } }

		/// <summary>
		/// Gets or sets the size for the client area of the control
		/// </summary>
		/// <remarks>
		/// The client size differs from the <see cref="Control.Size"/> in that it excludes the decorations of
		/// the container, such as the title bar and border around a <see cref="Window"/>, or the title and line 
		/// around a <see cref="GroupBox"/>.
		/// </remarks>
		/// <value>The size of the client area</value>
		public Size ClientSize
		{
			get { return Handler.ClientSize; }
			set { Handler.ClientSize = value; }
		}

		/// <summary>
		/// Gets an enumeration of controls that are directly contained by this container
		/// </summary>
		/// <value>The contained controls.</value>
		public abstract IEnumerable<Control> Controls { get; }

		/// <summary>
		/// Gets an enumeration of all contained child controls, including controls within child containers
		/// </summary>
		/// <value>The children.</value>
		public IEnumerable<Control> Children
		{
			get
			{
				foreach (var control in Controls)
				{
					yield return control;
					var container = control as Container;
					if (container != null)
					{
						foreach (var child in container.Children)
							yield return child;
					}
				}
			}
		}

		/// <summary>
		/// Raises the <see cref="Control.DataContextChanged"/> event
		/// </summary>
		/// <remarks>
		/// Implementors may override this to fire this event on child widgets in a heirarchy. 
		/// This allows a control to be bound to its own <see cref="Control.DataContext"/>, which would be set
		/// on one of the parent control(s).
		/// </remarks>
		/// <param name="e">Event arguments</param>
		protected internal override void OnDataContextChanged(EventArgs e)
		{
			base.OnDataContextChanged(e);

			if (Handler.RecurseToChildren)
			{
				foreach (var control in Controls)
				{
					control.OnDataContextChanged(e);
				}
			}
		}

		/// <summary>
		/// Raises the <see cref="Control.PreLoad"/> event, and recurses to this container's children
		/// </summary>
		/// <param name="e">Event arguments</param>
		public override void OnPreLoad(EventArgs e)
		{
			base.OnPreLoad(e);

			if (Handler.RecurseToChildren)
			{
				foreach (Control control in Controls)
				{
					control.OnPreLoad(e);
				}
			}
		}

		/// <summary>
		/// Raises the <see cref="Control.Load"/> event, and recurses to this container's children
		/// </summary>
		/// <param name="e">Event arguments</param>
		public override void OnLoad(EventArgs e)
		{
			if (Handler.RecurseToChildren)
			{
				foreach (Control control in Controls)
				{
					control.OnLoad(e);
				}
			}
			
			base.OnLoad(e);
		}

		/// <summary>
		/// Raises the <see cref="Control.LoadComplete"/> event, and recurses to this container's children
		/// </summary>
		/// <param name="e">Event arguments</param>
		public override void OnLoadComplete(EventArgs e)
		{
			if (Handler.RecurseToChildren)
			{
				foreach (Control control in Controls)
				{
					control.OnLoadComplete(e);
				}
			}
			
			base.OnLoadComplete(e);
		}

		/// <summary>
		/// Raises the <see cref="Control.UnLoad"/> event, and recurses to this container's children
		/// </summary>
		/// <param name="e">Event arguments</param>
		public override void OnUnLoad(EventArgs e)
		{
			if (Handler.RecurseToChildren)
			{
				foreach (Control control in Controls)
				{
					control.OnUnLoad(e);
				}
			}
			
			base.OnUnLoad(e);
		}

		protected Container()
		{
		}

		/// <summary>
		/// Initializes a new instance of the Container with the specified handler
		/// </summary>
		/// <param name="handler">Pre-created handler to attach to this instance</param>
		protected Container(IContainer handler)
			: base(handler)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.Container"/> class.
		/// </summary>
		/// <param name="generator">Generator to create the handler</param>
		/// <param name="type">Type of the handler to create (must implement <see cref="IContainer"/>)</param>
		/// <param name="initialize"><c>true</c> to initialize the handler, false if the caller will initialize</param>
		[Obsolete("Use default constructor and HandlerAttribute instead")]
		protected Container(Generator generator, Type type, bool initialize = true)
			: base(generator, type, initialize)
		{
		}

		/// <summary>
		/// Initializes a new instance of the Container with the specified handler
		/// </summary>
		/// <param name="generator">Generator for the widget</param>
		/// <param name="handler">Pre-created handler to attach to this instance</param>
		/// <param name="initialize">True to call handler's Initialze method, false otherwise</param>
		[Obsolete("Use Container(IContainer) instead")]
		protected Container(Generator generator, IContainer handler, bool initialize = true)
			: base(generator, handler, initialize)
		{
		}

		/// <summary>
		/// Unbinds any bindings in the <see cref="Control.Bindings"/> collection and removes the bindings, and recurses to this container's children
		/// </summary>
		public override void Unbind()
		{
			base.Unbind();
			if (Handler.RecurseToChildren)
			{
				foreach (var control in Controls)
				{
					control.Unbind();
				}
			}
		}

		/// <summary>
		/// Updates all bindings in this widget, and recurses to this container's children
		/// </summary>
		public override void UpdateBindings()
		{
			base.UpdateBindings();
			if (Handler.RecurseToChildren)
			{
				foreach (var control in Controls)
				{
					control.UpdateBindings();
				}
			}
		}

		/// <summary>
		/// Remove the specified <paramref name="controls"/> from this container
		/// </summary>
		/// <param name="controls">Controls to remove</param>
		public virtual void Remove(IEnumerable<Control> controls)
		{
			foreach (var control in controls)
				Remove(control);
		}

		/// <summary>
		/// Removes all controls from this container
		/// </summary>
		public virtual void RemoveAll()
		{
			Remove(Controls.ToArray());
		}

		/// <summary>
		/// Removes the specified <paramref name="child"/> control
		/// </summary>
		/// <param name="child">Child to remove</param>
		public abstract void Remove(Control child);

		/// <summary>
		/// Removes the specified control from the container.
		/// </summary>
		/// <remarks>
		/// This should only be called on controls that the container owns. Otherwise, call <see cref="Control.Detach"/>
		/// </remarks>
		/// <param name="child">Child to remove from this container</param>
		protected void RemoveParent(Control child)
		{
			if (child != null && !ReferenceEquals(child.Parent, null))
			{
#if DEBUG
				if (!ReferenceEquals(child.Parent, this))
					throw new EtoException("The child control is not a child of this container. Ensure you only remove children that you own.");
#endif
				if (child.Loaded)
				{
					child.OnUnLoad(EventArgs.Empty);
				}
				child.Parent = null;
				child.OnDataContextChanged(EventArgs.Empty);
			}
		}

		/// <summary>
		/// Sets the parent of the specified <paramref name="child"/> to this container
		/// </summary>
		/// <remarks>
		/// This is used by container authors to set the parent of a child before it is added to the underlying platform control.
		/// 
		/// If this returns <c>true</c>, you should call <see cref="Control.OnLoadComplete"/> after the control has been added
		/// to the underlying platform control.
		/// </remarks>
		/// <returns><c>true</c>, if parent was set, <c>false</c> otherwise.</returns>
		/// <param name="child">Child to set the parent</param>
		protected bool SetParent(Control child)
		{
			if (child != null && !ReferenceEquals(child.Parent, this))
			{
				// Detach so parent can remove from controls collection if necessary.
				// prevents UnLoad from being called more than once when containers think a control is still a child
				// no-op if there is no parent (handled in detach)
				child.Detach();

				child.Parent = this;
				if (Loaded && !child.Loaded)
				{
					child.OnPreLoad(EventArgs.Empty);
					child.OnLoad(EventArgs.Empty);
					child.OnDataContextChanged(EventArgs.Empty);
					return true;
				}
			}
			return false;
		}
	}
}
