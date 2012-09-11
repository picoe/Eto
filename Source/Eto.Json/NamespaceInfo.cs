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
	
	public class NamespaceInfo
	{
		public Assembly Assembly { get; set; }
		
		public string Namespace { get; set; }
		
		public string Prefix { get; set; }
		
		public NamespaceInfo ()
		{
		}
		
		public NamespaceInfo (string ns)
		{
			var val = ns.Split (',');
			if (val.Length == 2)
			{
				Namespace = val[0];
				Assembly = Assembly.LoadFile (val[1]);
			}
		}
		
		public NamespaceInfo (string ns, Assembly assembly)
		{
			this.Namespace = ns;
			this.Assembly = assembly;
		}
		
		public Type FindType (string typeName)
		{
			return Assembly.GetType (Namespace + "." + typeName);
		}
	}
	
}
