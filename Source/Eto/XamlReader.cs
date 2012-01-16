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
			
			public EtoXamlSchemaContext(IEnumerable<Assembly> assemblies)
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
		
		public static T Load<T> (Stream stream)
			where T: class
		{
			
			var context = new EtoXamlSchemaContext (new Assembly[] { Assembly.GetExecutingAssembly () });
			var reader = new XamlXmlReader (stream, context);
			return (T)XamlServices.Load (reader);
		}
	}
}

