using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

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
			public readonly string MethodName;
			public readonly Type Type;

			public EventDeclaration(Type type, string methodName, string identifier)
			{
				Type = type;
				MethodName = methodName;
				Identifier = identifier;
			}
		}

		public static void Register(Type type, string methodName, string identifier)
		{
			var declarations = GetDeclarations(type);
			declarations.Add(new EventDeclaration(type, identifier, methodName));
		}

		public static void HookupEvents(InstanceWidget widget)
		{
			var type = widget.GetType();
			if (type.Assembly == etoAssembly)
				return;
			widget.HandleEvents(GetEvents(type));
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
						foreach (var item in declarations)
						{
							foreach (var externalType in externalTypes)
							{
								var method = externalType.GetMethod(item.MethodName, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
								if (method != null)
								{
									var baseMethod = method.GetBaseDefinition();
									if (baseMethod != null && baseMethod.DeclaringType == item.Type)
										yield return item.Identifier;
								}
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

