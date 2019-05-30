using System;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Eto.Drawing;
using System.Linq;

namespace Eto.Serialization.Json.Converters
{
	public class FontConverter : TypeConverterConverter
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
				string familyName = null;
				float? size = null;
				var fontStyle = FontStyle.None;
				var decoration = FontDecoration.None;
				SystemFont? systemFont = null;
				string typefaceName = null;

				var items = JToken.ReadFrom(reader);
				foreach (var property in items.Children<JProperty>())
				{
					// Family, Size, FontStyle
					switch (property.Name.ToLowerInvariant())
					{
						case "family":
							familyName = property.Value.ToString();
							break;
						case "systemfont":
							SystemFont tsystemfont;
							if (Enum.TryParse(property.Value.ToString(), true, out tsystemfont))
								systemFont = tsystemfont;
							break;
						case "typeface":
							typefaceName = property.Value.ToString();
							break;
						case "size":
							float ret;
							if (float.TryParse(property.Value.ToString(), out ret) && ret > 0)
								size = ret;
							break;
						case "style":
						case "fontstyle":
							if (property.Value.Type == JTokenType.String)
							{
								FontStyle tstyle;
								if (Enum.TryParse(property.Value.ToString(), true, out tstyle))
									fontStyle |= tstyle;
							}
							else
							{
								foreach (var style in property.Value)
								{
									FontStyle tstyle;
									if (Enum.TryParse(style.ToString(), true, out tstyle))
										fontStyle |= tstyle;
								}
							}
							break;
						case "decoration":
						case "fontdecoration":
							if (property.Value.Type == JTokenType.String)
							{
								FontDecoration tdecoration;
								if (Enum.TryParse(property.Value.ToString(), true, out tdecoration))
									decoration |= tdecoration;
							}
							else
							{
								foreach (var style in property.Value)
								{
									FontDecoration tdecoration;
									if (Enum.TryParse(style.ToString(), true, out tdecoration))
										decoration |= tdecoration;
								}
							}
							break;
					}
				}
				if (systemFont != null)
					return new Font(systemFont.Value, size, decoration);
				
				if (familyName == null)
					familyName = SystemFonts.Default().FamilyName;
				if (size == null)
					size = SystemFonts.Default().Size;
				var family = new FontFamily(familyName);

				if (!string.IsNullOrEmpty(typefaceName))
				{
					var typeface = family.Typefaces.FirstOrDefault(r => string.Equals(r.Name, typefaceName, StringComparison.OrdinalIgnoreCase));
					if (typeface != null)
						return new Font(typeface, size.Value, decoration);
				}
				return new Font(family, size.Value, fontStyle, decoration);
			}
			// try base (type converter)
			var font = base.ReadJson(reader, objectType, existingValue, serializer);
			if (font == null)
				throw new JsonSerializationException(string.Format(CultureInfo.CurrentCulture, "Font must be defined as a string or an object with Family, Typeface, Size, Style, and Decoration properties"));
			return font;
		}

		public override bool CanConvert(Type objectType)
		{
			return typeof(Font).IsAssignableFrom(objectType);
		}
	}
}