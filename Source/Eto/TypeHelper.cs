using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace System
{
	/// <summary>
	/// Extension methods to replace methods missing in WinRT.
	/// 
	/// Some of these are from https://gist.github.com/jeffwilcox/2432351 (no attribution requested.)
	/// </summary>
	internal static class TypeHelper // internal so that unrelated assemblies can link to the same source file without errors
	{
		#region IsEnum
		public static bool IsEnum(this Type type)
		{
#if WINRT
			return type.GetTypeInfo().IsEnum;
#else
			return type.IsEnum;
#endif
		}
		#endregion


		#region GetAllProperties
#if WINRT
		public static List<PropertyInfo> GetAllProperties(this Type type)
		{
			var result = new List<PropertyInfo>();
			type.GetAllProperties(result);
			return result;
		}

		private static void GetAllProperties(this Type type, List<PropertyInfo> result)
		{
			var typeInfo = type.GetTypeInfo();

			if (result == null)
				result = typeInfo.DeclaredProperties.ToList();
			else
				result.AddRange(typeInfo.DeclaredProperties);

			if (typeInfo.BaseType != null)
				typeInfo.BaseType.GetAllProperties(result);
		}
#else

#endif
		#endregion

		#region GetRuntimeProperty

		/// <summary>
		/// Returns a PropertyInfo for the specified property of the current type.
		/// The property may be declared on the current type or an ancestor type.
		/// </summary>
		/// <returns></returns>
		public static PropertyInfo GetRuntimePropertyInfo(this Type type, string propertyName)
		{
#if WINRT			
			var typeInfo = type.GetTypeInfo();
			var result = typeInfo.GetRuntimeProperty(propertyName);
			if (result == null &&
			    typeInfo.BaseType != null)
				result = GetRuntimePropertyInfo(typeInfo.BaseType, propertyName); // recursive
#else
			return type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
#endif
		}

		#endregion

		#region GetRuntimeMethodInfo

		/// <summary>
		/// Returns a MethodInfo for the specified method of the current type.
		/// The Method may be declared on the current type or an ancestor type.
		/// </summary>
		/// <returns></returns>
		public static MethodInfo GetRuntimeMethodInfo(this Type type, string MethodName)
		{
#if WINRT			
			var typeInfo = type.GetTypeInfo();
			var result = typeInfo.GetRuntimeMethod(MethodName);
			if (result == null &&
			    typeInfo.BaseType != null)
				result = GetRuntimeMethodInfo(typeInfo.BaseType, MethodName); // recursive
#else
			return type.GetMethod(MethodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
#endif
		}

		#endregion

#if WINRT

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
			return o != null && type.IsAssignableFrom(o.GetType());
		}

		internal static bool ImplementInterface(this Type type, Type ifaceType)
		{
			while (type != null)
			{
				Type[] interfaces = type.GetTypeInfo().ImplementedInterfaces.ToArray(); //  .GetInterfaces();
				if (interfaces != null)
				{
					for (int i = 0; i < interfaces.Length; i++)
					{
						if (interfaces[i] == ifaceType || (interfaces[i] != null && interfaces[i].ImplementInterface(ifaceType)))
						{
							return true;
						}
					}
				}
				type = type.GetTypeInfo().BaseType;
				// type = type.BaseType;
			}
			return false;
		}

		public static bool IsAssignableFrom(this Type type, Type c)
		{
			if (c == null)
			{
				return false;
			}
			if (type == c)
			{
				return true;
			}

			//RuntimeType runtimeType = type.UnderlyingSystemType as RuntimeType;
			//if (runtimeType != null)
			//{
			//    return runtimeType.IsAssignableFrom(c);
			//}

			//if (c.IsSubclassOf(type))
			if (c.GetTypeInfo().IsSubclassOf(c))
			{
				return true;
			}

			//if (type.IsInterface)
			if (type.GetTypeInfo().IsInterface)
			{
				return c.ImplementInterface(type);
			}

			if (type.IsGenericParameter)
			{
				Type[] genericParameterConstraints = type.GetTypeInfo().GetGenericParameterConstraints();
				for (int i = 0; i < genericParameterConstraints.Length; i++)
				{
					if (!genericParameterConstraints[i].IsAssignableFrom(c))
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}
#endif
	}
}
