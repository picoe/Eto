using System;
using Newtonsoft.Json.Serialization;

namespace Eto.Serialization.Json
{
	
	public class EtoBinder : DefaultSerializationBinder
	{
		public object Instance { get; set; }

		public Type BindToType (string typeName)
		{
			var asmIndex = typeName.IndexOf(',');
			if (asmIndex > 0)
				return BindToType(typeName.Substring(0, asmIndex), typeName.Substring(asmIndex + 1));
			return BindToType(null, typeName);
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
