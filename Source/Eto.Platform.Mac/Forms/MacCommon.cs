using System;
using MonoMac.ObjCRuntime;
using MonoMac.Foundation;

namespace Eto.Platform.Mac.Forms
{
	public static class MacCommon
	{
		public static IntPtr CopyWithZoneHandle = Selector.GetHandle("copyWithZone:");

		public static IntPtr ReleaseHandle = Selector.GetHandle("release");

		public static void SafeDispose(this NSObject obj)
		{
			if (obj != null)
			{
				var count = obj.RetainCount;
				var handle = obj.Handle;

				obj.Dispose();
				// HACK: release handle since Dispose() won't do it properly yet
				if (handle != IntPtr.Zero && ApplicationHandler.Instance != null && count > 2)
					Messaging.void_objc_msgSend(handle, ReleaseHandle);
			}
		}
	}
}

