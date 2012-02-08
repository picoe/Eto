using System;
using MonoMac.ObjCRuntime;
using System.Runtime.InteropServices;

namespace Eto.Platform.Mac
{
	public static class ObjCExtensions
	{
		[DllImport ("/usr/lib/libobjc.dylib")]
		static extern IntPtr class_getClassMethod (IntPtr cls, IntPtr sel);
		
		public static IntPtr GetMethod (this Class cls, Selector selector)
		{
			return class_getClassMethod (cls.Handle, selector.Handle);
		}

		[DllImport ("/usr/lib/libobjc.dylib")]
		static extern bool class_addMethod (IntPtr cls, IntPtr sel, Delegate method, string argTypes);
		
		public static bool AddMethod (this Class cls, Selector selector, Delegate method, string arguments)
		{
			return class_addMethod (cls.Handle, selector.Handle, method, arguments);
		}
	}
}

