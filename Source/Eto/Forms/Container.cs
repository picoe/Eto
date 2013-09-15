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

	public partial interface IDockContainer : IContainer
	{
		Control Content { get; set; }

		Padding Padding { get; set; }
	}


	[ContentProperty("Content")]
	public abstract partial class DockContainer : Container
	{
		new IDockContainer Handler { get { return (IDockContainer)base.Handler; } }

		public static Padding DefaultPadding = Padding.Empty;

		public override IEnumerable<Control> Controls
		{
			get
			{ 
				var content = Handler.Content;
				if (content != null)
					yield return content; 
			}
		}

		public Padding Padding
		{
			get { return Handler.Padding; }
			set { Handler.Padding = value; }
		}

		public Control Content {
			get { return Handler.Content; }
			set {
				var old = Handler.Content;
				if (old != value) {
					if (old != null)
						old.SetParent(null);
					if (value != null) {
						value.SetParent(null, false);
						var load = Loaded && !value.Loaded;
						if (load) {
							value.OnPreLoad (EventArgs.Empty);
							value.OnLoad (EventArgs.Empty);
						}
						Handler.Content = value;
						value.SetParent(this);
						if (load)
							value.OnLoadComplete (EventArgs.Empty);
					}
					else
						Handler.Content = value;
				}
			}
		}

		[Obsolete("Use Content property instead")]
		public Control Layout { get { return Content; } set { Content = value; } }
	
		protected DockContainer (Generator generator, Type type, bool initialize = true)
			: base(generator, type, initialize)
		{
		}

		/// <summary>
		/// Initializes a new instance of the Container with the specified handler
		/// </summary>
		/// <param name="generator">Generator for the widget</param>
		/// <param name="handler">Pre-created handler to attach to this instance</param>
		/// <param name="initialize">True to call handler's Initialze method, false otherwise</param>
		protected DockContainer (Generator generator, IDockContainer handler, bool initialize = true)
			: base(generator, handler, initialize)
		{
		}

	}

	public abstract partial class Container : Control
	{
		new IContainer Handler { get { return (IContainer)base.Handler; } }

		public Size ClientSize {
			get { return Handler.ClientSize; }
			set { Handler.ClientSize = value; }
		}

		public abstract IEnumerable<Control> Controls
		{
			get;
		}

		public IEnumerable<Control> Children {
			get {
				foreach (var control in Controls) {
					yield return control;
					var container = control as Container;
					if (container != null) {
						foreach (var child in container.Children)
							yield return child;
					}
				}
			}
		}

		protected internal override void OnDataContextChanged (EventArgs e)
		{
			base.OnDataContextChanged (e);
			
			foreach (var control in Controls) {
				control.OnDataContextChanged (e);
			}
		}

		public override void OnPreLoad (EventArgs e)
		{
			base.OnPreLoad (e);
			
			foreach (Control control in Controls) {
				control.OnPreLoad (e);
			}
		}
		
		public override void OnLoad (EventArgs e)
		{
			foreach (Control control in Controls) {
				control.OnLoad (e);
			}
			
			base.OnLoad (e);
		}

		public override void OnLoadComplete (EventArgs e)
		{
			foreach (Control control in Controls) {
				control.OnLoadComplete (e);
			}
			
			base.OnLoadComplete (e);
		}

		public override void OnUnLoad (EventArgs e)
		{
			foreach (Control control in Controls) {
				control.OnUnLoad (e);
			}
			
			base.OnUnLoad (e);
		}

		protected Container (Generator g, Type type, bool initialize = true)
			: base(g, type, initialize)
		{
		}

		/// <summary>
		/// Initializes a new instance of the Container with the specified handler
		/// </summary>
		/// <param name="generator">Generator for the widget</param>
		/// <param name="handler">Pre-created handler to attach to this instance</param>
		/// <param name="initialize">True to call handler's Initialze method, false otherwise</param>
		protected Container (Generator generator, IContainer handler, bool initialize = true)
			: base(generator, handler, initialize)
		{
		}

		public override void Unbind ()
		{
			base.Unbind ();
			foreach (var control in Controls) {
				control.Unbind ();
			}
		}
		
		public override void UpdateBindings ()
		{
			base.UpdateBindings ();
			foreach (var control in Controls) {
				control.UpdateBindings ();
			}
		}
	}
}
