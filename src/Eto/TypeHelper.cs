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
#if NETSTANDARD
		static MethodInfo getCallingAssembly = typeof(Assembly).GetTypeInfo().GetDeclaredMethod("GetCallingAssembly");
		
		static TypeHelper()
		{
			var detectType = Type.GetType("Eto.PlatformDetect, Eto.Gtk", false);
			if (detectType != null)
				getCallingAssembly = detectType.GetRuntimeMethod("GetCallingAssembly", new Type[] { });
		}

		public static MethodInfo GetCallingAssembly { get { return getCallingAssembly; } }
#endif

				public static Assembly GetAssembly(this Type type)
		{
#if NETSTANDARD1_0
			return type.GetTypeInfo().Assembly;
#else
			return type.Assembly;
#endif
		}

		public static Type GetBaseType(this Type type)
		{
#if NETSTANDARD1_0
			return type.GetTypeInfo().BaseType;
#else
			return type.BaseType;
#endif
		}

		public static bool IsEnum(this Type type)
		{
#if NETSTANDARD1_0
			return type.GetTypeInfo().IsEnum;
#else
			return type.IsEnum;
#endif
		}

		public static MethodInfo GetGetMethod(this PropertyInfo propertyInfo)
		{
#if NETSTANDARD1_0
			return propertyInfo.GetMethod;
#else
			return propertyInfo.GetGetMethod(true);
#endif
		}

		public static MethodInfo GetSetMethod(this PropertyInfo propertyInfo)
		{
#if NETSTANDARD1_0
			return propertyInfo.SetMethod;
#else
			return propertyInfo.GetSetMethod(true);
#endif
		}

#if NETSTANDARD1_0
		public static T GetCustomAttribute<T>(this Type type, bool inherit)
			where T: Attribute
		{
			return CustomAttributeExtensions.GetCustomAttribute<T>(type.GetTypeInfo(), inherit);
		}

		public static MethodInfo GetAddMethod(this EventInfo eventInfo)
		{
			return eventInfo.AddMethod;
		}

		public static MethodInfo GetRemoveMethod(this EventInfo eventInfo)
		{
			return eventInfo.RemoveMethod;
		}
		/// <summary>
		/// Determines whether the specified object is an instance of the current Type.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="o">The object to compare with the current type.</param>
		/// <returns>true if the current Type is in the inheritance hierarchy of the 
		/// object represented by o, or if the current Type is an interface that o 
		/// supports. false if neither of these conditions is the case, or if o is 
		/// null, or if the current Type is an open generic type (that is, 
		/// ContainsGenericParameters returns true).</returns>
		public static bool IsInstanceOfType(this Type type, object o)
		{
			return o != null && IsAssignableFrom(type, o.GetType());
		}

		public static bool IsAssignableFrom(this Type type, Type c)
		{
			return c != null && type.GetTypeInfo().IsAssignableFrom(c.GetTypeInfo());
		}

		public static ConstructorInfo GetConstructor(this Type type, Type[] args)
		{
			return type.GetTypeInfo().DeclaredConstructors.FirstOrDefault(r => r.GetParameters().Select(p => p.ParameterType).SequenceEqual(args));
		}
#else

		public static T GetCustomAttribute<T>(this Type type, bool inherit)
			where T: Attribute
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
#endif
	}
}
