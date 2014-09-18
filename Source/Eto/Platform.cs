using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading;

namespace Eto
{
	/// <summary>
	/// Arguments for when a widget is created
	/// </summary>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class WidgetCreatedArgs : EventArgs
	{
		/// <summary>
		/// Gets the instance of the widget that was created
		/// </summary>
		public object Instance { get; private set; }

		/// <summary>
		/// Initializes a new instance of the WidgetCreatedArgs class
		/// </summary>
		/// <param name="instance">Instance of the widget that was created</param>
		public WidgetCreatedArgs(object instance)
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
	/// Base platform class
	/// </summary>
	/// <remarks>
	/// This class takes care of creating the platform-specific implementations of each control.
	/// All controls the platform can handle should be added using <see cref="Platform.Add{T}"/>.
	/// </remarks>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
#pragma warning disable 0612,0618
	public abstract class Platform : Generator
#pragma warning restore 0612,0618
	{
		readonly Dictionary<Type, Func<object>> instantiatorMap = new Dictionary<Type, Func<object>>();
		readonly Dictionary<Type, HandlerInfo> handlerMap = new Dictionary<Type, HandlerInfo>();
		readonly Dictionary<Type, object> sharedInstances = new Dictionary<Type, object>();
		readonly Dictionary<object, object> properties = new Dictionary<object, object>();
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
		public event EventHandler<WidgetCreatedArgs> WidgetCreated;

		/// <summary>
		/// Handles the <see cref="WidgetCreated"/> event
		/// </summary>
		/// <param name="e">Arguments for the event</param>
		protected virtual void OnWidgetCreated(WidgetCreatedArgs e)
		{
			if (WidgetCreated != null)
				WidgetCreated(this, e);
		}

		#endregion

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
				//if (current == null)
				//	throw new EtoException("Generator has not been initialized");
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
		public new static Platform Detect
		{
			get
			{
				if (globalInstance != null)
					return globalInstance;

				Platform detected = null;
			
				if (EtoEnvironment.Platform.IsMac)
				{
					if (EtoEnvironment.Is64BitProcess)
						detected = Platform.Get(Platforms.Mac64, true);
					if (detected == null)
						detected = Platform.Get(Platforms.XamMac, true);
					if (detected == null)
						detected = Platform.Get(Platforms.Mac, true);
				}
				else if (EtoEnvironment.Platform.IsWindows)
				{
					detected = Platform.Get(Platforms.Wpf, true);
					if (detected == null)
						detected = Platform.Get(Platforms.WinForms, true);
				}

				if (detected == null && EtoEnvironment.Platform.IsUnix)
					detected = Platform.Get(Platforms.Gtk2, true);
				
				if (detected == null)
					throw new EtoException("Could not detect platform. Are you missing a platform assembly?");
					
				Initialize(detected);
				return globalInstance;
			}
		}

		/// <summary>
		/// Initializes the specified <paramref name="platform"/> as the current generator, for the current thread
		/// </summary>
		/// <remarks>
		/// This is called automatically by the <see cref="Forms.Application"/> when it is constructed
		/// </remarks>
		/// <param name="platform">Generator to set as the current generator</param>
		public static void Initialize(Platform platform)
		{
			if (globalInstance == null)
				globalInstance = platform;
			instance.Value = platform;
		}

		/// <summary>
		/// Initialize the generator with the specified <paramref name="generatorType"/> as the current generator
		/// </summary>
		/// <param name="generatorType">Type of the generator to set as the current generator</param>
		public static new void Initialize(string generatorType)
		{
			Initialize(Get(generatorType));
		}

		/// <summary>
		/// Gets the generator of the specified type
		/// </summary>
		/// <param name="generatorType">Type of the generator to get</param>
		/// <returns>An instance of a Generator of the specified type, or null if it cannot be loaded</returns>
		public static Platform Get(string generatorType)
		{
			return Get(generatorType, true);
		}

		internal static Platform Get(string generatorType, bool allowNull)
		{
			Type type = Type.GetType(generatorType);
			if (type == null)
			{
				if (allowNull)
					return null;
				throw new EtoException("Generator not found. Are you missing the platform assembly?");
			}
			try
			{
				return (Platform)Activator.CreateInstance(type);
			}
			catch (Exception ex)
			{
				Debug.WriteLine(string.Format("Error creating instance of generator '{0}'\n{1}", generatorType, ex));
				if (allowNull)
					return null;
				throw;
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
		}

		/// <summary>
		/// Find the delegate to create instances of the specified <paramref name="type"/>
		/// </summary>
		/// <param name="type">Type of the handler interface to get the instantiator for (usually derived from <see cref="Widget.IHandler"/> or another type)</param>
		public Func<object> Find(Type type)
		{
			Func<object> activator;
			if (instantiatorMap.TryGetValue(type, out activator))
				return activator;

			var handler = type.GetCustomAttribute<HandlerAttribute>(true);
			if (handler != null && instantiatorMap.TryGetValue(handler.Type, out activator))
			{
				instantiatorMap.Add(type, activator);
				return activator;
			}
			return null;
		}

		internal HandlerInfo FindHandler(Type type)
		{
			HandlerInfo info;
			if (handlerMap.TryGetValue(type, out info))
				return info;

			var handler = type.GetCustomAttribute<HandlerAttribute>(true);
			Func<object> activator;
			if (handler != null && instantiatorMap.TryGetValue(handler.Type, out activator))
			{
				var autoInit = handler.Type.GetCustomAttribute<AutoInitializeAttribute>(true);
				info = new HandlerInfo(autoInit == null || autoInit.Initialize, activator);
				handlerMap.Add(type, info);
				return info;
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
					throw new HandlerInvalidException(string.Format(CultureInfo.CurrentCulture, "type {0} could not be found in this generator", type.FullName));

				var handler = instantiator();
				OnWidgetCreated(new WidgetCreatedArgs(handler));
				return handler;
			}
			catch (Exception e)
			{
				throw new HandlerInvalidException(string.Format(CultureInfo.CurrentCulture, "Could not create instance of type {0}", type), e);
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
