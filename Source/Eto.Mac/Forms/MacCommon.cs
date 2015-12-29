using System;
#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
#endif

namespace Eto.Mac.Forms
{
	public static class MacCommon
	{
		public static IntPtr CopyWithZoneHandle = Selector.GetHandle("copyWithZone:");

		public static IntPtr ReleaseHandle = Selector.GetHandle("release");

		public static void SafeDispose(this NSObject obj)
		{
			if (obj != null)
			{
				#if MONOMAC
				var count = obj.RetainCount;
				var handle = obj.Handle;

				obj.Dispose();
				// HACK: release handle since Dispose() won't do it properly yet
				if (handle != IntPtr.Zero && ApplicationHandler.Instance != null && count > 2)
					Messaging.void_objc_msgSend(handle, ReleaseHandle);
				#else
				obj.Dispose();
				#endif
			}
		}
	}
}

