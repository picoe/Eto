using System;
using System.IO;
using System.Reflection;
using System.Linq;
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
		static Stream GetStream(Type type)
		{
			return
				GetStream(type, type.FullName + ".xeto")
				?? GetStream(type, type.FullName + ".xaml")
				?? GetStream(type, type.Name + ".xeto"); // for f# projects
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
			where T : Widget, new()
		{
			using (var stream = GetStream(typeof(T)))
			{
				return Load<T>(stream, null);
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
			where T : Widget, new()
		{
			return Load<T>(stream, null);
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
			where T : Widget
		{
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
			where T : Widget
		{
			using (var stream = GetStream(typeof(T), resourceName))
			{
				Load<T>(stream, instance);
			}
		}

		static readonly EtoXamlSchemaContext context = new EtoXamlSchemaContext(new [] { typeof(XamlReader).GetTypeInfo().Assembly });

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

		class NameScope : INameScope
		{
			public object Instance { get; set; }

			public object FindName (string name)
			{
				return null;
			}

			public void RegisterName (string name, object scopedElement)
			{
				if (Instance == null)
					return;
				var instanceType = Instance.GetType();
				var obj = scopedElement as Widget;
				if (obj != null && !string.IsNullOrEmpty(name))
				{
					#if !NET40
					var property = instanceType.GetRuntimeProperties().FirstOrDefault(r => r.Name == name && r.SetMethod != null &&!r.SetMethod.IsStatic);
					if (property != null)
						property.SetValue(Instance, obj, null);
					else
					{
						var field = instanceType.GetRuntimeField(name);
						if (field != null && !field.IsStatic)
							field.SetValue(Instance, obj);
					}
					#else
					var property = instanceType.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					if (property != null)
					property.SetValue(Instance, obj, null);
					else
					{
					var field = instanceType.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
					if (field != null)
					field.SetValue(Instance, obj);
					}
					#endif
				}
			}

			public void UnregisterName (string name)
			{
			}
		}

		/// <summary>
		/// Loads the specified type from the specified xaml stream
		/// </summary>
		/// <typeparam name="T">Type of object to load from the specified xaml</typeparam>
		/// <param name="stream">Xaml content to load (e.g. from resources)</param>
		/// <param name="instance">Instance to use as the starting object, or null to create a new instance</param>
		/// <returns>A new or existing instance of the specified type with the contents loaded from the xaml stream</returns>
		public static T Load<T>(Stream stream, T instance)
			where T : Widget
		{
			var reader = new XamlXmlReader(stream, context);
			var writerSettings = new XamlObjectWriterSettings();
			writerSettings.ExternalNameScope = new NameScope { Instance = instance };
			writerSettings.RegisterNamesOnExternalNamescope = true;
			writerSettings.RootObjectInstance = instance;
			var writer = new XamlObjectWriter(context, writerSettings);
			
			XamlServices.Transform(reader, writer);
			return writer.Result as T;
		}
	}
}