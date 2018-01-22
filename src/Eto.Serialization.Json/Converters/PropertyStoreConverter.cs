using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Eto.Serialization.Json.Converters
{
	public class PropertyStoreConverter : JsonConverter
	{
		public override void WriteJson (JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotImplementedException ();
		}

		public override object ReadJson (Newtonsoft.Json.JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var store = (PropertyStore)existingValue;
			var items = JToken.ReadFrom (reader);

			foreach (var item in items)
			{
				var typeName = (string)item["$type"];
				if (typeName != null) {

					var type = ((EtoBinder)serializer.SerializationBinder).BindToType (typeName);
					if (type != null) {
						foreach (var prop in (IDictionary<string, JToken>)item) {
							if (prop.Key == "$type") continue;
							var memberName = "Set" + prop.Key;
							var member = type.GetRuntimeMethods().FirstOrDefault(r => r.IsStatic && r.Name == memberName);
							if (member == null)
								throw new JsonSerializationException(string.Format(CultureInfo.CurrentCulture, "Could not find attachable property {0}.{1}", type.Name, memberName));
							var parameters = member.GetParameters();
							if (parameters.Length != 2)
								throw new JsonSerializationException(string.Format(CultureInfo.CurrentCulture, "Invalid number of parameters"));
							var propType = parameters[1].ParameterType;
							using (var propReader = new JTokenReader(prop.Value)) {
								var propValue = serializer.Deserialize(propReader, propType);
								member.Invoke (null, new object[] { store.Parent, propValue });
							}
						}
					}
				}
			}
			return existingValue;
		}

		public override bool CanConvert (Type objectType)
		{
			return typeof(PropertyStore).IsAssignableFrom(objectType);
		}
	}
}

