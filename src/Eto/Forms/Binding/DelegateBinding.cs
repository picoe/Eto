using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq.Expressions;
using Eto.Forms;

namespace Eto.Forms
{
	/// <summary>
	/// Direct delegate binding.
	/// </summary>
	/// <remarks>
	/// This is a direct binding, in that the get/set delegates can get/set the value directly without any associated object
	/// instance.
	/// This is used when binding directly to a property or when an object instance isn't needed to get/set a value.
	/// </remarks>
	public class DelegateBinding<TValue> : DirectBinding<TValue>
	{
		/// <summary>
		/// Gets or sets the delegate to get the value for this binding.
		/// </summary>
		/// <value>The get value delegate.</value>
		public Func<TValue> GetValue { get; set; }

		/// <summary>
		/// Gets or sets the delegate to set the value for this binding.
		/// </summary>
		/// <value>The set value delegate.</value>
		public Action<TValue> SetValue { get; set; }

		/// <summary>
		/// Gets or sets the delegate to register the change event, when needed by the consumer of this binding.
		/// </summary>
		/// <value>The add change event delegate.</value>
		public Action<EventHandler<EventArgs>> AddChangeEvent { get; set; }

		/// <summary>
		/// Gets or sets the delegate to remove the change event.
		/// </summary>
		/// <value>The remove change event delegate.</value>
		public Action<EventHandler<EventArgs>> RemoveChangeEvent { get; set; }

		/// <summary>
		/// Gets or sets the value of this binding
		/// </summary>
		/// <value>The data value.</value>
		public override TValue DataValue
		{
			get { return GetValue != null ? GetValue() : default(TValue); }
			set { if (SetValue != null) SetValue(value); }
		}

		void HandleChangedEvent(object sender, EventArgs e)
		{
			OnDataValueChanged(e);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DelegateBinding{TValue}"/> class.
		/// </summary>
		public DelegateBinding()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DelegateBinding{TValue}"/> class with the specified delegates.
		/// </summary>
		/// <param name="getValue">Delegate to get the value for the binding.</param>
		/// <param name="setValue">Delegate to set the value for the binding.</param>
		/// <param name="addChangeEvent">Delegate to register the change event, when needed by the consumer of this binding.</param>
		/// <param name="removeChangeEvent">Delegate to remove the change event.</param>
		public DelegateBinding(Func<TValue> getValue = null, Action<TValue> setValue = null, Action<EventHandler<EventArgs>> addChangeEvent = null, Action<EventHandler<EventArgs>> removeChangeEvent = null)
		{
			GetValue = getValue;
			SetValue = setValue;
			AddChangeEvent = addChangeEvent;
			RemoveChangeEvent = removeChangeEvent;
		}

		/// <summary>
		/// Hooks up the late bound events for this object
		/// </summary>
		protected override void HandleEvent(string id)
		{
			switch (id)
			{
				case DataValueChangedEvent:
					if (AddChangeEvent != null)
						AddChangeEvent(new EventHandler<EventArgs>(HandleChangedEvent));
					break;
				default:
					base.HandleEvent(id);
					break;
			}
		}

		/// <summary>
		/// Removes the late bound events for this object
		/// </summary>
		protected override void RemoveEvent(string id)
		{
			switch (id)
			{
				case DataValueChangedEvent:
					if (RemoveChangeEvent != null)
						RemoveChangeEvent(new EventHandler<EventArgs>(HandleChangedEvent));
					break;
				default:
					base.RemoveEvent(id);
					break;
			}
		}
	}

