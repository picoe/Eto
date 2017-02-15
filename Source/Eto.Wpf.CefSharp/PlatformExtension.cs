using Eto;
using Eto.Forms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CefSharp;

[assembly: PlatformExtension(typeof(Eto.Wpf.CefSharp.PlatformExtension))]

namespace Eto.Wpf.CefSharp
{
    public class PlatformExtension : IPlatformExtension
    {
		static PlatformExtension()
		{
			AppDomain.CurrentDomain.AssemblyResolve += Resolver;
		}

		/// <summary>
		/// Gets or sets a value that enables the use of the CefSharp WebView.
		/// </summary>
		public static bool Enabled { get; set; } = true;

		public void Initialize(Eto.Platform platform)
		{
			if (!platform.IsWpf)
				return;

			var instantiator = platform.Find<WebView.IHandler>();
			platform.Add<WebView.IHandler>(() => Enabled ? new CefSharpWebViewHandler() : instantiator());
			LoadApp();
		}

		// Will attempt to load missing assembly from either x86 or x64 subdir
		static Assembly Resolver(object sender, ResolveEventArgs args)
		{
			if (args.Name.StartsWith("CefSharp"))
			{
				string assemblyName = args.Name.Split(new[] { ',' }, 2)[0] + ".dll";
				string archSpecificPath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
													   Environment.Is64BitProcess ? "x64" : "x86",
													   assemblyName);

				return File.Exists(archSpecificPath)
						   ? Assembly.LoadFile(archSpecificPath)
						   : null;
			}

			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		static void LoadApp()
		{
			var settings = new CefSettings();

			// Set BrowserSubProcessPath based on app bitness at runtime
			settings.BrowserSubprocessPath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
												   Environment.Is64BitProcess ? "x64" : "x86",
												   "CefSharp.BrowserSubprocess.exe");

			settings.SetOffScreenRenderingBestPerformanceArgs();
			// Make sure you set performDependencyCheck false
			Cef.Initialize(settings, performDependencyCheck: false, browserProcessHandler: null);
		}
	}
}
