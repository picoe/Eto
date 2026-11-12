using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Eto.Serialization.Json.Converters
{
	/// <summary>
	/// Reads <c>"{Binding Path, Mode=OneWay, Converter={StaticResource key}, ConverterParameter=value}"</c> values for a property, references to named objects, and anything else as usual.
	/// </summary>
	/// <remarks>
	/// References are <c>{ "$ref": "name" }</c> or <c>"{x:Reference name}"</c>, and can refer to an object later in the file.
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
					if (info.ConverterKey != null)
					{
						info.Converter = NameScope.Get(serializer).GetResource(info.ConverterKey, reader) as IValueConverter
							?? throw new JsonSerializationException($"Resource '{info.ConverterKey}' is not an IValueConverter");
					}
					if (objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(IIndirectBinding<>))
					{
						var valueType = objectType.GetGenericArguments()[0];
						if (info.Converter == null)
							return Activator.CreateInstance(typeof(PropertyBinding<>).MakeGenericType(valueType), info.Path, true);
						var convert = typeof(IndirectBinding<object>).GetMethods().First(r => r.Name == nameof(IndirectBinding<object>.Convert) && r.IsGenericMethod && r.GetParameters().FirstOrDefault()?.ParameterType == typeof(IValueConverter));
						return convert.MakeGenericMethod(valueType).Invoke(Binding.Property<object>(info.Path), new[] { info.Converter, info.ConverterParameter, null });
					}
					return info;
				}
			}

			var reference = ReferenceInfo.Read(reader, serializer, out var loaded);
			if (reference != null)
				return reference;
			if (loaded != null)
			{
				reader = loaded.CreateReader();
				reader.Read();
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

		/// <summary>Key of the converter given as <c>{StaticResource key}</c></summary>
		public string ConverterKey { get; set; }

		public IValueConverter Converter { get; set; }

		public object ConverterParameter { get; set; }

		// splits on commas that aren't inside nested {}, e.g. Converter={StaticResource key}
		static IEnumerable<string> SplitParts(string body)
		{
			var depth = 0;
			var start = 0;
			for (int i = 0; i < body.Length; i++)
			{
				var c = body[i];
				if (c == '{')
					depth++;
				else if (c == '}')
					depth--;
				else if (c == ',' && depth == 0)
				{
					yield return body.Substring(start, i - start);
					start = i + 1;
				}
			}
			yield return body.Substring(start);
		}

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
			foreach (var part in SplitParts(body))
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
					else if (string.Equals(name, "Converter", StringComparison.OrdinalIgnoreCase))
					{
						const string prefix = "{StaticResource";
						if (!value.StartsWith(prefix, StringComparison.Ordinal) || !value.EndsWith("}", StringComparison.Ordinal))
							throw new JsonSerializationException($"Converter must be a {{StaticResource key}} in binding '{text}'");
						info.ConverterKey = value.Substring(prefix.Length, value.Length - prefix.Length - 1).Trim();
					}
					else if (string.Equals(name, "ConverterParameter", StringComparison.OrdinalIgnoreCase))
						info.ConverterParameter = value;
					else
						throw new JsonSerializationException($"Unknown binding option '{name}' in '{text}'");
				}
				first = false;
			}
			return info;
		}
	}

	/// <summary>
	/// Sets up a data context binding when given a <see cref="BindingInfo"/>, the named object for a <see cref="ReferenceInfo"/>, otherwise sets the value.
	/// </summary>
	class BindingValueProvider : IValueProvider
	{
		readonly IValueProvider inner;
		readonly string propertyName;
		readonly Type propertyType;

		public BindingValueProvider(IValueProvider inner, string propertyName, Type propertyType)
		{
			this.inner = inner;
			this.propertyName = propertyName;
			this.propertyType = propertyType;
		}

		public object GetValue(object target) => inner.GetValue(target);

		public void SetValue(object target, object value)
		{
			if (value is BindingInfo info && target is IBindable bindable)
			{
				var source = Binding.Property<object>(info.Path);
				if (info.Converter != null)
					source = source.Convert(info.Converter, propertyType, info.ConverterParameter);
				var binding = bindable.BindDataContext(Binding.Property<object>(propertyName), source, info.Mode);
				binding.Mode = info.Mode;
				return;
			}
			if (value is ReferenceInfo reference)
			{
				reference.Scope.Resolve(reference, v => inner.SetValue(target, v));
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
