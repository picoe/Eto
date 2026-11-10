using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Eto.Serialization.Json.Converters;

namespace Eto.Serialization.Json
{
	/// <summary>
	/// Objects named with <c>$name</c>, <c>ID</c> or <c>$id</c> during one load, for <c>$ref</c> and <c>{x:Reference name}</c> to refer to.
	/// </summary>
	/// <remarks>
	/// Also Newtonsoft's reference resolver, which reads an object's leading <c>$id</c> itself.
	/// </remarks>
	class NameScope : IReferenceResolver
	{
		readonly Dictionary<string, object> names = new Dictionary<string, object>();
		readonly Dictionary<string, object> resources = new Dictionary<string, object>();
		List<(ReferenceInfo reference, Action<object> setValue)> pending;

		public object Instance { get; set; }

		public static NameScope Get(JsonSerializer serializer)
		{
			return serializer.ReferenceResolver as NameScope
				?? throw new JsonSerializationException("References can only be used when loading with Eto.Serialization.Json.JsonReader");
		}

		public void Add(string name, object value) => names[name] = value;

		/// <summary>Sets the value now if the name is known, otherwise once the load completes so it can refer to an object later on.</summary>
		public void Resolve(ReferenceInfo reference, Action<object> setValue)
		{
			if (names.TryGetValue(reference.Name, out var value))
				setValue(value);
			else
				(pending ??= new List<(ReferenceInfo, Action<object>)>()).Add((reference, setValue));
		}

		/// <summary>Gets the named object, which must already have been read.</summary>
		public object ResolveNow(string name, Newtonsoft.Json.JsonReader reader)
		{
			if (names.TryGetValue(name, out var value))
				return value;
			throw new ReferenceInfo(name, this, reader).CreateException($"Could not find an object named '{name}'. It must come earlier in the file when used here");
		}

		/// <summary>Adds a keyed resource from a <c>Properties</c> object, for <c>{StaticResource key}</c>.</summary>
		public void AddResource(string key, object value) => resources[key] = value;

		/// <summary>Gets a resource, which must already have been read.</summary>
		/// <remarks>Unlike in .xeto, resources are shared by the whole file rather than only the control they're on and its children.</remarks>
		public object GetResource(string key, Newtonsoft.Json.JsonReader reader)
		{
			if (resources.TryGetValue(key, out var value))
				return value;
			throw new ReferenceInfo(key, this, reader).CreateException($"Could not find a resource with key '{key}'. It must come earlier in the file");
		}

		public void Complete()
		{
			if (pending == null)
				return;
			foreach (var item in pending)
			{
				if (!names.TryGetValue(item.reference.Name, out var value))
					throw item.reference.CreateException($"Could not find an object named '{item.reference.Name}'");
				item.setValue(value);
			}
			pending = null;
		}

		void IReferenceResolver.AddReference(object context, string reference, object value)
		{
			// same as $name, so the object is assigned to a matching field too
			NameConverter.CreateInfo(Instance, reference)?.SetValue(value);
			Add(reference, value);
		}

		object IReferenceResolver.ResolveReference(object context, string reference)
		{
			names.TryGetValue(reference, out var value);
			return value;
		}

		string IReferenceResolver.GetReference(object context, object value) => throw new NotSupportedException();

		bool IReferenceResolver.IsReferenced(object context, object value) => false;
	}

	/// <summary>
	/// A <c>{ "$ref": "name" }</c> or <c>"{x:Reference name}"</c> value, set once the named object is known.
	/// </summary>
	class ReferenceInfo
	{
		public string Name { get; }

		public NameScope Scope { get; }

		readonly string path;
		readonly int line;
		readonly int position;

		internal ReferenceInfo(string name, NameScope scope, Newtonsoft.Json.JsonReader reader)
		{
			Name = name;
			Scope = scope;
			path = reader.Path;
			if (reader is IJsonLineInfo info && info.HasLineInfo())
			{
				line = info.LineNumber;
				position = info.LinePosition;
			}
		}

		public JsonSerializationException CreateException(string message)
		{
			return new JsonSerializationException(line > 0 ? $"{message}. Path '{path}', line {line}, position {position}." : $"{message}. Path '{path}'.");
		}

		/// <summary>Reads a reference from the current string or object, if it is one.</summary>
		/// <param name="loaded">The object, when it had to be loaded to look at it, so the reader is now past it.</param>
		public static ReferenceInfo Read(Newtonsoft.Json.JsonReader reader, JsonSerializer serializer, out JObject loaded)
		{
			loaded = null;
			if (reader.TokenType == JsonToken.String && reader.Value is string text)
			{
				var name = ParseMarkup(text);
				return name != null ? new ReferenceInfo(name, NameScope.Get(serializer), reader) : null;
			}
			if (reader.TokenType != JsonToken.StartObject)
				return null;

			// objects are usually read from one already loaded, so look at it rather than load it again
			if ((reader as JTokenReader)?.CurrentToken is JObject obj)
			{
				var name = GetRef(obj);
				if (name == null)
					return null;
				var reference = new ReferenceInfo(name, NameScope.Get(serializer), reader);
				reader.Skip();
				return reference;
			}

			var lineInfo = new ReferenceInfo(null, null, reader);
			loaded = JObject.Load(reader);
			var refName = GetRef(loaded);
			if (refName == null)
				return null;
			loaded = null;
			return new ReferenceInfo(refName, NameScope.Get(serializer), lineInfo);
		}

		ReferenceInfo(string name, NameScope scope, ReferenceInfo location)
		{
			Name = name;
			Scope = scope;
			path = location.path;
			line = location.line;
			position = location.position;
		}

		/// <summary>Gets the name from a <c>{ "$ref": "name" }</c> object.</summary>
		public static string GetRef(JObject obj)
		{
			if (obj.TryGetValue("$ref", out var token))
			{
				if (obj.Count > 1 || token.Type != JTokenType.String)
					throw new JsonSerializationException("A reference must be only { \"$ref\": \"name\" }");
				return (string)token;
			}
			return null;
		}

		/// <summary>Gets the name from <c>{x:Reference name}</c> or <c>{x:Reference Name=name}</c>.</summary>
		static string ParseMarkup(string text)
		{
			const string prefix = "{x:Reference";
			text = text.Trim();
			if (!text.StartsWith(prefix, StringComparison.Ordinal) || !text.EndsWith("}", StringComparison.Ordinal))
				return null;
			var body = text.Substring(prefix.Length, text.Length - prefix.Length - 1);
			if (body.Length == 0 || !char.IsWhiteSpace(body[0]))
				return null; // e.g. {x:ReferenceFoo}
			body = body.Trim();
			if (body.StartsWith("Name", StringComparison.Ordinal) && body.Substring(4).TrimStart().StartsWith("=", StringComparison.Ordinal))
				body = body.Substring(body.IndexOf('=') + 1).Trim();
			if (body.Length == 0)
				throw new JsonSerializationException($"Expected a name in '{text}'");
			return body;
		}
	}
}
