using System;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;

namespace Eto
{
	public abstract class Generator
	{
		Dictionary<Type, ConstructorInfo> constructorMap;
		Hashtable attributes;
		
		public abstract string ID
		{
			get;
		}

		public IDictionary Attributes
		{
			get { if (attributes == null) attributes = new Hashtable(); return attributes; }
		}
		
		protected Generator()
		{
			constructorMap = new Dictionary<Type, ConstructorInfo>();
		}
		
		public virtual bool Supports<T>()
			where T: IWidget
		{
			return Find<T>() != null;
		}

		private static Generator current;
		
		public static Generator Current
		{
			get
			{
				if (current == null) throw new ApplicationException("Generator has not been initialized");
				return current;
			}
		}
		
		public static void Initialize(Generator generator)
		{
			current = generator;
		}
		
		public static Generator GetGenerator(string generatorType)
		{
			Type type = Type.GetType(generatorType);
			if (type.IsSubclassOf(typeof(Generator)))
			{
				return (Generator)Activator.CreateInstance(type);
			}
			return null;
			
		}

		public ConstructorInfo Add<T>(Type handlerType)
			where T: IWidget
		{
			ConstructorInfo constructor = handlerType.GetConstructor(new Type[] { });
			if (constructor == null) 
				throw new ArgumentException(string.Format("the default constructor for class {0} cannot be found", handlerType.FullName));

			constructorMap.Add(typeof(T), constructor);
			return constructor;
		}

		protected ConstructorInfo Find<T>()
			where T: IWidget
		{
			var type = typeof(T);
			if (constructorMap.ContainsKey(type)) return constructorMap[type];
			
			var types = this.GetType().Assembly.GetExportedTypes();
			foreach (Type foundType in types)
			{
				if (foundType.IsClass && !foundType.IsAbstract && type.IsAssignableFrom(foundType))
				{
					return Add<T>(foundType);
				}
			}
			return null;
		}
		
		public T CreateControl<T>()
			where T: IWidget
		{
			var constructor = Find<T>();
			if (constructor == null) throw new ApplicationException(string.Format("the type {0} cannot be found in this generator", typeof(T).FullName));

			return (T)constructor.Invoke(new object[] { });
		}
		
		public ConstructorInfo Add(Type type, Type handlerType)
		{
			ConstructorInfo constructor = handlerType.GetConstructor(new Type[] { });
			if (constructor == null) 
				throw new ArgumentException(string.Format("the default constructor for class {0} cannot be found", handlerType.FullName));

			constructorMap.Add(type, constructor);
			return constructor;
		}
		
		protected ConstructorInfo Find(Type type)
		{
			lock (this)
			{
			if (constructorMap.ContainsKey(type)) return constructorMap[type];
			
				
			var types = this.GetType().Assembly.GetExportedTypes();
			foreach (Type foundType in types)
			{
				if (foundType.IsClass && !foundType.IsAbstract && type.IsAssignableFrom(foundType))
				{
					return Add(type, foundType);
				}
			}
			return null;
			}
		}
		
		
		public object CreateControl(Type type)
		{
			var constructor = Find(type);
			if (constructor == null) throw new ApplicationException(string.Format("the type {0} cannot be found in this generator", type.FullName));

			return constructor.Invoke(new object[] { });
		}

		public static MethodInfo GetEventMethod(Type type, string methodName)
		{
			return GetEventMethod(type, methodName, typeof(EventArgs));
		}

		public static MethodInfo GetEventMethod(Type type, string methodName, params Type[] parameters)
		{
			return type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic, null, parameters, null);
		}
		
		public virtual void ExecuteOnMainThread(System.Action action)
		{
			action();
		}
		
		public virtual IDisposable ThreadStart()
		{
			return null;
		}
	}
}
