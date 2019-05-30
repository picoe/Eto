using System;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
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
	/// Methods to help load/save Eto objects to/from xaml
	/// </summary>
	public static class XamlReader
	{
		/// <summary>
		/// Xaml Namespace for Eto.Forms elements
		/// </summary>
		public static readonly string EtoFormsNamespace = EtoXamlSchemaContext.EtoFormsNamespace;

		static Stream GetStream(Type type)
		{
			return
				GetStream(type, type.FullName + ".xeto")
				?? GetStream(type, type.FullName + ".xaml")
				?? GetStream(type, type.Name + ".xeto") // for F#/VB.NET projects
				?? throw new InvalidOperationException($"Embedded resource '{type.FullName}.xeto' not found in assembly '{type.GetAssembly()}'");
		}

		static Stream GetStream(Type type, string resourceName)
		{
			return type.GetAssembly().GetManifestResourceStream(resourceName);
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
			using (var stream = GetStream(typeof(T)))
			{
				return Load<T>(stream, default(T));
			}
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
		public static void Load<T>(T instance)
		{
			if (Equals(instance, null))
				throw new ArgumentNullException(nameof(instance));

			using (var stream = GetStream(typeof(T)))
			{
				Load<T>(stream, instance);
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
					throw new ArgumentException(nameof(resourceName), $"Embedded resource '{resourceName}' not found in assembly '{typeof(T).GetAssembly()}'");

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
		public static T Load<T>(Stream stream, T instance)
		{
			var readerSettings = new XamlXmlReaderSettings();
			if (!DesignMode)
				readerSettings.LocalAssembly = typeof(T).GetAssembly();
			return Load<T>(new XamlXmlReader(stream, context, readerSettings), instance);
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
			var readerSettings = new XamlXmlReaderSettings();
			if (!DesignMode)
				readerSettings.LocalAssembly = typeof(T).GetAssembly();
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
			var readerSettings = new XamlXmlReaderSettings();
			if (!DesignMode)
				readerSettings.LocalAssembly = typeof(T).GetAssembly();
			return Load<T>(new XamlXmlReader(reader, context, readerSettings), instance);
		}

		static T Load<T>(XamlXmlReader reader, T instance)
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