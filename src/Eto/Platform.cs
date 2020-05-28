using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Threading;
using Eto.Drawing;

namespace Eto
{
	/// <summary>
	/// Arguments for when a widget is created
	/// </summary>
	/// <copyright>(c) 2012-2015 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class WidgetCreatedEventArgs : EventArgs
	{
		/// <summary>
		/// Gets the instance of the widget that was created
		/// </summary>
		public Widget Instance { get; private set; }

		/// <summary>
		/// Initializes a new instance of the WidgetCreatedArgs class
		/// </summary>
		/// <param name="instance">Instance of the widget that was created</param>
		public WidgetCreatedEventArgs(Widget instance)
		{
			this.Instance = instance;
		}
	}

	/// <summary>
	/// Arguments for when a widget is created
	/// </summary>
	/// <copyright>(c) 2012-2015 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class HandlerCreatedEventArgs : EventArgs
	{
		/// <summary>
		/// Gets the instance of the widget that was created
		/// </summary>
		public object Instance { get; private set; }

		/// <summary>
		/// Initializes a new instance of the WidgetCreatedArgs class
		/// </summary>
		/// <param name="instance">Instance of the widget that was created</param>
		public HandlerCreatedEventArgs(object instance)
		{
			this.Instance = instance;
		}
	}

	class HandlerInfo
	{
		public bool Initialize { get; private set; }
		public Func<object> Instantiator { get; private set; }

		public HandlerInfo(bool initialize, Func<object> instantiator)
		{
			Initialize = initialize;
			Instantiator = instantiator;
		}
	}

	/// <summary>
	/// Flags to specify which global features are supported for a platform
	/// </summary>
	[Flags]
	public enum PlatformFeatures
	{
		/// <summary>
		/// No extra features supported.
		/// </summary>
		None = 0,

		/// <summary>
		/// Specifies that the <see cref="Forms.CustomCell"/> supports creating a Control for each cell.
		/// If not specified, then the CustomCell will paint its content when not in edit mode.
		/// </summary>
		CustomCellSupportsControlView = 1 << 0,

		/// <summary>
		/// Specifies that the <see cref="Forms.Drawable"/> supports automatic transparent background for its <c>Content</c>.
		/// If not specified, then setting the content may not work as intended
		/// (most often not rendering with transparent background, thus overpainting the drawable).
		/// </summary>
		DrawableWithTransparentContent = 1 << 1,

		/// <summary>
		/// Specifies the <see cref="Forms.Control.TabIndex"/> is based on the logical tree, not the visual tree.
		/// Both GTK and WinForms do not support creating a custom tab focus and is based on the direct containers.
		/// 
		/// For example, setting a TabIndex for controls in DynamicLayout and StackLayout might not behave as expected on platforms
		/// that do not support this.
		/// </summary>
		TabIndexWithCustomContainers = 1 << 2
	}

