using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.ObjectModel;

namespace Eto
{
	public class ObjectBinding<T, TValue> : ObjectBinding
	{
		public new T DataItem
		{
			get { return (T)base.DataItem; }
			set { base.DataItem = value; }
		}

		public new TValue DataValue
		{
			get { return (TValue)base.DataValue; }
			set { base.DataValue = value; }
		}

		public ObjectBinding (T dataItem, Func<T, TValue> getValue, Action<T, TValue> setValue = null, Action<T, EventHandler<EventArgs>> addChangeEvent = null, Action<T, EventHandler<EventArgs>> removeChangeEvent = null)
			: this(dataItem, new DelegateBinding<T, TValue>(getValue, setValue, addChangeEvent, removeChangeEvent))
		{
		}
		
		/// <summary>
		/// Initializes a new instance of the ObjectBinding with the specified object and binding to get/set values with
		/// </summary>
		/// <param name="dataItem">object to get/set values from</param>
		/// <param name="innerBinding">binding to use to get/set the values from the dataItem</param>
		public ObjectBinding (T dataItem, IndirectBinding innerBinding)
			: base (dataItem, innerBinding)
		{
		}

		public DualBinding Bind<TObject>(Func<TObject, TValue> getValue, Action<TObject, TValue> setValue = null, Action<TObject, EventHandler<EventArgs>> addChangeEvent = null, Action<TObject, EventHandler<EventArgs>> removeChangeEvent = null, DualBindingMode mode = DualBindingMode.TwoWay)
		{
			return Bind(new DelegateBinding<TObject, TValue>(getValue, setValue, addChangeEvent, removeChangeEvent), mode);
		}

		public DualBinding Bind<TObject>(DelegateBinding<TObject, TValue> binding, DualBindingMode mode = DualBindingMode.TwoWay)
		{
			return BindingExtensions.Bind(controlBinding: this, dataContextBinding: binding, mode: mode);
		}

		public DualBinding Bind<TObject>(TObject objectValue, Func<TObject, TValue> getValue, Action<TObject, TValue> setValue = null, Action<TObject, EventHandler<EventArgs>> addChangeEvent = null, Action<TObject, EventHandler<EventArgs>> removeChangeEvent = null, DualBindingMode mode = DualBindingMode.TwoWay)
		{
			return Bind(objectValue, new DelegateBinding<TObject, TValue>(getValue, setValue, addChangeEvent, removeChangeEvent), mode);
		}
		
		public DualBinding Bind<TObject>(TObject objectValue, DelegateBinding<TObject, TValue> objectBinding, DualBindingMode mode = DualBindingMode.TwoWay)
		{
			var valueBinding = new ObjectBinding(objectValue, objectBinding);
			return BindingExtensions.Bind(controlBinding: this, valueBinding: valueBinding, mode: mode);
		}
	}

	/// <summary>
	/// Binding for a particular object to get/set values from/to
	/// </summary>
	/// <remarks>
	/// This binding provides a way to get/set values for a particular object.  This uses
	/// a <see cref="IndirectBinding"/> as its logic to actually retrieve/set the values.
	/// 
	/// This acts as a bridge between the <see cref="IndirectBinding"/> and <see cref="DirectBinding"/>
	/// so that you can utilize the <see cref="DirectBinding.DataValueChanged"/> method.
	/// 
	/// Typically, one would use the <see cref="PropertyBinding"/>, or the <see cref="ObjectBinding (object, string)"/>
	/// constructor to hook up this binding to a particular property of the specified object
	/// </remarks>
	public class ObjectBinding : DirectBinding
	{
		object dataValueChangedReference;
		object dataItem;
		
		/// <summary>
		/// Gets the binding used to get/set the values from the <see cref="DataItem"/>
		/// </summary>
		public IndirectBinding InnerBinding { get; private set; }
		
		/// <summary>
		/// Gets the object to get/set the values using the <see cref="InnerBinding"/>
		/// </summary>
		public object DataItem
		{
			get { return dataItem; }
			set {
				dataItem = value;
				OnDataValueChanged (EventArgs.Empty);
			}
		}
		
		/// <summary>
		/// Gets or sets the default value to use when setting the value for this binding when input value is null
		/// </summary>
		public object SettingNullValue
		{
			get;
			set;
		}
		
		/// <summary>
		/// Gets or sets the default value to use when getting the value for this binding when the <see cref="DataItem"/> or property value is null
		/// </summary>
		public object GettingNullValue
		{
			get;
			set;
		}
		
		/// <summary>
		/// Initializes a new instance of the ObjectBinding with the specified object and property for a <see cref="PropertyBinding"/>
		/// </summary>
		/// <remarks>
		/// This is a shortcut to set up the binding to get/set values from a particular property of the specified object
		/// </remarks>
		/// <param name="dataItem">object to get/set values from</param>
		/// <param name="property">property of the dataItem to get/set values</param>
		public ObjectBinding (object dataItem, string property)
			: this(dataItem, new PropertyBinding(property))
		{
		}
		
		/// <summary>
		/// Initializes a new instance of the ObjectBinding with the specified object and binding to get/set values with
		/// </summary>
		/// <param name="dataItem">object to get/set values from</param>
		/// <param name="innerBinding">binding to use to get/set the values from the dataItem</param>
		public ObjectBinding (object dataItem, IndirectBinding innerBinding)
		{
			this.DataItem = dataItem;
			this.InnerBinding = innerBinding;
			this.InnerBinding.Changed += HandleInnerBindingChanged;
			this.InnerBinding.Changing += HandleInnerBindingChanging;
		}
		
		void HandleInnerBindingChanging (object sender, BindingChangingEventArgs e)
		{
			OnChanging (e);
		}
		
		void HandleInnerBindingChanged (object sender, BindingChangedEventArgs e)
		{
			OnChanged (e);
		}
		
		/// <summary>
		/// Gets or sets the value of this binding on the bound object
		/// </summary>
		/// <remarks>
		/// This uses the <see cref="InnerBinding"/> on the <see cref="DataItem"/> to get/set the value
		/// </remarks>
		public override object DataValue {
			get {
				return InnerBinding.GetValue (DataItem) ?? GettingNullValue;
			}
			set {
				InnerBinding.SetValue (DataItem, value ?? SettingNullValue);
			}
		}
		
		/// <summary>
		/// Hooks up the late bound events for this object
		/// </summary>
		protected override void HandleEvent (string handler)
		{
			switch (handler) {
			case DataValueChangedEvent:
				if (dataValueChangedReference == null)
					dataValueChangedReference = InnerBinding.AddValueChangedHandler (
						DataItem,
						new EventHandler<EventArgs>(HandleChangedEvent)
						);
				break;
			default:
				base.HandleEvent (handler);
				break;
			}
		}
		
		/// <summary>
		/// Removes the late bound events for this object
		/// </summary>
		protected override void RemoveEvent (string handler)
		{
			switch (handler) {
			case DataValueChangedEvent:
				if (dataValueChangedReference != null) {
					InnerBinding.RemoveValueChangedHandler (
						dataValueChangedReference,
						new EventHandler<EventArgs>(HandleChangedEvent)
						);
					dataValueChangedReference = null;
				}
				break;
			default:
				base.RemoveEvent (handler);
				break;
			}
		}
		
		/// <summary>
		/// Unbinds this binding
		/// </summary>
		public override void Unbind ()
		{
			base.Unbind ();
			
			RemoveEvent (DataValueChangedEvent);
			InnerBinding.Unbind ();
		}
		
		void HandleChangedEvent (object sender, EventArgs e)
		{
			OnDataValueChanged (e);
		}
	}
}
