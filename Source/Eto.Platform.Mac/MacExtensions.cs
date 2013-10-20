using System;
using System.Runtime.InteropServices;
using System.Drawing;
using MonoMac.Foundation;

namespace Eto.Platform.Mac
{
	public static class MacExtensions
	{
		static readonly IntPtr selBoundingRectWithSize = MonoMac.ObjCRuntime.Selector.GetHandle("boundingRectWithSize:options:");

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend_stret")]
		public static extern void RectangleF_objc_msgSend_stret_SizeF_int(out RectangleF retval, IntPtr receiver, IntPtr selector, SizeF arg1, int arg2);

		public static RectangleF BoundingRect(this NSAttributedString str, SizeF size, NSStringDrawingOptions options)
		{
			RectangleF rect;
			RectangleF_objc_msgSend_stret_SizeF_int(out rect, str.Handle, selBoundingRectWithSize, size, (int)options);
			return rect;
		}
	}
}

