using System;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Eto.Drawing;
using System.Linq;

namespace Eto.Serialization.Json
{
	public class FontConverter : JsonConverter
	{
		public override bool CanWrite { get { return false; } }

		public override bool CanRead { get { return true; } }

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}

		public override object ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.StartObject)
			{
				var defaultFont = SystemFonts.Default();
				var familyName = defaultFont.FamilyName;
				var size = defaultFont.Size;
				var fontStyle = FontStyle.None;
				var decoration = FontDecoration.None;
				string typefaceName = null;

				var items = JToken.ReadFrom(reader);
				foreach (var property in items.Children<JProperty>())
				{
					// Family, Size, FontStyle
					switch (property.Name)
					{
						case "Family":
							familyName = property.Value.ToString();
							break;
						case "Typeface":
							typefaceName = property.Value.ToString();
							break;
						case "Size":
							float ret;
							if (float.TryParse(property.Value.ToString(), out ret) && ret > 0)
								size = ret;
							break;
						case "FontStyle":
							if (property.Value.Type == JTokenType.String)
							{
								fontStyle |= ParseFontStyle(property.Value.Value<string>());
							}
							else
							{
								foreach (var style in property.Value)
								{
									fontStyle |= ParseFontStyle(style.Value<string>());
								}
							}
							break;
						case "FontDecoration":
							if (property.Value.Type == JTokenType.String)
							{
								decoration |= ParseFontDecoration(property.Value.Value<string>());
							}
							else
							{
								foreach (var style in property.Value)
								{
									decoration |= ParseFontDecoration(style.Value<string>());
								}
							}
							break;
					}
				}
				var family = new FontFamily(familyName);

				if (!string.IsNullOrEmpty(typefaceName))
				{
					var typeface = family.Typefaces.FirstOrDefault(r => string.Equals(r.Name, typefaceName, StringComparison.OrdinalIgnoreCase));
					if (typeface != null)
						return new Font(typeface, size, decoration);
				}
				return new Font(family, size, fontStyle, decoration);
			}
			throw new JsonSerializationException(string.Format(CultureInfo.CurrentCulture, "Font must be defined as a property from Family, Typeface, Size, FontStyle"));
		}

		static FontStyle ParseFontStyle(string value)
		{
			switch (value)
			{
				case "Bold":
					return FontStyle.Bold;
				case "Italic":
					return FontStyle.Italic;
			}

			return FontStyle.None;
		}

		static FontDecoration ParseFontDecoration(string value)
		{
			switch (value)
			{
				case "Underline":
					return FontDecoration.Underline;
				case "Strikethrough":
					return FontDecoration.Strikethrough;
			}

			return FontDecoration.None;
		}

		public override bool CanConvert(Type objectType)
		{
			return typeof(Font).IsAssignableFrom(objectType);
		}
	}
}