using System;
using System.Collections.Generic;

namespace Eto.Forms
{
	/// <summary>
	/// Interface to specify that the <see cref="BindableWidget"/> contains child bindable widgets that
	/// should participate in binding events.
	/// </summary>
	public interface IBindableWidgetContainer
	{
		/// <summary>
		/// Gets the children that in the container.
		/// </summary>
		/// <value>The children bindable widgets.</value>
		IEnumerable<BindableWidget> Children { get; }
	}

	/// <summary>
	/// Base widget to support binding with the <see cref="IBindable"/> interface.
	/// </summary>
	public abstract class BindableWidget : Widget, IBindable
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="BindableWidget"/> class.
		/// </summary>
		protected BindableWidget()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="BindableWidget"/> class with the specified platform handler.
		/// </summary>
		/// <param name="handler">Handler interface for the widget.</param>
		protected BindableWidget(IHandler handler)
			: base(handler)
		{
		}

		#region IBindable implementation

		/// <summary>
		/// Event to handle when the <see cref="DataContext"/> has changed
		/// </summary>
		/// <remarks>
		/// This may be fired in the event of a parent in the hierarchy setting the data context.
		/// For example, the <see cref="Forms.Container"/> widget fires this event when it's event is fired.
		/// </remarks>
		public event EventHandler<EventArgs> DataContextChanged
		{
			add { Properties.AddEvent(DataContextChangedKey, value); }
			remove { Properties.RemoveEvent(DataContextChangedKey, value); }
		}

		static readonly object DataContextChangedKey = new object();

		/// <summary>
		/// Raises the <see cref="DataContextChanged"/> event
		/// </summary>
		/// <remarks>
		/// Implementors may override this to fire this event on child widgets in a heirarchy. 
		/// 
		/// Alternatively, implement <see cref="IBindableWidgetContainer"/> for your widget and this will
		/// trigger any events on the children automatically.
		/// 
		/// This allows a control to be bound to its own <see cref="DataContext"/>, which would be set
		/// on one of the parent control(s).
		/// </remarks>
		/// <param name="e">Event arguments</param>
		protected virtual void OnDataContextChanged(EventArgs e)
		{
			Properties.TriggerEvent(DataContextChangedKey, this, e);

			if (this is IBindableWidgetContainer container)
			{
				foreach (var child in container.Children)
					child.TriggerDataContextChanged();
			}
		}

		static readonly object Parent_Key = new object();

		/// <summary>
		/// Gets or sets the parent widget which this widget has been added to, if any
		/// </summary>
		/// <remarks>
		/// When implementing child widgets that can participate in the context based binding (MVVM),
		/// You can set this to the parent widget when it is added or removed to it (usually via a collection).
		/// 
		/// Note that you should typically provide a <code>public new [ParentWidget] Parent { get; }</code> property 
		/// so that consumers of your API cannot set the parent to an invalid value.
		/// </remarks>
		/// <value>The parent widget, or null if there is no parent</value>
		public Widget Parent
		{
			get => Properties.Get<Widget>(Parent_Key);
			internal protected set
			{
				if (Properties.TrySet(Parent_Key, value))
				{
					if (!HasDataContext && !(DataContext is null))
						OnDataContextChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Finds a control in the parent hierarchy with the specified type and <see cref="Widget.ID"/> if specified
		/// </summary>
		/// <returns>The parent if found, or null if not found</returns>
		/// <param name="id">Identifier of the parent control to find, or null to ignore</param>
		/// <typeparam name="T">The type of control to find</typeparam>
		public T FindParent<T>(string id = null)
			where T : BindableWidget
		{
			var control = Parent;
			while (control != null)
			{
				if (control is T ctl && (string.IsNullOrEmpty(id) || control.ID == id))
				{
					return ctl;
				}
				var bindable = control as BindableWidget;
				control = bindable?.Parent;
			}
			return default(T);
		}

		/// <summary>
		/// Finds a control in the parent hierarchy with the specified type and <see cref="Widget.ID"/> if specified
		/// </summary>
		/// <returns>The parent if found, or null if not found.</returns>
		/// <param name="type">The type of control to find.</param>
		/// <param name="id">Identifier of the parent control to find, or null to find by type only.</param>
		public Widget FindParent(Type type, string id = null)
		{
			var control = Parent;
			while (control != null)
			{
				if ((type == null || type.IsInstanceOfType(control)) && (string.IsNullOrEmpty(id) || control.ID == id))
				{
					return control;
				}
				var bindable = control as BindableWidget;
				control = bindable?.Parent;
			}
			return null;
		}

		/// <summary>
		/// Finds a control in the parent hierarchy with the specified <paramref name="id"/>
		/// </summary>
		/// <returns>The parent if found, or null if not found.</returns>
		/// <param name="id">Identifier of the parent control to find.</param>
		public Widget FindParent(string id) => FindParent(null, id);

		/// <summary>
		/// Gets an enumeration of all parent widgets in the heirarchy by traversing the <see cref="Parent"/> property.
		/// </summary>
		public IEnumerable<Widget> Parents
		{
			get
			{
				var control = Parent;
				while (control != null)
				{
					yield return control;

					var bindable = control as BindableWidget;
					control = bindable?.Parent;
				}
			}
		}

		static readonly object DataContext_Key = new object();

		/// <summary>
		/// Gets or sets the data context for this widget for binding
		/// </summary>
		/// <remarks>
		/// Subclasses may override the standard behaviour so that hierarchy of widgets can be taken into account.
		/// 
		/// For example, a Control may return the data context of a parent, if it is not set explicitly.
		/// </remarks>
		public object DataContext
		{
			get
			{
				if (Properties.TryGetValue(DataContext_Key, out var context))
					return context;
				return (Parent as IBindable)?.DataContext;
			}
			set
			{
				if (Properties.TrySet(DataContext_Key, value))
					OnDataContextChanged(EventArgs.Empty);
			}
		}

		internal bool HasDataContext => Properties.ContainsKey(DataContext_Key);

		static readonly  object Bindings_Key = new object();

		/// <summary>
		/// Gets the collection of bindings that are attached to this widget
		/// </summary>
		public BindingCollection Bindings => Properties.Create(Bindings_Key, () => new BindingCollection());

		#endregion

		internal void TriggerDataContextChanged()
		{
			if (!HasDataContext)
				OnDataContextChanged(EventArgs.Empty);
		}

		/// <summary>
		/// Unbinds any bindings in the <see cref="Bindings"/> collection and removes the bindings
		/// </summary>
		/// <remarks>
		/// When you implement <see cref="IBindableWidgetContainer"/>, this will also unbind any of its children.
		/// </remarks>
		public virtual void Unbind()
		{
			var bindings = Properties.Get<BindingCollection>(Bindings_Key);
			if (bindings != null)
			{
				bindings.Unbind();
				Properties.Remove(Bindings_Key);
			}

			if (this is IBindableWidgetContainer container)
			{
				foreach (var child in container.Children)
					child.Unbind();
			}
		}

		/// <summary>
		/// Updates all bindings in this widget
		/// </summary>
		/// <remarks>
		/// When you implement <see cref="IBindableWidgetContainer"/>, this will also update bindings for all of its children.
		/// </remarks>
		public virtual void UpdateBindings(BindingUpdateMode mode = BindingUpdateMode.Source)
		{
			var bindings = Properties.Get<BindingCollection>(Bindings_Key);
			if (bindings != null)
			{
				bindings.Update(mode);
			}

			if (this is IBindableWidgetContainer container)
			{
				foreach (var child in container.Children)
					child.UpdateBindings(mode);
			}
		}
	}
}
