using System;
using Newtonsoft.Json;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json.Linq;

namespace Eto.Serialization.Json.Converters
{
	public class TypeConverterConverter : JsonConverter
	{
		readonly Dictionary<Type, TypeConverter> converters = new Dictionary<Type, TypeConverter>();

		public override bool CanRead { get { return true; } }
		public override bool CanWrite { get { return false; } }

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
		}

		public override object ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			TypeConverter converter;
			if (reader.ValueType == null)
			{
				var obj = JObject.Load(reader);
				Type type = null;
				var typeToken = obj["$type"];
				if (typeToken != null)
				{
					string assemblyName = null;
					var typeName = typeToken.Value<string>();
					var assemblyIndex = typeName.IndexOf(',');
					if (assemblyIndex > 0)
					{
						assemblyName = typeName.Substring(assemblyIndex + 1).Trim();
						typeName = typeName.Substring(0, assemblyIndex);
					}
					type = serializer.Binder.BindToType(assemblyName, typeName);
				}
				else
					type = objectType;

				var target = Activator.CreateInstance(type);
				serializer.Populate(obj.CreateReader(), target);
				return target;
			}
			if (objectType == reader.ValueType)
				return reader.Value;
			if (converters.TryGetValue(objectType, out converter))
			{
				if (converter.CanConvertFrom(reader.ValueType))
					return converter.ConvertFrom(reader.Value);
				else
					return reader.Value;
			}
			return existingValue;
		}

		public override bool CanConvert(Type objectType)
		{
			if (converters.ContainsKey(objectType))
				return true;
			var converter = TypeDescriptor.GetConverter(objectType);
			if (converter != null && converter.CanConvertFrom(typeof(string)))
			{
				converters.Add(objectType, converter);
				return true;
			}
			return false;
		}
	}
}