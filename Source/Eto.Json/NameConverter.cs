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
			public FieldInfo FieldInfo { get; set; }

			public void SetValue(object value)
			{
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
			var obj = JValue.ReadFrom (reader);
			var id = (string)obj;
			var instance = serializer.Context.Context;
			if (instance != null) {
				var instanceType = instance.GetType ();

				var fieldInfo = instanceType.GetField (id, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
				if (fieldInfo != null)
					return new Info {
						FieldInfo = fieldInfo,
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

