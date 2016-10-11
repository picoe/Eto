using Eto.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace Eto.Designer
{
	class AppDomainProxy : MarshalByRefObject
	{
		IDesignHost designPanel;
		#pragma warning disable 414
		IDisposable resolver;
		#pragma warning restore 414

		public Action ControlCreated
		{
			get { return designPanel.ControlCreated; }
			set { designPanel.ControlCreated = value; }
		}

		public Action<Exception> Error
		{
			get { return designPanel.Error; }
			set { designPanel.Error = value; }
		}

		public void Init(string platformType, string initializeAssembly, string mainAssembly, IEnumerable<string> references)
		{
			if (Platform.Instance == null)
			{
				var refs = new[] { Path.GetDirectoryName(typeof(AppDomainProxy).Assembly.Location), mainAssembly };
				resolver = AssemblyResolver.Register(refs.Union(references));

				var plat = Activator.CreateInstance(Type.GetType(platformType)) as Platform;
				Platform.Initialize(plat);
				if (!string.IsNullOrEmpty(initializeAssembly))
				{
					plat.LoadAssembly(initializeAssembly);
				}
				var app = new Application();
				app.Attach();
				app.Terminating += App_Terminating;

				app.UnhandledException += App_UnhandledException;

				AppDomain.CurrentDomain.DomainUnload += CurrentDomain_DomainUnload;
			}

			designPanel = new DesignPanel();
			designPanel.MainAssembly = mainAssembly;
			designPanel.References = references.ToList();
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
			EtoAdapter.Unload();
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

		public void ControlCreated()
		{
			Host.ControlCreated?.Invoke();
		}

		public void Error(Exception ex)
		{
			Host.Error?.Invoke(ex);
		}

		public override object InitializeLifetimeService()
		{
			return null;
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
				SetupAppDomain(true);
				ContainerChanged?.Invoke();
				if (!ReferenceEquals(oldDomain, domain))
					Application.Instance.AsyncInvoke(() => UnloadDomain(oldDomain));
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
				domain.DomainUnload += Domain_DomainUnload;

				proxy.ControlCreated = eventSink.ControlCreated;
				proxy.Error = eventSink.Error;
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

		static void Domain_DomainUnload(object sender, EventArgs e)
		{
			Debug.WriteLine("Unloaded");
		}

		public Action ContainerChanged { get; set; }

		public Action ControlCreated { get; set; }

		public Action<Exception> Error { get; set; }

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
			if (domain != null && !domain.IsFinalizingForUnload())
			{
				AppDomain.Unload(domain);
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
