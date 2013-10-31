using System;
using System.Runtime.InteropServices;
using System.Drawing;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;

namespace Eto.Platform.Mac
{
	/// <summary>
	/// These are extensions for missing methods in monomac/xamarin.mac, or incorrectly bound.
	/// </summary>
	/// <remarks>
	/// Once monomac/xam.mac supports these methods or are implemented properly, then remove from here.
	/// </remarks>
	public static class MacExtensions
	{
		static readonly IntPtr selBoundingRectWithSize = MonoMac.ObjCRuntime.Selector.GetHandle("boundingRectWithSize:options:");

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend_stret")]
		static extern void RectangleF_objc_msgSend_stret_SizeF_int(out RectangleF retval, IntPtr receiver, IntPtr selector, SizeF arg1, int arg2);

		// not bound
		public static RectangleF BoundingRect(this NSAttributedString str, SizeF size, NSStringDrawingOptions options)
		{
			RectangleF rect;
			RectangleF_objc_msgSend_stret_SizeF_int(out rect, str.Handle, selBoundingRectWithSize, size, (int)options);
			return rect;
		}

		static readonly IntPtr selNextEventMatchingMask = Selector.GetHandle("nextEventMatchingMask:untilDate:inMode:dequeue:");

		// untilDate isn't allowed null
		public static NSEvent NextEventEx(this NSApplication app, NSEventMask mask, NSDate untilDate, NSString mode, bool dequeue)
		{
			return (NSEvent)Runtime.GetNSObject(Messaging.IntPtr_objc_msgSend_UInt32_IntPtr_IntPtr_bool(app.Handle, selNextEventMatchingMask, (uint)mask, untilDate != null ? untilDate.Handle : IntPtr.Zero, mode.Handle, dequeue));
		}

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		static extern void void_objc_msgSend_NSRange_NSPoint(IntPtr receiver, IntPtr selector, NSRange arg1, PointF arg2);

		static readonly IntPtr selDrawGlyphs = Selector.GetHandle("drawGlyphsForGlyphRange:atPoint:");

		// not bound
		public static void DrawGlyphs(this NSLayoutManager layout, NSRange range, PointF point)
		{
			void_objc_msgSend_NSRange_NSPoint(layout.Handle, selDrawGlyphs, range, point);
		}
	}
}

