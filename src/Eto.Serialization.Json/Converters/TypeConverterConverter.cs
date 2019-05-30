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
#pragma warning disable 618
		readonly Dictionary<Type, TypeConverter> converters = new Dictionary<Type, TypeConverter>();
#pragma warning restore 618

		public override bool CanRead { get { return true; } }
		public override bool CanWrite { get { return false; } }

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
		}

		public override object ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
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
					type = serializer.SerializationBinder.BindToType(assemblyName, typeName);
				}
				else
					type = objectType;

				var target = Activator.CreateInstance(type);
				serializer.Populate(obj.CreateReader(), target);
				return target;
			}
			if (objectType == reader.ValueType)
				return reader.Value;
			var converter = GetConverter(objectType, reader.ValueType);
			if (converter != null)
			{
				return converter.ConvertFrom(reader.Value);
			}
			return reader.Value;
		}

#pragma warning disable 618
		TypeConverter GetConverter(Type objectType, Type destinationType)
		{
			TypeConverter converter;
			if (converters.TryGetValue(objectType, out converter))
				return converter;
			converter = TypeDescriptor.GetConverter(objectType);
			if (converter != null && converter.CanConvertFrom(destinationType))
			{
				converters.Add(objectType, converter);
				return converter;
			}
			return null;
		}
#pragma warning restore 618

		public override bool CanConvert(Type objectType)
		{
			return GetConverter(objectType, typeof(string)) != null;
		}
	}
}