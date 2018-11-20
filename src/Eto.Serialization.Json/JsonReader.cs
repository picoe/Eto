using System;
using System.IO;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.Reflection;
using Eto.Serialization.Json.Converters;

namespace Eto.Serialization.Json
{
	public static class JsonReader
	{
		static Stream GetStream(Type type)
		{
			return
				GetStream(type, type.FullName + ".jeto")
				?? GetStream(type, type.FullName + ".json")
				?? GetStream(type, type.Name + ".jeto") // for f# projects
				?? throw new InvalidOperationException($"Embedded resource '{type.FullName}.jeto' not found in assembly '{type.GetAssembly()}'");
		}

		static Stream GetStream(Type type, string resourceName)
		{
#if PCL
			return type.GetTypeInfo().Assembly.GetManifestResourceStream(resourceName);
#else
			return type.Assembly.GetManifestResourceStream(resourceName);
#endif
		}

		/// <summary>
		/// Loads the specified type from a json of the same name
		/// </summary>
		/// <remarks>
		/// If your class name is MyNamespace.MyType, then this will attempt to load MyNamespace.MyType.json
		/// for the json definition in the same assembly.
		/// 
		/// If you want to specify a different json, use <see cref="Load{T}(Stream)"/>
		/// </remarks>
		/// <typeparam name="T">Type of object to load from json</typeparam>
		/// <returns>A new instance of the specified type with the contents loaded from json</returns>
		public static T Load<T>(NamespaceManager namespaceManager = null)
			where T : new()
		{
			var type = typeof(T);

			using (var stream = GetStream(type))
			{
				return Load<T>(stream, default(T), namespaceManager);
			}
		}

		/// <summary>
		/// Loads the specified type from the specified json stream
		/// </summary>
		/// <remarks>
		/// If your class name is MyNamespace.MyType, then this will attempt to load MyNamespace.MyType.json
		/// for the json definition in the same assembly.
		/// </remarks>
		/// <typeparam name="T">Type of object to load from the specified json</typeparam>
		/// <param name="stream">json content to load (e.g. from resources)</param>
		/// <param name="namespaceManager">Namespace manager to use when loading</param>
		/// <returns>A new instance of the specified type with the contents loaded from the json stream</returns>
		public static T Load<T>(Stream stream, NamespaceManager namespaceManager = null)
			where T : new()
		{
			return Load<T>(stream, default(T), namespaceManager);
		}

		/// <summary>
		/// Loads the specified instance with json of the same name
		/// </summary>
		/// <remarks>
		/// If your class name is MyNamespace.MyType, then this will attempt to load MyNamespace.MyType.json
		/// for the json definition in the same assembly.
		/// 
		/// If you want to specify a different json, use <see cref="Load{T}(Stream, T)"/>
		/// </remarks>
		/// <typeparam name="T">Type of object to load from the specified json</typeparam>
		/// <param name="instance">Instance to use as the starting object</param>
		/// <param name="namespaceManager">Namespace manager to use when loading</param>
		public static void Load<T>(T instance, NamespaceManager namespaceManager = null)
		{
			if (Equals(instance, null))
				throw new ArgumentNullException(nameof(instance));

			using (var stream = GetStream(typeof(T)))
			{
				Load<T>(stream, instance, namespaceManager);
			}
		}

		/// <summary>
		/// Loads the specified instance with json from the specified embedded resource.
		/// </summary>
		/// <remarks>
		/// This will load the embedded resource from the same assembly as <paramref name="instance"/> with the 
		/// specified <paramref name="resourceName"/> embedded resource.
		/// 
		/// If you want to specify a different json, use <see cref="Load{T}(Stream, T)"/>
		/// </remarks>
		/// <typeparam name="T">Type of object to load from the specified json</typeparam>
		/// <param name="instance">Instance to use as the starting object</param>
		/// <param name="namespaceManager">Namespace manager to use when loading</param>
		/// <param name="resourceName">Fully qualified name of the embedded resource to load.</param>
		public static void Load<T>(T instance, string resourceName, NamespaceManager namespaceManager = null)
		{
			if (Equals(instance, null))
				throw new ArgumentNullException(nameof(instance));

			using (var stream = GetStream(typeof(T), resourceName))
			{
				if (stream == null)
					throw new ArgumentException(nameof(resourceName), $"Embedded resource '{resourceName}' not found in assembly '{typeof(T).GetAssembly()}'");

				Load<T>(stream, instance, namespaceManager);
			}
		}

		static JsonSerializer serializer;

		/// <summary>
		/// Loads the specified type from the specified json stream
		/// </summary>
		/// <typeparam name="T">Type of object to load from the specified json</typeparam>
		/// <param name="stream">json content to load (e.g. from resources)</param>
		/// <param name="instance">Instance to use as the starting object, or null to create a new instance</param>
		/// <param name="namespaceManager">Namespace manager to use to lookup type names</param>
		/// <returns>A new or existing instance of the specified type with the contents loaded from the json stream</returns>
		public static T Load<T>(Stream stream, T instance, NamespaceManager namespaceManager = null)
		{
			using (var reader = new StreamReader(stream))
			{
				return Load<T>(reader, instance, namespaceManager);
			}
		}

		/// <summary>
		/// Loads the specified type from the specified json <paramref name="reader"/>
		/// </summary>
		/// <typeparam name="T">Type of object to load from the specified json</typeparam>
		/// <param name="reader">Reader for the json content to load</param>
		/// <param name="instance">Instance to use as the starting object, or null to create a new instance</param>
		/// <param name="namespaceManager">Namespace manager to use to lookup type names</param>
		/// <returns>A new or existing instance of the specified type with the contents loaded from the json stream</returns>
		public static T Load<T>(TextReader reader, T instance, NamespaceManager namespaceManager = null)
		{
			if (serializer == null)
			{
				serializer = new JsonSerializer
				{
					TypeNameHandling = TypeNameHandling.Auto,
					MissingMemberHandling = MissingMemberHandling.Error,
					ContractResolver = new EtoContractResolver(),
				};
				serializer.Converters.Add(new TableLayoutConverter());
				serializer.Converters.Add(new DynamicLayoutConverter());
				serializer.Converters.Add(new DelegateConverter());
				serializer.Converters.Add(new PropertyStoreConverter());
				serializer.Converters.Add(new ImageConverter());
				serializer.Converters.Add(new FontConverter());
				serializer.Converters.Add(new StackLayoutConverter());
				serializer.Converters.Add(new ListItemConverter());
				serializer.Converters.Add(new TypeConverterConverter());
			}
			serializer.SerializationBinder = new EtoBinder
			{
				NamespaceManager = namespaceManager ?? new DefaultNamespaceManager(),
				Instance = instance
			};

			if (ReferenceEquals(instance, default(T)))
				return (T)serializer.Deserialize(reader, typeof(T));

			serializer.Populate(reader, instance);
			return instance;
		}

	}
}

