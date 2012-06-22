using System;
using System.IO;
using System.Reflection;
using System.Xaml;
using System.Collections.Generic;
using System.Windows.Markup;

namespace Eto
{
	public static class XamlReader
	{
		
		class EtoXamlType : XamlType
		{
			public EtoXamlType (Type type, XamlSchemaContext schema) : base(type, schema)
			{
			}
			
			protected override XamlMember LookupAttachableMember (string name)
			{
				var member = base.LookupAttachableMember (name);
				if (member != null)
					Console.WriteLine ("Looking up attachable {0}, {1}", name, member.Name);
				return member;
			}
			
			protected override XamlMember LookupMember (string name, bool skipReadOnlyCheck)
			{
				//Console.WriteLine ("Looking up member {0}", name);
				return base.LookupMember (name, skipReadOnlyCheck);
			}
		}
		
		class EtoXamlSchemaContext : XamlSchemaContext
		{
			Dictionary<Type, XamlType> _masterTypeTable = new Dictionary<Type, XamlType> ();
			object _syncObject = new object ();
			
			public EtoXamlSchemaContext (IEnumerable<Assembly> assemblies)
				: base(assemblies)
			{
			}

			public override XamlType GetXamlType (Type type)
			{
				if (type == null) {
					throw new ArgumentNullException ("type");
				}
				XamlType xamlType;
				lock (this._syncObject) {
					if (!this._masterTypeTable.TryGetValue (type, out xamlType)) {
						xamlType = new EtoXamlType (type, this);
						this._masterTypeTable.Add (type, xamlType);
					}
				}
				return xamlType;
			}

		}

		public static T Load<T> ()
			where T: InstanceWidget, new()
		{
			var type = typeof(T);
			var stream = type.Assembly.GetManifestResourceStream (type.FullName + ".xaml");
			return Load<T> (stream, null);
		}
		
		public static T Load<T> (Stream stream)
			where T: InstanceWidget, new()
		{
			return Load<T> (stream, null);
		}
		
		public static T Load<T> (T instance)
			where T: InstanceWidget
		{
			var type = typeof(T);
			var stream = type.Assembly.GetManifestResourceStream (type.FullName + ".xaml");
			return Load<T> (stream, instance);
		}

		public static T Load<T> (Stream stream, T instance)
			where T : InstanceWidget
		{
			var type = typeof(T);
			var context = new XamlSchemaContext (new Assembly[] { typeof(XamlReader).Assembly });
			var reader = new XamlXmlReader (stream, context);
			var writerSettings = new XamlObjectWriterSettings {
				RootObjectInstance = instance
			};
			writerSettings.AfterPropertiesHandler += delegate (object sender, XamlObjectEventArgs e) {
				var obj = e.Instance as InstanceWidget;
				if (obj != null && !string.IsNullOrEmpty (obj.ID)) {
					var field = type.GetField (obj.ID, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
					if (field != null)
						field.SetValue (instance, obj);
				}
			};

			var writer = new XamlObjectWriter (context, writerSettings);
			XamlServices.Transform (reader, writer);
			return writer.Result as T;
		}

	}
}

