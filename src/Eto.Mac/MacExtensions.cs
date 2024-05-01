namespace Eto.Mac
{
	/// <summary>
	/// These are extensions for missing methods in monomac, incorrectly bound, or bad performance.
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
#if USE_CFSTRING
			IntPtr intPtr = replacementString != null ? CFString.CreateNative(replacementString) : IntPtr.Zero;
			bool result;
			result = Messaging.bool_objc_msgSend_NSRange_IntPtr(textView.Handle, selShouldChangeTextInRangeReplacementString_Handle, affectedCharRange, intPtr);
			if (intPtr != IntPtr.Zero)
				CFString.ReleaseNative(intPtr);
#else
			IntPtr intPtr = replacementString != null ? NSString.CreateNative(replacementString) : IntPtr.Zero;
			bool result;
			result = Messaging.bool_objc_msgSend_NSRange_IntPtr(textView.Handle, selShouldChangeTextInRangeReplacementString_Handle, affectedCharRange, intPtr);
			if (intPtr != IntPtr.Zero)
				NSString.ReleaseNative(intPtr);
#endif
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


#if MONOMAC
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

		static readonly IntPtr selFrameSizeForContentSize_HorizontalScrollerClass_VerticalScrollerClass_BorderType_ControlSize_ScrollerStyle_Handle = Selector.GetHandle("frameSizeForContentSize:horizontalScrollerClass:verticalScrollerClass:borderType:controlSize:scrollerStyle:");
		static readonly IntPtr selContentSizeForFrameSize_HorizontalScrollerClass_VerticalScrollerClass_BorderType_ControlSize_ScrollerStyle_Handle = Selector.GetHandle("contentSizeForFrameSize:horizontalScrollerClass:verticalScrollerClass:borderType:controlSize:scrollerStyle:");
		static readonly IntPtr classScroller_Handle = Class.GetHandle(typeof(NSScroller));

		public static CGSize FrameSizeForContentSize(this NSScrollView scrollView, CGSize size, bool hbar, bool vbar)
		{
			var hbarPtr = hbar ? classScroller_Handle : IntPtr.Zero;
			var vbarPtr = vbar ? classScroller_Handle : IntPtr.Zero;
			// 10.7+, use MacOS api when it supports null scroller class parameters
			return Messaging.CGSize_objc_msgSend_CGSize_IntPtr_IntPtr_UInt64_UInt64_Int64(scrollView.ClassHandle, selFrameSizeForContentSize_HorizontalScrollerClass_VerticalScrollerClass_BorderType_ControlSize_ScrollerStyle_Handle, size, hbarPtr, vbarPtr, (ulong)scrollView.BorderType, (ulong)scrollView.VerticalScroller.ControlSize, (long)scrollView.VerticalScroller.ScrollerStyle);
		}

		public static CGSize ContentSizeForFrame(this NSScrollView scrollView, CGSize size, bool hbar, bool vbar)
		{
			var hbarPtr = hbar ? classScroller_Handle : IntPtr.Zero;
			var vbarPtr = vbar ? classScroller_Handle : IntPtr.Zero;
			// 10.7+, use MacOS api when it supports null scroller class parameters
			return Messaging.CGSize_objc_msgSend_CGSize_IntPtr_IntPtr_UInt64_UInt64_Int64(scrollView.ClassHandle, selContentSizeForFrameSize_HorizontalScrollerClass_VerticalScrollerClass_BorderType_ControlSize_ScrollerStyle_Handle, size, hbarPtr, vbarPtr, (ulong)scrollView.BorderType, (ulong)scrollView.VerticalScroller.ControlSize, (long)scrollView.VerticalScroller.ScrollerStyle);
		}


		static readonly IntPtr selSetClipsToBounds = Selector.GetHandle("setClipsToBounds:");
		static readonly bool supportsClipsToBounds = ObjCExtensions.InstancesRespondToSelector<NSView>(selSetClipsToBounds);

		public static void SetClipsToBounds(this NSView view, bool clipsToBounds)
		{
			if (!supportsClipsToBounds)
				return;
			Messaging.void_objc_msgSend_bool(view.Handle, selSetClipsToBounds, clipsToBounds);
		}
	}
}

