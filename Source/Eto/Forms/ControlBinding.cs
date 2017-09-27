using System;
using Eto.Forms;
using System.Linq.Expressions;

namespace Eto.Forms
{
	/// <summary>
	/// Binding object to easily bind a property of a <see cref="Control"/>.
	/// </summary>
	/// <remarks>
	/// This provides control-specific binding, such as binding to a <see cref="BindableWidget.DataContext"/>.
	/// Any bindings created using this will also add to the <see cref="BindableWidget.Bindings"/> collection to keep its
	/// reference.
	/// </remarks>
	[Obsolete("Since 2.1: Use Eto.Forms.BindableBinding<T, TValue> instead")]
	public class ControlBinding<T, TValue> : BindableBinding<T, TValue>
		where T: Control
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ControlBinding{T,TValue}"/> class.
		/// </summary>
		/// <param name="dataItem">Data item to get/set the values from/to.</param>
		/// <param name="getValue">Delegate to get the value from the object.</param>
		/// <param name="setValue">Delegate to set the value to the object.</param>
		/// <param name="addChangeEvent">Delegate to add the change event.</param>
		/// <param name="removeChangeEvent">Delegate to remove the chang event.</param>
		public ControlBinding(T dataItem, Func<T, TValue> getValue, Action<T, TValue> setValue = null, Action<T, EventHandler<EventArgs>> addChangeEvent = null, Action<T, EventHandler<EventArgs>> removeChangeEvent = null)
			: base(dataItem, getValue, setValue, addChangeEvent, removeChangeEvent)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ControlBinding{T,TValue}"/> class.
		/// </summary>
		/// <param name="dataItem">Control the binding is attached to.</param>
		/// <param name="innerBinding">Inner binding.</param>
		public ControlBinding(T dataItem, IndirectBinding<TValue> innerBinding)
			: base(dataItem, innerBinding)
		{
		}
	}
}
