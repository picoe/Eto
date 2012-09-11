using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace Eto.Json
{
	public class DelegateConverter : JsonConverter
	{
		public override void WriteJson (JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotImplementedException ();
		}

		public override object ReadJson (Newtonsoft.Json.JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var instance = serializer.Context.Context;
			if (instance != null) {
				var obj = JObject.ReadFrom (reader);
				if (obj is JValue) {
					var methodName = Convert.ToString (((JValue)obj).Value);
					var instanceType = instance.GetType ();
					var method = instanceType.GetMethod (methodName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
					if (method == null)
						throw new JsonSerializationException(string.Format ("Could not find method {0} of type {1}", methodName, instanceType));
					return Delegate.CreateDelegate (objectType, instance, method);
				}
			}
			return null;
		}

		public override bool CanConvert (Type objectType)
		{
			return typeof(Delegate).IsAssignableFrom(objectType);
		}
	}
}

