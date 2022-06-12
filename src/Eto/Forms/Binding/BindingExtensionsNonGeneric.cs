using System;
using System.Linq;
using System.Reflection;

namespace Eto.Forms
{
	/// <summary>
	/// (internal) extensions to bind to types without using generics.
	/// 
	/// Could eventually expose a better API for doing this via non-generic interfaces like IIndirectBinding, IDirectBinding, etc
	/// which could have a property to get the type of the binding.
	/// </summary>
	static class BindingExtensionsNonGeneric
	{
		public static IBinding BindingOfType(this IBinding binding, Type fromType, Type toType)
		{
			var defaultToValue = toType.GetTypeInfo().IsValueType ? Activator.CreateInstance(toType) : null;
			var defaultFromValue = fromType.GetTypeInfo().IsValueType ? Activator.CreateInstance(fromType) : null;
			var bindingType = binding.GetType();
			var ofTypeMethod = bindingType.GetRuntimeMethods().FirstOrDefault(r => r.Name == "OfType" && r.GetParameters().Length == 2);
			return (IBinding)ofTypeMethod.MakeGenericMethod(toType).Invoke(binding, new object[] { defaultToValue, defaultFromValue });
		}

		static PropertyInfo GetFirstDeclaredProperty(Type type, string propertyName)
		{
			while (type != null)
			{
				var property = type.GetTypeInfo().GetDeclaredProperty(propertyName);
				if (property != null)
					return property;
				type = type.GetBaseType();
			}
			return null;
		}

		public static IBinding BindDataContextProperty(this IBindable bindable, string bindingPropertyName, Type valueType, IBinding binding)
		{
			var type = bindable.GetType();
			var bindingProperty = GetFirstDeclaredProperty(type, bindingPropertyName).GetValue(bindable);
			var types = new Type[] { typeof(IndirectBinding<>).MakeGenericType(valueType), typeof(DualBindingMode), valueType, valueType };

			var bindDataContextMethod = bindingProperty.GetType().GetRuntimeMethod("BindDataContext", types);
			var defaultValue = Activator.CreateInstance(valueType);
			return (IBinding)bindDataContextMethod.Invoke(bindingProperty, new object[] { binding, DualBindingMode.TwoWay, defaultValue, defaultValue });
		}

	}
}
