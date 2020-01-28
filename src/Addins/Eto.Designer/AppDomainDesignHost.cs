using Eto.Drawing;
using Eto.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace Eto.Designer
{
	public class DesignError : MarshalByRefObject
	{
		public string Message { get; set; }
		public string Details { get; set; }
	}

	class DomainPlatformThemeHandler : IPlatformTheme
	{
		AppDomainProxy proxy;
		Dictionary<string, Color> colors = new Dictionary<string, Color>();
		public DomainPlatformThemeHandler(AppDomainProxy proxy)
		{
			this.proxy = proxy;
		}
		Color GetColor(string name)
		{
			if (colors.TryGetValue(name, out var value))
				return value;

			return colors[name] = Color.FromArgb(proxy.GetThemeColor(name));
		}

		public Color ProjectBackground => GetColor("ProjectBackground");
		public Color ProjectForeground => GetColor("ProjectForeground");
		public Color ProjectDialogBackground => GetColor("ProjectDialogBackground");
		public Color ErrorForeground => GetColor("ErrorForeground");
		public Color SummaryBackground => GetColor("SummaryBackground");
		public Color SummaryForeground => GetColor("SummaryForeground");
		public Color DesignerBackground => GetColor("DesignerBackground");
		public Color DesignerPanel => GetColor("DesignerPanel");
		public IEnumerable<PlatformColor> AllColors { get; } = Enumerable.Empty<PlatformColor>();
	}

	class AppDomainProxy : MarshalByRefObject
	{
		string mainAssembly;
		List<string> references;
		IDesignHost designPanel;
		#pragma warning disable 414
		IDisposable resolver;
		#pragma warning restore 414

		public Func<string,int> GetThemeColor { get; set; }

		public void Init(string platformType, string initializeAssembly, string mainAssembly, IEnumerable<string> references)
		{
			this.mainAssembly = mainAssembly;
			this.references = references.ToList();
			if (Platform.Instance == null)
			{
				var refs = new[] { Path.GetDirectoryName(typeof(AppDomainProxy).Assembly.Location), mainAssembly };
				resolver = AssemblyResolver.Register(refs.Union(this.references));

				var plat = Activator.CreateInstance(Type.GetType(platformType)) as Platform;
				Platform.Initialize(plat);
				if (!string.IsNullOrEmpty(initializeAssembly))
				{
					plat.LoadAssembly(initializeAssembly);
				}
				plat.Add(typeof(IPlatformTheme), () => new DomainPlatformThemeHandler(this));
				var app = new Application();
				app.Attach();
				app.Terminating += App_Terminating;

				app.UnhandledException += App_UnhandledException;

				AppDomain.CurrentDomain.DomainUnload += CurrentDomain_DomainUnload;
			}
		}
		public void HookupEvents(AppDomainEventSink eventSink)
		{
			GetThemeColor = eventSink.GetThemeColor;

			designPanel = new DesignPanel();
			designPanel.MainAssembly = mainAssembly;
			designPanel.References = references;

			designPanel.Error = eventSink.Error;
			designPanel.ControlCreating = eventSink.ControlCreating;
			designPanel.ControlCreated = eventSink.ControlCreated;
		}

		void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			// error!
		}

		void App_Terminating(object sender, CancelEventArgs e)
		{
			e.Cancel = true;
		}

		void CurrentDomain_DomainUnload(object sender, EventArgs e)
		{
			//EtoAdapter.Unload();
		}

		Control container;
		public object GetContainer()
		{
			// keep ref otherwise it gets gc'd in the app domain
			container = designPanel.GetContainer();
			return container.ToContract();
		}

		public void Invalidate()
		{
			designPanel.Invalidate();
		}

		public bool SetBuilder(string fileName)
		{
			return designPanel.SetBuilder(fileName);
		}

		public void Update(string code)
		{
			designPanel.Update(code);
		}

		public string GetCodeFile(string fileName)
		{
			return designPanel.GetCodeFile(fileName);
		}

		public void Dispose()
		{
			EtoAdapter.Unload();
			designPanel.Dispose();
		}

		public override object InitializeLifetimeService()
		{
			return null;
		}
	}

	class AppDomainEventSink : MarshalByRefObject
	{
		public AppDomainDesignHost Host { get; set; }

		public void ControlCreated() => Host.ControlCreated?.Invoke();
		public void ControlCreating() => Host.ControlCreated?.Invoke();
		public void Error(DesignError ex) => Host.Error?.Invoke(ex);
		public override object InitializeLifetimeService() => null;

		public int GetThemeColor(string name)
		{
			switch (name)
			{
				case "DesignerPanel": return Global.Theme.DesignerPanel.ToArgb();
				case "DesignerBackground": return Global.Theme.DesignerBackground.ToArgb();
				case "ErrorForeground": return Global.Theme.ErrorForeground.ToArgb();
				case "ProjectBackground": return Global.Theme.ProjectBackground.ToArgb();
				case "ProjectDialogBackground": return Global.Theme.ProjectDialogBackground.ToArgb();
				case "ProjectForeground": return Global.Theme.ProjectForeground.ToArgb();
				case "SummaryBackground": return Global.Theme.SummaryBackground.ToArgb();
				case "SummaryForeground": return Global.Theme.SummaryForeground.ToArgb();
				default:
					return Colors.White.ToArgb();
			}
		}
	}

	public class AppDomainDesignHost : IDesignHost, IDisposable
	{
		List<string> references;

		public IEnumerable<string> References
		{
			get { return references; }
			set { references = value.ToList(); }
		}

		public string MainAssembly { get; set; }

		public AppDomainDesignHost()
		{
			eventSink = new AppDomainEventSink { Host = this };
			timer = new UITimer { Interval = 1 };
			timer.Elapsed += Timer_Elapsed;
		}

		~AppDomainDesignHost()
		{
			Dispose(false);
		}

		void Timer_Elapsed(object sender, EventArgs e)
		{
			timer.Stop();

			if (domain != null)
			{
				var oldDomain = domain;
				requiresNewDomain = true;
				var newDomain = SetupAppDomain(true);
				ContainerChanged?.Invoke();
				if (newDomain && !ReferenceEquals(oldDomain, domain))
					Task.Run(() => UnloadDomain(oldDomain));
			}
		}

		bool requiresNewDomain;
		AppDomain domain;
		FileSystemWatcher watcher;
		AppDomainProxy proxy;
		AppDomainEventSink eventSink;
		static int domainCount = 0;
		UITimer timer;

		IEnumerable<string> GetShadowCopyDirs()
		{
			if (!string.IsNullOrEmpty(MainAssembly))
				yield return Path.GetDirectoryName(MainAssembly);
			if (References != null)
			{
				foreach (var r in References)
				{
					if (!string.IsNullOrEmpty(r))
						yield return Path.GetDirectoryName(r);
				}
			}
		}

		bool SetupAppDomain(bool setBuilder)
		{
			if (!requiresNewDomain && domain != null)
				return false;

			requiresNewDomain = false;
#pragma warning disable 618
			// doesn't work without for some reason, and there's no non-obsolete alternative.
			if (!AppDomain.CurrentDomain.ShadowCopyFiles)
				AppDomain.CurrentDomain.SetShadowCopyFiles();
#pragma warning restore 618

			var baseDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			var initializeAssembly = Builders.BaseCompiledInterfaceBuilder.InitializeAssembly;

			var shadowCopyDirs = string.Join(";", GetShadowCopyDirs().Distinct());
			var setup = new AppDomainSetup
			{
				ApplicationBase = AppDomain.CurrentDomain.BaseDirectory,
				PrivateBinPath = $"{baseDir};{shadowCopyDirs}",

				ShadowCopyFiles = "true",
				ShadowCopyDirectories = shadowCopyDirs,
				CachePath = Path.Combine(Path.GetDirectoryName(MainAssembly), "Eto.Designer"),

				LoaderOptimization = LoaderOptimization.MultiDomain,
				//LoaderOptimization = LoaderOptimization.NotSpecified
			};

			proxy = null;
			domain = AppDomain.CreateDomain("eto.designer." + domainCount++, null, setup);
			try
			{
				using (AssemblyResolver.Register(baseDir))
				{
					var proxyObject = domain.CreateInstanceFromAndUnwrap(typeof(AppDomainProxy).Assembly.Location, typeof(AppDomainProxy).FullName) as AppDomainProxy;
					proxy = proxyObject as AppDomainProxy;
					if (proxy == null)
						throw new InvalidOperationException($"Could not create proxy for domain\nApplicationBase: {AppDomain.CurrentDomain.BaseDirectory}\nBaseDir: {baseDir}\nShadowCopyDirs: {shadowCopyDirs}");
				}
				proxy.Init(Platform.Instance.GetType().AssemblyQualifiedName, initializeAssembly, MainAssembly, references);

				proxy.HookupEvents(eventSink);

				if (setBuilder)
					proxy.SetBuilder(fileName);
				if (!string.IsNullOrEmpty(code))
					proxy.Update(code);
			}
			catch (Exception ex)
			{
				UnloadDomain(domain);
				domain = null;
				proxy = null;
				throw new InvalidOperationException($"Could not set up proxy for domain: {ex.GetBaseException().Message}", ex);
			}

			if (watcher == null && !string.IsNullOrEmpty(MainAssembly))
			{
				watcher = new FileSystemWatcher(Path.GetDirectoryName(MainAssembly), "*.dll");
				watcher.Changed += (sender, e) => Application.Instance.AsyncInvoke(() => timer.Start());
				watcher.Created += (sender, e) => Application.Instance.AsyncInvoke(() => timer.Start());
				watcher.EnableRaisingEvents = true;
			}

			return true;
		}

		public Action ContainerChanged { get; set; }

		public Action ControlCreated { get; set; }
		public Action ControlCreating { get; set; }

		public Action<DesignError> Error { get; set; }

		public Control GetContainer()
		{
			SetupAppDomain(true);
			return EtoAdapter.ToControl(proxy.GetContainer());
		}

		public void Invalidate()
		{
			proxy?.Invalidate();
		}

		string fileName;
		public bool SetBuilder(string fileName)
		{
			this.fileName = fileName;
			SetupAppDomain(false);
			return proxy.SetBuilder(fileName);
		}

		string code;
		public void Update(string code)
		{
			SetupAppDomain(true);

			this.code = code;
			proxy.Update(code);
		}

		public string GetCodeFile(string fileName)
		{
			return proxy.GetCodeFile(fileName);
		}

		void UnloadDomain(AppDomain domain)
		{
			try
			{
				if (domain != null && !domain.IsFinalizingForUnload())
				{
					AppDomain.Unload(domain);
				}
			}
			catch
			{
				Debug.WriteLine("Could not unload domain");
				// ignore
			}
		}

		public void Dispose()
		{
			Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				proxy?.Dispose();
				
				UnloadDomain(domain);
				domain = null;
				proxy = null;
			}
		}
	}
}
