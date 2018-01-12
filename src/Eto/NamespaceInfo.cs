using System;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace Eto
{
	/// <summary>
	/// Helper class to get information about a namespace and assembly
	/// </summary>
	/// <remarks>
	/// This is used to easily split the namespace and assembly parts from a string, and get the assembly instance.
	/// For example, "Eto.Drawing, Eto" will be split into "Eto.Drawing" for the namespace and the <see cref="Assembly"/>
	/// instance for the Eto.dll.
	/// 
	/// Used primarily for serialization and conversion of types.
	/// </remarks>
	public class NamespaceInfo
	{
		string assemblyName;
		Assembly assembly;

		/// <summary>
		/// Gets or sets the assembly.
		/// </summary>
		/// <value>The assembly.</value>
		public Assembly Assembly
		{
			set
			{
				assembly = value;
				assemblyName = null;
			}
			get
			{
				if (assembly == null && !string.IsNullOrEmpty(assemblyName))
					assembly = Assembly.Load(new AssemblyName(assemblyName));
				return assembly;
			}
		}

		/// <summary>
		/// Gets the namespace.
		/// </summary>
		/// <value>The namespace.</value>
		public string Namespace { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.NamespaceInfo"/> class.
		/// </summary>
		/// <param name="ns">Namespace in the form of "My.Namespace, MyAssembly"</param>
		public NamespaceInfo(string ns)
		{
			SetNamespace(ns);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.NamespaceInfo"/> class.
		/// </summary>
		/// <param name="ns">Namespace part in the form of "My.Namespace"</param>
		/// <param name="assembly">Assembly for the namespace</param>
		public NamespaceInfo(string ns, Assembly assembly)
		{
			if (ns.Contains(","))
				SetNamespace(ns);
			else
			{
				this.Namespace = ns;
				this.Assembly = assembly;
			}
		}

		void SetNamespace(string ns)
		{
			var val = ns.Split(',');
			if (val.Length == 2)
			{
				Namespace = val[0].Trim();
				assemblyName = val[1].Trim();
			}
			else
				throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Namespace must include the assembly name in the form of: My.Namespace, MyAssembly"), ns);
		}

		/// <summary>
		/// Finds the specified type in the <see cref="Namespace"/> of this <see cref="Assembly"/>
		/// </summary>
		/// <returns>The type if found, otherwise null</returns>
		/// <param name="typeName">Type name without namespace</param>
		public Type FindType(string typeName)
		{
			return Assembly.GetType(Namespace + "." + typeName);
		}

		/// <summary>
		/// Finds the resource in the <see cref="Namespace"/> of this <see cref="Assembly"/>
		/// </summary>
		/// <returns>The resource stream if found, otherwise null</returns>
		/// <param name="resourceName">Resource name without namespace</param>
		public Stream FindResource(string resourceName)
		{
			return Assembly.GetManifestResourceStream(Namespace + "." + resourceName);
		}

		/// <summary>
		/// Finds the resource in the <see cref="Assembly"/> with the specified <see cref="Namespace"/>
		/// </summary>
		/// <returns>The resource if found, otherwise null</returns>
		public Stream FindResource()
		{
			return Assembly.GetManifestResourceStream(Namespace);
		}
	}
}
