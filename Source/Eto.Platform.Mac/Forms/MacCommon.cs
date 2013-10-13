using System;
using MonoMac.ObjCRuntime;
using MonoMac.Foundation;

namespace Eto.Platform.Mac.Forms
{
	public static class MacCommon
	{
		public static Selector selCopyWithZone = new Selector("copyWithZone:");

		public static IntPtr ReleaseHandle = Selector.GetHandle("release");

		public static void SafeDispose(this NSObject obj)
		{
			if (obj != null)
			{
				var handle = obj.Handle;
				obj.Dispose();
				// HACK: release handle since Dispose() won't do it properly yet
				if (handle != IntPtr.Zero)
					Messaging.void_objc_msgSend(handle, ReleaseHandle);
			}
		}
	}
}

