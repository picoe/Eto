using System;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace Eto
{
	/// <summary>
	/// Arguments for when a widget is created
	/// </summary>
	public class WidgetCreatedArgs : EventArgs
	{
		/// <summary>
		/// Gets the instance of the widget that was created
		/// </summary>
		public IWidget Instance { get; private set; }
		
		/// <summary>
		/// Initializes a new instance of the WidgetCreatedArgs class
		/// </summary>
		/// <param name="instance">Instance of the widget that was created</param>
		public WidgetCreatedArgs (IWidget instance)
		{
			this.Instance = instance;
		}
	}
	
	/// <summary>
	/// Base generator class for each platform
	/// </summary>
	/// <remarks>
	/// The generator takes care of creating the platform-specific implementations of each
	/// control. Typically, the types are automatically found from the platform assembly, however
	/// you can also create your own platform-specific controls by adding the types manually via
	/// <see cref="Generator.Add"/>, or <see cref="Generator.AddAssembly"/>.
	/// 
	/// The types are found by the interface of the control.  For example the <see cref="Forms.Label"/> control
	/// uses the <see cref="Forms.ILabel"/> interface for its platform implementation.  The generator
	/// will automatically scan an assembly for a class that directly implements this interface
	/// for its platform implementation (if it hasn't been added manually).
	/// </remarks>
	public abstract class Generator
	{
		Dictionary<string, ConstructorInfo> constructorMap = new Dictionary<string, ConstructorInfo> ();
		List<Type> types = new List<Type>();
		HashSet<Assembly> typeAssemblies = new HashSet<Assembly>();

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
			AddAssembly(this.GetType ().Assembly);
		}

		/// <summary>
		/// Gets a value indicating that the specified type is supported by this generator
		/// </summary>
		/// <typeparam name="T">type to test for</typeparam>
		/// <returns>true if the specified type is supported, false otherwise</returns>
		public virtual bool Supports<T> ()
			where T: IWidget
		{
			return Find<T> () != null;
		}

		static Generator current;

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
		public static Generator Current {
			get {
				if (current == null)
					throw new ApplicationException ("Generator has not been initialized");
				return current;
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
		public static Generator Detect {
			get {
				if (current != null)
					return current;
#if MOBILE
				current = Generator.GetGenerator (Generators.IosAssembly, true);
#elif DESKTOP
			
				if (EtoEnvironment.Platform.IsMac)
					current = Generator.GetGenerator (Generators.MacAssembly, true);
				else if (EtoEnvironment.Platform.IsWindows) {
					current = Generator.GetGenerator (Generators.WpfAssembly, true);
					if (current == null)
						current = Generator.GetGenerator (Generators.WinAssembly, true);
				}

				if (current == null && EtoEnvironment.Platform.IsUnix)
					current = Generator.GetGenerator (Generators.GtkAssembly, true);
#endif
				
				if (current == null)
					throw new EtoException ("Could not detect platform. Are you missing a platform assembly?");
					
				return current;
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


		static Generator GetGenerator (string generatorType, bool allowNull)
		{
			Type type = Type.GetType (generatorType);
			if (type == null) {
				if (allowNull) 
					return null;
				else
					throw new EtoException ("Generator not found. Are you missing the platform assembly?");
			}
			try
			{
				return (Generator)Activator.CreateInstance(type);
			}
			catch (TargetInvocationException e)
			{
				throw e.InnerException;
			}
		}

		/// <summary>
		/// Adds the specified handler type to this generator
		/// </summary>
		/// <remarks>
		/// This can be used to add a single handler to this generator.  Typically you would do this
		/// before running your application.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// var generator = Generator.Detect;
		///	generator.Add<IMyControl>(typeof(MyControlHandler));
		/// ]]></code>
		/// </example>
		/// <typeparam name="T">Type of the handler interface (derived from <see cref="IWidget"/> or another type)</typeparam>
		/// <param name="handlerType">Type of the backend handler type that implements the interface</param>
		/// <returns>An instance of the constructor info used to create instances of this type</returns>
		public ConstructorInfo Add<T> (Type handlerType)
			where T: IWidget
		{
			return Add (typeof(T), handlerType);
		}
		
		/// <summary>
		/// Finds the constructor info for the specified type
		/// </summary>
		/// <typeparam name="T">Type of the handler interface (derived from <see cref="IWidget"/> or another type)</typeparam>
		/// <returns>An instance of the constructor info used to create instances of this type</returns>
		protected ConstructorInfo Find<T> ()
			where T: IWidget
		{
			return Find (typeof(T));
		}
		
		/// <summary>
		/// Creates a new instance of the handler of the specified type
		/// </summary>
		/// <typeparam name="T">Type of handler to create</typeparam>
		/// <param name="widget">Widget instance to attach to the handler</param>
		/// <returns>A new instance of a handler</returns>
		public T CreateHandler<T> (Widget widget = null)
			where T: IWidget
		{
			return (T)CreateHandler (typeof (T), widget);
		}

		/// <summary>
		/// Adds the specified handler type to this generator
		/// </summary>
		/// <remarks>
		/// This can be used to add a single handler to this generator.  Typically you would do this
		/// before running your application.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// var generator = Generator.Detect;
		///	generator.Add<IMyControl>(typeof(MyControlHandler));
		/// ]]></code></example>
		/// <param name="type">Type of the handler interface (derived from <see cref="IWidget"/> or another type)</param>
		/// <param name="handlerType">Type of the backend handler type that implements the interface</param>
		/// <returns>An instance of the constructor info used to create instances of this type</returns>
		public ConstructorInfo Add (Type type, Type handlerType)
		{
			ConstructorInfo constructor = handlerType.GetConstructor (new Type[] { });
			if (constructor == null) 
				throw new ArgumentException (string.Format ("the default constructor for class {0} cannot be found", handlerType.FullName));

			constructorMap[type.Name] = constructor;
			return constructor;
		}

		/// <summary>
		/// Adds the specified assembly to scan for handler impelementations
		/// </summary>
		/// <remarks>
		/// If you create your own controls with custom handlers, you can use this method
		/// to add the assembly to the list of assemblies that will be scanned for the handler
		/// implementations.
		/// </remarks>
		/// <param name="assembly">Assembly with handler implementations to add</param>
		public void AddAssembly (Assembly assembly)
		{
			if (!typeAssemblies.Contains (assembly))
			{
				typeAssemblies.Add (assembly);
				IEnumerable<Type> exportedTypes;
				try
				{
					exportedTypes = assembly.GetTypes ();
				}
				catch (ReflectionTypeLoadException ex)
				{
					Debug.WriteLine (string.Format ("Could not load type(s) from assembly '{0}': {1}", assembly.FullName, ex.GetBaseException ()));
					exportedTypes = ex.Types;
				}

				exportedTypes = exportedTypes.Where (r => typeof (IWidget).IsAssignableFrom (r) && r.IsClass && !r.IsAbstract);
				types.InsertRange (0, exportedTypes);
			}
		}

		ConstructorInfo Find (Type type)
		{
			lock (this) {
				ConstructorInfo info;
				if (constructorMap.TryGetValue (type.Name, out info))
					return info;

				List<Type > removalTypes = null;
				foreach (Type foundType in types) {
					try {
						if (foundType.IsClass && !foundType.IsAbstract && type.IsAssignableFrom (foundType)) {
							if (removalTypes != null)
								foreach (var t in removalTypes)
									types.Remove (t);
							return Add (type, foundType);
						}
					} catch (Exception e) {
						Debug.WriteLine (string.Format ("Could not instantiate type '{0}'\n{1}", type, e));
						if (removalTypes == null)
							removalTypes = new List<Type> ();
						removalTypes.Add (foundType);
					}
				}
				if (removalTypes != null)
					foreach (var t in removalTypes)
						types.Remove (t);
				return null;
			}
		}

		/// <summary>
		/// Creates a new instance of the handler of the specified type
		/// </summary>
		/// <param name="type">Type of handler to create</param>
		/// <param name="widget">Widget instance to attach to the handler</param>
		/// <returns>A new instance of a handler</returns>
		public IWidget CreateHandler (Type type, Widget widget)
		{
			var constructor = Find (type);
			if (constructor == null)
				throw new HandlerInvalidException (string.Format ("type {0} could not be found in this generator", type.FullName));
			try {
				var val = constructor.Invoke (new object[] { }) as IWidget;
				if (widget != null) {
					widget.Handler = val;
					val.Widget = widget;
				}
				OnWidgetCreated (new WidgetCreatedArgs (val));
				return val;
			} catch (Exception e) {
				throw new HandlerInvalidException (string.Format ("Could not create instance of type {0}", type), e);
			}
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
