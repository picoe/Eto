using System;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Threading;

namespace Eto
{
	/// <summary>
	/// Arguments for when a widget is created
	/// </summary>
	/// <copyright>(c) 2012 by Curtis Wensley</copyright>
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
		public WidgetCreatedArgs (object instance)
		{
			this.Instance = instance;
		}
	}

	/// <summary>
	/// Extensions for the <see cref="Generator"/> class
	/// </summary>
	/// <copyright>(c) 2012 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public static class GeneratorExtensions
	{
		/// <summary>
		/// Creates a new instance of the handler of the specified type of <typeparamref name="T"/>
		/// </summary>
		/// <remarks>
		/// This extension should be used when creating instances of a fixed type.
		/// This is a helper so that you can use a null generator variable to create instances with the current
		/// generator without having to do the extra check
		/// </remarks>
		/// <typeparam name="T">Type of handler to create</typeparam>
		/// <param name="generator">Generator to create the instance, or null to use the current generator</param>
		/// <returns>A new instance of a handler</returns>
		public static T Create<T> (this Generator generator)
		{
			return (T)(generator ?? Generator.Current).Create (typeof (T));
		}

		/// <summary>
		/// Creates a shared singleton instance of the specified type of <typeparamref name="T"/>
		/// </summary>
		/// <remarks>
		/// This extension should be used when creating shared instances of a fixed type.
		/// This is a helper so that you can use a null generator variable to create instances with the current
		/// generator without having to do the extra check
		/// </remarks>
		/// <param name="generator">Generator to create or get the shared instance, or null to use the current generator</param>
		/// <typeparam name="T">The type of handler to get a shared instance for</typeparam>
		/// <returns>The shared instance of a handler of the given type, or a new instance if not already created</returns>
		public static T CreateShared<T> (this Generator generator)
		{
			return (T)(generator ?? Generator.Current).CreateShared (typeof(T));
		}

		/// <summary>
		/// Finds the delegate to create instances of the specified type
		/// </summary>
		/// <typeparam name="T">Type of the handler interface (usually derived from <see cref="IWidget"/> or another type)</typeparam>
		/// <returns>The delegate to use to create instances of the specified type</returns>
		public static Func<T> Find<T> (this Generator generator)
			where T: class
		{
			return (Func<T>)(generator ?? Generator.Current).Find (typeof(T));
		}

		internal static Dictionary<K, V> Cache<K, V> (this Generator generator, object cacheKey)
		{
			return (generator ?? Generator.Current).GetSharedProperty <Dictionary<K, V>> (cacheKey, () => new Dictionary<K, V> ());
		}
	}
	
	/// <summary>
	/// Base generator class for each platform
	/// </summary>
	/// <remarks>
	/// The generator takes care of creating the platform-specific implementations of each
	/// control. Typically, the types are automatically found from the platform assembly, however
	/// you can also create your own platform-specific controls by adding the types manually via
	/// <see cref="Generator.Add"/>
	/// 
	/// The types are found by the interface of the control.  For example the <see cref="Forms.Label"/> control
	/// uses the <see cref="Forms.ILabel"/> interface for its platform implementation.  The generator
	/// will automatically scan an assembly for a class that directly implements this interface
	/// for its platform implementation (if it hasn't been added manually).
	/// </remarks>
	/// <copyright>(c) 2012 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public abstract class Generator
	{
		Dictionary<Type, Func<object>> instantiatorMap = new Dictionary<Type, Func<object>> ();
		Dictionary<Type, object> sharedInstances = new Dictionary<Type, object> ();
		Dictionary<object, object> properties = new Dictionary<object, object> ();
		static Generator current;

		internal T GetSharedProperty<T> (object key, Func<T> instantiator)
		{
			object value;
			lock (properties) {
				if (!properties.TryGetValue (key, out value)) {
					value = instantiator ();
					properties [key] = value;
				}
			}
			return (T)value;
		}

		#region Events

		/// <summary>
		/// Event to handle when widgets are created by this generator
		/// </summary>
		public event EventHandler<WidgetCreatedArgs> WidgetCreated;
		
		/// <summary>
		/// Handles the <see cref="WidgetCreated"/> event
		/// </summary>
		/// <param name="e">Arguments for the event</param>
		protected virtual void OnWidgetCreated (WidgetCreatedArgs e)
		{
			if (WidgetCreated != null)
				WidgetCreated (this, e);
		}

		#endregion

		/// <summary>
		/// Gets the ID of this generator
		/// </summary>
		/// <remarks>
		/// The generator ID can be used to determine which generator is currently in use.  The generator
		/// does not necessarily correspond to the OS that it is running on, as for example the GTK platform
		/// can run on OS X and Windows.
		/// </remarks>
		public abstract string ID { get; }

		/// <summary>
		/// Initializes a new instance of the Generator class
		/// </summary>
		protected Generator ()
		{
		}

		/// <summary>
		/// Gets a value indicating that the specified type is supported by this generator
		/// </summary>
		/// <typeparam name="T">type to test for</typeparam>
		/// <returns>true if the specified type is supported, false otherwise</returns>
		public virtual bool Supports<T> ()
			where T: class
		{
			return this.Find<T> () != null;
		}

		/// <summary>
		/// Gets the current generator
		/// </summary>
		/// <remarks>
		/// Typically you'd have only one platform generator active at a time, and this holds an instance
		/// to that value.  The current generator is set automatically by the <see cref="Forms.Application"/> class
		/// when it is initially created.
		/// 
		/// This will be used when creating controls, unless explicitly passed through the constructor of the
		/// control. This allows you to use multiple generators at one time.
		/// </remarks>
		public static Generator Current
		{
			get
			{
                if (current == null)
                    throw new ApplicationException("Generator has not been initialized");
                return current;
            }
        }

        public static void Initialize(string assembly)
        {
            Initialize(GetGenerator(assembly));
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
		public static Generator Detect
		{
			get
			{
				if (current != null)
					return current;
#if MOBILE
				Initialize(Generator.GetGenerator (Generators.IosAssembly, true));
#elif DESKTOP
			
				if (EtoEnvironment.Platform.IsMac)
					Initialize(Generator.GetGenerator (Generators.MacAssembly, true));
				else if (EtoEnvironment.Platform.IsWindows) {
					Initialize(Generator.GetGenerator (Generators.WpfAssembly, true));
					if (current == null)
						Initialize(Generator.GetGenerator (Generators.WinAssembly, true));
				}

				if (current == null && EtoEnvironment.Platform.IsUnix)
					Initialize(Generator.GetGenerator (Generators.GtkAssembly, true));
#endif
				
				if (current == null)
					throw new EtoException ("Could not detect platform. Are you missing a platform assembly?");
					
				return current;
			}
		}

        /// <summary>
        /// Can be used by apps that switch between generators.
        /// 
        /// Set this property at the start of a block of code.
        /// All objects created after that point are verified to
        /// use this generator.
        /// 
        /// If null, no validation is performed.
        /// </summary>
        public static Generator ValidateGenerator { get; set; }

        /// <summary>
        /// Called by handlers to make sure they use the generator
        /// specified by ValidateGenerator
        /// </summary>
        /// <param name="generator"></param>
        public static void Validate(Generator generator)
        {
            if (ValidateGenerator != null &&
                !object.ReferenceEquals(
                generator,
                ValidateGenerator))
            {
                throw new EtoException(
                    string.Format("Expected to use generator {0}", ValidateGenerator));
            }
        }

		/// <summary>
		/// Initializes this generator as the current generator
		/// </summary>
		/// <remarks>
		/// This is called automatically by the <see cref="Forms.Application"/> when it is constructed
		/// </remarks>
		/// <param name="generator">Generator to set as the current generator</param>
		public static void Initialize (Generator generator)
		{
			current = generator;
		}
		
		/// <summary>
		/// Gets the generator of the specified type
		/// </summary>
		/// <param name="generatorType">Type of the generator to get</param>
		/// <returns>An instance of a Generator of the specified type</returns>
		public static Generator GetGenerator (string generatorType)
		{
			return GetGenerator (generatorType, false);
		}

        // A static dictionary of generator instances.
        static Dictionary<Type, Generator> GeneratorInstances = 
            new Dictionary<Type, Generator>();

		static Generator GetGenerator (string generatorType, bool allowNull)
		{
			Type type = Type.GetType (generatorType);
			if (type == null) {
				if (allowNull) 
					return null;
				else
					throw new EtoException ("Generator not found. Are you missing the platform assembly?");
			}
			try {
                Generator result = null;

                if (!GeneratorInstances.TryGetValue(type, out result) ||
                    result == null)
                {
                    result = (Generator)Activator.CreateInstance(type);
                    GeneratorInstances[type] = result;
                }

                return result;
				} catch (TargetInvocationException e) {
				throw e.InnerException;
			}
		}

		/// <summary>
		/// Add the <paramref name="instantiator"/> for the specified handler type of <typeparamref name="T"/>
		/// </summary>
		/// <param name="instantiator">Instantiator to create an instance of the handler</param>
		/// <typeparam name="T">The handler type to add the instantiator for (usually an interface derived from <see cref="IWidget"/>)</typeparam>
		public void Add<T> (Func<T> instantiator)
			where T: class
		{
			Add (typeof(T), instantiator);
		}
		
		/// <summary>
		/// Add the specified type and instantiator.
		/// </summary>
		/// <param name="type">Type of the handler (usually an interface derived from <see cref="IWidget"/>)</param>
		/// <param name="instantiator">Instantiator to create an instance of the handler</param>
		public void Add (Type type, Func<object> instantiator)
		{
			instantiatorMap [type] = instantiator;
		}
		
		/// <summary>
		/// Find the delegate to create instances of the specified <paramref name="type"/>
		/// </summary>
		/// <param name="type">Type of the handler interface to get the instantiator for (usually derived from <see cref="IWidget"/> or another type)</param>
		public Func<object> Find (Type type)
		{
			Func<object> activator;
			if (instantiatorMap.TryGetValue (type, out activator))
				return activator;
			else
				return null;
		}

		/// <summary>
		/// Creates a new instance of the handler of the specified type
		/// </summary>
		/// <param name="type">Type of handler to create</param>
		/// <returns>A new instance of a handler</returns>
		public object Create (Type type)
		{
			try {
				var instantiator = Find (type);
				if (instantiator == null)
					throw new HandlerInvalidException (string.Format ("type {0} could not be found in this generator", type.FullName));

				var handler = instantiator ();
				OnWidgetCreated (new WidgetCreatedArgs (handler));
				return handler;
			} catch (Exception e) {
				throw new HandlerInvalidException (string.Format ("Could not create instance of type {0}", type), e);
			}
		}

		/// <summary>
		/// Creates a shared singleton instance of the specified type of <paramref name="type"/>
		/// </summary>
		/// <param name="type">The type of handler to get a shared instance for</param>
		/// <returns>The shared instance of a handler of the given type, or a new instance if not already created</returns>
		public object CreateShared (Type type)
		{
			object instance;
			lock (sharedInstances) {
				if (!sharedInstances.TryGetValue (type, out instance)) {
					instance = Create (type);
					var widget = instance as IWidget;
					if (widget != null) {
						widget.Generator = this;
					}
					sharedInstances[type] = instance;
				}
			}
			return instance;
		}

		/// <summary>
		/// Executes the specified action on the main thread
		/// </summary>
		/// <param name="action">Action to invoke</param>
		[Obsolete("Use Application.InvokeOnMainThread")]
		public void ExecuteOnMainThread (System.Action action)
		{
			Forms.Application.Instance.Invoke (action);
		}

		/// <summary>
		/// Used at the start of your custom threads
		/// </summary>
		/// <returns></returns>
		public virtual IDisposable ThreadStart ()
		{
			return null;
		}
	}
}
