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
		static extern IntPtr class_getClassMethod(IntPtr cls, IntPtr sel);

		public static IntPtr GetMethod(this Class cls, IntPtr selector)
		{
			return class_getClassMethod(cls.Handle, selector);
		}

		public static IntPtr GetMethod(IntPtr cls, IntPtr selector)
		{
			return class_getClassMethod(cls, selector);
		}

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
			var method1 = class_getClassMethod(cls.Handle, selMethod1);
			var method2 = GetMethod(cls, selMethod2);
			method_exchangeImplementations(method1, method2);
		}

		[DllImport("/usr/lib/libobjc.dylib")]
		public static extern IntPtr object_getClass(IntPtr obj);

		public static Class GetClass(IntPtr obj)
		{
			return new Class(object_getClass(obj));
		}

		[DllImport("/usr/lib/libobjc.dylib")]
		public static extern IntPtr class_getSuperclass(IntPtr obj);

		public static Class GetSuperclass(IntPtr cls)
		{
			return new Class(class_getSuperclass(cls));
		}

		[DllImport("/usr/lib/libobjc.dylib")]
		static extern IntPtr objc_getMetaClass(string metaClassName);

		public static Class GetMetaClass(string metaClassName)
		{
			return new Class(objc_getMetaClass(metaClassName));
		}

		static readonly IntPtr selInstancesRespondToSelector = Selector.GetHandle("instancesRespondToSelector:");

		public static bool ClassInstancesRespondToSelector(IntPtr cls, IntPtr selector)
		{
			return Messaging.bool_objc_msgSend_IntPtr(cls, selInstancesRespondToSelector, selector);
		}
	}
}

