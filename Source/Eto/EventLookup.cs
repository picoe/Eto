using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;

namespace Eto
{
	static class EventLookup
	{
		static readonly Dictionary<Type, List<EventDeclaration>> registeredEvents = new Dictionary<Type, List<EventDeclaration>>();
		static readonly Assembly etoAssembly = typeof(EventLookup).Assembly;
		static readonly Dictionary<Type, string[]> externalEvents = new Dictionary<Type, string[]>();

		struct EventDeclaration
		{
			public readonly string Identifier;
			public readonly MethodInfo Method;

			public EventDeclaration(MethodInfo method, string identifier)
			{
				Method = method;
				Identifier = identifier;
			}
		}

		public static void Register<T>(Expression<Action<T>> expression, string identifier)
		{
			var method = ((MethodCallExpression)expression.Body).Method;
			var declarations = GetDeclarations(typeof(T));
			declarations.Add(new EventDeclaration(method, identifier));
		}

		public static void HookupEvents(InstanceWidget widget)
		{
			var type = widget.GetType();
			if (type.Assembly == etoAssembly)
				return;
			widget.HandleDefaultEvents(GetEvents(type));
		}

		public static bool IsDefault(InstanceWidget widget, string identifier)
		{
			var type = widget.GetType();
			if (type.Assembly == etoAssembly)
				return false;
			var events = GetEvents(type);
			return Array.IndexOf(events, identifier) >= 0;
		}

		static string[] GetEvents(Type type)
		{
			string[] events;
			if (!externalEvents.TryGetValue(type, out events))
			{
				events = FindTypeEvents(type).ToArray();
				externalEvents.Add(type, events);
			}
			return events;
		}

		static IEnumerable<string> FindTypeEvents(Type type)
		{
			var externalTypes = new List<Type>();
			var current = type;
			while (current != null)
			{
				if (current.Assembly == etoAssembly)
				{
					List<EventDeclaration> declarations;
					if (registeredEvents.TryGetValue(current, out declarations))
					{
						foreach (var externalType in externalTypes)
						{
							var methods = externalType.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
							foreach (var item in declarations)
							{
								var currentItem = item;
								if (methods.Any(r => r.GetBaseDefinition() == currentItem.Method))
									yield return item.Identifier;
							}
						}
					}
				}
				else
					externalTypes.Add(current);
				current = current.BaseType;
			}
		}

		static List<EventDeclaration> GetDeclarations(Type type)
		{
			List<EventDeclaration> declarations;
			if (!registeredEvents.TryGetValue(type, out declarations))
			{
				declarations = new List<EventDeclaration>();
				registeredEvents.Add(type, declarations);
			}
			return declarations;
		}
	}
}

