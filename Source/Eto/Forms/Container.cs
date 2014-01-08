using System;
using System.Collections.Generic;
using Eto.Drawing;
using System.Linq;

namespace Eto.Forms
{
	public interface IContainer : IControl
	{
		Size ClientSize { get; set; }

		bool RecurseToChildren { get; }
	}

	public abstract class Container : Control
	{
		new IContainer Handler { get { return (IContainer)base.Handler; } }

		public Size ClientSize
		{
			get { return Handler.ClientSize; }
			set { Handler.ClientSize = value; }
		}

		public abstract IEnumerable<Control> Controls
		{
			get;
		}

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

		protected Container(Generator g, Type type, bool initialize = true)
			: base(g, type, initialize)
		{
		}

		/// <summary>
		/// Initializes a new instance of the Container with the specified handler
		/// </summary>
		/// <param name="generator">Generator for the widget</param>
		/// <param name="handler">Pre-created handler to attach to this instance</param>
		/// <param name="initialize">True to call handler's Initialze method, false otherwise</param>
		protected Container(Generator generator, IContainer handler, bool initialize = true)
			: base(generator, handler, initialize)
		{
		}

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

		public virtual void Remove(IEnumerable<Control> controls)
		{
			foreach (var control in controls)
				Remove(control);
		}

		public virtual void RemoveAll()
		{
			Remove(Controls.ToArray());
		}

		public abstract void Remove(Control child);

		/// <summary>
		/// Removes the specified control from the container.
		/// This should only be called on controls that the container owns. Otherwise, call <see cref="Control.Detach"/>
		/// </summary>
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
