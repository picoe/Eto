using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Eto
{
	/// <summary>
	/// Extension methods to replace methods missing in WinRT.
	/// 
	/// Some of these are from https://gist.github.com/jeffwilcox/2432351 (no attribution requested.)
	/// </summary>
	internal static class TypeHelper // internal so that unrelated assemblies can link to the same source file without errors
	{
		public static bool IsEnum(this Type type)
		{
#if WINRT
			return type.GetTypeInfo().IsEnum;
#else
			return type.IsEnum;
#endif
		}

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
