namespace Eto.Mac
{
	public static class ObjCExtensions
	{
		[DllImport("/usr/lib/libobjc.dylib")]
		static extern IntPtr class_getInstanceMethod(IntPtr cls, IntPtr sel);

		public static IntPtr GetInstanceMethod(this Class cls, IntPtr selector) => class_getInstanceMethod(cls.Handle, selector);

		public static IntPtr GetInstanceMethod(IntPtr cls, IntPtr selector) => class_getInstanceMethod(cls, selector);

		[DllImport("/usr/lib/libobjc.dylib")]
		static extern bool class_addMethod(IntPtr cls, IntPtr sel, Delegate method, string argTypes);

		public static bool AddMethod(this Class cls, IntPtr selector, Delegate method, string arguments)
		{
			return class_addMethod(cls.Handle, selector, method, arguments);
		}

		public static bool AddMethod(IntPtr classHandle, IntPtr selector, Delegate method, string arguments)
		{
			return class_addMethod(classHandle, selector, method, arguments);
		}

		[DllImport("/usr/lib/libobjc.dylib")]
		static extern bool method_exchangeImplementations(IntPtr method1, IntPtr method2);

		public static void ExchangeMethod(this Class cls, IntPtr selMethod1, IntPtr selMethod2)
		{
			var method1 = class_getInstanceMethod(cls.Handle, selMethod1);
			var method2 = GetInstanceMethod(cls, selMethod2);
			method_exchangeImplementations(method1, method2);
		}

		[DllImport("/usr/lib/libobjc.dylib")]
		static extern IntPtr object_getClass(IntPtr obj);


		[DllImport("/usr/lib/libobjc.dylib")]
		static extern IntPtr class_getSuperclass(IntPtr obj);

		[DllImport("/usr/lib/libobjc.dylib")]
		static extern IntPtr objc_getMetaClass(string metaClassName);

		public static Class GetMetaClass(string metaClassName) => new Class(objc_getMetaClass(metaClassName));

		static readonly IntPtr selInstancesRespondToSelector = Selector.GetHandle("instancesRespondToSelector:");
		static readonly IntPtr selRespondsToSelector = Selector.GetHandle("respondsToSelector:");

		public static bool ClassInstancesRespondToSelector(IntPtr cls, IntPtr selector)
		{
			if (cls == IntPtr.Zero)
				return false;
			return Messaging.bool_objc_msgSend_IntPtr(cls, selInstancesRespondToSelector, selector);
		}

		public static bool InstancesRespondToSelector<T>(IntPtr selector)
		{
			var cls = Class.GetHandle(typeof(T));
			return ClassInstancesRespondToSelector(cls, selector);
		}

		public static bool InstancesRespondToSelector<T>(string selector) => InstancesRespondToSelector<T>(Selector.GetHandle(selector));

		public static bool InstancesRespondToSelector(this Class cls, Selector selector) => ClassInstancesRespondToSelector(cls.Handle, selector.Handle);

		public static bool ClassRespondsToSelector(IntPtr cls, IntPtr selector)
		{
			return Messaging.bool_objc_msgSend_IntPtr(cls, selRespondsToSelector, selector);
		}

		public static bool RespondsToSelector<T>(IntPtr selector)
		{
			var cls = Class.GetHandle(typeof(T));
			return ClassRespondsToSelector(cls, selector);
		}

		public static bool RespondsToSelector<T>(string selector) => RespondsToSelector<T>(Selector.GetHandle(selector));

		public static bool RespondsToSelector(this Class cls, Selector selector) => ClassRespondsToSelector(cls.Handle, selector.Handle);
		
		
		[DllImport("/usr/lib/libobjc.dylib")]
		static extern bool class_addProtocol(IntPtr cls, IntPtr protocol);
		
		[DllImport("/usr/lib/libobjc.dylib")]
		static extern bool class_conformsToProtocol(IntPtr cls, IntPtr protocol);
		
		[DllImport("/usr/lib/libobjc.dylib")]
		static extern IntPtr objc_getProtocol(string name);
		
		public static bool ClassAddProtocol(IntPtr cls, IntPtr protocol) => class_addProtocol(cls, protocol);
		public static bool AddProtocol(this Class cls, IntPtr protocol) => class_addProtocol(cls.Handle, protocol);
		
		public static IntPtr GetProtocolHandle(string name) => objc_getProtocol(name);
		
		public static bool ClassConformsToProtocol(IntPtr cls, IntPtr protocol) => class_conformsToProtocol(cls, protocol);
		public static bool ConformsToProtocol(this Class cls, IntPtr protocol) => class_conformsToProtocol(cls.Handle, protocol);


		public static IntPtr GetClass(this NSObject obj)
		{
			if (obj == null)
				return IntPtr.Zero;
			return Messaging.IntPtr_objc_msgSend(obj.Handle, Forms.MacView.selClass_Handle);
		}
		
		public static IntPtr GetClass(IntPtr obj) => Messaging.IntPtr_objc_msgSend(obj, Forms.MacView.selClass_Handle);
		public static IntPtr GetSuperclass(this NSObject obj)
		{
			if (obj == null)
				return IntPtr.Zero;
			return GetSuperclass(obj.Handle);
		}
		public static IntPtr GetSuperclass(IntPtr obj)
		{
			var cls = GetClass(obj);
			if (cls == IntPtr.Zero)
				return IntPtr.Zero;
			return class_getSuperclass(cls);
		}
		public static IntPtr ClassGetSuperclass(IntPtr cls) => class_getSuperclass(cls);

		public static bool ClassSuperClassInstancesRespondsToSelector(IntPtr cls, IntPtr sel)
		{
			if (cls == IntPtr.Zero)
				return false;
			var superClass = ClassGetSuperclass(cls);
			return ClassInstancesRespondToSelector(superClass, sel);
		}

		public static bool SuperClassInstancesRespondsToSelector(NSObject obj, IntPtr sel)
		{
			if (obj == null)
				return false;
			return SuperClassInstancesRespondsToSelector(obj.Handle, sel);
		}

		public static bool SuperClassInstancesRespondsToSelector(IntPtr obj, IntPtr sel)
		{
			if (obj == IntPtr.Zero)
				return false;
				
			// we must get the class for the managed type, as the native class might actually be a KVO subclass...
			var superClass = GetSuperclass(obj);
			return ClassInstancesRespondToSelector(superClass, sel);
		}
	}
}

