using System;
using Eto.Forms;
using System.Linq.Expressions;

namespace Eto.Forms
{
	/// <summary>
	/// Binding for a particular object to get/set values from/to
	/// </summary>
	/// <remarks>
	/// This binding provides a way to get/set values for a particular object.  This uses
	/// a <see cref="IndirectBinding{T}"/> as its logic to actually retrieve/set the values.
	/// 
	/// This acts as a bridge between the <see cref="IndirectBinding{T}"/> and <see cref="DirectBinding{T}"/>
	/// so that you can utilize the <see cref="DirectBinding{T}.DataValueChanged"/> method.
	/// 
	/// Typically, one would use the <see cref="PropertyBinding{T}"/>, or the <see cref="C:ObjectBinding{T,TValue}(T, string)"/>
	/// constructor to hook up this binding to a particular property of the specified object
	/// </remarks>
	/// <typeparam name="TValue">The type of value for the binding.</typeparam>
	public class ObjectBinding<TValue> : ObjectBinding<object, TValue>
	{
		/// <summary>
		/// Initializes a new instance of the ObjectBinding with the specified object and binding to get/set values with
		/// </summary>
		/// <param name="dataItem">object to get/set values from</param>
		/// <param name="innerBinding">binding to use to get/set the values from the dataItem</param>
		public ObjectBinding(object dataItem, IndirectBinding<TValue> innerBinding)
			: base(dataItem, innerBinding)
		{
		}

		/// <summary>
		/// Initializes a new instance of the ObjectBinding with the specified object and property for a <see cref="PropertyBinding{T}"/>
		/// </summary>
		/// <remarks>
		/// This is a shortcut to set up the binding to get/set values from a particular property of the specified object
		/// </remarks>
		/// <param name="dataItem">object to get/set values from</param>
		/// <param name="property">property of the dataItem to get/set values</param>
		public ObjectBinding(object dataItem, string property)
			: base(dataItem, property)
		{
		}

	}

