using System;
using Eto.Forms;

namespace Eto
{
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
		/// Initializes a new instance of the ObjectBinding with the specified object and property for a <see cref="PropertyBinding"/>
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
				dataItem = value;
				OnDataValueChanged(EventArgs.Empty);
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

		public ObjectBinding(T dataItem, Func<T, TValue> getValue, Action<T, TValue> setValue = null, Action<T, EventHandler<EventArgs>> addChangeEvent = null, Action<T, EventHandler<EventArgs>> removeChangeEvent = null)
			: this(dataItem, new DelegateBinding<T, TValue>(getValue, setValue, addChangeEvent, removeChangeEvent))
		{
		}

		/// <summary>
		/// Initializes a new instance of the ObjectBinding with the specified object and property for a <see cref="PropertyBinding"/>
		/// </summary>
		/// <remarks>
		/// This is a shortcut to set up the binding to get/set values from a particular property of the specified object
		/// </remarks>
		/// <param name="dataItem">object to get/set values from</param>
		/// <param name="property">property of the dataItem to get/set values</param>
		public ObjectBinding(T dataItem, string property)
			: this(dataItem, new PropertyBinding<TValue>(property))
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
					if (dataValueChangedReference == null)
						dataValueChangedReference = InnerBinding.AddValueChangedHandler(
							DataItem,
							new EventHandler<EventArgs>(HandleChangedEvent)
						);
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
					if (dataValueChangedReference != null)
					{
						InnerBinding.RemoveValueChangedHandler(
							dataValueChangedReference,
							new EventHandler<EventArgs>(HandleChangedEvent)
						);
						dataValueChangedReference = null;
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

		[Obsolete("Use BindDataContext() instead")]
		public DualBinding<TValue> Bind(IndirectBinding<TValue> dataContextBinding, DualBindingMode mode = DualBindingMode.TwoWay, TValue defaultControlValue = default(TValue), TValue defaultContextValue = default(TValue))
		{
			return BindDataContext(dataContextBinding, mode, defaultControlValue, defaultContextValue);
		}

		public DualBinding<TValue> BindDataContext(IndirectBinding<TValue> dataContextBinding, DualBindingMode mode = DualBindingMode.TwoWay, TValue defaultControlValue = default(TValue), TValue defaultContextValue = default(TValue))
		{
			var control = DataItem as Control;
			if (control == null)
				throw new InvalidOperationException("Binding must be attached to a control");
			var contextBinding = new ObjectBinding<object>(control, new DelegateBinding<Control, object>(w => w.DataContext, null, (w, h) => w.DataContextChanged += h, (w, h) => w.DataContextChanged -= h));
			var valueBinding = new ObjectBinding<TValue>(control.DataContext, dataContextBinding)
			{
				GettingNullValue = defaultControlValue,
				SettingNullValue = defaultContextValue
			};
			DualBinding<TValue> binding = Bind(valueBinding: valueBinding, mode: mode);
			contextBinding.DataValueChanged += delegate
			{
				((ObjectBinding<TValue>)binding.Source).DataItem = contextBinding.DataValue;
			};
			control.Bindings.Add(contextBinding);
			return binding;
		}

		public DualBinding<TValue> Bind(DirectBinding<TValue> valueBinding, DualBindingMode mode = DualBindingMode.TwoWay)
		{
			var binding = new DualBinding<TValue>(
				              valueBinding,
				              this,
				              mode
			              );
			var control = DataItem as Control;
			if (control != null)
				control.Bindings.Add(binding);
			return binding;
		}

		public DualBinding<TValue> Bind(Func<TValue> getValue, Action<TValue> setValue = null, Action<EventHandler<EventArgs>> addChangeEvent = null, Action<EventHandler<EventArgs>> removeChangeEvent = null, DualBindingMode mode = DualBindingMode.TwoWay)
		{
			return Bind(new DelegateBinding<TValue> { GetValue = getValue, SetValue = setValue, AddChangeEvent = addChangeEvent, RemoveChangeEvent = removeChangeEvent }, mode);
		}

		[Obsolete("Use BindDataContext<T> instead")]
		public DualBinding<TValue> Bind<TObject>(Func<TObject, TValue> getValue, Action<TObject, TValue> setValue = null, Action<TObject, EventHandler<EventArgs>> addChangeEvent = null, Action<TObject, EventHandler<EventArgs>> removeChangeEvent = null, DualBindingMode mode = DualBindingMode.TwoWay, TValue defaultGetValue = default(TValue), TValue defaultSetValue = default(TValue))
		{
			return BindDataContext(new DelegateBinding<TObject, TValue>(getValue, setValue, addChangeEvent, removeChangeEvent, defaultGetValue, defaultSetValue), mode);
		}

		[Obsolete("Use BindDataContext<T> instead")]
		public DualBinding<TValue> Bind<TObject>(DelegateBinding<TObject, TValue> binding, DualBindingMode mode = DualBindingMode.TwoWay)
		{
			return Bind(dataContextBinding: binding, mode: mode);
		}

		public DualBinding<TValue> BindDataContext<TObject>(Func<TObject, TValue> getValue, Action<TObject, TValue> setValue = null, Action<TObject, EventHandler<EventArgs>> addChangeEvent = null, Action<TObject, EventHandler<EventArgs>> removeChangeEvent = null, DualBindingMode mode = DualBindingMode.TwoWay, TValue defaultGetValue = default(TValue), TValue defaultSetValue = default(TValue))
		{
			return BindDataContext(new DelegateBinding<TObject, TValue>(getValue, setValue, addChangeEvent, removeChangeEvent, defaultGetValue, defaultSetValue), mode);
		}

		public DualBinding<TValue> BindDataContext<TObject>(DelegateBinding<TObject, TValue> binding, DualBindingMode mode = DualBindingMode.TwoWay)
		{
			return Bind(dataContextBinding: binding, mode: mode);
		}

		public DualBinding<TValue> Bind<TObject>(TObject objectValue, Func<TObject, TValue> getValue, Action<TObject, TValue> setValue = null, Action<TObject, EventHandler<EventArgs>> addChangeEvent = null, Action<TObject, EventHandler<EventArgs>> removeChangeEvent = null, DualBindingMode mode = DualBindingMode.TwoWay, TValue defaultGetValue = default(TValue), TValue defaultSetValue = default(TValue))
		{
			return Bind(objectValue, new DelegateBinding<TObject, TValue>(getValue, setValue, addChangeEvent, removeChangeEvent, defaultGetValue, defaultSetValue), mode);
		}

		public DualBinding<TValue> Bind<TObject>(TObject objectValue, DelegateBinding<TObject, TValue> objectBinding, DualBindingMode mode = DualBindingMode.TwoWay)
		{
			var valueBinding = new ObjectBinding<TValue>(objectValue, objectBinding);
			return Bind(valueBinding: valueBinding, mode: mode);
		}

	}
}
