using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Eto
{
	/// <summary>
	/// Extension methods to provide a consistent .
	/// 
	/// Some of these are from https://gist.github.com/jeffwilcox/2432351 (no attribution requested.)
	/// </summary>
	static class TypeHelper
	{
		public static Assembly GetAssembly(this Type type)
		{
			return type.Assembly;
		}

		public static Type GetBaseType(this Type type)
		{
			return type.BaseType;
		}

		public static bool IsEnum(this Type type)
		{
			return type.IsEnum;
		}

		public static MethodInfo GetGetMethod(this PropertyInfo propertyInfo)
		{
			return propertyInfo.GetGetMethod(true);
		}

		public static MethodInfo GetSetMethod(this PropertyInfo propertyInfo)
		{
			return propertyInfo.GetSetMethod(true);
		}

		public static T GetCustomAttribute<T>(this Type type, bool inherit)
			where T : Attribute
		{
			return (T)type.GetCustomAttributes(typeof(T), inherit).FirstOrDefault();
		}

		/// <summary>
		/// Returns a PropertyInfo for the specified property of the current type.
		/// The property may be declared on the current type or an ancestor type.
		/// </summary>
		/// <returns></returns>
		public static PropertyInfo GetRuntimeProperty(this Type type, string propertyName)
		{
			return type.GetProperty(propertyName);
		}

		public static FieldInfo GetRuntimeField(this Type type, string fieldName)
		{
			return type.GetField(fieldName);
		}

		public static EventInfo GetRuntimeEvent(this Type type, string eventName)
		{
			return type.GetEvent(eventName);
		}

		public static IEnumerable<EventInfo> GetRuntimeEvents(this Type type)
		{
			return type.GetEvents(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		}

		public static Delegate CreateDelegate(this MethodInfo method, Type objectType, object instance)
		{
			return Delegate.CreateDelegate(objectType, instance, method);
		}

		public static MethodInfo GetRuntimeMethod(this Type type, string methodName, Type[] parameters)
		{
			return type.GetMethod(methodName, parameters);
		}

		public static IEnumerable<MethodInfo> GetRuntimeMethods(this Type type)
		{
			return type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		}

		public static IEnumerable<PropertyInfo> GetRuntimeProperties(this Type type)
		{
			return type.GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		}

		public static IEnumerable<FieldInfo> GetRuntimeFields(this Type type)
		{
			return type.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		}
	}
}