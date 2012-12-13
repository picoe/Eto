using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Drawing;
using System.Text.RegularExpressions;
using Eto.Forms;
using swi = System.Windows.Input;
using swm = System.Windows.Media;
using sw = System.Windows;
using System.Reflection;
using System.IO;
using Eto.Platform.Wpf.Drawing;

namespace Eto.Platform.Wpf
{
	public class Generator : Eto.Generator
	{
		public override string ID
		{
			get { return Generators.Wpf; }
		}

		static Dictionary<string, Assembly> loadedAssemblies = new Dictionary<string, Assembly> ();

		static Generator ()
		{
			AppDomain.CurrentDomain.AssemblyResolve += (sender, args) => {
				var assemblyName = new AssemblyName (args.Name);
				if (assemblyName.Name.EndsWith (".resources")) return null;

				string resourceName = "Eto.Platform.Wpf.CustomControls.Assemblies." + assemblyName.Name + ".dll";
				Assembly assembly = null;
				lock (loadedAssemblies) {
					if (!loadedAssemblies.TryGetValue (resourceName, out assembly)) {
						using (var stream = Assembly.GetExecutingAssembly ().GetManifestResourceStream (resourceName)) {
							if (stream != null) {
								using (var binaryReader = new BinaryReader (stream)) {
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

		public Generator ()
		{
			AddAssembly (typeof (Generator).Assembly);
			//Add<IMatrixHandler, MatrixHandler> ();
			Add<IMatrixHandler> (() => new MatrixHandler ());

			// by default, use WinForms web view (it has more features we can control)
			UseSwfWebView ();
		}

		public void UseWpfWebView ()
		{
			//Add<IWebView, Forms.Controls.WpfWebViewHandler> ();
			Add<IWebView> (() => new Forms.Controls.WpfWebViewHandler ());
		}

		public void UseSwfWebView ()
		{
			//Add<IWebView, Forms.Controls.SwfWebViewHandler> ();
			Add<IWebView> (() => new Forms.Controls.SwfWebViewHandler ());
		}
	}
}
