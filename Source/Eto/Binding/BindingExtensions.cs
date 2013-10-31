using System;
using System.Linq.Expressions;
using Eto.Forms;

namespace Eto
{
	/// <summary>
	/// Extensions for bindings
	/// </summary>
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