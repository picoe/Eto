using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Eto.Serialization.Json.Converters
{
	/// <summary>
	/// Reads <c>"{Binding Path, Mode=OneWay}"</c> values for a property, and anything else as usual.
	/// </summary>
	/// <remarks>
	/// Use <c>"{}{Binding}"</c> for text that should literally start with <c>{Binding</c>.
	/// </remarks>
	class BindingConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType) => true;

		public override bool CanWrite => false;

		public override void WriteJson(Newtonsoft.Json.JsonWriter writer, object value, JsonSerializer serializer) => throw new NotSupportedException();

		public override object ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.String && reader.Value is string text)
			{
				if (text.StartsWith("{}", StringComparison.Ordinal))
					return serializer.Deserialize(new JTokenReader(new JValue(text.Substring(2))), objectType);

				var info = BindingInfo.Parse(text);
				if (info != null)
				{
					if (objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(IIndirectBinding<>))
						return Activator.CreateInstance(typeof(PropertyBinding<>).MakeGenericType(objectType.GetGenericArguments()), info.Path, true);
					return info;
				}
			}

			// match the default of filling in objects that already exist, e.g. a Menu's items
			if (existingValue != null && !objectType.IsValueType && (reader.TokenType == JsonToken.StartObject || reader.TokenType == JsonToken.StartArray))
			{
				serializer.Populate(reader, existingValue);
				return existingValue;
			}

			return serializer.Deserialize(reader, objectType);
		}
	}

	class BindingInfo
	{
		public string Path { get; set; }

		public DualBindingMode Mode { get; set; } = DualBindingMode.TwoWay;

		public static BindingInfo Parse(string text)
		{
			text = text.Trim();
			if (!text.StartsWith("{Binding", StringComparison.Ordinal) || !text.EndsWith("}", StringComparison.Ordinal))
				return null;
			var body = text.Substring(8, text.Length - 9);
			if (body.Length > 0 && !char.IsWhiteSpace(body[0]) && body[0] != ',')
				return null; // e.g. {BindingFoo}

			var info = new BindingInfo();
			var first = true;
			foreach (var part in body.Split(','))
			{
				var item = part.Trim();
				var equals = item.IndexOf('=');
				if (equals < 0)
				{
					if (!first)
						throw new JsonSerializationException($"Expected Name=Value in binding '{text}'");
					if (item.Length > 0)
						info.Path = item;
				}
				else
				{
					var name = item.Substring(0, equals).Trim();
					var value = item.Substring(equals + 1).Trim();
					if (string.Equals(name, "Path", StringComparison.OrdinalIgnoreCase))
						info.Path = value;
					else if (string.Equals(name, "Mode", StringComparison.OrdinalIgnoreCase))
						info.Mode = (DualBindingMode)Enum.Parse(typeof(DualBindingMode), value, true);
					else
						throw new JsonSerializationException($"Unknown binding option '{name}' in '{text}'");
				}
				first = false;
			}
			return info;
		}
	}

	/// <summary>
	/// Sets up a data context binding when given a <see cref="BindingInfo"/>, otherwise sets the value.
	/// </summary>
	class BindingValueProvider : IValueProvider
	{
		readonly IValueProvider inner;
		readonly string propertyName;

		public BindingValueProvider(IValueProvider inner, string propertyName)
		{
			this.inner = inner;
			this.propertyName = propertyName;
		}

		public object GetValue(object target) => inner.GetValue(target);

		public void SetValue(object target, object value)
		{
			if (value is BindingInfo info && target is IBindable bindable)
			{
				var binding = bindable.BindDataContext(Binding.Property<object>(propertyName), Binding.Property<object>(info.Path), info.Mode);
				binding.Mode = info.Mode;
				return;
			}
			inner.SetValue(target, value);
		}
	}

	/// <summary>
	/// Reads <c>"d:DataContext"</c>, which is only used by the designer.
	/// </summary>
	/// <remarks>
	/// The value is either a type name to create, or an object with a <c>$type</c> and sample property values.
	/// </remarks>
	class DesignDataContextConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType) => true;

		public override bool CanWrite => false;

		public override void WriteJson(Newtonsoft.Json.JsonWriter writer, object value, JsonSerializer serializer) => throw new NotSupportedException();

		public override object ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (!Control.IsDesignRootLoad)
			{
				reader.Skip();
				return null;
			}

			if (reader.TokenType == JsonToken.String && reader.Value is string typeName)
			{
				var type = ((EtoBinder)serializer.SerializationBinder).BindToType(typeName);
				// type-only is still useful for completion, so no default constructor isn't an error
				if (type == null || (!type.IsValueType && type.GetConstructor(Type.EmptyTypes) == null))
					return null;
				return Activator.CreateInstance(type);
			}

			return serializer.Deserialize(reader, typeof(object));
		}
	}

	class DesignDataContextValueProvider : IValueProvider
	{
		public object GetValue(object target) => null;

		public void SetValue(object target, object value)
		{
			if (value != null && target is BindableWidget widget)
				widget.DataContext = value;
		}
	}
}
