using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Eto
{
	/// <summary>
	/// Loads assemblies from embedded resources instead of from disk
	/// </summary>
	/// <remarks>
	/// This is useful when you want to create a single assembly/executable without having to ship referenced dll's 
	/// alongside your application.
	/// 
	/// If you wish to embed Eto.dll as well, you will have to copy this class to your executable.
	/// </remarks>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class EmbeddedAssemblyLoader
	{
		readonly Dictionary<string, Assembly> loadedAssemblies = new Dictionary<string, Assembly>();

		/// <summary>
		/// Gets the assembly in which this loader will load assembly resources from
		/// </summary>
		public Assembly Assembly { get; private set; }

		/// <summary>
		/// Gets the namespace in the <see cref="Assembly"/> to get the assembly resources from
		/// </summary>
		public string ResourceNamespace { get; private set; }

		/// <summary>
		/// Registers the specified namespace for loading embedded assemblies
		/// </summary>
		/// <param name="resourceNamespace">Namespace of where the embedded assemblies should be loaded</param>
		/// <param name="assembly">Assembly to load the embedded assemblies from, or null to use the calling assembly</param>
		/// <param name="domain">Application domain to load the assemblies in, or null to use the current app domain</param>
		/// <returns>A new instance of an EmbeddedAssemblyLoader, registered for the specified namespace and assembly</returns>
		public static EmbeddedAssemblyLoader Register(string resourceNamespace, Assembly assembly = null, AppDomain domain = null)
		{
			assembly = assembly ?? Assembly.GetCallingAssembly();
			var loader = new EmbeddedAssemblyLoader(resourceNamespace, assembly);
			loader.Register(domain);
			return loader;
		}

		/// <summary>
		/// Initializes a new instance of the EmbeddedAssemblyLoader
		/// </summary>
		/// <param name="resourceNamespace">Namespace of where the embedded assemblies should be loaded</param>
		/// <param name="assembly">Assembly to load the embedded assemblies from, or null to use the calling assembly</param>
		public EmbeddedAssemblyLoader(string resourceNamespace, Assembly assembly = null)
		{
			Assembly = assembly ?? Assembly.GetCallingAssembly();
			ResourceNamespace = resourceNamespace;
		}

		/// <summary>
		/// Registers this loader for the specified <paramref name="domain"/>
		/// </summary>
		/// <param name="domain">App domain to register this loader for, or null to use the current domain</param>
		public void Register(AppDomain domain = null)
		{
			var resolving = false;
			domain = domain ?? AppDomain.CurrentDomain;
			domain.AssemblyResolve += (sender, args) =>
			{
				// Prevent stack overflow in the case when someone handles AppDomain.ResourceResolve which will be called
				// if the resource cannot be found below with GetManifestResourceStream, then tries to load a non-existant assembly
				if (resolving)
					return null;

				resolving = true;
				Assembly loadedAssembly;
				try
				{
					var assemblyName = new AssemblyName(args.Name);
					if (assemblyName.Name.EndsWith(".resources", StringComparison.OrdinalIgnoreCase)) return null;

					string resourceName = $"{ResourceNamespace}.{assemblyName.Name}.dll";
					lock (loadedAssemblies)
					{
						if (!loadedAssemblies.TryGetValue(resourceName, out loadedAssembly))
						{
							using (var stream = Assembly.GetManifestResourceStream(resourceName))
							{
								if (stream != null)
								{
									using (var binaryReader = new BinaryReader(stream))
									{
										loadedAssembly = Assembly.Load(binaryReader.ReadBytes((int)stream.Length));
										loadedAssemblies.Add(resourceName, loadedAssembly);
									}
								}
							}
						}
					}
				}
				catch (Exception ex)
				{
					// ignore errors here, just in case.
					Debug.WriteLine($"Error trying to load assembly '{args.Name}':\n{ex}");
					loadedAssembly = null;
				}
				finally
				{
					resolving = false;
				}
				return loadedAssembly;
			};
		}
	}
}