using System.Runtime.CompilerServices;
using System.Xml;


#if PORTABLE
using Portable.Xaml;
using Portable.Xaml.Markup;
#else
using System.Xaml;
using System.Windows.Markup;
#endif

namespace Eto.Serialization.Xaml
{
	/// <summary>
	/// Settings for reading Eto Xaml files
	/// </summary>
	public class XamlReaderSettings
	{
		/// <summary>
		/// Gets the namespaces to register with the Xaml reader so they don't need to be specified in every file
		/// </summary>
		public Dictionary<string, string> Namespaces { get; } = new();
	}

	/// <summary>
	/// Methods to help load/save Eto objects to/from xaml
	/// </summary>
	public static class XamlReader
	{
		/// <summary>
		/// Xaml Namespace for Eto.Forms elements
		/// </summary>
		public static readonly string EtoFormsNamespace = EtoXamlSchemaContext.EtoFormsNamespace;

		static readonly Dictionary<Type, string> hotReloadFilePaths = new();

		static IEnumerable<string> GetTypeFiles(Type type)
		{
			yield return type.FullName + ".xeto";
			yield return type.FullName + ".xaml";
			yield return type.Name + ".xeto"; // for F#/VB.NET projects
		}

		static Stream GetStream(Type type)
		{
			// try in order of preference: .xeto, .xaml, short name .xeto (for F#/VB.NET projects)
			foreach (var file in GetTypeFiles(type))
			{
				var stream = GetStream(type, file);
				if (stream != null)
					return stream;
			}
			throw new InvalidOperationException($"Embedded resource '{type.FullName}.xeto' not found in assembly '{type.Assembly}'");
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
		/// Watches the .xeto file for changes and triggers the method specified by the <see cref="HotReloadAttribute"/>, if any, when the file changes.
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

			// no HotReloadAttribute, so just reload the xaml into the control
			return HotReloadService.WatchFile(control, fileName, c =>
			{
				if (c is T t)
					Load(t);
			}, sourcePath);
		}

		/// <summary>
		/// Loads the specified type from a xaml of the same name
		/// </summary>
		/// <remarks>
		/// If your class name is MyNamespace.MyType, then this will attempt to load MyNamespace.MyType.xaml
		/// for the xaml definition in the same assembly.
		/// 
		/// If you want to specify a different xaml, use <see cref="Load{T}(Stream)"/>
		/// </remarks>
		/// <typeparam name="T">Type of object to load from xaml</typeparam>
		/// <returns>A new instance of the specified type with the contents loaded from xaml</returns>
		public static T Load<T>()
			where T : new()
		{
			using var stream = GetStream(typeof(T));
			var instance = Load<T>(stream, default);
			HotReload(instance);
			return instance;
		}

		/// <summary>
		/// Loads the specified type from the specified xaml stream
		/// </summary>
		/// <remarks>
		/// If your class name is MyNamespace.MyType, then this will attempt to load MyNamespace.MyType.xaml
		/// for the xaml definition in the same assembly.
		/// </remarks>
		/// <typeparam name="T">Type of object to load from the specified xaml</typeparam>
		/// <param name="stream">Xaml content to load (e.g. from resources)</param>
		/// <returns>A new instance of the specified type with the contents loaded from the xaml stream</returns>
		public static T Load<T>(Stream stream)
			where T : new()
		{
			return Load<T>(stream, default(T));
		}

		/// <summary>
		/// Loads the specified instance with xaml of the same name
		/// </summary>
		/// <remarks>
		/// If your class name is MyNamespace.MyType, then this will attempt to load MyNamespace.MyType.xaml
		/// for the xaml definition in the same assembly.
		/// 
		/// If you want to specify a different xaml, use <see cref="Load{T}(Stream, T)"/>
		/// </remarks>
		/// <typeparam name="T">Type of object to load from the specified xaml</typeparam>
		/// <param name="instance">Instance to use as the starting object</param>
		/// <returns>A new or existing instance of the specified type with the contents loaded from the xaml stream</returns>
		public static void Load<T>(T instance) => Load<T>(instance, (XamlReaderSettings)null);

