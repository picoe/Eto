using Newtonsoft.Json;
using Eto.Serialization.Json.Converters;
using System.Runtime.CompilerServices;

namespace Eto.Serialization.Json
{
	public static class JsonReader
	{
		static readonly Dictionary<Type, string> hotReloadFilePaths = new();

		static IEnumerable<string> GetTypeFiles(Type type)
		{
			yield return type.FullName + ".jeto";
			yield return type.FullName + ".json";
			yield return type.Name + ".jeto"; // for F#/VB.NET projects
		}
		
		static Stream GetStream(Type type)
		{
			// try in order of preference: .jeto, .json, short name .jeto (for F#/VB.NET projects)
			foreach (var file in GetTypeFiles(type))
			{
				var stream = GetStream(type, file);
				if (stream != null)
					return stream;
			}
			throw new InvalidOperationException($"Embedded resource '{type.FullName}.jeto' not found in assembly '{type.Assembly}'");
		}
		
		static Stream GetStream(Type type, string resourceName)
		{
			if (hotReloadFilePaths.TryGetValue(type, out var filePath) && File.Exists(filePath))
				return File.OpenRead(filePath);
			
			return type.Assembly.GetManifestResourceStream(resourceName);
		}

		internal static void RegisterHotReloadFile(Type type, string filePath)
		{
			hotReloadFilePaths[type] = filePath;
		}

		/// <summary>
		/// Watches the .jeto file for changes and triggers the method specified by the <see cref="HotReloadAttribute"/>, if any, when the file changes.
		/// Otherwise, it will reload the xeto into the control using <see cref="Load{T}"/>.
		/// </summary>
		/// <typeparam name="T">Control type</typeparam>
		/// <param name="control">Instance of the control to watch for changes</param>
		/// <param name="fileName">Name of the file to watch, or null to use the default based on the class name</param>
		/// <param name="sourcePath">Source path of the file to watch</param>
		/// <returns>true if the file is being watched, false otherwise</returns>
		public static bool Watch<T>(T control, string fileName = null, [CallerFilePath] string sourcePath = null)
		{
			if (control == null)
				throw new ArgumentNullException(nameof(control));

			if (!FindFileNameFromType(typeof(T), ref fileName, sourcePath))
			{
				Trace.WriteLine($"File {fileName ?? $"for {typeof(T).FullName}"} could not be found to watch for hot reload.");
				return false;
			}

			RegisterHotReloadFile(control.GetType(), fileName);

			// see if the type has a HotReloadAttribute
			if (HotReloadService.WatchFile(control, fileName, sourcePath))
				return true;

			// no HotReloadAttribute, so just reload the json into the control
			return HotReloadService.WatchFile(control, fileName, c =>
			{
				if (c is T t)
					Load(t);
			}, sourcePath);
		}

		private static bool FindFileNameFromType(Type type, ref string fileName, string sourcePath)
		{
			if (sourcePath != null && File.Exists(sourcePath))
				sourcePath = Path.GetDirectoryName(sourcePath);
				
			if (hotReloadFilePaths.TryGetValue(type, out var existing))
			{
				fileName = existing;
				return true;
			}

			if (fileName != null)
			{
				if (File.Exists(fileName))
					return true;
				if (sourcePath != null)
				{
					var path = Path.Combine(sourcePath, fileName);
					if (File.Exists(path))
					{
						fileName = path;
						return true;
					}
				}
				return false;
			}
			
			foreach (var file in GetTypeFiles(type))
			{
				if (File.Exists(file))
				{
					fileName = file;
					return true;
				}
				if (sourcePath != null)
				{
					var path = Path.Combine(sourcePath, file);
					if (File.Exists(path))
					{
						fileName = path;
						return true;
					}
				}
			}
			return false;
		}

		/// <summary>
		/// Loads the specified type from a json of the same name
		/// </summary>
		/// <remarks>
		/// If your class name is MyNamespace.MyType, then this will attempt to load MyNamespace.MyType.json
		/// for the json definition in the same assembly.
		/// 
		/// If you want to specify a different json, use <see cref="Load{T}(Stream, NamespaceManager)"/>
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
		/// If you want to specify a different json, use <see cref="Load{T}(Stream, T, NamespaceManager)"/>
		/// </remarks>
		/// <typeparam name="T">Type of object to load from the specified json</typeparam>
		/// <param name="instance">Instance to use as the starting object</param>
		/// <param name="namespaceManager">Namespace manager to use when loading</param>
		public static void Load<T>(T instance, NamespaceManager namespaceManager = null)
		{
			if (Equals(instance, null))
				throw new ArgumentNullException(nameof(instance));

			using var stream = GetStream(typeof(T));
			Load<T>(stream, instance, namespaceManager);
			HotReload(instance);
		}

		private static string FindSourceForType(Type type)
		{
			var stack = new StackTrace(true);
			for (int i = 0; i < stack.FrameCount; i++)
			{
				var frame = stack.GetFrame(i);
				var method = frame.GetMethod();
				if (method.DeclaringType == type)
					return frame.GetFileName();
			}
			return null;
		}

		private static void HotReload<T>(T instance)
		{
			if (HotReloadService.Enabled)
			{
				// scan stack trace for type and get source location
				var sourceFile = FindSourceForType(typeof(T));
				if (sourceFile != null)
					Watch<T>(instance, null, sourceFile);
			}
		}

		/// <summary>
		/// Loads the specified instance with json from the specified embedded resource.
		/// </summary>
		/// <remarks>
		/// This will load the embedded resource from the same assembly as <paramref name="instance"/> with the 
		/// specified <paramref name="resourceName"/> embedded resource.
		/// 
		/// If you want to specify a different json, use <see cref="Load{T}(Stream, T, NamespaceManager)"/>
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
					throw new ArgumentException(nameof(resourceName), $"Embedded resource '{resourceName}' not found in assembly '{typeof(T).Assembly}'");

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
					ContractResolver = new EtoContractResolver()
				};
				serializer.Converters.Add(new TableLayoutConverter());
				serializer.Converters.Add(new DynamicLayoutConverter());
				serializer.Converters.Add(new DelegateConverter());
				serializer.Converters.Add(new PropertyStoreConverter());
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

