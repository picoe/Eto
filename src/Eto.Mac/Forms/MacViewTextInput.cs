using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Eto.Forms;
using System.Diagnostics;

#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
using CoreImage;
using MobileCoreServices;
#else
using MonoMac;
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
using MonoMac.CoreImage;
using MonoMac.MobileCoreServices;
using nfloat = System.Double;
using nint = System.Int64;
using nuint = System.UInt64;
#endif

namespace Eto.Mac.Forms
{
	class MacViewTextInput
	{
		internal static IntPtr HasMarkedText_Selector = Selector.GetHandle("hasMarkedText");
		internal static MarshalDelegates.Func_IntPtr_IntPtr_bool HasMarkedText_Delegate = HasMarkedText;
		static bool HasMarkedText(IntPtr sender, IntPtr sel) => false;


		internal static IntPtr MarkedRange_Selector = Selector.GetHandle("markedRange");
		internal static MarshalDelegates.Func_IntPtr_IntPtr_NSRange MarkedRange_Delegate = MarkedRange;
		static NSRange MarkedRange(IntPtr sender, IntPtr sel) => new NSRange(0, 0);

		internal static IntPtr SelectedRange_Selector = Selector.GetHandle("selectedRange");
		internal static MarshalDelegates.Func_IntPtr_IntPtr_NSRange SelectedRange_Delegate = SelectedRange;
		static NSRange SelectedRange(IntPtr sender, IntPtr sel) => new NSRange(0, 0);

		internal static IntPtr SetMarkedText_Selector = Selector.GetHandle("setMarkedText:selectedRange:replacementRange:");
		internal static MarshalDelegates.Action_IntPtr_IntPtr_IntPtr_NSRange_NSRange SetMarkedText_Delegate = SetMarkedText;
		static void SetMarkedText(IntPtr sender, IntPtr sel, IntPtr text, NSRange selectedRange, NSRange replacementRange)
		{
		}

		internal static IntPtr UnmarkText_Selector = Selector.GetHandle("unmarkText");
		internal static MarshalDelegates.Action_IntPtr_IntPtr UnmarkText_Delegate = UnmarkText;
		static void UnmarkText(IntPtr sender, IntPtr sel)
		{
		}

		internal static IntPtr ValidAttributesForMarkedText_Selector = Selector.GetHandle("validAttributesForMarkedText");
		internal static MarshalDelegates.Func_IntPtr_IntPtr_IntPtr ValidAttributesForMarkedText_Delegate = ValidAttributesForMarkedText;
		static IntPtr ValidAttributesForMarkedText(IntPtr sender, IntPtr sel) => IntPtr.Zero;

		internal static IntPtr AttributedStringForProposedRange_Selector = Selector.GetHandle("attributedSubstringForProposedRange:actualRange:");
		internal static MarshalDelegates.Func_IntPtr_IntPtr_NSRange_IntPtr_IntPtr AttributedStringForProposedRange_Delegate = AttributedStringForProposedRange;
		static IntPtr AttributedStringForProposedRange(IntPtr sender, IntPtr sel, NSRange proposedRange, IntPtr actualRange) => IntPtr.Zero;


		internal static IntPtr CharacterIndexForPoint_Selector = Selector.GetHandle("characterIndexForPoint:");
		internal static MarshalDelegates.Func_IntPtr_IntPtr_CGPoint_nuint CharacterIndexForPoint_Delegate = CharacterIndexForPoint;
		static nuint CharacterIndexForPoint(IntPtr sender, IntPtr sel, CGPoint point) => 0;

		internal static IntPtr FirstRectForCharacterRange_Selector = Selector.GetHandle("firstRectForCharacterRange:actualRange:");
		internal static MarshalDelegates.Func_IntPtr_IntPtr_NSRange_IntPtr_CGRect FirstRectForCharacterRange_Delegate = FirstRectForCharacterRange;
		static CGRect FirstRectForCharacterRange(IntPtr sender, IntPtr sel, NSRange range, IntPtr actualRange)
		{
			var obj = Runtime.GetNSObject(sender);
			if (obj is NSView ctl && MacBase.GetHandler(obj) is IMacViewHandler handler)
			{
				var rect = ctl.ConvertRectToView(ctl.Bounds, null);
				return ctl.Window.ConvertRectToScreen(rect);
			}
			return CGRect.Empty;
		}