		public static void Load<T>(T instance, XamlReaderSettings settings)
		{
			if (Equals(instance, null))
				throw new ArgumentNullException(nameof(instance));

			using var stream = GetStream(typeof(T));
			Load<T>(stream, instance, settings);
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
		/// Loads the specified instance with a specified fully qualified xaml embedded resource
		/// </summary>
		/// <remarks>
		/// This will load the embedded resource from the same assembly as <paramref name="instance"/> with the 
		/// specified <paramref name="resourceName"/> embedded resource.
		/// 
		/// If you want to specify a different xaml, use <see cref="Load{T}(Stream, T)"/>
		/// </remarks>
		/// <typeparam name="T">Type of object to load from the specified xaml</typeparam>
		/// <param name="instance">Instance to use as the starting object</param>
		/// <param name="resourceName">Fully qualified name of the embedded resource to load.</param>
		/// <returns>An existing instance of the specified type with the contents loaded from the xaml stream</returns>
		public static void Load<T>(T instance, string resourceName)
		{
			if (Equals(instance, null))
				throw new ArgumentNullException(nameof(instance));

			using (var stream = GetStream(typeof(T), resourceName))
			{
				if (stream == null)
					throw new ArgumentException(nameof(resourceName), $"Embedded resource '{resourceName}' not found in assembly '{typeof(T).Assembly}'");

				Load<T>(stream, instance);
			}
		}

		internal static readonly EtoXamlSchemaContext context = new EtoXamlSchemaContext();

		/// <summary>
		/// Gets or sets a value indicating that the reader is used in design mode
		/// </summary>
		/// <remarks>
		/// In Design mode, events are not wired up and will not cause exceptions due to missing methods to wire up to.
		/// </remarks>
		public static bool DesignMode
		{
			get { return context.DesignMode; }
			set { context.DesignMode = value; }
		}

		/// <summary>
		/// Loads the specified type from the specified xaml stream
		/// </summary>
		/// <typeparam name="T">Type of object to load from the specified xaml</typeparam>
		/// <param name="stream">Xaml content to load (e.g. from resources)</param>
		/// <param name="instance">Instance to use as the starting object, or null to create a new instance</param>
		/// <returns>A new or existing instance of the specified type with the contents loaded from the xaml stream</returns>
		public static T Load<T>(Stream stream, T instance) => Load(stream, instance, null);

		public static T Load<T>(Stream stream, T instance, XamlReaderSettings settings)
		{
			var readerSettings = CreateReaderSettings<T>(settings);
			return Load<T>(new XamlXmlReader(stream, context, readerSettings), instance);
		}

		private static XamlXmlReaderSettings CreateReaderSettings<T>(XamlReaderSettings settings = null)
		{
			var readerSettings = new XamlXmlReaderSettings();

			readerSettings.AddNamespace(null, EtoFormsNamespace);
			readerSettings.AddNamespace("x", XamlLanguage.Xaml2006Namespace);

			if (!DesignMode && typeof(T).Assembly != typeof(Control).Assembly)
			{
				readerSettings.LocalAssembly = typeof(T).Assembly;
				readerSettings.AddNamespace("local", $"clr-namespace:{typeof(T).Namespace}");
			}

			if (settings != null)
			{
				foreach (var ns in settings.Namespaces)
				{
					readerSettings.AddNamespace(ns.Key, ns.Value);
				}

			}
			return readerSettings;
		}

		/// <summary>
		/// Loads the specified type from the specified text <paramref name="reader"/>.
		/// </summary>
		/// <typeparam name="T">Type of object to load from the specified xaml</typeparam>
		/// <param name="reader">Reader to read the Xaml content</param>
		/// <param name="instance">Instance to use as the starting object, or null to create a new instance</param>
		/// <returns>A new or existing instance of the specified type with the contents loaded from the xaml stream</returns>
		public static T Load<T>(TextReader reader, T instance)
		{
			var readerSettings = CreateReaderSettings<T>();
			return Load<T>(new XamlXmlReader(reader, context, readerSettings), instance);
		}

		/// <summary>
		/// Loads the specified type from the specified XML <paramref name="reader"/>.
		/// </summary>
		/// <typeparam name="T">Type of object to load from the specified xaml</typeparam>
		/// <param name="reader">XmlReader to read the Xaml content</param>
		/// <param name="instance">Instance to use as the starting object, or null to create a new instance</param>
		/// <returns>A new or existing instance of the specified type with the contents loaded from the xaml stream</returns>
		public static T Load<T>(XmlReader reader, T instance)
		{
			var readerSettings = CreateReaderSettings<T>();
			return Load<T>(new XamlXmlReader(reader, context, readerSettings), instance);
		}

		private static T Load<T>(XamlXmlReader reader, T instance)
		{
			var writerSettings = new XamlObjectWriterSettings();
			writerSettings.ExternalNameScope = new EtoNameScope { Instance = instance };
			writerSettings.RegisterNamesOnExternalNamescope = true;
			writerSettings.RootObjectInstance = instance;
			var writer = new XamlObjectWriter(context, writerSettings);

			XamlServices.Transform(reader, writer);
			return (T)writer.Result;
		}
		
	}
}