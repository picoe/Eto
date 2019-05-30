using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Linq;

namespace Eto.Serialization.Json.Converters
{
	public class DelegateConverter : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}

		public override object ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var binder = serializer.SerializationBinder as EtoBinder;
			if (binder != null)
			{
				var instance = binder.Instance;
				if (instance != null)
				{
					var obj = JToken.ReadFrom(reader) as JValue;
					if (obj != null)
					{
						var methodName = Convert.ToString(obj.Value);
						var instanceType = instance.GetType();
						var method = instanceType.GetRuntimeMethods().FirstOrDefault(r => r.Name == methodName);
						if (method == null)
							throw new JsonSerializationException(string.Format("Could not find method {0} of type {1}", methodName, instanceType));

						return method.CreateDelegate(objectType, instance);
					}
				}
			}
			return null;
		}

		public override bool CanConvert(Type objectType)
		{
			return typeof(Delegate).IsAssignableFrom(objectType);
		}
	}
}

