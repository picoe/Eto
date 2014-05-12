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
		public static DualBinding Bind<TWidget,TWidgetProperty,TSource,TSourceProperty>(this TWidget control, Expression<Func<TWidget,TWidgetProperty>> controlProperty, TSource source, Expression<Func<TSource, TSourceProperty>> sourceProperty, DualBindingMode mode = DualBindingMode.TwoWay)
			where TWidget: Control
		{
			var controlExpression = (MemberExpression)controlProperty.Body;
			var sourceExpression = (MemberExpression)sourceProperty.Body;
			return control.Bind(controlExpression.Member.Name, source, sourceExpression.Member.Name, mode);
		}

		public static DualBinding Bind<TWidget, TWidgetProperty, TContextProperty, TContext>(this TWidget control, Expression<Func<TWidget, TWidgetProperty>> controlProperty, Expression<Func<TContext, TContextProperty>> sourceProperty, DualBindingMode mode = DualBindingMode.TwoWay, object defaultControlValue = null, object defaultContextValue = null)
			where TWidget : Control
		{
			var controlExpression = (MemberExpression)controlProperty.Body;
			var sourceExpression = (MemberExpression)sourceProperty.Body;
			return control.Bind(controlExpression.Member.Name, sourceExpression.Member.Name, mode, defaultControlValue, defaultContextValue);
		}

	}
}