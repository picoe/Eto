using System;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Eto.Drawing;

namespace Eto.Serialization.Json.Converters
{
	public class ImageConverter : JsonConverter
	{
		public override void WriteJson (JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotImplementedException ();
		}

		public override object ReadJson (Newtonsoft.Json.JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.String)
			{
				var val = (string)((JValue)JToken.ReadFrom(reader)).Value;
				var converter = new Eto.Drawing.ImageConverter();
				return converter.ConvertFrom(val);
			}
			throw new JsonSerializationException(string.Format(CultureInfo.CurrentCulture, "Image or Icon must be defined as a resource or file string"));
		}

		public override bool CanConvert (Type objectType)
		{
			return typeof(Image).IsAssignableFrom (objectType);
		}
	}
}

