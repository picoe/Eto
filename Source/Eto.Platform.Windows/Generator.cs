using System;
using System.Reflection;
using Eto.Forms;
using Eto.Drawing;
using Eto.IO;
using SD = System.Drawing;
using SWF = System.Windows.Forms;
using Eto.Platform.Windows.Drawing;
using System.Collections.Generic;
using System.IO;

namespace Eto.Platform.Windows
{
	public class Generator : Eto.Generator
	{
		public override string ID {
			get {
				return Generators.Windows;
			}
		}

		static Dictionary<string, Assembly> loadedAssemblies = new Dictionary<string, Assembly> ();

		static Generator ()
		{
			AppDomain.CurrentDomain.AssemblyResolve += (sender, args) => {
				var assemblyName = new AssemblyName (args.Name);
				if (assemblyName.Name.EndsWith (".resources"))
					return null;

				string resourceName = "Eto.Platform.Windows.CustomControls.Assemblies." + assemblyName.Name + ".dll";
				Assembly assembly = null;
				lock (loadedAssemblies)
				{
					if (!loadedAssemblies.TryGetValue (resourceName, out assembly))
					{
						using (var stream = Assembly.GetExecutingAssembly ().GetManifestResourceStream (resourceName))
						{
							if (stream != null)
							{
								using (var binaryReader = new BinaryReader (stream))
								{
									assembly = Assembly.Load (binaryReader.ReadBytes ((int)stream.Length));
									loadedAssemblies.Add (resourceName, assembly);
								}
							}
						}
					}
				}
				return assembly;
			};
		}

        public Generator()
        {
            AddAssembly(typeof(Generator).Assembly);
        }
    }
}
