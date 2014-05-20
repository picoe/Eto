using System;
using System.Linq.Expressions;
using Eto.Forms;

namespace Eto
{
	/// <summary>
	/// Extensions for bindings
	/// </summary>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public static class BindingExtensions
	{
		/// <summary>
		/// Binds a control property to a <paramref name="source"/> property
		/// </summary>
		/// <param name="control">Control to bind to.</param>
		/// <param name="controlProperty">Control property.</param>
		/// <param name="source">Source object to get/set the value from.</param>
		/// <param name="sourceProperty">Source property from the data context.</param>
		/// <param name="mode">Mode of the binding.</param>
		/// <typeparam name="TWidget">The type of control.</typeparam>
		/// <typeparam name="TWidgetProperty">The type of the widget property. Must be convertible to the TSourceProperty type</typeparam>
		/// <typeparam name="TSource">The type of the source object.</typeparam>
		/// <typeparam name="TSourceProperty">The type of the source property. Must be convertible to the TWidgetProperty type.</typeparam>
		public static DualBinding Bind<TWidget,TWidgetProperty,TSource,TSourceProperty>(this TWidget control, Expression<Func<TWidget,TWidgetProperty>> controlProperty, TSource source, Expression<Func<TSource, TSourceProperty>> sourceProperty, DualBindingMode mode = DualBindingMode.TwoWay)
			where TWidget: Control
		{
			var controlExpression = (MemberExpression)controlProperty.Body;
			var sourceExpression = (MemberExpression)sourceProperty.Body;
			return control.Bind(controlExpression.Member.Name, source, sourceExpression.Member.Name, mode);
		}

		/// <summary>
		/// Binds a control property to a <see cref="Control.DataContext"/> property
		/// </summary>
		/// <param name="control">Control to bind to.</param>
		/// <param name="controlProperty">Control property.</param>
		/// <param name="sourceProperty">Source property from the data context.</param>
		/// <param name="mode">Mode of the binding.</param>
		/// <param name="defaultControlValue">Default control value, if the control value is null.</param>
		/// <param name="defaultContextValue">Default context value, if the context value is null.</param>
		/// <typeparam name="TWidget">The type of control.</typeparam>
		/// <typeparam name="TWidgetProperty">The type of the widget property. Must be convertible to the TContextProperty type</typeparam>
		/// <typeparam name="TContextProperty">The type of the context property. Must be convertible to the TWidgetProperty type.</typeparam>
		/// <typeparam name="TContext">The type of the data context object.</typeparam>
		[Obsolete("Use BindDataContext instead")]
		public static DualBinding Bind<TWidget, TWidgetProperty, TContextProperty, TContext>(this TWidget control, Expression<Func<TWidget, TWidgetProperty>> controlProperty, Expression<Func<TContext, TContextProperty>> sourceProperty, DualBindingMode mode = DualBindingMode.TwoWay, object defaultControlValue = null, object defaultContextValue = null)
			where TWidget : Control
		{
			var controlExpression = (MemberExpression)controlProperty.Body;
			var sourceExpression = (MemberExpression)sourceProperty.Body;
			return control.BindDataContext(controlExpression.Member.Name, sourceExpression.Member.Name, mode, defaultControlValue, defaultContextValue);
		}

		/// <summary>
		/// Binds a control property to a <see cref="Control.DataContext"/> property
		/// </summary>
		/// <param name="control">Control to bind to.</param>
		/// <param name="controlProperty">Control property.</param>
		/// <param name="sourceProperty">Source property from the data context.</param>
		/// <param name="mode">Mode of the binding.</param>
		/// <param name="defaultControlValue">Default control value, if the control value is null.</param>
		/// <param name="defaultContextValue">Default context value, if the context value is null.</param>
		/// <typeparam name="TWidget">The type of control.</typeparam>
		/// <typeparam name="TWidgetProperty">The type of the widget property. Must be convertible to the TContextProperty type</typeparam>
		/// <typeparam name="TContextProperty">The type of the context property. Must be convertible to the TWidgetProperty type.</typeparam>
		/// <typeparam name="TContext">The type of the data context object.</typeparam>
		public static DualBinding BindDataContext<TWidget, TWidgetProperty, TContextProperty, TContext>(this TWidget control, Expression<Func<TWidget, TWidgetProperty>> controlProperty, Expression<Func<TContext, TContextProperty>> sourceProperty, DualBindingMode mode = DualBindingMode.TwoWay, object defaultControlValue = null, object defaultContextValue = null)
			where TWidget : Control
		{
			var controlExpression = (MemberExpression)controlProperty.Body;
			var sourceExpression = (MemberExpression)sourceProperty.Body;
			return control.BindDataContext(controlExpression.Member.Name, sourceExpression.Member.Name, mode, defaultControlValue, defaultContextValue);
		}
	}
}