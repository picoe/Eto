using System;
using System.Runtime.InteropServices;
using System.Drawing;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;
#if Mac64
using CGFloat = System.Double;
using NSInteger = System.Int64;
using NSUInteger = System.UInt64;
#else
using NSSize = System.Drawing.SizeF;
using NSRect = System.Drawing.RectangleF;
using NSPoint = System.Drawing.PointF;
using CGFloat = System.Single;
using NSInteger = System.Int32;
using NSUInteger = System.UInt32;
#endif

namespace Eto.Mac
{
	/// <summary>
	/// These are extensions for missing methods in monomac/xamarin.mac, or incorrectly bound.
	/// </summary>
	/// <remarks>
	/// Once monomac/xam.mac supports these methods or are implemented properly, then remove from here.
	/// </remarks>
	public static class MacExtensions
	{
		static readonly IntPtr selBoundingRectWithSize = Selector.GetHandle("boundingRectWithSize:options:");

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend_stret")]
		static extern void RectangleF_objc_msgSend_stret_SizeF_int(out NSRect retval, IntPtr receiver, IntPtr selector, NSSize arg1, NSInteger arg2);

		// not bound
		public static NSRect BoundingRect(this NSAttributedString str, NSSize size, NSStringDrawingOptions options)
		{
			NSRect rect;
			RectangleF_objc_msgSend_stret_SizeF_int(out rect, str.Handle, selBoundingRectWithSize, size, (int)options);
			return rect;
		}

		static readonly IntPtr selNextEventMatchingMask = Selector.GetHandle("nextEventMatchingMask:untilDate:inMode:dequeue:");

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		static extern IntPtr IntPtr_objc_msgSend_NSUInteger_IntPtr_IntPtr_bool(IntPtr receiver, IntPtr selector, NSUInteger mask, IntPtr untilDate, IntPtr mode, bool dequeue);

		// untilDate isn't allowed null
		public static NSEvent NextEventEx(this NSApplication app, NSEventMask mask, NSDate untilDate, NSString mode, bool dequeue)
		{
			return (NSEvent)Runtime.GetNSObject(IntPtr_objc_msgSend_NSUInteger_IntPtr_IntPtr_bool(app.Handle, selNextEventMatchingMask, (NSUInteger)mask, untilDate != null ? untilDate.Handle : IntPtr.Zero, mode.Handle, dequeue));
		}

		[DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		static extern void void_objc_msgSend_NSRange_NSPoint(IntPtr receiver, IntPtr selector, NSRange arg1, NSPoint arg2);

		static readonly IntPtr selDrawGlyphs = Selector.GetHandle("drawGlyphsForGlyphRange:atPoint:");

		// not bound
		public static void DrawGlyphs(this NSLayoutManager layout, NSRange range, NSPoint point)
		{
			void_objc_msgSend_NSRange_NSPoint(layout.Handle, selDrawGlyphs, range, point);
		}
	}
}

