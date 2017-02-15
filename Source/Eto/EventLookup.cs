using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using Eto.Forms;

namespace Eto
{
	static class EventLookup
	{
		static readonly Dictionary<Type, List<EventDeclaration>> registeredEvents = new Dictionary<Type, List<EventDeclaration>>();
		static readonly Assembly etoAssembly = typeof(EventLookup).GetAssembly();
		static readonly Dictionary<Type, object[]> externalEvents = new Dictionary<Type, object[]>();

		struct EventDeclaration
		{
			public readonly object Identifier;
			public readonly MethodInfo Method;

			public EventDeclaration(MethodInfo method, object identifier)
			{
				Method = method;
				Identifier = identifier;
			}
		}

		public static void Register<T, TArgs>(Expression<Action<T, TArgs>> expression, object identifier)
		{
			var method = ((MethodCallExpression)expression.Body).Method;
			var declarations = GetDeclarations(typeof(T));
			declarations.Add(new EventDeclaration(method, identifier));
		}

		public static void Register<T>(Expression<Action<T>> expression, string identifier)
		{
			var method = ((MethodCallExpression)expression.Body).Method;
			var declarations = GetDeclarations(typeof(T));
			declarations.Add(new EventDeclaration(method, identifier));
		}

		public static void HookupEvents(Widget widget)
		{
			var type = widget.GetType();

			if (type.GetAssembly() == etoAssembly)
				return;

			var handler2 = widget.Handler as IHandler2;
			var handler = widget.Handler as Widget.IHandler;

			var events = GetEvents(type);
			for (int i = 0; i < events.Length; i++)
			{
				var evt = events[i];
				var id = evt as string;
				if (id != null)
					handler?.HandleEvent(id, true);
				else
					handler2?.AttachEvent(widget, evt);
			}
		}

		public static bool IsDefault(Widget widget, string identifier)
		{
			var type = widget.GetType();

			if (type.GetAssembly() == etoAssembly)
				return false;
			var events = GetEvents(type);
			return Array.IndexOf(events, identifier) >= 0;
		}

		static object[] GetEvents(Type type)
		{
			object[] events;
			if (!externalEvents.TryGetValue(type, out events))
			{
				events = FindTypeEvents(type).Distinct().ToArray();
				externalEvents.Add(type, events);
			}
			return events;
		}

		static IEnumerable<object> FindTypeEvents(Type type)
		{
			var externalTypes = new List<Type>();
			var current = type;
			while (current != null)
			{
				if (current.GetAssembly() == etoAssembly)
				{
					List<EventDeclaration> declarations;
					if (registeredEvents.TryGetValue(current, out declarations))
					{
						foreach (var externalType in externalTypes)
						{
#if PCL
							var methods = (from m in externalType.GetTypeInfo().DeclaredMethods select m).ToList();
#else
							var methods = externalType.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
#endif
							foreach (var item in declarations)
							{
								var currentItem = item;
#if PCL
								if (methods.Any(r => r.GetRuntimeBaseDefinition() == currentItem.Method))
									yield return item.Identifier;
#else
								if (methods.Any(r => r.GetBaseDefinition() == currentItem.Method))
									yield return item.Identifier;
#endif
							}
						}
					}
				}
				else
					externalTypes.Add(current);
				current = current.GetBaseType();
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

