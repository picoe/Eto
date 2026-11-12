using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
namespace Eto.Serialization.Json.Converters
{
	public class NameConverter : JsonConverter
	{
		public class Info
		{
			public object Instance { get; set; }

			public PropertyInfo PropertyInfo { get; set; }

			public FieldInfo FieldInfo { get; set; }

			internal string Name { get; set; }

			internal NameScope Scope { get; set; }

			public void SetValue(object value)
			{
				if (PropertyInfo != null)
					PropertyInfo.SetValue(Instance, value, null);
				else
					FieldInfo?.SetValue(Instance, value);
				if (Name != null)
					Scope?.Add(Name, value);
			}
		}

		public class ValueProvider : IValueProvider
		{
			public void SetValue(object target, object value)
			{
				var info = value as Info;
				if (info != null)
					info.SetValue(target);
			}

			public object GetValue(object target)
			{
				return null;
			}
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}

		public override object ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var obj = JToken.ReadFrom(reader);
			var id = (string)obj;
			var binder = serializer.SerializationBinder as EtoBinder;
			var info = CreateInfo(binder?.Instance, id) ?? new Info();
			info.Name = id;
			info.Scope = serializer.ReferenceResolver as NameScope;
			return info;
		}

		/// <summary>Gets the field or property of <paramref name="instance"/> to assign the object named <paramref name="id"/> to, if any.</summary>
		internal static Info CreateInfo(object instance, string id)
		{
			if (instance == null || id == null)
				return null;
			var instanceType = instance.GetType();

			var property = instanceType.GetRuntimeProperties().FirstOrDefault(r => r.Name == id);
			if (property != null)
				return new Info
				{
					PropertyInfo = property,
					Instance = instance
				};

			var field = instanceType.GetRuntimeFields().FirstOrDefault(r => r.Name == id);
			if (field != null)
				return new Info
				{
					FieldInfo = field,
					Instance = instance
				};
			return null;
		}

		public override bool CanConvert(Type objectType)
		{
			return true;
		}
	}
}

