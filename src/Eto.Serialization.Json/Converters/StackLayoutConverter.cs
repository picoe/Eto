using System;
using System.Globalization;
using System.Reflection;
using Eto.Forms;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Eto.Serialization.Json.Converters
{
	public class StackLayoutConverter : JsonConverter
	{
		public override bool CanWrite
		{
			get { return false; }
		}

		public override bool CanConvert(Type objectType)
		{
			return typeof(StackLayoutItem).IsAssignableFrom(objectType);
		}

		public override object ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			object instance;
			if (reader.TokenType == JsonToken.Null)
			{
				return null;
			}
			if (reader.TokenType == JsonToken.String)
			{
				return new StackLayoutItem { Control = Convert.ToString(reader.Value) };
			}

			var container = JObject.Load(reader);
			if (container["$type"] == null)
			{
				instance = new StackLayoutItem();
				serializer.Populate(container.CreateReader(), instance);
			}
			else
			{
				var type = Type.GetType((string)container["$type"]);
				if (!typeof(StackLayoutItem).IsAssignableFrom(type))
				{
					var item = new StackLayoutItem();
					item.Control = serializer.Deserialize(container.CreateReader()) as Control;
					instance = item;
				}
				else
				{
					instance = serializer.Deserialize(container.CreateReader());
				}
			}
			return instance;
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}
	}
}