		internal static IntPtr DoCommandBySelector_Selector = Selector.GetHandle("doCommandBySelector:");
		internal static MarshalDelegates.Action_IntPtr_IntPtr_IntPtr DoCommandBySelector_Delegate = DoCommandBySelector;
		static void DoCommandBySelector(IntPtr sender, IntPtr sel, IntPtr selector)
		{
		}

		internal static IntPtr NSTextInputClientProtocol_Handle = ObjCExtensions.GetProtocolHandle("NSTextInputClient");
	}

	partial class MacView<TControl, TWidget, TCallback>
	{
		public virtual NSView TextInputControl => EventControl;
		public bool EnsureTextInputImplemented(NSView view = null)
		{
			view = view ?? TextInputControl;
			
			// determine whether we need to call InterpretKeyEvents ourselves or if it is already handled by the super class (e.g. NSTextView)
			// for NSTextField (TextBox, etc), we handle the TextInput event via MacFieldEditor
			TextInputImplemented = !ObjCExtensions.ClassConformsToProtocol(view.GetSuperclass(), MacViewTextInput.NSTextInputClientProtocol_Handle);
			
			// if it already conforms to the protocol, add the insertText:replacementRange method only
			if (view.ConformsToProtocol(MacViewTextInput.NSTextInputClientProtocol_Handle))
			{
				AddMethod(MacView.selInsertTextReplacementRange, MacView.TriggerTextInput_Delegate, EtoEnvironment.Is64BitProcess ? "v@:@{NSRange=QQ}" : "v@:@{NSRange=II}");
				return false;
			}

			// Debug.WriteLine($"Adding TextInputClient to {view.GetType()}, Widget: {Widget.GetType()}");
			
			// add the NSTextInputClient protocol to the class
			var cls = Class.GetHandle(view.GetType());
			ObjCExtensions.ClassAddProtocol(cls, MacViewTextInput.NSTextInputClientProtocol_Handle);

			// add required methods for the NSTextInputClient protocol
			AddMethod(MacViewTextInput.HasMarkedText_Selector, MacViewTextInput.HasMarkedText_Delegate, "B@:", view);
			AddMethod(MacViewTextInput.MarkedRange_Selector, MacViewTextInput.MarkedRange_Delegate, "{NSRange=QQ}@:", view);
			AddMethod(MacViewTextInput.SelectedRange_Selector, MacViewTextInput.SelectedRange_Delegate, "{NSRange=QQ}@:", view);
			AddMethod(MacViewTextInput.SetMarkedText_Selector, MacViewTextInput.SetMarkedText_Delegate, "v@:@{NSRange=QQ}{NSRange=QQ}", view);
			AddMethod(MacViewTextInput.UnmarkText_Selector, MacViewTextInput.UnmarkText_Delegate, "v@:", view);
			AddMethod(MacViewTextInput.ValidAttributesForMarkedText_Selector, MacViewTextInput.ValidAttributesForMarkedText_Delegate, "@@:", view);
			AddMethod(MacViewTextInput.AttributedStringForProposedRange_Selector, MacViewTextInput.AttributedStringForProposedRange_Delegate, "@@:{NSRange=QQ}^{NSRange=QQ}", view);
			AddMethod(MacViewTextInput.CharacterIndexForPoint_Selector, MacViewTextInput.CharacterIndexForPoint_Delegate, "Q@:{CGPoint=gg}", view);
			AddMethod(MacViewTextInput.FirstRectForCharacterRange_Selector, MacViewTextInput.FirstRectForCharacterRange_Delegate, "{CGRect=gggg}@:{NSRange=QQ}^{NSRange=QQ}", view);
			AddMethod(MacViewTextInput.DoCommandBySelector_Selector, MacViewTextInput.DoCommandBySelector_Delegate, "v@:#", view);
			
			AddMethod(MacView.selInsertTextReplacementRange, MacView.TriggerTextInput_Delegate, "v@:@{NSRange=QQ}", view);
			return true;
		}

	}
}