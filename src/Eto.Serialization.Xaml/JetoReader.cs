using System.Runtime.CompilerServices;
using Portable.Xaml.Json;

namespace Eto.Serialization.Xaml
{
	/// <summary>
	/// Methods to load Eto objects from .jeto (JSON) files
	/// </summary>
	/// <remarks>
	/// Set <c>EtoXamlJeto</c> to true in your project to embed .jeto files, and <c>EtoXamlCodeGen</c> to compile them.
	/// Use <see cref="XamlReader"/> for .xeto files.
	/// </remarks>
	public static class JetoReader
	{
		// in order of preference
		static IEnumerable<string> GetTypeFiles(Type type)
		{
			yield return type.FullName + ".jeto";
			yield return type.Name + ".jeto"; // for F#/VB.NET projects
		}

		/// <summary>
		/// Watches the .jeto file for changes and triggers the method specified by the <see cref="HotReloadAttribute"/>, if any, when the file changes.
		/// Otherwise, it will reload the jeto into the control using <see cref="Load{T}(T)"/>.
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

			if (!XamlReader.FindFileNameFromType(typeof(T), GetTypeFiles(typeof(T)), ref fileName, sourcePath))
			{
				Trace.WriteLine($"File {fileName ?? $"for {typeof(T).FullName}"} could not be found to watch for hot reload.");
				return false;
			}

			XamlReader.RegisterHotReloadFile(control.GetType(), fileName);

			if (HotReloadService.WatchFile(control, fileName, sourcePath))
				return true;

			return HotReloadService.WatchFile(control, fileName, c =>
			{
				if (c is T t)
					Load(t);
			}, sourcePath);
		}

		static void HotReload<T>(T instance)
		{
			if (HotReloadService.Enabled)
			{
				var sourceFile = XamlReader.FindSourceForType(typeof(T));
				if (sourceFile != null)
					Watch<T>(instance, null, sourceFile);
			}
		}

		/// <summary>
		/// Loads the specified type from the embedded .jeto of the same name, e.g. MyNamespace.MyType.jeto
		/// </summary>
		/// <typeparam name="T">Type of object to load</typeparam>
		/// <returns>A new instance of the specified type with the contents loaded from the jeto</returns>
		public static T Load<T>()
			where T : new()
		{
			using var stream = XamlReader.GetStream(typeof(T), GetTypeFiles(typeof(T)));
			var instance = Load<T>(stream, default);
			HotReload(instance);
			return instance;
		}

		/// <summary>
		/// Loads the specified type from the specified jeto stream
		/// </summary>
		/// <typeparam name="T">Type of object to load</typeparam>
		/// <param name="stream">Jeto content to load (e.g. from resources)</param>
		/// <returns>A new instance of the specified type with the contents loaded from the jeto stream</returns>
		public static T Load<T>(Stream stream)
			where T : new()
		{
			return Load<T>(stream, default(T));
		}

		/// <summary>
		/// Loads the specified instance from the embedded .jeto of the same name, e.g. MyNamespace.MyType.jeto,
		/// or runs its compiled code when <c>EtoXamlCodeGen</c> is enabled.
		/// </summary>
		/// <typeparam name="T">Type of object to load</typeparam>
		/// <param name="instance">Instance to use as the starting object</param>
		public static void Load<T>(T instance) => Load<T>(instance, (XamlReaderSettings)null);

		/// <summary>
		/// Loads the specified instance from the embedded .jeto of the same name, e.g. MyNamespace.MyType.jeto
		/// </summary>
		/// <typeparam name="T">Type of object to load</typeparam>
		/// <param name="instance">Instance to use as the starting object</param>
		/// <param name="settings">Settings for reading the jeto, or null to use compiled code when available</param>
		public static void Load<T>(T instance, XamlReaderSettings settings)
		{
			if (Equals(instance, null))
				throw new ArgumentNullException(nameof(instance));

			if (settings == null && XamlReader.TryLoadGenerated(instance))
			{
				HotReload(instance);
				return;
			}

			using var stream = XamlReader.GetStream(typeof(T), GetTypeFiles(typeof(T)));
			Load<T>(stream, instance, settings);
			HotReload(instance);
		}

		/// <summary>
		/// Loads the specified instance from a fully qualified .jeto embedded resource in the same assembly as <typeparamref name="T"/>
		/// </summary>
		/// <typeparam name="T">Type of object to load</typeparam>
		/// <param name="instance">Instance to use as the starting object</param>
		/// <param name="resourceName">Fully qualified name of the embedded resource to load.</param>
		public static void Load<T>(T instance, string resourceName)
		{
			if (Equals(instance, null))
				throw new ArgumentNullException(nameof(instance));

			using var stream = XamlReader.GetStream(typeof(T), resourceName);
			if (stream == null)
				throw new ArgumentException(nameof(resourceName), $"Embedded resource '{resourceName}' not found in assembly '{typeof(T).Assembly}'");

			Load<T>(stream, instance);
		}

		/// <summary>
		/// Loads the specified type from the specified jeto stream
		/// </summary>
		/// <typeparam name="T">Type of object to load</typeparam>
		/// <param name="stream">Jeto content to load (e.g. from resources)</param>
		/// <param name="instance">Instance to use as the starting object, or null to create a new instance</param>
		/// <returns>A new or existing instance of the specified type with the contents loaded from the jeto stream</returns>
		public static T Load<T>(Stream stream, T instance) => Load(stream, instance, null);

		/// <inheritdoc cref="Load{T}(Stream, T)"/>
		/// <param name="settings">Settings for reading the jeto</param>
		public static T Load<T>(Stream stream, T instance, XamlReaderSettings settings)
		{
			return Load<T>(new StreamReader(stream), instance, settings);
		}

		/// <summary>
		/// Loads the specified type from the specified jeto <paramref name="reader"/>.
		/// </summary>
		/// <typeparam name="T">Type of object to load</typeparam>
		/// <param name="reader">Reader to read the jeto content</param>
		/// <param name="instance">Instance to use as the starting object, or null to create a new instance</param>
		/// <returns>A new or existing instance of the specified type with the contents loaded from the jeto</returns>
		public static T Load<T>(TextReader reader, T instance) => Load(reader, instance, null);

		static T Load<T>(TextReader reader, T instance, XamlReaderSettings settings)
		{
			return XamlReader.Load<T>(new EtoJsonReader(reader, XamlReader.context, CreateReaderSettings<T>(settings)), instance);
		}

		static XamlJsonReaderSettings CreateReaderSettings<T>(XamlReaderSettings settings)
		{
			var readerSettings = new XamlJsonReaderSettings { RootType = XamlReader.context.GetXamlType(typeof(T)) };
			EtoJsonReader.AddNamespaces(readerSettings);

			if (!XamlReader.DesignMode && typeof(T).Assembly != typeof(Control).Assembly)
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
	}
}
