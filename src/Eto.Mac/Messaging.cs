#if IOS
namespace Eto.iOS
#else
namespace Eto.Mac
#endif
{
	public static class Messaging
	{
		const string LIBOBJC_DYLIB = "/usr/lib/libobjc.dylib";
		public static readonly IntPtr AppKitHandle = Dlfcn.dlopen("/System/Library/Frameworks/AppKit.framework/AppKit", 0);

		public static readonly IntPtr CoreTextHandle = Dlfcn.dlopen("/System/Library/Frameworks/ApplicationServices.framework/Frameworks/CoreText.framework/CoreText", 0);

		public static T GetNSObject<T>(IntPtr ptr)
			where T : NSObject
		{
			return Runtime.GetNSObject<T>(ptr);
		}

		[DllImport(LIBOBJC_DYLIB, EntryPoint = "objc_msgSendSuper")]
		public static extern IntPtr IntPtr_objc_msgSendSuper_IntPtr(IntPtr receiver, IntPtr selector, IntPtr arg1);
		
		[DllImport(LIBOBJC_DYLIB, EntryPoint = "objc_msgSendSuper")]
		public static extern void void_objc_msgSendSuper_IntPtr_NSRange(IntPtr receiver, IntPtr selector, IntPtr arg1, NSRange arg2);

		[DllImport(LIBOBJC_DYLIB, EntryPoint = "objc_msgSendSuper")]
		public static extern bool bool_objc_msgSendSuper_IntPtr_IntPtr_IntPtr(IntPtr receiver, IntPtr selector, IntPtr arg1, IntPtr arg2, IntPtr arg3);

		[DllImport(LIBOBJC_DYLIB, EntryPoint = "objc_msgSendSuper")]
		public static extern void void_objc_msgSendSuper_SizeF(IntPtr receiver, IntPtr selector, CGSize arg1);

		[DllImport(LIBOBJC_DYLIB, EntryPoint = "objc_msgSendSuper")]
		public static extern nfloat nfloat_objc_msgSendSuper_nint(IntPtr receiver, IntPtr selector, nint segment);

		[DllImport(LIBOBJC_DYLIB, EntryPoint = "objc_msgSendSuper")]
		public static extern bool bool_objc_msgSendSuper(IntPtr receiver, IntPtr selector);

		[DllImport(LIBOBJC_DYLIB, EntryPoint = "objc_msgSendSuper")]
		public static extern bool bool_objc_msgSendSuper_IntPtr(IntPtr receiver, IntPtr selector, IntPtr arg1);

		[DllImport(LIBOBJC_DYLIB, EntryPoint = "objc_msgSendSuper")]
		public static extern bool bool_objc_msgSendSuper_NSRange_IntPtr(IntPtr receiver, IntPtr sel, NSRange arg1, IntPtr arg2);

		[DllImport(LIBOBJC_DYLIB, EntryPoint = "objc_msgSendSuper")]
		public static extern void void_objc_msgSendSuper_IntPtr(IntPtr receiver, IntPtr selector, IntPtr arg1);

		[DllImport(LIBOBJC_DYLIB, EntryPoint = "objc_msgSendSuper")]
		public static extern void void_objc_msgSendSuper_CGRect(IntPtr receiver, IntPtr selector, CGRect arg1);

		[DllImport(LIBOBJC_DYLIB, EntryPoint = "objc_msgSendSuper")]
		public static extern void void_objc_msgSendSuper(IntPtr receiver, IntPtr selector);

		[DllImport(LIBOBJC_DYLIB, EntryPoint = "objc_msgSend")]
		public static extern bool bool_objc_msgSend(IntPtr receiver, IntPtr selector);

		[DllImport(LIBOBJC_DYLIB, EntryPoint = "objc_msgSend")]
		public static extern bool bool_objc_msgSend_IntPtr(IntPtr receiver, IntPtr selector, IntPtr arg1);

		[DllImport(LIBOBJC_DYLIB, EntryPoint = "objc_msgSend")]
		public static extern bool bool_objc_msgSend_IntPtr_IntPtr(IntPtr receiver, IntPtr selector, IntPtr arg1, IntPtr arg2);

		[DllImport(LIBOBJC_DYLIB, EntryPoint = "objc_msgSend")]
		public static extern void void_objc_msgSend(IntPtr receiver, IntPtr selector);

		[DllImport(LIBOBJC_DYLIB, EntryPoint = "objc_msgSend")]
		public static extern void void_objc_msgSend_IntPtr(IntPtr receiver, IntPtr selector, IntPtr arg1);

		[DllImport(LIBOBJC_DYLIB, EntryPoint = "objc_msgSend")]
		public static extern void void_objc_msgSend_bool(IntPtr receiver, IntPtr selector, bool arg1);

		[DllImport(LIBOBJC_DYLIB, EntryPoint = "objc_msgSend")]
		public static extern IntPtr IntPtr_objc_msgSend(IntPtr receiver, IntPtr selector);

		[DllImport(LIBOBJC_DYLIB, EntryPoint = "objc_msgSend")]
		public static extern IntPtr IntPtr_objc_msgSend_nuint(IntPtr receiver, IntPtr selector, nuint arg1);

		[DllImport(LIBOBJC_DYLIB, EntryPoint = "objc_msgSend")]
		public static extern nuint nuint_objc_msgSend(IntPtr receiver, IntPtr selector);

		[DllImport(LIBOBJC_DYLIB, EntryPoint = "objc_msgSend")]
		public static extern void RectangleF_objc_msgSend_stret(out CGRect rect, IntPtr receiver, IntPtr selector);

		[DllImport(LIBOBJC_DYLIB, EntryPoint = "objc_msgSend_stret")]
		public static extern void RectangleF_objc_msgSend_stret_SizeF_int(out CGRect retval, IntPtr receiver, IntPtr selector, CGSize arg1, nint arg2);

		[DllImport(LIBOBJC_DYLIB, EntryPoint = "objc_msgSend")]
		public static extern IntPtr IntPtr_objc_msgSend_nuint_IntPtr_IntPtr_bool(IntPtr receiver, IntPtr selector, nuint mask, IntPtr untilDate, IntPtr mode, bool dequeue);

		[DllImport(LIBOBJC_DYLIB, EntryPoint = "objc_msgSend")]
		public static extern void void_objc_msgSend_NSRange_CGPoint(IntPtr receiver, IntPtr selector, NSRange arg1, CGPoint arg2);

		[DllImport(LIBOBJC_DYLIB, EntryPoint = "objc_msgSend")]
		public static extern bool bool_objc_msgSend_NSRange_IntPtr(IntPtr receiver, IntPtr selector, NSRange arg1, IntPtr arg2);

		[DllImport(LIBOBJC_DYLIB, EntryPoint = "objc_msgSend")]
		public static extern IntPtr IntPtr_objc_msgSend_IntPtr(IntPtr reciever, IntPtr selector, IntPtr arg1);

		[DllImport(LIBOBJC_DYLIB, EntryPoint = "objc_msgSend")]
		public static extern IntPtr IntPtr_objc_msgSend_IntPtr_IntPtr(IntPtr reciever, IntPtr selector, IntPtr arg1, IntPtr arg2);

		[DllImport(LIBOBJC_DYLIB, EntryPoint = "objc_msgSend")]
		public static extern IntPtr IntPtr_objc_msgSend_IntPtr_IntPtr_IntPtr(IntPtr reciever, IntPtr selector, IntPtr arg1, IntPtr arg2, IntPtr arg3);

		[DllImport(LIBOBJC_DYLIB, EntryPoint = "objc_msgSend")]
		public static extern CGSize CGSize_objc_msgSend_CGSize_IntPtr_IntPtr_UInt64_UInt64_Int64(IntPtr receiver, IntPtr selector, CGSize arg1, IntPtr arg2, IntPtr arg3, ulong arg4, ulong arg5, long arg6);

		[DllImport(LIBOBJC_DYLIB, EntryPoint = "objc_msgSendSuper")]
		public static extern void void_objc_msgSendSuper_IntPtr_IntPtr_nuint_IntPtr_CGAffineTransform_IntPtr_IntPtr(IntPtr receiver, IntPtr selector, IntPtr arg1, IntPtr arg2, nuint arg3, IntPtr arg4, CGAffineTransform arg5, IntPtr arg6, IntPtr arg7);

		[DllImport(LIBOBJC_DYLIB, EntryPoint = "objc_msgSendSuper")]
		public static extern IntPtr IntPtr_objc_msgSendSuper_IntPtr_IntPtr_IntPtr_ref_CGPoint(IntPtr receiver, IntPtr selector, IntPtr arg1, IntPtr arg2, IntPtr arg3, ref CGPoint arg4);
		[DllImport(LIBOBJC_DYLIB, EntryPoint = "objc_msgSend")]
		public extern static void void_objc_msgSend_IntPtr_IntPtr(IntPtr receiver, IntPtr selector, IntPtr arg1, IntPtr arg2);

		[DllImport(LIBOBJC_DYLIB, EntryPoint = "objc_msgSend")]
		[return: MarshalAs(UnmanagedType.I1)]
		public static extern bool bool_objc_msgSend_CGRect_CGRect_UIntPtr_nfloat_bool_IntPtr(IntPtr receiver, IntPtr selector, CGRect arg1, CGRect arg2, UIntPtr arg3, nfloat arg4, [MarshalAs(UnmanagedType.I1)] bool arg5, IntPtr arg6);

	}
}