	/// <summary>
	/// Base platform class
	/// </summary>
	/// <remarks>
	/// This class takes care of creating the platform-specific implementations of each control.
	/// All controls the platform can handle should be added using <see cref="Platform.Add{T}"/>.
	/// </remarks>
	/// <copyright>(c) 2012-2015 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public abstract class Platform
	{
		readonly Dictionary<Type, Func<object>> instantiatorMap = new Dictionary<Type, Func<object>>();
		readonly Dictionary<Type, HandlerInfo> handlerMap = new Dictionary<Type, HandlerInfo>();
		readonly Dictionary<Type, object> sharedInstances = new Dictionary<Type, object>();
		readonly Dictionary<object, object> properties = new Dictionary<object, object>();
		readonly HashSet<Assembly> loadedAssemblies = new HashSet<Assembly>();
		static Platform globalInstance;
		static readonly ThreadLocal<Platform> instance = new ThreadLocal<Platform>(() => globalInstance);

		internal T GetSharedProperty<T>(object key, Func<T> instantiator)
		{
			object value;
			lock (properties)
			{
				if (!properties.TryGetValue(key, out value))
				{
					value = instantiator();
					properties[key] = value;
				}
			}
			return (T)value;
		}

		internal void SetSharedProperty(object key, object value)
		{
			lock (properties)
			{
				properties[key] = value;
			}
		}

		#region Events

		/// <summary>
		/// Event to handle when widgets are created by this platform
		/// </summary>
		public event EventHandler<HandlerCreatedEventArgs> HandlerCreated;

		/// <summary>
		/// Handles the <see cref="WidgetCreated"/> event
		/// </summary>
		/// <param name="e">Arguments for the event</param>
		protected virtual void OnHandlerCreated(HandlerCreatedEventArgs e)
		{
			if (HandlerCreated != null)
				HandlerCreated(this, e);
		}

		/// <summary>
		/// Event to handle when widgets are created by this platform
		/// </summary>
		public event EventHandler<WidgetCreatedEventArgs> WidgetCreated;

		/// <summary>
		/// Handles the <see cref="WidgetCreated"/> event
		/// </summary>
		/// <param name="e">Arguments for the event</param>
		protected virtual void OnWidgetCreated(WidgetCreatedEventArgs e)
		{
			Eto.Style.Provider?.ApplyDefault(e.Instance);
			WidgetCreated?.Invoke(this, e);
		}

		internal void TriggerWidgetCreated(WidgetCreatedEventArgs args)
		{
			OnWidgetCreated(args);
		}

		#endregion

		/// <summary>
		/// Gets the ID of this platform
		/// </summary>
		/// <remarks>
		/// The platform ID can be used to determine which platform is currently in use.  The platform
		/// does not necessarily correspond to the OS that it is running on, as for example the GTK platform
		/// can run on OS X and Windows.
		/// </remarks>
		public abstract string ID { get; }

		/// <summary>
		/// Gets a value indicating whether this platform is a mac based platform (MonoMac/XamMac)
		/// </summary>
		/// <value><c>true</c> if this platform is mac; otherwise, <c>false</c>.</value>
		public virtual bool IsMac { get { return false; } }

		/// <summary>
		/// Gets a value indicating whether this platform is based on Windows Forms
		/// </summary>
		/// <value><c>true</c> if this platform is window forms; otherwise, <c>false</c>.</value>
		public virtual bool IsWinForms { get { return false; } }

		/// <summary>
		/// Gets a value indicating whether this platform is based on WPF
		/// </summary>
		/// <value><c>true</c> if this platform is wpf; otherwise, <c>false</c>.</value>
		public virtual bool IsWpf { get { return false; } }

		/// <summary>
		/// Gets a value indicating whether this platform is based on GTK# (2 or 3)
		/// </summary>
		/// <value><c>true</c> if this platform is gtk; otherwise, <c>false</c>.</value>
		public virtual bool IsGtk { get { return false; } }

		/// <summary>
		/// Gets a value indicating whether this platform is based on Xamarin.iOS
		/// </summary>
		/// <value><c>true</c> if this platform is ios; otherwise, <c>false</c>.</value>
		public virtual bool IsIos { get { return false; } }

		/// <summary>
		/// Gets a value indicating whether this platform is based on Xamarin.Android.
		/// </summary>
		/// <value><c>true</c> if this platform is android; otherwise, <c>false</c>.</value>
		public virtual bool IsAndroid { get { return false; } }

		/// <summary>
		/// Gets a value indicating whether this is a desktop platform. This includes Mac, Gtk, WinForms, Wpf, Direct2D.
		/// </summary>
		/// <remarks>
		/// A desktop platform is usually used via mouse &amp; keyboard, and can have complex user interface elements.
		/// </remarks>
		/// <value><c>true</c> if this is a desktop platform; otherwise, <c>false</c>.</value>
		public virtual bool IsDesktop { get { return false; } }

		/// <summary>
		/// Gets a value indicating whether this is a mobile platform. This includes iOS, Android, and WinRT.
		/// </summary>
		/// <remarks>
		/// A mobile platform is usually touch friendly, and have a simpler interface.
		/// </remarks>
		/// <value><c>true</c> if this instance is mobile; otherwise, <c>false</c>.</value>
		public virtual bool IsMobile { get { return false; } }

		/// <summary>
		/// Gets a value indicating that this platform is valid on the running device
		/// </summary>
		/// <remarks>
		/// This is used in platform detection to ensure that the correct platform is loaded.
		/// For example, the Mac platforms are only valid when run in an .app bundle.
		/// </remarks>
		/// <value><c>true</c> if this platform is valid and can be run; otherwise, <c>false</c>.</value>
		public virtual bool IsValid { get { return true; } }

		/// <summary>
		/// Initializes a new instance of the Platform class
		/// </summary>
		protected Platform()
		{
		}

		/// <summary>
		/// Gets a value indicating that the specified type is supported by this platform
		/// </summary>
		/// <typeparam name="T">Type of the handler or class with HandlerAttribute to test for.</typeparam>
		/// <returns>true if the specified type is supported, false otherwise</returns>
		public bool Supports<T>()
			where T: class
		{
			return Supports(typeof(T));
		}

		/// <summary>
		/// Gets a value indicating that the specified <paramref name="type"/> is supported by this platform
		/// </summary>
		/// <param name="type">Type of the handler or class with HandlerAttribute to test for.</param>
		/// <returns>true if the specified type is supported, false otherwise</returns>
		public virtual bool Supports(Type type)
		{
			return Find(type) != null;
		}

		/// <summary>
		/// Loads the extensions specified by the assembly attribute <see cref="PlatformExtensionAttribute"/>.
		/// </summary>
		/// <seealso cref="LoadAssembly(Assembly)"/>
		/// <param name="assemblyName">Name of the assembly to load the extensions for.</param>
		public void LoadAssembly(string assemblyName)
		{
			LoadAssembly(Assembly.Load(new AssemblyName(assemblyName)));
		}

		/// <summary>
		/// Loads the extensions specified by the assembly attribute <see cref="PlatformExtensionAttribute"/>.
		/// </summary>
		/// <remarks>
		/// This is useful for 3rd party libraries and when defining your own custom controls.
		/// 
		/// This method is called automatically on the same assembly of a custom control when its handler cannot be found.
		/// It will also be called for the same assembly with the prefix of the platform ID.
		/// 
		/// For example, if <c>MyControl</c> was declared in <c>MyControls.dll</c>, then Eto will automatically also
		/// load <c>MyControls.Wpf.dll</c> for the Wpf platform, and <c>MyControls.XamMac2.dll</c> for the XamMac2 platform, etc.
		/// 
		/// Use <see cref="ExportHandlerAttribute"/> and <see cref="ExportInitializerAttribute"/> to register
		/// handlers with the platform when the assembly is loaded or perform other logic.
		/// </remarks>
		/// <param name="assembly">Assembly to load the extensions for.</param>
		public void LoadAssembly(Assembly assembly)
		{
			RegisterAssembly(assembly);
			loadedAssemblies.Add(assembly);

			// also load associated platform assembly if one is available
			var an = new AssemblyName(assembly.FullName);
			an.Name += $".{ID}";

			try
			{
				var platformAssembly = Assembly.Load(an);
				if (platformAssembly != null)
				{
					RegisterAssembly(platformAssembly);
					loadedAssemblies.Add(platformAssembly);
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Error loading assembly {an}\n{ex}");
			}
		}

		void RegisterAssembly(Assembly assembly)
		{
			foreach (var extensionInfo in assembly.GetCustomAttributes<PlatformExtensionAttribute>())
			{
				if (extensionInfo.Supports(this))
					extensionInfo.Register(this);
			}
		}

		/// <summary>
		/// Gets the supported features of the platform.
		/// </summary>
		/// <value>The supported features.</value>
		public virtual PlatformFeatures SupportedFeatures
		{
			get { return PlatformFeatures.None; }
		}

		/// <summary>
		/// Gets the platform for the current thread
		/// </summary>
		/// <remarks>
		/// Typically you'd have only one platform active at a time, and this holds an instance to that value.
		/// The current platform is set automatically by the <see cref="Forms.Application"/> class
		/// when it is initially created.
		/// 
		/// This will return a different value for each thread, so if you have multiple platforms running
		/// (e.g. GTK + Mac for OS X), then this will allow for that.
		/// 
		/// This will be used when creating controls. To create controls on a different platform outside of its own thread, 
		/// use the <see cref="Context"/> property.
		/// </remarks>
		public static Platform Instance
		{
			get
			{
				return instance.Value;
			}
		}

		/// <summary>
		/// Returns the current generator, or detects the generator to use if no current generator is set.
		/// </summary>
		/// <remarks>
		/// This detects the platform to use based on the generator assemblies available and the current OS.
		/// 
		/// For windows, it will prefer WPF to Windows Forms.
		/// Mac OS X will prefer the Mac platform.
		/// Other unix-based platforms will prefer GTK.
		/// </remarks>
		public static Platform Detect
		{
			get
			{
				if (globalInstance != null)
					return globalInstance;

				var errors = new List<Exception>();
				Platform detected = null;
			
				if (EtoEnvironment.Platform.IsMac)
				{
					if (EtoEnvironment.Is64BitProcess)
						detected = Get(Platforms.Mac64, true, errors);
					if (detected == null)
						detected = Get(Platforms.XamMac2, true, errors);
					if (detected == null)
						detected = Get(Platforms.XamMac, true, errors);
					if (detected == null)
						detected = Get(Platforms.Mac, true, errors);
				}
				else if (EtoEnvironment.Platform.IsWindows)
				{
					detected = Get(Platforms.Wpf, true, errors);
					if (detected == null)
						detected = Get(Platforms.WinForms, true, errors);
				}

				if (detected == null && EtoEnvironment.Platform.IsUnix)
				{
					detected = Get(Platforms.Gtk, true, errors);
#pragma warning disable CS0618
					if (detected == null)
						detected = Get(Platforms.Gtk3, true, errors);
					if (detected == null)
						detected = Get(Platforms.Gtk2, true, errors);
#pragma warning restore CS0618
				}

				if (detected == null)
				{
					var message = "Could not detect platform. Are you missing a platform assembly?";
					if (errors.Count > 1)
						throw new AggregateException(message, errors);
					if (errors.Count == 1)
						throw new InvalidOperationException(message, errors[0]);
					throw new InvalidOperationException(message);
				}
					
				Initialize(detected);
				return globalInstance;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating that the platform should allow reinitialization.
		/// </summary>
		/// <remarks>
		/// When false, prevents incorrect logic that may create multiple instances of <see cref="Forms.Application"/>
		/// or <see cref="Platform"/> in the same thread which can causes issues where handlers are no longer registered, etc.
		/// 
		/// Multiple instances should still be able to be created in separate threads (for platforms that support it) when this is false.
		/// </remarks>
		public static bool AllowReinitialize { get; set; }

		/// <summary>
		/// Initializes the specified <paramref name="platform"/> as the current generator, for the current thread
		/// </summary>
		/// <remarks>
		/// This is called automatically by the <see cref="Forms.Application"/> when it is constructed
		/// </remarks>
		/// <param name="platform">Generator to set as the current generator</param>
		public static void Initialize(Platform platform)
		{
			if (!AllowReinitialize && instance.IsValueCreated && instance.Value != null && !ReferenceEquals(platform, instance.Value))
				throw new InvalidOperationException("The Eto.Forms Platform is already initialized.");

			if (globalInstance == null)
				globalInstance = platform;

			SetInstance(platform);
		}

		internal static void SetInstance(Platform platform)
		{
			instance.Value = platform;
		}

		/// <summary>
		/// Initialize the generator with the specified <paramref name="platformType"/> as the current generator
		/// </summary>
		/// <param name="platformType">Type of the generator to set as the current generator</param>
		public static void Initialize(string platformType)
		{
			Initialize(Get(platformType));
		}

		/// <summary>
		/// Gets the generator of the specified type
		/// </summary>
		/// <param name="generatorType">Type of the generator to get</param>
		/// <returns>An instance of a Generator of the specified type, or null if it cannot be loaded</returns>
		public static Platform Get(string generatorType)
		{
			return Get(generatorType, true, null);
		}

		internal static Platform Get(string platformType, bool allowNull, List<Exception> errors)
		{
			Type type = Type.GetType(platformType);
			if (type == null)
			{
				if (allowNull)
					return null;
				throw new InvalidOperationException($"Platform type '{platformType}' was not found. Are you missing the platform assembly?");
			}
			try
			{
				var platform = (Platform)Activator.CreateInstance(type);
				if (platform.IsValid)
					return platform;

				var message = $"Platform type '{platformType}' was loaded but is not valid in the current context.  E.g. Mac platforms require to be in an .app bundle to run";
				var error = new InvalidOperationException(message);
				errors?.Add(error);
				if (!allowNull)
					throw error;

				Debug.WriteLine(message);
				return null;
			}
			catch (Exception ex)
			{
				var message = $"Error creating instance of platform type '{platformType}'";
				var error = new InvalidOperationException(message, ex);
				errors?.Add(error);
				if (!allowNull)
					throw error;

				Debug.WriteLine(message);
				Debug.WriteLine(ex);
				return null;
			}
		}

		/// <summary>
		/// Add the <paramref name="instantiator"/> for the specified handler type of <typeparamref name="T"/>
		/// </summary>
		/// <param name="instantiator">Instantiator to create an instance of the handler</param>
		/// <typeparam name="T">The handler type to add the instantiator for (usually an interface derived from <see cref="Widget.IHandler"/>)</typeparam>
		public void Add<T>(Func<T> instantiator)
			where T: class
		{
			Add(typeof(T), instantiator);
		}

		/// <summary>
		/// Add the specified type and instantiator.
		/// </summary>
		/// <param name="type">Type of the handler (usually an interface derived from <see cref="Widget.IHandler"/>)</param>
		/// <param name="instantiator">Instantiator to create an instance of the handler</param>
		public void Add(Type type, Func<object> instantiator)
		{
			var handler = type.GetCustomAttribute<HandlerAttribute>(true);
			if (handler != null)
				instantiatorMap[handler.Type] = instantiator; // for backward compatibility, for now
			instantiatorMap[type] = instantiator;
			// clear handler map so it can pick up the new instantiator next time it is created
			handlerMap.Clear();
		}

		/// <summary>
		/// Find the delegate to create instances of the specified <paramref name="type"/>
		/// </summary>
		/// <param name="type">Type of the handler interface to get the instantiator for (usually derived from <see cref="Widget.IHandler"/> or another type)</param>
		public Func<object> Find(Type type)
		{
			if (type == null)
				throw new ArgumentNullException(nameof(type));

			Func<object> activator;
			if (instantiatorMap.TryGetValue(type, out activator))
				return activator;

			// the type is not mapped, try the type from the handler attribute
			var handler = type.GetCustomAttribute<HandlerAttribute>(true);
			if (handler != null && instantiatorMap.TryGetValue(handler.Type, out activator))
			{
				instantiatorMap.Add(type, activator);
				return activator;
			}

			// load the handler type assembly and try again (as type could be a derived class)
			var handlerAssembly = handler?.Type.GetAssembly();
			if (handlerAssembly != null && !loadedAssemblies.Contains(handlerAssembly))
			{
				LoadAssembly(handlerAssembly);
				// since we recurse here it will fall to the next one if this fails.
				return Find(type);
			}

			// finally, try the assembly of the current type if we still can't find it
			var typeAssembly = type.GetAssembly();
			if (!loadedAssemblies.Contains(typeAssembly))
			{
				LoadAssembly(typeAssembly);
				return Find(type);
			}

			return null;
		}

		/// <summary>
		/// Finds the delegate to create instances of the specified type
		/// </summary>
		/// <typeparam name="T">Type of the handler interface (usually derived from <see cref="Widget.IHandler"/> or another type)</typeparam>
		/// <returns>The delegate to use to create instances of the specified type</returns>
		public Func<T> Find<T>()
			where T: class
		{
			return (Func<T>)Find(typeof(T));
		}

		internal HandlerInfo FindHandler(Type type)
		{
			HandlerInfo info;
			if (handlerMap.TryGetValue(type, out info))
				return info;

			var handler = type.GetCustomAttribute<HandlerAttribute>(true);
			if (handler != null)
			{
				if (instantiatorMap.TryGetValue(handler.Type, out var activator))
				{
					var autoInit = handler.Type.GetCustomAttribute<AutoInitializeAttribute>(true);
					info = new HandlerInfo(autoInit == null || autoInit.Initialize, activator);
					handlerMap.Add(type, info);
					return info;
				}
				// load the assembly of the handler type (needed when type is a subclass)
				if (!loadedAssemblies.Contains(handler.Type.GetAssembly()))
				{
					LoadAssembly(handler.Type.GetAssembly());
					return FindHandler(type);
				}
			}

			// load the assembly of the target type (can be a subclass)
			if (!loadedAssemblies.Contains(type.GetAssembly()))
			{
				LoadAssembly(type.GetAssembly());
				return FindHandler(type);
			}
			return null;
		}


		/// <summary>
		/// Creates a new instance of the handler of the specified type of <typeparamref name="T"/>
		/// </summary>
		/// <remarks>
		/// This extension should be used when creating instances of a fixed type.
		/// </remarks>
		/// <typeparam name="T">Type of handler to create</typeparam>
		/// <returns>A new instance of a handler</returns>
		public T Create<T>()
		{
			return (T)Create(typeof(T));
		}

		/// <summary>
		/// Creates a new instance of the handler of the specified type
		/// </summary>
		/// <param name="type">Type of handler to create</param>
		/// <returns>A new instance of a handler</returns>
		public object Create(Type type)
		{
			try
			{
				var instantiator = Find(type);
				if (instantiator == null)
					throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Type {0} could not be found in this generator", type.FullName));

				var handler = instantiator();
				OnHandlerCreated(new HandlerCreatedEventArgs(handler));
				return handler;
			}
			catch (Exception e)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Could not create instance of type {0}", type), e);
			}
		}

		/// <summary>
		/// Creates a shared singleton instance of the specified type of <paramref name="type"/>
		/// </summary>
		/// <param name="type">The type of handler to get a shared instance for</param>
		/// <returns>The shared instance of a handler of the given type, or a new instance if not already created</returns>
		public object CreateShared(Type type)
		{
			object instance;
			lock (sharedInstances)
			{
				if (!sharedInstances.TryGetValue(type, out instance))
				{
					instance = Create(type);
					sharedInstances[type] = instance;
				}
			}
			return instance;
		}

		/// <summary>
		/// Creates a shared singleton instance of the specified type of <typeparamref name="T"/>
		/// </summary>
		/// <remarks>
		/// This extension should be used when creating shared instances of a fixed type.
		/// </remarks>
		/// <typeparam name="T">The type of handler to get a shared instance for</typeparam>
		/// <returns>The shared instance of a handler of the given type, or a new instance if not already created</returns>
		public T CreateShared<T>()
		{
			return (T)CreateShared(typeof(T));
		}

		/// <summary>
		/// Gets a shared cache dictionary
		/// </summary>
		/// <remarks>
		/// This is used to cache things like brushes and pens, but can also be used to cache other things for your
		/// application's use.
		/// </remarks>
		/// <param name="cacheKey">Unique cache key to load the cache instance</param>
		/// <typeparam name="TKey">The type of the lookup key</typeparam>
		/// <typeparam name="TValue">The type of the lookup value</typeparam>
		public Dictionary<TKey, TValue> Cache<TKey, TValue>(object cacheKey)
		{
			return GetSharedProperty<Dictionary<TKey, TValue>>(cacheKey, () => new Dictionary<TKey, TValue>());
		}


		/// <summary>
		/// Used at the start of your custom threads
		/// </summary>
		/// <returns></returns>
		public virtual IDisposable ThreadStart()
		{
			return null;
		}

		/// <summary>
		/// Gets an object to wrap in the platform's context, when using multiple platforms.
		/// </summary>
		/// <remarks>
		/// This sets this platform as current, and reverts back to the previous platform when disposed.
		/// 
		/// This value may be null.
		/// </remarks>
		/// <example>
		/// <code>
		///		using (platform.Context)
		///		{
		///			// do some stuff with the specified platform
		///		}
		/// </code>
		/// </example>
		public IDisposable Context
		{
			get
			{
				return instance.Value != this ? new PlatformContext(this) : null;
			}
		}

		Func<Matrix.IHandler> createMatrix;
		internal Func<Matrix.IHandler> CreateMatrix => createMatrix ?? (createMatrix = Find<Matrix.IHandler>());

		Func<GraphicsPath.IHandler> createGraphicsPath;
		internal Func<GraphicsPath.IHandler> CreateGraphicsPath => createGraphicsPath ?? (createGraphicsPath = Find<GraphicsPath.IHandler>());

		Pen.IHandler penHandler;
		internal Pen.IHandler PenHandler => penHandler ?? (penHandler = CreateShared<Pen.IHandler>());

		LinearGradientBrush.IHandler linearGradientBrushHandler;
		internal LinearGradientBrush.IHandler LinearGradientBrushHandler => linearGradientBrushHandler ?? (linearGradientBrushHandler = CreateShared<LinearGradientBrush.IHandler>());

		RadialGradientBrush.IHandler radialGradientBrushHandler;
		internal RadialGradientBrush.IHandler RadialGradientBrushHandler => radialGradientBrushHandler ?? (radialGradientBrushHandler = CreateShared<RadialGradientBrush.IHandler>());

		SolidBrush.IHandler solidBrushHandler;
		internal SolidBrush.IHandler SolidBrushHandler => solidBrushHandler ?? (solidBrushHandler = CreateShared<SolidBrush.IHandler>());

		TextureBrush.IHandler textureBrushHandler;
		internal TextureBrush.IHandler TextureBrushHandler => textureBrushHandler ?? (textureBrushHandler = CreateShared<TextureBrush.IHandler>());

		/// <summary>
		/// Invoke the specified action within the context of this platform
		/// </summary>
		/// <remarks>
		/// This is useful when you are using multiple platforms at the same time, and gives you an easy
		/// way to execute code within the context of this platform.
		/// </remarks>
		/// <param name="action">Action to execute.</param>
		public void Invoke(Action action)
		{
			using (Context)
			{
				action();
			}
		}

		/// <summary>
		/// Invoke the specified function within the context of this platform, returning its value.
		/// </summary>
		/// <remarks>
		/// This is useful when you are using multiple platforms at the same time, and gives you an easy
		/// way to execute code within the context of this platform, and return its value.
		/// </remarks>
		/// <example>
		/// <code>
		/// 	var mycontrol = MyPlatform.Invoke(() => new MyControl());
		/// </code>
		/// </example>
		/// <param name="action">Action to execute.</param>
		/// <typeparam name="T">The type of value to return.</typeparam>
		public T Invoke<T>(Func<T> action)
		{
			using (Context)
			{
				return action();
			}
		}
	}
}