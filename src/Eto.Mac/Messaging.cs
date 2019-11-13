using System;
using System.Runtime.InteropServices;

#if IOS
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
#elif XAMMAC2
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
#if Mac64
using nfloat = System.Double;
using nint = System.Int64;
using nuint = System.UInt64;
#else
using nfloat = System.Single;
using nint = System.Int32;
using nuint = System.UInt32;
#endif
#if SDCOMPAT
using CGSize = System.Drawing.SizeF;
using CGRect = System.Drawing.RectangleF;
using CGPoint = System.Drawing.PointF;
#endif
#endif

#if IOS
namespace Eto.iOS
#else
namespace Eto.Mac
#endif
{
	public static class Messaging
	{
		public static readonly IntPtr AppKitHandle = Dlfcn.dlopen("/System/Library/Frameworks/AppKit.framework/AppKit", 0);

		public static readonly IntPtr CoreTextHandle = Dlfcn.dlopen("/System/Library/Frameworks/ApplicationServices.framework/Frameworks/CoreText.framework/CoreText", 0);

		public static T GetNSObject<T>(IntPtr ptr)
			where T : NSObject
		{
			return Runtime.GetNSObject<T>(ptr);
		}

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSendSuper")]
		public static extern IntPtr IntPtr_objc_msgSendSuper_IntPtr(IntPtr receiver, IntPtr selector, IntPtr arg1);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSendSuper")]
		public static extern bool bool_objc_msgSendSuper_IntPtr_IntPtr_IntPtr(IntPtr receiver, IntPtr selector, IntPtr arg1, IntPtr arg2, IntPtr arg3);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSendSuper")]
		public static extern void void_objc_msgSendSuper_SizeF(IntPtr receiver, IntPtr selector, CGSize arg1);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSendSuper")]
		public static extern nfloat nfloat_objc_msgSendSuper_nint(IntPtr receiver, IntPtr selector, nint segment);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSendSuper")]
		public static extern bool bool_objc_msgSendSuper(IntPtr receiver, IntPtr selector);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSendSuper")]
		public static extern bool bool_objc_msgSendSuper_IntPtr(IntPtr receiver, IntPtr selector, IntPtr arg1);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSendSuper")]
		public static extern bool bool_objc_msgSendSuper_NSRange_IntPtr(IntPtr receiver, IntPtr sel, NSRange arg1, IntPtr arg2);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSendSuper")]
		public static extern void void_objc_msgSendSuper_IntPtr(IntPtr receiver, IntPtr selector, IntPtr arg1);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSendSuper")]
		public static extern void void_objc_msgSendSuper_CGRect(IntPtr receiver, IntPtr selector, CGRect arg1);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSendSuper")]
		public static extern void void_objc_msgSendSuper(IntPtr receiver, IntPtr selector);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		public static extern bool bool_objc_msgSend(IntPtr receiver, IntPtr selector);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		public static extern bool bool_objc_msgSend_IntPtr(IntPtr receiver, IntPtr selector, IntPtr arg1);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		public static extern bool bool_objc_msgSend_IntPtr_IntPtr(IntPtr receiver, IntPtr selector, IntPtr arg1, IntPtr arg2);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		public static extern void void_objc_msgSend(IntPtr receiver, IntPtr selector);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		public static extern void void_objc_msgSend_IntPtr(IntPtr receiver, IntPtr selector, IntPtr arg1);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		public static extern void void_objc_msgSend_bool(IntPtr receiver, IntPtr selector, bool arg1);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		public static extern IntPtr IntPtr_objc_msgSend(IntPtr receiver, IntPtr selector);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		public static extern IntPtr IntPtr_objc_msgSend_nuint(IntPtr receiver, IntPtr selector, nuint arg1);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		public static extern nuint nuint_objc_msgSend(IntPtr receiver, IntPtr selector);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		public static extern void RectangleF_objc_msgSend_stret(out CGRect rect, IntPtr receiver, IntPtr selector);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend_stret")]
		public static extern void RectangleF_objc_msgSend_stret_SizeF_int(out CGRect retval, IntPtr receiver, IntPtr selector, CGSize arg1, nint arg2);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		public static extern IntPtr IntPtr_objc_msgSend_nuint_IntPtr_IntPtr_bool(IntPtr receiver, IntPtr selector, nuint mask, IntPtr untilDate, IntPtr mode, bool dequeue);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		public static extern void void_objc_msgSend_NSRange_CGPoint(IntPtr receiver, IntPtr selector, NSRange arg1, CGPoint arg2);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		public static extern bool bool_objc_msgSend_NSRange_IntPtr(IntPtr receiver, IntPtr selector, NSRange arg1, IntPtr arg2);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		public static extern IntPtr IntPtr_objc_msgSend_IntPtr(IntPtr reciever, IntPtr selector, IntPtr arg1);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		public static extern IntPtr IntPtr_objc_msgSend_IntPtr_IntPtr(IntPtr reciever, IntPtr selector, IntPtr arg1, IntPtr arg2);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		public static extern IntPtr IntPtr_objc_msgSend_IntPtr_IntPtr_IntPtr(IntPtr reciever, IntPtr selector, IntPtr arg1, IntPtr arg2, IntPtr arg3);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		public static extern CGSize CGSize_objc_msgSend_CGSize_IntPtr_IntPtr_UInt64_UInt64_Int64(IntPtr receiver, IntPtr selector, CGSize arg1, IntPtr arg2, IntPtr arg3, ulong arg4, ulong arg5, long arg6);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSendSuper")]
		public static extern void void_objc_msgSendSuper_IntPtr_IntPtr_nuint_IntPtr_CGAffineTransform_IntPtr_IntPtr(IntPtr receiver, IntPtr selector, IntPtr arg1, IntPtr arg2, nuint arg3, IntPtr arg4, CGAffineTransform arg5, IntPtr arg6, IntPtr arg7);

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSendSuper")]
		public static extern IntPtr IntPtr_objc_msgSendSuper_IntPtr_IntPtr_IntPtr_ref_CGPoint(IntPtr receiver, IntPtr selector, IntPtr arg1, IntPtr arg2, IntPtr arg3, ref CGPoint arg4);
	}
}

