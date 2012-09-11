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
	
	public class EtoBinder : DefaultSerializationBinder
	{
		public Type BindToType (string typeName)
		{
			var asmIndex = typeName.IndexOf (',');
			if (asmIndex > 0)
				return BindToType (typeName.Substring (0, asmIndex), typeName.Substring (asmIndex + 1));
			else
				return BindToType (null, typeName);
		}
		
		public NamespaceManager NamespaceManager
		{
			get; set;
		}
		
		public override Type BindToType (string assemblyName, string typeName)
		{
			if (string.IsNullOrEmpty (assemblyName) && NamespaceManager != null) {
				var type = NamespaceManager.LookupType (typeName);
				if (type != null)
					return type;
			}
			return base.BindToType (assemblyName, typeName);
		}
	}
	
}
