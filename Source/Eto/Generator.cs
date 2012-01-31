using System;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace Eto
{
	public class WidgetCreatedArgs : EventArgs
	{
		public IWidget Instance { get; private set; }
		
		public WidgetCreatedArgs (IWidget instance)
		{
			this.Instance = instance;
		}
	}
	
	public abstract class Generator
	{
		Dictionary<string, ConstructorInfo> constructorMap;
		Hashtable attributes;
		List<Type> types;
		
		public event EventHandler<WidgetCreatedArgs> WidgetCreated;
		
		protected virtual void OnWidgetCreated (WidgetCreatedArgs e)
		{
			if (WidgetCreated != null)
				WidgetCreated (this, e);
		}
		
		public abstract string ID {
			get;
		}

		public IDictionary Attributes {
			get {
				if (attributes == null)
					attributes = new Hashtable ();
				return attributes;
			}
		}
		
		protected Generator ()
		{
			constructorMap = new Dictionary<string, ConstructorInfo> ();
		}
		
		
		public virtual bool Supports<T> ()
			where T: IWidget
		{
			return Find<T> () != null;
		}

		private static Generator current;
		
		public static Generator Current {
			get {
				if (current == null)
					throw new ApplicationException ("Generator has not been initialized");
				return current;
			}
		}
		
		public static void Initialize (Generator generator)
		{
			current = generator;
		}
		
		public static Generator GetGenerator (string generatorType)
		{
			Type type = Type.GetType (generatorType);
			if (type == null) {
				throw new EtoException("Generator not found. Are you missing the platform assembly?");
			}
			if (type.IsSubclassOf (typeof(Generator))) {
				return (Generator)Activator.CreateInstance (type);
			}
			return null;
		}

		public ConstructorInfo Add<T> (Type handlerType)
			where T: IWidget
		{
			ConstructorInfo constructor = handlerType.GetConstructor (new Type[] { });
			if (constructor == null) 
				throw new ArgumentException (string.Format ("the default constructor for class {0} cannot be found", handlerType.FullName));

			constructorMap.Add (typeof(T).Name, constructor);
			return constructor;
		}

		protected ConstructorInfo Find<T> ()
			where T: IWidget
		{
			var type = typeof(T);
			return Find (type);
		}
		
		public T CreateControl<T> (Widget widget = null)
			where T: IWidget
		{
			var constructor = Find<T> ();
			if (constructor == null)
				throw new ApplicationException (string.Format ("the type {0} cannot be found in this generator", typeof(T).FullName));

			T val = (T)constructor.Invoke (new object[] { });
			if (widget != null) widget.Handler = val;
			OnWidgetCreated (new WidgetCreatedArgs (val as IWidget));
			return val;
		}
		
		public ConstructorInfo Add (Type type, Type handlerType)
		{
			ConstructorInfo constructor = handlerType.GetConstructor (new Type[] { });
			if (constructor == null) 
				throw new ArgumentException (string.Format ("the default constructor for class {0} cannot be found", handlerType.FullName));

			constructorMap.Add (type.Name, constructor);
			return constructor;
		}
		
		protected ConstructorInfo Find (Type type)
		{
			lock (this) {
				ConstructorInfo info;
				if (constructorMap.TryGetValue (type.Name, out info))
					return info;

				List<Type > removalTypes = null;
				if (types == null)
					types = new List<Type> (this.GetType ().Assembly.GetExportedTypes ());
				
				foreach (Type foundType in types) {
					try {
						if (foundType.IsClass && !foundType.IsAbstract && type.IsAssignableFrom (foundType)) {
							if (removalTypes != null)
								foreach (var t in removalTypes)
									types.Remove (t);
							return Add (type, foundType);
						}
					} catch {
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
		
		public IWidget CreateControl (Type type, Widget widget)
		{
			var constructor = Find (type);
			if (constructor == null)
				throw new ApplicationException (string.Format ("the type {0} cannot be found in this generator", type.FullName));
			try {
				var val = constructor.Invoke (new object[] { }) as IWidget;
				if (widget != null) {
					widget.Handler = val;
					val.Handler = widget;
				}
				OnWidgetCreated (new WidgetCreatedArgs (val));
				return val;
			}
			catch (Exception e) {
				throw new EtoException (string.Format("Could not create instance of type {0}", type), e);
			}
		}

		public static MethodInfo GetEventMethod (Type type, string methodName)
		{
			return GetEventMethod (type, methodName, typeof(EventArgs));
		}

		public static MethodInfo GetEventMethod (Type type, string methodName, params Type[] parameters)
		{
			return type.GetMethod (methodName, BindingFlags.Instance | BindingFlags.NonPublic, null, parameters, null);
		}
		
		[Obsolete("Use Application.InvokeOnMainThread")]
		public void ExecuteOnMainThread (System.Action action)
		{
			Forms.Application.Instance.InvokeOnMainThread (action);
		}

		public virtual IDisposable ThreadStart ()
		{
			return null;
		}
	}
}
