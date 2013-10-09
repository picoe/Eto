using System;
using System.Collections;
using System.Collections.Generic;
using Eto.Drawing;
using System.Linq;

#if XAML
using System.Windows.Markup;
#endif

namespace Eto.Forms
{
	public partial interface IContainer : IControl
	{
		Size ClientSize { get; set; }
	}

	public abstract partial class Container : Control
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
			
			foreach (var control in Controls)
			{
				control.OnDataContextChanged(e);
			}
		}

		public override void OnPreLoad(EventArgs e)
		{
			base.OnPreLoad(e);
			
			foreach (Control control in Controls)
			{
				control.OnPreLoad(e);
			}
		}

		public override void OnLoad(EventArgs e)
		{
			foreach (Control control in Controls)
			{
				control.OnLoad(e);
			}
			
			base.OnLoad(e);
		}

		public override void OnLoadComplete(EventArgs e)
		{
			foreach (Control control in Controls)
			{
				control.OnLoadComplete(e);
			}
			
			base.OnLoadComplete(e);
		}

		public override void OnUnLoad(EventArgs e)
		{
			foreach (Control control in Controls)
			{
				control.OnUnLoad(e);
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
			foreach (var control in Controls)
			{
				control.Unbind();
			}
		}

		public override void UpdateBindings()
		{
			base.UpdateBindings();
			foreach (var control in Controls)
			{
				control.UpdateBindings();
			}
		}

		public virtual void Remove(IEnumerable<Control> controls)
		{
			foreach (var control in controls)
				Remove(control);
		}

		public virtual void RemoveAll()
		{
			Remove(this.Controls.ToArray());
		}

		public abstract void Remove(Control child);

		protected void RemoveParent(Control child, bool changeContext)
		{
			child.SetParent(null, changeContext);
		}

		protected void SetParent(Control child)
		{
			child.SetParent(this, true);
		}
	}
}