	/// <summary>
	/// Binding for a particular object to get/set values from/to
	/// </summary>
	/// <remarks>
	/// This binding provides a way to get/set values for a particular object.  This uses
	/// a <see cref="IndirectBinding{T}"/> as its logic to actually retrieve/set the values.
	/// 
	/// This acts as a bridge between the <see cref="IndirectBinding{T}"/> and <see cref="DirectBinding{T}"/>
	/// so that you can utilize the <see cref="DirectBinding{T}.DataValueChanged"/> method.
	/// 
	/// Typically, one would use the <see cref="PropertyBinding{T}"/>, or the <see cref="C:ObjectBinding{T,TValue}(T, string)"/>
	/// constructor to hook up this binding to a particular property of the specified object
	/// </remarks>
	/// <typeparam name="T">The type of object to bind to.</typeparam>
	/// <typeparam name="TValue">The type of value for the binding.</typeparam>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class ObjectBinding<T, TValue> : DirectBinding<TValue>
	{
		object dataValueChangedReference;
		bool dataValueChangedHandled;
		T dataItem;

		/// <summary>
		/// Gets the binding used to get/set the values from the <see cref="DataItem"/>
		/// </summary>
		public IndirectBinding<TValue> InnerBinding { get; private set; }

		/// <summary>
		/// Gets the object to get/set the values using the <see cref="InnerBinding"/>
		/// </summary>
		public T DataItem
		{
			get { return dataItem; }
			set
			{
				var hasValueChanged = dataValueChangedHandled;
				if (hasValueChanged)
					RemoveEvent(DataValueChangedEvent);
				dataItem = value;
				OnDataValueChanged(EventArgs.Empty);
				if (hasValueChanged)
					HandleEvent(DataValueChangedEvent);
			}
		}

		/// <summary>
		/// Gets or sets the default value to use when setting the value for this binding when input value is null
		/// </summary>
		public TValue SettingNullValue
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the default value to use when getting the value for this binding when the <see cref="DataItem"/> or property value is null
		/// </summary>
		public TValue GettingNullValue
		{
			get;
			set;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ObjectBinding{T,TValue}"/> class.
		/// </summary>
		/// <param name="dataItem">Data item to get/set the values from/to.</param>
		/// <param name="getValue">Delegate to get the value from the object.</param>
		/// <param name="setValue">Delegate to set the value to the object.</param>
		/// <param name="addChangeEvent">Delegate to add the change event.</param>
		/// <param name="removeChangeEvent">Delegate to remove the chang event.</param>
		public ObjectBinding(T dataItem, Func<T, TValue> getValue, Action<T, TValue> setValue = null, Action<T, EventHandler<EventArgs>> addChangeEvent = null, Action<T, EventHandler<EventArgs>> removeChangeEvent = null)
			: this(dataItem, new DelegateBinding<T, TValue>(getValue, setValue, addChangeEvent, removeChangeEvent))
		{
		}

		/// <summary>
		/// Initializes a new instance of the ObjectBinding with the specified object and property for a <see cref="PropertyBinding{T}"/>
		/// </summary>
		/// <remarks>
		/// This is a shortcut to set up the binding to get/set values from a particular property of the specified object
		/// </remarks>
		/// <param name="dataItem">object to get/set values from</param>
		/// <param name="property">property of the dataItem to get/set values</param>
		public ObjectBinding(T dataItem, string property)
			: this(dataItem, Property<TValue>(property))
		{
		}

		/// <summary>
		/// Initializes a new instance of the ObjectBinding with the specified object and binding to get/set values with
		/// </summary>
		/// <param name="dataItem">object to get/set values from</param>
		/// <param name="innerBinding">binding to use to get/set the values from the dataItem</param>
		public ObjectBinding(T dataItem, IndirectBinding<TValue> innerBinding)
		{
			this.dataItem = dataItem;
			InnerBinding = innerBinding;
			InnerBinding.Changed += HandleInnerBindingChanged;
			InnerBinding.Changing += HandleInnerBindingChanging;
		}

		void HandleInnerBindingChanging(object sender, BindingChangingEventArgs e)
		{
			OnChanging(e);
		}

		void HandleInnerBindingChanged(object sender, BindingChangedEventArgs e)
		{
			OnChanged(e);
		}

		/// <summary>
		/// Gets or sets the value of this binding on the bound object
		/// </summary>
		/// <remarks>
		/// This uses the <see cref="InnerBinding"/> on the <see cref="DataItem"/> to get/set the value
		/// </remarks>
		public override TValue DataValue
		{
			get
			{
				var val = InnerBinding.GetValue(DataItem);
				return Equals(val, default(T)) ? GettingNullValue : val;
			}
			set
			{
				InnerBinding.SetValue(DataItem, Equals(value, default(T)) ? SettingNullValue : value);
			}
		}

		/// <summary>
		/// Hooks up the late bound events for this object
		/// </summary>
		protected override void HandleEvent(string id)
		{
			switch (id)
			{
				case DataValueChangedEvent:
					if (!dataValueChangedHandled)
					{
						dataValueChangedReference = InnerBinding.AddValueChangedHandler(
							DataItem,
							new EventHandler<EventArgs>(HandleChangedEvent)
						);
						dataValueChangedHandled = true;
					}
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
					if (dataValueChangedHandled)
					{
						InnerBinding.RemoveValueChangedHandler(
							dataValueChangedReference,
							new EventHandler<EventArgs>(HandleChangedEvent)
						);
						dataValueChangedReference = null;
						dataValueChangedHandled = false;
					}
					break;
				default:
					base.RemoveEvent(id);
					break;
			}
		}

		/// <summary>
		/// Unbinds this binding
		/// </summary>
		public override void Unbind()
		{
			base.Unbind();
			
			RemoveEvent(DataValueChangedEvent);
			InnerBinding.Unbind();
		}

		void HandleChangedEvent(object sender, EventArgs e)
		{
			OnDataValueChanged(e);
		}

		/// <summary>
		/// Creates a new dual binding between the specified <paramref name="sourceBinding"/> and this binding.
		/// </summary>
		/// <remarks>
		/// This creates a <see cref="DualBinding{TValue}"/> between the specified <paramref name="sourceBinding"/> and this binding.
		/// You must keep a reference to the binding to unbind when finished.
		/// </remarks>
		/// <param name="sourceBinding">Source binding to bind from.</param>
		/// <param name="mode">Dual binding mode.</param>
		public virtual DualBinding<TValue> Bind(DirectBinding<TValue> sourceBinding, DualBindingMode mode = DualBindingMode.TwoWay)
		{
			return new DualBinding<TValue>(sourceBinding, this, mode);
		}

		/// <summary>
		/// Creates a new dual binding using a <see cref="DelegateBinding{TValue}"/> with the specified delegates and this binding.
		/// </summary>
		/// <remarks>
		/// This creates a <see cref="DualBinding{TValue}"/> between a new <see cref="DelegateBinding{TValue}"/> and this binding.
		/// This does not require an object instance for the delegates to get/set the value.
		/// You must keep a reference to the binding to unbind when finished.
		/// </remarks>
		/// <param name="getValue">Delegate to get the value.</param>
		/// <param name="setValue">Delegate to set the value when changed.</param>
		/// <param name="addChangeEvent">Delegate to add a change event when the value changes.</param>
		/// <param name="removeChangeEvent">Delegate to remove the change event.</param>
		/// <param name="mode">Dual binding mode.</param>
		public DualBinding<TValue> Bind(Func<TValue> getValue, Action<TValue> setValue = null, Action<EventHandler<EventArgs>> addChangeEvent = null, Action<EventHandler<EventArgs>> removeChangeEvent = null, DualBindingMode mode = DualBindingMode.TwoWay)
		{
			return Bind(new DelegateBinding<TValue> { GetValue = getValue, SetValue = setValue, AddChangeEvent = addChangeEvent, RemoveChangeEvent = removeChangeEvent }, mode);
		}

		/// <summary>
		/// Creates a new dual binding between the specified <paramref name="objectBinding"/> and this binding.
		/// </summary>
		/// <param name="objectValue">Object to get/set the values from/to.</param>
		/// <param name="objectBinding">Indirect binding to get/set the values from the <paramref name="objectValue"/>.</param>
		/// <param name="mode">Dual binding mode.</param>
		/// <typeparam name="TObject">The type of the object that is being bound to.</typeparam>
		public DualBinding<TValue> Bind<TObject>(TObject objectValue, IndirectBinding<TValue> objectBinding, DualBindingMode mode = DualBindingMode.TwoWay)
		{
			var valueBinding = new ObjectBinding<TObject, TValue>(objectValue, objectBinding);
			return Bind(sourceBinding: valueBinding, mode: mode);
		}

		/// <summary>
		/// Creates a binding to the <paramref name="propertyName"/> of the specified <paramref name="objectValue"/>.
		/// </summary>
		/// <remarks>
		/// This is a shortcut to using the <see cref="PropertyBinding{TValue}"/>.
		/// This has the advantage of registering automatically to <see cref="System.ComponentModel.INotifyPropertyChanged"/> 
		/// or to an event named after the property with a "Changed" suffix.
		/// </remarks>
		/// <param name="objectValue">Object to bind to.</param>
		/// <param name="propertyName">Name of the property to bind to on the <paramref name="objectValue"/>.</param>
		/// <param name="mode">Direction of the binding.</param>
		public DualBinding<TValue> Bind(object objectValue, string propertyName, DualBindingMode mode = DualBindingMode.TwoWay)
		{
			return Bind(objectValue, Property<TValue>(propertyName), mode: mode);
		}

		/// <summary>
		/// Creates a binding to the specified <paramref name="objectValue"/> with the specified <paramref name="propertyExpression"/>.
		/// </summary>
		/// <remarks>
		/// This has the advantage of registering automatically to <see cref="System.ComponentModel.INotifyPropertyChanged"/> 
		/// or to an event named after the property with a "Changed" suffix, if the expression is a property.
		/// When the expression does not evaluate to a property, it will not be able to bind to the changed events and will
		/// use the expression as a delegate directly.
		/// </remarks>
		/// <typeparam name="TObject">Type of the data context to bind to</typeparam>
		/// <param name="objectValue">Object to bind to.</param>
		/// <param name="propertyExpression">Expression for a property of the <paramref name="objectValue"/>, or a non-property expression with no change event binding.</param>
		/// <param name="mode">Direction of the binding</param>
		/// <returns>The binding between the data context and this binding</returns>
		public DualBinding<TValue> Bind<TObject>(TObject objectValue, Expression<Func<TObject, TValue>> propertyExpression, DualBindingMode mode = DualBindingMode.TwoWay)
		{
			return Bind(objectValue, Property(propertyExpression), mode);
		}

		/// <summary>
		/// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:Eto.Forms.ObjectBinding`2"/>.
		/// </summary>
		/// <returns>A <see cref="T:System.String"/> that represents the current <see cref="T:Eto.Forms.ObjectBinding`2"/>.</returns>
		public override string ToString()
		{
			return $"Object: {DataItem}, {InnerBinding}";
		}
	}
}
