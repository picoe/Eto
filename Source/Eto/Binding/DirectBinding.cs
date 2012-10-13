using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.ObjectModel;

namespace Eto
{
	/// <summary>
	/// Abstraction to get/set values from a provided object
	/// </summary>
	/// <remarks>
	/// This binding provides a way to get/set values of an object that is provided by the binding,
	/// and not passed in.
	/// 
	/// This differs from the <see cref="IndirectBinding"/>, which requires that the caller pass in the
	/// object to get/set the value from/to.
	/// </remarks>
	public abstract class DirectBinding : Binding
	{
		#region Events

		/// <summary>
		/// Identifier for the <see cref="DataValueChanged"/> event
		/// </summary>
		public const string DataValueChangedEvent = "ObjectBinding.DataValueChangedEvent";

		event EventHandler<EventArgs> _DataValueChanged;

		/// <summary>
		/// Event to handle when the value changes on the bound object
		/// </summary>
		public event EventHandler<EventArgs> DataValueChanged {
			add {
				var shouldHandle = _DataValueChanged == null;
				_DataValueChanged += value;
				if (shouldHandle)
					HandleEvent (DataValueChangedEvent);
			}
			remove {
				_DataValueChanged -= value;
				if (_DataValueChanged == null)
					RemoveEvent (DataValueChangedEvent);
			}
		}

		/// <summary>
		/// Handles the <see cref="DataValueChanged"/> event
		/// </summary>
		/// <remarks>
		/// Implementors of this class should call this method when the value changes
		/// on the bound object. Make sure to also override the <see cref="Binding.HandleEvent"/> 
		/// and <see cref="Binding.RemoveEvent"/> methods to hook up/remove any event bindings 
		/// you need on the bound object.
		/// </remarks>
		public virtual void OnDataValueChanged (EventArgs e)
		{
			if (_DataValueChanged != null)
				_DataValueChanged (this, e);
		}

		#endregion
	
		/// <summary>
		/// Gets or sets the value of this binding on the bound object
		/// </summary>
		/// <remarks>
		/// This uses the <see cref="InnerBinding"/> on the <see cref="DataItem"/> to get/set the value
		/// </remarks>
		public abstract object DataValue { get; set; }
	}
}