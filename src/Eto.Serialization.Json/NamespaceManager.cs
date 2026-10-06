namespace Eto.Serialization.Json
{
	public class NamespaceManager
	{
		readonly Dictionary<string, NamespaceInfo> namespaces = new Dictionary<string, NamespaceInfo>();
		
		public NamespaceInfo DefaultNamespace { get; set; }

		/// <summary>
		/// Gets or sets the assembly to find types in when a $type has no assembly name, eg. "MyApp.MyControl".
		/// </summary>
		/// <remarks>
		/// When null, the assembly of the object being loaded is used.
		/// </remarks>
		public Assembly LocalAssembly { get; set; }
		
		public IDictionary<string, NamespaceInfo> Namespaces 
		{
			get { return namespaces;}
		}

		public NamespaceInfo LookupNamespace (string prefix)
		{
			if (string.IsNullOrEmpty (prefix))
				return DefaultNamespace;
			NamespaceInfo val;
			return namespaces.TryGetValue(prefix, out val) ? val : null;
			
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
			
			return ns != null ? ns.FindType(typeName) : null;
			
		}
	}
}
