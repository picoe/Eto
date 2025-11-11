using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;


#nullable enable

#if NET
[assembly: System.Reflection.Metadata.MetadataUpdateHandler(typeof(Eto.HotReloadService))]
#endif

namespace Eto;


/// <summary>
/// Indicates that a class supports hot reload functionality
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class HotReloadAttribute : Attribute
{
	/// <summary>
	/// Name of the method to invoke when a hot reload occurs.  Default is "InitializeComponent".
	/// </summary>
	public string MethodName { get; }

	/// <summary>
	/// Initializes a new instance of the <see cref="HotReloadAttribute"/> class.
	/// </summary>
	/// <param name="methodName">Name of the method to invoke when a hot reload occurs.</param>
	public HotReloadAttribute(string methodName = "InitializeComponent")
	{
		MethodName = methodName;
	}
}

/// <summary>
/// Enables hot reload functionality in Eto
/// </summary>
public static class HotReloadService
{
	/// <summary>
	/// Gets a value indicating whether hot reload is enabled.
	/// </summary>
	public static bool Enabled { get; private set; }

	static event Action<Type[]?>? Update;

	static Dictionary<string, WatcherInfo> hotReloadWatchers = new();

	class WatcherInfo
	{
		public FileSystemWatcher Watcher { get; }
		public List<InstanceInfo> Instances { get; set; } = new();

		public WatcherInfo(string dir)
		{
			Watcher = new FileSystemWatcher(dir)
			{
				NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.CreationTime,
				EnableRaisingEvents = true
			};
			Watcher.Changed += Watcher_Changed;
		}

		private void Watcher_Changed(object sender, FileSystemEventArgs e)
		{
			var fileName = Path.GetFileName(e.FullPath);
			Application.Instance?.Invoke(() =>
			{
				var cleanWatchers = new List<int>();
				
				for (int i = 0; i < Instances.Count; i++)
				{
					InstanceInfo? instance = Instances[i];
					
					if (instance.File != fileName)
						continue;

					if (!instance.Update())
					{
						cleanWatchers.Add(i);
					}
				}
				for (int i = cleanWatchers.Count - 1; i >= 0; i--)
				{
					int index = cleanWatchers[i];
					Instances.RemoveAt(index);
				}
			});
		}

		internal bool AddWatcher(object control, string file, Action<object> onChanged)
		{
			void DoUpdate(object? arg)
			{
				try
				{
					if (arg != null)
						onChanged?.Invoke(arg);

					// Ensure we clean up old references
					GC.Collect();
					GC.WaitForPendingFinalizers();
				}
				catch (Exception ex)
				{
					Trace.WriteLine(ex);
				}
			}			

			var cleanWatchers = new List<int>();
			bool found = false;
			for (int i = 0; i < Instances.Count; i++)
			{
				var instanceInfo = Instances[i];
				if (!instanceInfo.Instance.IsAlive || instanceInfo.Instance.Target == null)
					cleanWatchers.Add(i);
				if (ReferenceEquals(instanceInfo.Instance.Target, control))
					found = true; // already watching this instance
			}

			// clean up any dead references
			for (int j = cleanWatchers.Count - 1; j >= 0; j--)
			{
				int index = cleanWatchers[j];
				Instances.RemoveAt(index);
			}
			
			if (found)
				return true;

			Instances.Add(new InstanceInfo(control, file, DoUpdate));

#if NET
			if (Watcher.Filters.Contains(file))
				return true; // already watching this file

			// create a filter for the file
			Watcher.Filters.Add(file);
#else
			Watcher.Filter = file;
#endif
			return true;
		}
	}

	class InstanceInfo
	{
		public WeakReference Instance { get; }
		public string File { get; }
		public Action<object> OnChanged { get; }

		public InstanceInfo(object instance, string file, Action<object> onChanged)
		{
			Instance = new WeakReference(instance);
			File = file;
			OnChanged = onChanged;
		}

		internal bool Update()
		{
			var target = Instance.Target;
			if (target != null)
			{
				OnChanged?.Invoke(target);
				return true;
			}
			return false;
		}
	}

