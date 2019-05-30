using System;
using System.Runtime.InteropServices;
#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
using CoreImage;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
using MonoMac.CoreImage;
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

namespace Eto.Mac
{
	/// <summary>
	/// These are extensions for missing methods in monomac/xamarin.mac, incorrectly bound, or bad performance.
	/// </summary>
	/// <remarks>
	/// Once monomac/xam.mac supports these methods or are implemented properly, then remove from here.
	/// </remarks>
	public static class MacExtensions
	{
		static readonly IntPtr selBoundingRectWithSize = Selector.GetHandle("boundingRectWithSize:options:");


		// not bound
		public static CGRect BoundingRect(this NSAttributedString str, CGSize size, NSStringDrawingOptions options)
		{
			CGRect rect;
			Messaging.RectangleF_objc_msgSend_stret_SizeF_int(out rect, str.Handle, selBoundingRectWithSize, size, (int)options);
			return rect;
		}

		static readonly IntPtr selNextEventMatchingMask = Selector.GetHandle("nextEventMatchingMask:untilDate:inMode:dequeue:");

		// untilDate isn't allowed null
		public static NSEvent NextEventEx(this NSApplication app, NSEventMask mask, NSDate untilDate, NSString mode, bool dequeue)
		{
			return Runtime.GetNSObject<NSEvent>(Messaging.IntPtr_objc_msgSend_nuint_IntPtr_IntPtr_bool(app.Handle, selNextEventMatchingMask, (nuint)(uint)mask, untilDate != null ? untilDate.Handle : IntPtr.Zero, mode.Handle, dequeue));
		}


		static readonly IntPtr selDrawGlyphs = Selector.GetHandle("drawGlyphsForGlyphRange:atPoint:");

		// not bound
		public static void DrawGlyphs(this NSLayoutManager layout, NSRange range, CGPoint point)
		{
			Messaging.void_objc_msgSend_NSRange_CGPoint(layout.Handle, selDrawGlyphs, range, point);
		}

		static readonly IntPtr selRetain = Selector.GetHandle("retain");
		static readonly IntPtr selRelease = Selector.GetHandle("release");

		public static void Retain(IntPtr handle)
		{
			Messaging.void_objc_msgSend(handle, selRetain);
		}

		public static void Release(IntPtr handle)
		{
			Messaging.void_objc_msgSend(handle, selRelease);
		}

		static readonly IntPtr selShouldChangeTextInRangeReplacementString_Handle = Selector.GetHandle("shouldChangeTextInRange:replacementString:");

		// replacementString should allow nulls
		public static bool ShouldChangeTextNew(this NSTextView textView, NSRange affectedCharRange, string replacementString)
		{
			IntPtr intPtr = replacementString != null ? NSString.CreateNative(replacementString) : IntPtr.Zero;
			bool result;
			result = Messaging.bool_objc_msgSend_NSRange_IntPtr(textView.Handle, selShouldChangeTextInRangeReplacementString_Handle, affectedCharRange, intPtr);
			if (intPtr != IntPtr.Zero)
				NSString.ReleaseNative(intPtr);
			return result;
		}

		static readonly IntPtr selColorUsingColorSpaceName_Handle = Selector.GetHandle("colorUsingColorSpaceName:");

		public static NSColor UsingColorSpaceFast(this NSColor color, NSString colorSpace)
		{
			if (color == null)
				return null;
			var intPtr = Messaging.IntPtr_objc_msgSend_IntPtr(color.Handle, selColorUsingColorSpaceName_Handle, colorSpace.Handle);
			return Runtime.GetNSObject<NSColor>(intPtr);
		}

		static readonly IntPtr selCanReadItemWithDataConformingToTypes_Handle = Selector.GetHandle("canReadItemWithDataConformingToTypes:");
		public static bool CanReadItemWithDataConformingToTypes(this NSPasteboard pasteboard, NSString[] utiTypes)
		{
			NSApplication.EnsureUIThread();
			if (utiTypes == null)
			{
				throw new ArgumentNullException(nameof(utiTypes));
			}
			NSArray nSArray = NSArray.FromNSObjects(utiTypes);
			bool result = Messaging.bool_objc_msgSend_IntPtr(pasteboard.Handle, selCanReadItemWithDataConformingToTypes_Handle, nSArray.Handle);
			nSArray.Dispose();
			return result;
		}


#if !XAMMAC
		public static void DangerousRetain(this NSObject obj)
		{
			obj.Retain();
		}

		public static void DangerousRelease(this NSObject obj)
		{
			obj.Release();
		}

		static readonly IntPtr NSColorClassPtr = Class.GetHandle("NSColor");
		static readonly IntPtr selColorWithCGColor = Selector.GetHandle("colorWithCGColor:");

		public static NSColor NSColorFromCGColor(CGColor cgColor)
		{
			return Messaging.GetNSObject<NSColor>(Messaging.IntPtr_objc_msgSend_IntPtr(NSColorClassPtr, selColorWithCGColor, cgColor.Handle));
		}
#endif
	}
}

