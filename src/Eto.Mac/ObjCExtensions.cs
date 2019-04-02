using System;
using System.Runtime.InteropServices;
#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
#elif OSX
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
#elif IOS
using ObjCRuntime;
using Eto.iOS;
#endif

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
		public static extern IntPtr object_getClass(IntPtr obj);

		public static Class GetClass(IntPtr obj) => new Class(object_getClass(obj));

		[DllImport("/usr/lib/libobjc.dylib")]
		public static extern IntPtr class_getSuperclass(IntPtr obj);

		public static Class GetSuperclass(IntPtr cls) => new Class(class_getSuperclass(cls));

		[DllImport("/usr/lib/libobjc.dylib")]
		static extern IntPtr objc_getMetaClass(string metaClassName);

		public static Class GetMetaClass(string metaClassName) => new Class(objc_getMetaClass(metaClassName));

		static readonly IntPtr selInstancesRespondToSelector = Selector.GetHandle("instancesRespondToSelector:");
		static readonly IntPtr selRespondsToSelector = Selector.GetHandle("respondsToSelector:");

		public static bool ClassInstancesRespondToSelector(IntPtr cls, IntPtr selector)
		{
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
	}
}

