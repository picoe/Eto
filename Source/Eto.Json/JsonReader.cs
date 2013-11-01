using System;
using System.IO;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Eto.Json
{
	public static class JsonReader
	{

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
		public static T Load<T> (NamespaceManager namespaceManager = null)
			where T: InstanceWidget, new()
		{
			var type = typeof(T);
			var stream = type.Assembly.GetManifestResourceStream (type.FullName + ".json");
			return Load<T> (stream, null, namespaceManager);
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
		public static T Load<T> (Stream stream, NamespaceManager namespaceManager = null)
			where T: InstanceWidget, new()
		{
			return Load<T> (stream, null, namespaceManager);
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
		/// <returns>A new or existing instance of the specified type with the contents loaded from the json stream</returns>
		public static T Load<T> (T instance, NamespaceManager namespaceManager = null)
			where T: InstanceWidget
		{
			var type = typeof(T);
			var stream = type.Assembly.GetManifestResourceStream (type.FullName + ".json");
			return Load<T> (stream, instance, namespaceManager);
		}

		/// <summary>
		/// Loads the specified type from the specified json stream
		/// </summary>
		/// <typeparam name="T">Type of object to load from the specified json</typeparam>
		/// <param name="stream">json content to load (e.g. from resources)</param>
		/// <param name="instance">Instance to use as the starting object</param>
		/// <param name="namespaceManager">Namespace manager to use to lookup type names</param>
		/// <returns>A new or existing instance of the specified type with the contents loaded from the json stream</returns>
		public static T Load<T> (Stream stream, T instance, NamespaceManager namespaceManager = null)
			where T : InstanceWidget
		{
			var type = typeof(T);

			using (var reader = new StreamReader(stream))
			{
				if (namespaceManager == null)
				{
					namespaceManager = new DefaultNamespaceManager();
				}
				var serializer = new JsonSerializer
				{
					TypeNameHandling = TypeNameHandling.Auto,
					MissingMemberHandling = MissingMemberHandling.Error,
					ContractResolver = new EtoContractResolver(),
					Context = new StreamingContext(StreamingContextStates.All, instance),
					Binder = new EtoBinder { NamespaceManager = namespaceManager }
				};
				serializer.Converters.Add(new DynamicLayoutConverter());
				serializer.Converters.Add(new DelegateConverter());
				serializer.Converters.Add(new PropertyStoreConverter());
				serializer.Converters.Add(new ImageConverter());

				if (instance == null)
					return serializer.Deserialize(reader, type) as T;

				serializer.Populate(reader, instance);
				return instance;
			}
		}
	}
}

