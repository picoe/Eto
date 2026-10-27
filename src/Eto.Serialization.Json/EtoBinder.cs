using Newtonsoft.Json.Serialization;

namespace Eto.Serialization.Json
{
	
	public class EtoBinder : DefaultSerializationBinder
	{
		// the designer loads json without the class it belongs to, so it can't know the local assembly
		static Type FindLoadedType(string typeName)
		{
			foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				if (assembly.IsDynamic)
					continue;
				try
				{
					var type = assembly.GetType(typeName, false);
					if (type != null)
						return type;
				}
				catch
				{
					// the project's types can fail to load when one of their dependencies is missing
				}
			}
			return null;
		}

		public object Instance { get; set; }

		public Type BindToType (string typeName)
		{
			var asmIndex = typeName.IndexOf(',');
			if (asmIndex > 0)
				return BindToType(typeName.Substring(asmIndex + 1).Trim(), typeName.Substring(0, asmIndex).Trim());
			return BindToType(null, typeName);
		}
		
		public NamespaceManager NamespaceManager
		{
			get; set;
		}

		/// <summary>
		/// Gets or sets the assembly to find types in when a type name has no assembly
		/// </summary>
		public Assembly LocalAssembly { get; set; }
		
		public override Type BindToType (string assemblyName, string typeName)
		{
			if (string.IsNullOrEmpty (assemblyName)) {
				var type = NamespaceManager?.LookupType (typeName)
					?? LocalAssembly?.GetType (typeName)
					?? (Control.IsDesignMode ? FindLoadedType (typeName) : null);
				if (type != null)
					return type;
			}
			return base.BindToType (assemblyName, typeName);
		}
	}
	
}
