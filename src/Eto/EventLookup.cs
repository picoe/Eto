namespace Eto;

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

	public static void HookupEvents(Widget widget)
	{
		var type = widget.GetType();

		if (type.Assembly == etoAssembly)
			return;

		if (widget.Handler is Widget.IHandler handler)
		{
			var ids = GetEvents(type);
			for (int i = 0; i < ids.Length; i++)
			{
				handler.HandleEvent(ids[i], true);
			}
		}
	}

	public static bool IsDefault(Widget widget, string identifier)
	{
		var type = widget.GetType();

		if (type.Assembly == etoAssembly)
			return false;
		var events = GetEvents(type);
		return Array.IndexOf(events, identifier) >= 0;
	}

	static string[] GetEvents(Type type)
	{
		if (externalEvents.TryGetValue(type, out var events))
			return events;

		events = FindTypeEvents(type).Distinct().ToArray();
		externalEvents.Add(type, events);
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
				if (registeredEvents.TryGetValue(current, out var declarations))
				{
					for (int i = 0; i < externalTypes.Count; i++)
					{
						var externalType = externalTypes[i];
						foreach (var method in externalType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
						{
							for (int j = 0; j < declarations.Count; j++)
							{
								var declaration = declarations[j];
								if (method.GetRuntimeBaseDefinition() == declaration.Method)
									yield return declaration.Identifier;
							}
						}
					}
				}
			}
			else
			{
				externalTypes.Add(current);
			}
			current = current.BaseType;
		}
	}

	static List<EventDeclaration> GetDeclarations(Type type)
	{
		if (!registeredEvents.TryGetValue(type, out var declarations))
		{
			declarations = new List<EventDeclaration>();
			registeredEvents.Add(type, declarations);
		}
		return declarations;
	}
}