	/// <summary>
	/// Watches a file for changes and triggers the method specified in the <see cref="HotReloadAttribute"/> when it changes.
	/// </summary>
	/// <param name="instance">Object to trigger changes to</param>
	/// <param name="fileName">File to watch for changes</param>
	/// <param name="sourcePath">Source path of the file</param>
	/// <returns>true if the file is being watched, false otherwise</returns>
	/// <exception cref="ArgumentNullException"></exception>
	public static bool WatchFile(object instance, string fileName, [CallerFilePath] string? sourcePath = null)
	{
		if (instance == null)
			throw new ArgumentNullException(nameof(instance));
		if (fileName == null)
			throw new ArgumentNullException(nameof(fileName));
			
		var type = instance.GetType();
		
		var hotReload = type.GetCustomAttribute<HotReloadAttribute>(true);
		if (hotReload == null)
		{
			Trace.WriteLine($"Type '{type.FullName}' must have the {nameof(HotReloadAttribute)} to use this overload of {nameof(WatchFile)}.", nameof(instance));
			return false;
		}
		
		var hotReloadMethod = type.GetMethod(hotReload.MethodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		if (hotReloadMethod == null)
		{
			Trace.WriteLine($"Type '{type.FullName}' does not have a method named '{hotReload.MethodName}' to use for hot reloading.", nameof(instance));
			return false;
		}
		
		return WatchFile(instance, fileName, c => hotReloadMethod.Invoke(c, null), sourcePath);
	}

	/// <summary>
	/// Watches a file for changes and triggers the specified action when it changes.
	/// </summary>
	/// <param name="control">Control to trigger changes to</param>
	/// <param name="fileName">File to watch for changes</param>
	/// <param name="onChanged">Action to invoke when the file changes</param>
	/// <param name="sourcePath">Source path of the file</param>
	/// <returns>true if the file is being watched, false otherwise</returns>
	/// <exception cref="ArgumentNullException"></exception>
	public static bool WatchFile(object control, string fileName, Action<object> onChanged, [CallerFilePath] string? sourcePath = null)
	{
		if (control == null)
			throw new ArgumentNullException(nameof(control));
		if (fileName == null)
			throw new ArgumentNullException(nameof(fileName));
		if (onChanged == null)
			throw new ArgumentNullException(nameof(onChanged));

		if (!File.Exists(fileName))
		{
			if (sourcePath != null && File.Exists(sourcePath))
				sourcePath = Path.GetDirectoryName(sourcePath);

			fileName = Path.Combine(sourcePath ?? ".", fileName);
		}

		if (!File.Exists(fileName))
		{
			Trace.WriteLine($"File '{fileName}' does not exist to watch for hot reload.");
			return false;
		}

		// create or update the file watcher
		var dir = Path.GetDirectoryName(fileName) ?? ".";
		var file = Path.GetFileName(fileName);
#if NET
		var watcherKey = dir;
#else
		var watcherKey = fileName;
#endif
		if (!hotReloadWatchers.TryGetValue(watcherKey, out var info))
		{
			// create a new watcher for this directory
			info = new WatcherInfo(dir);
			hotReloadWatchers[watcherKey] = info;
		}

		info.AddWatcher(control, file, onChanged);

		return true;
	}

	class HotReloadInstance
	{
		public WeakReference Instance { get; set; }
		public Action<object> OnChanged { get; set; }

		public HotReloadInstance(object instance, Action<object> onChanged)
		{
			Instance = new WeakReference(instance);
			OnChanged = onChanged;
			Update += OnUpdate;
		}

		private void OnUpdate(Type[]? obj)
		{
			var target = Instance.Target;
			if (target == null)
			{
				Update -= OnUpdate;
				return;
			}
			OnChanged?.Invoke(target);
		}
	}

	/// <summary>
	/// Initializes hot reload of controls, if it is supported.
	/// </summary>
	/// <remarks>
	/// Code-based hot reload will only work when a debugger is attached.
	/// This will also enable support to hot reload xeto and jeto files if in use.
	/// 
	/// Note that at the time of this writing, code based hot reload only works with Visual Studio 2022 on Windows.
	/// </remarks>
	public static void Initialize()
	{
		if (Enabled) return;
		Enabled = true;
		if (!Debugger.IsAttached)
			return;

		Eto.Style.Add<Container>(null, container =>
		{
			if (container.Properties.ContainsKey(typeof(HotReloadInstance)))
					return; // already registered

			var type = container.GetType();

			// ignore base eto types
			if (type.Assembly == typeof(Container).Assembly)
				return;

			// only when the type has the HotReloadAttribute
			var hotReload = type.GetCustomAttribute<HotReloadAttribute>();
			if (hotReload == null)
				return;
			var hotReloadMethod = type.GetMethod(hotReload.MethodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (hotReloadMethod == null)
			{
				Trace.WriteLine($"Type '{type.FullName}' does not have a method named '{hotReload.MethodName}' to use for hot reloading.", nameof(container));	
				return;
			}

			container.Properties[typeof(HotReloadInstance)] = "hotreloadenabled";

			// Note: reference not needed here, we subscribe to a global event
			new HotReloadInstance(container, c =>
			{
				try
				{
					hotReloadMethod.Invoke(c, null);
				}
				catch (Exception ex)
				{
					Trace.WriteLine(ex);
				}
			});
		});
	}

	internal static void ClearCache(Type[]? updatedTypes)
	{
	}

	internal static void UpdateApplication(Type[]? updatedTypes)
	{
		Application.Instance?.Invoke(() => Update?.Invoke(updatedTypes));
	}
}