	/// <summary>
	/// Indirect binding using delegate methods
	/// </summary>
	/// <remarks>
	/// This is an indirect binding, in that the object to get/set the values from/to is passed to each of the delegates
	/// to get/set the value.
	/// This is used for things like columns in a <see cref="Forms.Grid"/> control to bind to specific values of each item
	/// in the grid.
	/// </remarks>
	/// <typeparam name="T">Type of the object this binding will get/set the values from/to</typeparam>
	/// <typeparam name="TValue">Type of the value this binding will get/set</typeparam>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class DelegateBinding<T, TValue> : IndirectBinding<TValue>
	{
		/// <summary>
		/// Gets or sets the delegate to get the value for this binding.
		/// </summary>
		/// <value>The get value delegate.</value>
		public new Func<T, TValue> GetValue { get; set; }

		/// <summary>
		/// Gets or sets the delegate to set the value for this binding.
		/// </summary>
		/// <value>The set value delegate.</value>
		public new Action<T, TValue> SetValue { get; set; }

		/// <summary>
		/// Gets or sets the delegate to register the change event, when needed by the consumer of this binding.
		/// </summary>
		/// <value>The add change event delegate.</value>
		public Func<T, EventHandler<EventArgs>, object> AddChangeEvent { get; set; }

		/// <summary>
		/// Gets or sets the delegate to remove the change event.
		/// </summary>
		/// <value>The remove change event delegate.</value>
		public Action<object, EventHandler<EventArgs>> RemoveChangeEvent { get; set; }

		/// <summary>
		/// Gets or sets the default get value, when the object instance is null.
		/// </summary>
		/// <value>The default get value.</value>
		public TValue DefaultGetValue { get; set; }

		/// <summary>
		/// Gets or sets the default set value, when the incoming value is null.
		/// </summary>
		/// <value>The default set value.</value>
		public TValue DefaultSetValue { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="DelegateBinding{T,TValue}"/> class.
		/// </summary>
		/// <param name="getValue">Delegate to get the value for the binding.</param>
		/// <param name="setValue">Delegate to set the value for the binding.</param>
		/// <param name="addChangeEvent">Delegate to register the change event, when needed by the consumer of this binding.</param>
		/// <param name="removeChangeEvent">Delegate to remove the change event.</param>
		/// <param name="defaultGetValue">Default get value, when the object instance is null.</param>
		/// <param name="defaultSetValue">Default set value, when the incoming value is null.</param>
		public DelegateBinding(Func<T, TValue> getValue, Action<T, TValue> setValue, Action<T, EventHandler<EventArgs>> addChangeEvent, Action<T, EventHandler<EventArgs>> removeChangeEvent, TValue defaultGetValue = default(TValue), TValue defaultSetValue = default(TValue))
		{
			GetValue = getValue;
			DefaultGetValue = defaultGetValue;
			SetValue = setValue;
			DefaultSetValue = defaultSetValue;
			if (addChangeEvent != null && removeChangeEvent != null)
			{
				AddChangeEvent = (obj, eh) => { addChangeEvent(obj, eh); return obj; };
				RemoveChangeEvent = (obj, eh) => { if (obj is T tobj) removeChangeEvent(tobj, eh); };
			}
			else if (addChangeEvent != null || removeChangeEvent != null)
				throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "You must either specify both the add and remove change event delegates, or pass null for both"));
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DelegateBinding{T,TValue}"/> class.
		/// </summary>
		/// <param name="getValue">Delegate to get the value for the binding.</param>
		/// <param name="setValue">Delegate to set the value for the binding.</param>
		/// <param name="addChangeEvent">Delegate to register the change event, when needed by the consumer of this binding.</param>
		/// <param name="removeChangeEvent">Delegate to remove the change event.</param>
		/// <param name="defaultGetValue">Default get value, when the object instance is null.</param>
		/// <param name="defaultSetValue">Default set value, when the incoming value is null.</param>
		public DelegateBinding(Func<T, TValue> getValue = null, Action<T, TValue> setValue = null, Func<T, EventHandler<EventArgs>, object> addChangeEvent = null, Action<object, EventHandler<EventArgs>> removeChangeEvent = null, TValue defaultGetValue = default(TValue), TValue defaultSetValue = default(TValue))
		{
			GetValue = getValue;
			DefaultGetValue = defaultGetValue;
			SetValue = setValue;
			DefaultSetValue = defaultSetValue;
			if (addChangeEvent != null && removeChangeEvent != null)
			{
				AddChangeEvent = addChangeEvent;
				RemoveChangeEvent = removeChangeEvent;
			}
			else if (addChangeEvent != null || removeChangeEvent != null)
				throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "You must either specify both the add and remove change event delegates, or pass null for both"));
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DelegateBinding{T,TValue}"/> class.
		/// </summary>
		/// <param name="getValue">Delegate to get the value for the binding.</param>
		/// <param name="setValue">Delegate to set the value for the binding.</param>
		/// <param name="notifyProperty">Name of the property to listen for change events of this binding.</param>
		/// <param name="defaultGetValue">Default get value, when the object instance is null.</param>
		/// <param name="defaultSetValue">Default set value, when the incoming value is null.</param>
		public DelegateBinding(Func<T, TValue> getValue, Action<T, TValue> setValue, string notifyProperty, TValue defaultGetValue = default(TValue), TValue defaultSetValue = default(TValue))
		{
			GetValue = getValue;
			DefaultGetValue = defaultGetValue;
			SetValue = setValue;
			DefaultSetValue = defaultSetValue;
			if (!string.IsNullOrEmpty(notifyProperty))
			{
				AddChangeEvent = (obj, eh) => { AddPropertyEvent(obj, notifyProperty, eh); return obj; };
				RemoveChangeEvent = (obj, eh) => RemovePropertyEvent(obj, eh);
			}
		}

		/// <summary>
		/// Implements the logic to get the value from the specified object
		/// </summary>
		/// <remarks>Implementors of this binding must implement this method to get the value from the specified object</remarks>
		/// <param name="dataItem">object to get the value from</param>
		/// <returns>value from this binding of the specified object</returns>
		protected override TValue InternalGetValue(object dataItem)
		{
			if (GetValue != null && dataItem is T)
			{
				return GetValue((T)dataItem);
			}
			return DefaultGetValue;
		}

		/// <summary>
		/// Implements the logic to set the value to the specified object
		/// </summary>
		/// <param name="dataItem">object to set the value to</param>
		/// <param name="value">value to set on the dataItem for this binding</param>
		protected override void InternalSetValue(object dataItem, TValue value)
		{
			if (SetValue != null && dataItem is T)
				SetValue((T)dataItem, value);
		}

		/// <summary>
		/// Wires an event handler to fire when the property of the dataItem is changed
		/// </summary>
		/// <param name="dataItem">object to detect changes on</param>
		/// <param name="handler">handler to fire when the property changes on the specified dataItem</param>
		/// <returns>binding reference used to track the event hookup, to pass to <see cref="RemoveValueChangedHandler"/> when removing the handler</returns>
		public override object AddValueChangedHandler(object dataItem, EventHandler<EventArgs> handler)
		{
			if (AddChangeEvent != null && dataItem is T typedItem)
			{
				return AddChangeEvent(typedItem, handler);
			}
			return null;
		}

		/// <summary>
		/// Removes the handler for the specified reference from <see cref="AddValueChangedHandler"/>
		/// </summary>
		/// <param name="bindingReference">Reference from the call to <see cref="AddValueChangedHandler"/></param>
		/// <param name="handler">Same handler that was set up during the <see cref="AddValueChangedHandler"/> call</param>
		public override void RemoveValueChangedHandler(object bindingReference, EventHandler<EventArgs> handler)
		{
			if (RemoveChangeEvent != null)
			{
				RemoveChangeEvent(bindingReference, handler);
			}
		}
	}
}
