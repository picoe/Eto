#if XAML

using System;
using System.IO;
using System.Reflection;
using System.Xaml;
using System.Collections.Generic;
using System.Windows.Markup;

namespace Eto.Xaml
{

	/// <summary>
	/// Methods to help load/save Eto objects to/from xaml
	/// </summary>
	public static class XamlReader
	{

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
		public static T Load<T> ()
			where T: InstanceWidget, new()
		{
			var type = typeof(T);
			var stream = type.Assembly.GetManifestResourceStream (type.FullName + ".xaml");
			return Load<T> (stream, null);
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
		public static T Load<T> (Stream stream)
			where T: InstanceWidget, new()
		{
			return Load<T> (stream, null);
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
		public static T Load<T> (T instance)
			where T: InstanceWidget
		{
			var type = typeof(T);
			var stream = type.Assembly.GetManifestResourceStream (type.FullName + ".xaml");
			return Load<T> (stream, instance);
		}

		/// <summary>
		/// Loads the specified type from the specified xaml stream
		/// </summary>
		/// <typeparam name="T">Type of object to load from the specified xaml</typeparam>
		/// <param name="stream">Xaml content to load (e.g. from resources)</param>
		/// <param name="instance">Instance to use as the starting object</param>
		/// <returns>A new or existing instance of the specified type with the contents loaded from the xaml stream</returns>
		public static T Load<T> (Stream stream, T instance)
			where T : InstanceWidget
		{
			var type = typeof(T);
			var context = new EtoXamlSchemaContext (new Assembly[] { typeof(XamlReader).Assembly });
			var reader = new XamlXmlReader (stream, context);
			var writerSettings = new XamlObjectWriterSettings {
				RootObjectInstance = instance
			};
			writerSettings.AfterPropertiesHandler += delegate (object sender, XamlObjectEventArgs e) {
				var obj = e.Instance as InstanceWidget;
				if (obj != null && !string.IsNullOrEmpty (obj.ID)) {
					var property = type.GetProperty (obj.ID, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
					if (property != null)
						property.SetValue (instance, obj, null);
					else {
						var field = type.GetField (obj.ID, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
						if (field != null)
							field.SetValue (instance, obj);
					}
				}
			};

			var writer = new XamlObjectWriter (context, writerSettings);
			XamlServices.Transform (reader, writer);
			return writer.Result as T;
		}
	}
}

#endif