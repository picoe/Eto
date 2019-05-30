using System;
using System.Linq.Expressions;
using Eto.Forms;

namespace Eto.Forms
{
	/// <summary>
	/// Extensions for bindings
	/// </summary>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public static class BindingExtensions
	{
		/// <summary>
		/// Causes the change event of the binding to occur only when the <see cref="Control.LostFocus"/> event is triggered.
		/// </summary>
		/// <remarks>
		/// This is useful for text-based input controls such as the <see cref="NumericStepper"/> when a partial input can be invalid.
		/// The binding will only be updated when the control has lost the input focus, where by default it will be updated for every
		/// change while the user is updating the control.
		/// </remarks>
		/// <returns>A control binding that updates only when the control's input focus is lost.</returns>
		public static BindableBinding<T, TValue> WhenLostFocus<T, TValue>(this BindableBinding<T, TValue> binding)
			where T: Control
		{
			return new BindableBinding<T, TValue>(
				binding.DataItem,
				c => binding.DataValue,
				(c, v) => binding.DataValue = v,
				addChangeEvent: (c, ev) => binding.DataItem.LostFocus += ev,
				removeChangeEvent: (c, ev) => binding.DataItem.LostFocus -= ev
			);
		}
	}
}