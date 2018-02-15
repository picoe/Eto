using System;
using System.Globalization;
using System.Reflection;
using Eto.Forms;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Eto.Serialization.Json.Converters
{
	public class ListItemConverter : JsonConverter
	{
		public override bool CanWrite
		{
			get { return false; }
		}

		public override bool CanConvert(Type objectType)
		{
			return typeof(IListItem).IsAssignableFrom(objectType);
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
				return new ListItem { Text = Convert.ToString(reader.Value) };
			}

			var container = JObject.Load(reader);
			if (container["$type"] == null)
			{
				instance = new ListItem();
				serializer.Populate(container.CreateReader(), instance);
			}
			else
			{
				instance = serializer.Deserialize(container.CreateReader());
			}
			return instance;
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}
	}
}

