using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using Eto.Serialization.Json.Converters;

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

			var idprop = list.FirstOrDefault(r => r.PropertyName == "ID");
			if (idprop != null)
			{
				var propertyInfo = type.GetRuntimeProperty("ID");
				if (propertyInfo != null)
				{
					list.Remove(idprop); // replaced with our own.

					// allow for $name or ID.
					var prop = CreateProperty(propertyInfo, memberSerialization);
					prop.PropertyName = "$name";
					prop.PropertyType = typeof(NameConverter.Info);
					prop.Converter = new NameConverter();
					prop.ValueProvider = new NameConverter.ValueProvider();
					list.Add(prop);

					prop = CreateProperty(propertyInfo, memberSerialization);
					prop.PropertyName = "ID";
					prop.PropertyType = typeof(NameConverter.Info);
					prop.Converter = new NameConverter();
					prop.ValueProvider = new NameConverter.ValueProvider();
					list.Add(prop);
				}
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
