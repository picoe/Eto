using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;
using Newtonsoft.Json.Serialization;

namespace Eto.Json
{
	public class NameConverter : JsonConverter
	{
		public class Info
		{
			public object Instance { get; set; }
			public PropertyInfo PropertyInfo { get; set; }
			public FieldInfo FieldInfo { get; set; }

			public void SetValue(object value)
			{
				if (PropertyInfo != null)
					PropertyInfo.SetValue (Instance, value, null);
				else
					FieldInfo.SetValue (Instance, value);
			}
		}

		public class ValueProvider : IValueProvider
		{
			public void SetValue (object target, object value)
			{
				var info = value as Info;
				if (info != null)
					info.SetValue (target);
			}

			public object GetValue (object target)
			{
				return null;
			}
		}

		public override void WriteJson (JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotImplementedException ();
		}

		public override object ReadJson (Newtonsoft.Json.JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var obj = JToken.ReadFrom (reader);
			var id = (string)obj;
			var instance = serializer.Context.Context;
			if (instance != null) {
				var instanceType = instance.GetType ();

				var property = instanceType.GetProperty (id, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
				if (property != null)
					return new Info {
						PropertyInfo = property,
						Instance = instance
					};

				var field = instanceType.GetField (id, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
				if (field != null)
					return new Info {
						FieldInfo = field,
						Instance = instance
					};
			}
			return null;
		}

		public override bool CanConvert (Type objectType)
		{
			return true;
		}
	}
}

