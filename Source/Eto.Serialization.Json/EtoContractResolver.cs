using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace Eto.Serialization.Json
{
	public class EtoContractResolver : DefaultContractResolver
	{
		protected override IValueProvider CreateMemberValueProvider(MemberInfo member)
		{
			var eventInfo = member as EventInfo;
			if (eventInfo != null)
			{
				return new EventValueProvider { EventInfo = eventInfo };
			}
			return base.CreateMemberValueProvider(member);
		}

		protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
		{
			var list = base.CreateProperties(type, memberSerialization);

			// add events as json properties
			foreach (var eventInfo in type.GetRuntimeEvents().Where(r => 
			{
				var addMethod = r.GetAddMethod();
				return addMethod.IsPublic && !addMethod.IsStatic 
					&& (
						(
							#if PCL
							r.EventHandlerType.GetTypeInfo().IsGenericType
							#else
							r.EventHandlerType.IsGenericType
							#endif
							&& r.EventHandlerType.GetGenericTypeDefinition() == typeof(EventHandler<>)
						)
						|| r.EventHandlerType == typeof(EventHandler)
					);
			}))
			{
				var prop = new JsonProperty
				{
					PropertyName = ResolvePropertyName(eventInfo.Name),
					DeclaringType = eventInfo.DeclaringType,
					PropertyType = eventInfo.EventHandlerType,
					ValueProvider = CreateMemberValueProvider(eventInfo),
					Writable = true
				};
				list.Add(prop);
			}

			var idprop = type.GetRuntimeProperty("ID");
			if (idprop != null)
			{
				var prop = CreateProperty(idprop, memberSerialization);
				prop.PropertyName = "$name";
				prop.PropertyType = typeof(NameConverter.Info);
				prop.MemberConverter = new NameConverter();
				prop.ValueProvider = new NameConverter.ValueProvider();
				list.Add(prop);
			}

			if (list.All(r => r.PropertyName != "$type"))
			{
				var prop = new JsonProperty();
				prop.PropertyName = "$type";
				prop.Ignored = true;
				list.Add(prop);
			}

			return list;
		}
	}
}
