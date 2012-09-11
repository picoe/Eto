using System;
using System.IO;
using json = Newtonsoft.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace Eto.Json
{
	
	public class NamespaceManager
	{
		Dictionary<string, NamespaceInfo> namespaces = new Dictionary<string, NamespaceInfo>();
		
		public NamespaceInfo DefaultNamespace { get; set; }
		
		public IDictionary<string, NamespaceInfo> Namespaces 
		{
			get { return namespaces;}
		}
		
		public NamespaceInfo LookupNamespace (string prefix)
		{
			if (string.IsNullOrEmpty (prefix))
				return DefaultNamespace;
			NamespaceInfo val;
			if (namespaces.TryGetValue(prefix, out val))
				return val;
			
			return null;
		}
		
		public Type LookupType (string typeName)
		{
			var prefixIndex = typeName.IndexOf (':');
			NamespaceInfo ns;
			if (prefixIndex > 0)
			{
				var prefix = typeName.Substring (0, prefixIndex);
				typeName = typeName.Substring (prefixIndex + 1);
				ns = LookupNamespace(prefix);
			}
			else
				ns = DefaultNamespace;
			
			if (ns == null)
				return null;
			
			return ns.FindType (typeName);
		}
		
	}
	
}
