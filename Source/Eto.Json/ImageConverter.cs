using System;
using Newtonsoft.Json;
using Eto.Drawing;
using Newtonsoft.Json.Linq;
using Eto.Forms;
using System.IO;

namespace Eto.Json
{
	public class ImageConverter : JsonConverter
	{
		public override void WriteJson (JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotImplementedException ();
		}

		const string ResourcePrefix = "resource:";
		const string FilePrefix = "file:";

		bool IsIcon(string fileName)
		{
			return fileName.EndsWith (".ico");
		}

		public override object ReadJson (Newtonsoft.Json.JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.String) {
				var val = (string)((JValue)JValue.ReadFrom (reader)).Value;
				if (val.StartsWith (FilePrefix)) {
					var fileName = val.Substring (FilePrefix.Length);
					if (!Path.IsPathRooted (fileName))
						fileName = Path.Combine (EtoEnvironment.GetFolderPath(EtoSpecialFolder.ApplicationResources), fileName);
					if (IsIcon (fileName))
						return new Icon (fileName);
					else
						return new Bitmap (fileName);
				} else {
					if (val.StartsWith (ResourcePrefix))
						val = val.Substring (ResourcePrefix.Length);
					var ns = new NamespaceInfo (val);
					var isIcon = typeof(Icon).IsAssignableFrom (objectType);
					isIcon |= IsIcon (ns.Namespace);
					if (isIcon)
						return Icon.FromResource (ns.Assembly, ns.Namespace);
					else
						return Bitmap.FromResource (ns.Assembly, ns.Namespace);
				}
			}
			else
				throw new JsonSerializationException("Image or Icon must be defined as a resource or file string");
		}

		public override bool CanConvert (Type objectType)
		{
			return typeof(Image).IsAssignableFrom (objectType);
		}
	}
}

