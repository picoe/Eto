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
		/// <typeparam name="TSource">The type of the source object.</typeparam>
		/// <typeparam name="TValue">The type of the property.</typeparam>
		public static DualBinding<TValue> Bind<TWidget,TSource,TValue>(this TWidget control, Expression<Func<TWidget,TValue>> controlProperty, TSource source, Expression<Func<TSource, TValue>> sourceProperty, DualBindingMode mode = DualBindingMode.TwoWay)
			where TWidget: Control
		{
			var controlExpression = (MemberExpression)controlProperty.Body;
			var sourceExpression = sourceProperty.Body as MemberExpression;
			if (sourceExpression == null)
			{
				var unaryExpression = sourceProperty.Body as UnaryExpression;
				if (unaryExpression != null && unaryExpression.NodeType == ExpressionType.Convert)
				{
					sourceExpression = unaryExpression.Operand as MemberExpression;
				}
			}
			var binding = control.Bind<TValue>(controlExpression.Member.Name, source, sourceExpression.Member.Name, mode);

			return binding;
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
		/// <typeparam name="TContext">The type of the data context object.</typeparam>
		[Obsolete("Use BindDataContext instead")]
		public static DualBinding<TValue> Bind<TWidget,TContext,TValue>(this TWidget control, Expression<Func<TWidget, TValue>> controlProperty, Expression<Func<TContext, TValue>> sourceProperty, DualBindingMode mode = DualBindingMode.TwoWay, TValue defaultControlValue = default(TValue), TValue defaultContextValue = default(TValue))
			where TWidget : Control
		{
			var controlExpression = (MemberExpression)controlProperty.Body;
			var sourceExpression = sourceProperty.Body as MemberExpression;
			if (sourceExpression == null)
			{
				var unaryExpression = sourceProperty.Body as UnaryExpression;
				if (unaryExpression != null && unaryExpression.NodeType == ExpressionType.Convert)
				{
					sourceExpression = unaryExpression.Operand as MemberExpression;
				}
			}
			return control.BindDataContext<TValue>(controlExpression.Member.Name, sourceExpression.Member.Name, mode, defaultControlValue, defaultContextValue);
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
		/// <typeparam name="TContext">The type of the data context object.</typeparam>
		/// <typeparam name="TValue">The type of the property.</typeparam>
		public static DualBinding<TValue> BindDataContext<TWidget, TContext, TValue>(this TWidget control, Expression<Func<TWidget, TValue>> controlProperty, Expression<Func<TContext, TValue>> sourceProperty, DualBindingMode mode = DualBindingMode.TwoWay, TValue defaultControlValue = default(TValue), TValue defaultContextValue = default(TValue))
			where TWidget : Control
		{
			var controlExpression = (MemberExpression)controlProperty.Body;
			var sourceExpression = sourceProperty.Body as MemberExpression;
			if (sourceExpression == null)
			{
				var unaryExpression = sourceProperty.Body as UnaryExpression;
				if (unaryExpression != null && unaryExpression.NodeType == ExpressionType.Convert)
				{
					sourceExpression = unaryExpression.Operand as MemberExpression;
				}
			}
			return control.BindDataContext<TValue>(controlExpression.Member.Name, sourceExpression.Member.Name, mode, defaultControlValue, defaultContextValue);
		}
	}
}