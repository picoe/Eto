using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Eto.Drawing;

namespace Eto.Forms
{
	/// <summary>
	/// Base class for controls that contain children controls
	/// </summary>
	public abstract class Container : Control, IBindableWidgetContainer
	{
		new IHandler Handler => (IHandler)base.Handler;

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
		/// Gets an enumeration of logical controls that are directly contained by this container
		/// </summary>
		/// <remarks>
		/// These controls are directly contained by the container, but do not include controls that are used to form
		/// the visual appearance, if any.  Controls like the <see cref="StackLayout"/>, <see cref="DynamicLayout"/> or a 
		/// customized container using the <see cref="ThemedContainerHandler{TControl,TWidget,TCallback}"/> should not 
		/// include any intermediate controls.
		/// </remarks>
		/// <value>The contained controls.</value>
		public abstract IEnumerable<Control> Controls { get; }

		/// <summary>
		/// Gets the logical controls, so we don't chain binding events from the visual tree to the logical tree
		/// </summary>
		/// <value>The logical controls.</value>
		IEnumerable<Control> LogicalControls
		{
			get
			{
				foreach (var control in Controls)
				{
					if (ReferenceEquals(control.InternalLogicalParent, this))
						yield return control;
				}
			}
		}

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
					if (control is Container container)
					{
						foreach (var child in container.Children)
							yield return child;
					}
				}
			}
		}

		/// <summary>
		/// Gets an enumeration of all contained child controls in the visual tree, including controls within child containers.
		/// </summary>
		/// <value>The visual children.</value>
		public IEnumerable<Control> VisualChildren
		{
			get
			{
				foreach (var control in VisualControls)
				{
					yield return control;
					if (control is Container container)
					{
						foreach (var child in container.VisualChildren)
							yield return child;
					}
				}
			}
		}

		static readonly object StyleProvider_Key = new object();
		static readonly object DefaultStyleProvider_Key = new object();

		/// <summary>
		/// Gets or sets the style provider for this container.
		/// </summary>
		/// <remarks>
		/// The style provider is used to style this container and its children.
		/// </remarks>
		/// <value>The style provider.</value>
		public IStyleProvider StyleProvider
		{
			get => Properties.Get<IStyleProvider>(StyleProvider_Key) ?? Properties.Get<DefaultStyleProvider>(DefaultStyleProvider_Key);
			set => Properties.Set(StyleProvider_Key, value);
		}

		/// <summary>
		/// Gets the default style provider for this container.
		/// </summary>
		/// <remarks>
		/// Use this to apply styles to any child controls of this container.
		/// By default, styles will apply to all children, including children of children unless
		/// <see cref="DefaultStyleProvider.Inherit"/> is set to <c>false</c>.
		/// 
		/// Typically, you would set Inherit to false when creating composite controls
		/// that already have all their styles applied and you don't want any other styles
		/// to be inherited.
		/// </remarks>
		/// <value>The default style provider for this container.</value>
		public DefaultStyleProvider Styles => Properties.Create<DefaultStyleProvider>(DefaultStyleProvider_Key);

		IEnumerable<BindableWidget> IBindableWidgetContainer.Children => LogicalControls;

		/// <inheritdoc />
		protected override void ApplyStyles(object widget, string style)
		{
			var styleProvider = StyleProvider;

			if (styleProvider == null)
			{
				// no styles for this container, check the parent
				Parent?.ApplyStyles(widget, style);
				return;
			}

			// apply parent styles first, if this provider allows inheriting them
			if (styleProvider.Inherit)
				Parent?.ApplyStyles(widget, style);

			// now apply any of this providers' styles, which may override parent styles
			styleProvider.ApplyCascadingStyle(this, widget, style);
		}

		/// <summary>
		/// Handles when the <see cref="Style"/> is changed.
		/// </summary>
		/// <remarks>
		/// This applies the cascading styles to the control and any of its children.
		/// </remarks>
		protected override void OnStyleChanged(EventArgs e)
		{
			base.OnStyleChanged(e);
			if (Loaded && Handler.RecurseToChildren)
			{
				foreach (Control control in VisualControls)
				{
					control.TriggerStyleChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Raises the <see cref="Control.PreLoad"/> event, and recurses to this container's children
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected override void OnPreLoad(EventArgs e)
		{
			base.OnPreLoad(e);

			if (Handler.RecurseToChildren)
			{
				foreach (Control control in VisualControls)
				{
					control.TriggerPreLoad(e);
				}
			}
		}

		/// <summary>
		/// Raises the <see cref="Control.Load"/> event, and recurses to this container's children
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected override void OnLoad(EventArgs e)
		{
			if (Handler.RecurseToChildren)
			{
				foreach (Control control in VisualControls)
				{
					control.TriggerLoad(e);
				}
			}
			
			base.OnLoad(e);
		}

		/// <summary>
		/// Raises the <see cref="Control.LoadComplete"/> event, and recurses to this container's children
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected override void OnLoadComplete(EventArgs e)
		{
			base.OnLoadComplete(e);

			if (Handler.RecurseToChildren)
			{
				foreach (Control control in VisualControls)
				{
					control.TriggerLoadComplete(e);
				}
			}
		}

		/// <summary>
		/// Raises the <see cref="Control.UnLoad"/> event, and recurses to this container's children
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected override void OnUnLoad(EventArgs e)
		{
			if (!IsDisposed && Handler != null && Handler.RecurseToChildren)
			{
				foreach (Control control in VisualControls)
				{
					control.TriggerUnLoad(e);
				}
			}
			
			base.OnUnLoad(e);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.Container"/> class.
		/// </summary>
		protected Container()
		{
		}

		/// <summary>
		/// Initializes a new instance of the Container with the specified handler
		/// </summary>
		/// <param name="handler">Pre-created handler to attach to this instance</param>
		protected Container(IHandler handler)
			: base(handler)
		{
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
			Remove(VisualControls.ToArray());
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
			if (Handler is IThemedControlHandler)
			{
				if (!ReferenceEquals(child, null))
					RemoveLogicalParent(child);
				return;
			}
			if (!ReferenceEquals(child, null) && !ReferenceEquals(child.VisualParent, null))
			{
#if DEBUG
				if (!ReferenceEquals(child.VisualParent, this))
					throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "The child control is not a child of this container. Ensure you only remove children that you own."));
#endif
				if (child.Loaded)
				{
					child.TriggerUnLoad(EventArgs.Empty);
				}
				child.VisualParent = null;
				if (ReferenceEquals(child.InternalLogicalParent, this))
					child.InternalLogicalParent = null;
			}
		}

		/// <summary>
		/// Sets the logical (non-visual) parent of the specified child control.
		/// </summary>
		/// <remarks>
		/// This is used by layout containers that may use other Eto controls to layout its children that shouldn't be 
		/// necessary for users to be concerned about.
		/// 
		/// For example, the <see cref="StackLayout"/> uses a TableLayout internally (for now), but the TableLayout
		/// should not be part of the logical tree of controls.
		/// 
		/// If this is not set, then the logical parent of a child is the same as the visual parent.
		/// </remarks>
		/// <param name="child">Child to set the logical parent to this container.</param>
		protected void SetLogicalParent(Control child)
		{
			if (ReferenceEquals(child, null))
				return;
			child.InternalLogicalParent = this;
		}

		/// <summary>
		/// Removes the child control from the logical parent of this container.
		/// </summary>
		/// <remarks>
		/// This should be used from containers that use other Eto controls to layout its children but should not be part
		/// of the logical heirarchy.
		/// </remarks>
		/// <seealso cref="SetLogicalParent"/>
		/// <param name="child">Child to remove from this container as the logical parent.</param>
		protected void RemoveLogicalParent(Control child)
		{
			if (ReferenceEquals(child, null))
				return;
			if (!ReferenceEquals(child.InternalLogicalParent, this))
			{
				throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "The child control is not a logical child of this container. Ensure you only remove children that you own."));
			}
			child.Detach();
			child.InternalLogicalParent = null;
		}

		/// <summary>
		/// Sets the parent of the specified <paramref name="child"/> to this container
		/// </summary>
		/// <remarks>
		/// This is used by container authors to set the parent of a child before it is added to the underlying platform control.
		/// 
		/// The <paramref name="assign"/> parameter should call the handler method to add the child to the parent.
		/// </remarks>
		/// <returns><c>true</c>, if parent was set, <c>false</c> otherwise.</returns>
		/// <param name="child">Child to set the parent</param>
		/// <param name="assign">Method to assign the child to the handler</param>
		/// <param name="previousChild">Previous child that the new child is replacing.</param>
		protected void SetParent(Control child, Action assign = null, Control previousChild = null)
		{
			if (ReferenceEquals(child, this))
			{
				throw new InvalidOperationException("Cannot assign a control as a child of itself.");
			}
			if (Handler is IThemedControlHandler)
			{
				if (!ReferenceEquals(previousChild, null))
					RemoveLogicalParent(previousChild);

				if (!ReferenceEquals(child, null) && ReferenceEquals(child.LogicalParent, null))
					SetLogicalParent(child);
				assign();
				return;
			}
			if (!ReferenceEquals(previousChild, null) && !ReferenceEquals(previousChild, child))
			{
#if DEBUG
				if (!ReferenceEquals(previousChild.VisualParent, this))
					throw new ArgumentException("The previous child control is not a child of this container. Ensure you only remove children that you own.");
#endif

				if (!ReferenceEquals(previousChild.VisualParent, null))
				{
					if (previousChild.Loaded)
					{
						previousChild.TriggerUnLoad(EventArgs.Empty);
					}
					previousChild.VisualParent = null;
				}

				if (ReferenceEquals(previousChild.InternalLogicalParent, this))
				{
					previousChild.InternalLogicalParent = null;
				}
			}
			if (!ReferenceEquals(child, null) && !ReferenceEquals(child.VisualParent, this))
			{
				// Detach so parent can remove from controls collection if necessary.
				// prevents UnLoad from being called more than once when containers think a control is still a child
				// no-op if there is no parent (handled in detach)
				child.Detach();

				if (ReferenceEquals(child.InternalLogicalParent, null))
					SetLogicalParent(child);
				child.VisualParent = this;
				if (Loaded && !child.Loaded)
				{
					using (child.Platform.Context)
					{
						child.TriggerPreLoad(EventArgs.Empty);
						child.TriggerLoad(EventArgs.Empty);
						assign?.Invoke();
						child.TriggerLoadComplete(EventArgs.Empty);
					}
					return;
				}
			}
			assign?.Invoke();
		}

		/// <summary>
		/// Finds a child control in this container or any of its child containers with the specified <paramref name="id"/>
		/// </summary>
		/// <returns>The child control if found, or null if not.</returns>
		/// <param name="id">Optional identifier of the control to find that matches the <see cref="Widget.ID"/>.</param>
		/// <typeparam name="T">The type of control to find.</typeparam>
		public T FindChild<T>(string id = null)
			where T: Control
		{
			if (string.IsNullOrEmpty(id))
				return Children.OfType<T>().FirstOrDefault();
			else
				return Children.OfType<T>().FirstOrDefault(r => r.ID == id);
		}

		/// <summary>
		/// Finds a child control in this container or any of its child containers with the specified <paramref name="type"/>
		/// </summary>
		/// <returns>The child control if found, or null if not.</returns>
		/// <param name="type">The type of control to find.</param>
		/// <param name="id">Optional identifier of the control to find that matches the <see cref="Widget.ID"/>.</param>
		public Control FindChild(Type type, string id = null)
		{
			if (id == null)
				throw new ArgumentNullException(nameof(type));
			if (string.IsNullOrEmpty(id))
				return Children.FirstOrDefault(r => type.IsInstanceOfType(r));
			else
				return Children.FirstOrDefault(r => type.IsInstanceOfType(r) && r.ID == id);
		}

		/// <summary>
		/// Finds a child control in this container or any of its child containers with the specified <paramref name="id"/>.
		/// </summary>
		/// <returns>The child control if found, or null if not.</returns>
		/// <param name="id">Identifier of the control to find that matches the <see cref="Widget.ID"/>.</param>
		public Control FindChild(string id)
		{
			if (id == null)
				throw new ArgumentNullException(nameof(id));
			return Children.FirstOrDefault(r => r.ID == id);
		}


		/// <summary>
		/// Handler interface for the <see cref="Container"/> control
		/// </summary>
		public new interface IHandler : Control.IHandler
		{
			/// <summary>
			/// Gets or sets the size for the client area of the control
			/// </summary>
			/// <remarks>
			/// The client size differs from the <see cref="P:Eto.Forms.Control.IHandler.Size"/> in that it excludes the decorations of
			/// the container, such as the title bar and border around a <see cref="Window"/>, or the title and line 
			/// around a <see cref="GroupBox"/>.
			/// </remarks>
			/// <value>The size of the client area</value>
			Size ClientSize { get; set; }

			/// <summary>
			/// Gets a value indicating whether PreLoad/Load/LoadComplete/Unload events are propegated to the children controls
			/// </summary>
			/// <remarks>
			/// This is mainly used when you want to use Eto controls in your handler, such as with the <see cref="ThemedContainerHandler{TContainer,TWidget,TCallback}"/>
			/// </remarks>
			/// <value><c>true</c> to recurse events to children; otherwise, <c>false</c>.</value>
			bool RecurseToChildren { get; }
		}
	}
}
