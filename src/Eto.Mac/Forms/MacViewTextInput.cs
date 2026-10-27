namespace Eto.Mac.Forms
{
	interface IMacTextInputHandler
	{
		bool HasMarkedText { get; }
		NSRange MarkedRange { get; }
		NSRange SelectedRange { get; }
		CGRect FirstRectForCharacterRange(NSRange range);
		void FinishComposition();
		void SetMarkedText(string text, NSRange selectedRange, NSRange replacementRange);
		void UnmarkText();
	}

	static class MacViewTextInput
	{
		internal static IntPtr HasMarkedText_Selector = Selector.GetHandle("hasMarkedText");
		internal static MarshalDelegates.Func_IntPtr_IntPtr_bool HasMarkedText_Delegate = HasMarkedText;
		static bool HasMarkedText(IntPtr sender, IntPtr sel)
		{
			var obj = Runtime.GetNSObject(sender);
			return MacBase.GetHandler(obj) is IMacTextInputHandler handler && handler.HasMarkedText;
		}


		internal static IntPtr MarkedRange_Selector = Selector.GetHandle("markedRange");
		internal static MarshalDelegates.Func_IntPtr_IntPtr_NSRange MarkedRange_Delegate = MarkedRange;
		static NSRange MarkedRange(IntPtr sender, IntPtr sel)
		{
			var obj = Runtime.GetNSObject(sender);
			return MacBase.GetHandler(obj) is IMacTextInputHandler handler
				? handler.MarkedRange
				: new NSRange(NSRange.NotFound, 0);
		}

		internal static IntPtr SelectedRange_Selector = Selector.GetHandle("selectedRange");
		internal static MarshalDelegates.Func_IntPtr_IntPtr_NSRange SelectedRange_Delegate = SelectedRange;
		static NSRange SelectedRange(IntPtr sender, IntPtr sel)
		{
			var obj = Runtime.GetNSObject(sender);
			return MacBase.GetHandler(obj) is IMacTextInputHandler handler
				? handler.SelectedRange
				: new NSRange(0, 0);
		}

		internal static IntPtr SetMarkedText_Selector = Selector.GetHandle("setMarkedText:selectedRange:replacementRange:");
		internal static MarshalDelegates.Action_IntPtr_IntPtr_IntPtr_NSRange_NSRange SetMarkedText_Delegate = SetMarkedText;
		static void SetMarkedText(IntPtr sender, IntPtr sel, IntPtr text, NSRange selectedRange, NSRange replacementRange)
		{
			var obj = Runtime.GetNSObject(sender);
			if (MacBase.GetHandler(obj) is IMacTextInputHandler handler)
			{
				handler.SetMarkedText(GetText(text), selectedRange, replacementRange);
			}
		}

		internal static IntPtr UnmarkText_Selector = Selector.GetHandle("unmarkText");
		internal static MarshalDelegates.Action_IntPtr_IntPtr UnmarkText_Delegate = UnmarkText;
		static void UnmarkText(IntPtr sender, IntPtr sel)
		{
			var obj = Runtime.GetNSObject(sender);
			if (MacBase.GetHandler(obj) is IMacTextInputHandler handler)
			{
				handler.UnmarkText();
			}
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
			if (MacBase.GetHandler(obj) is IMacTextInputHandler textInputHandler)
			{
				return textInputHandler.FirstRectForCharacterRange(range);
			}
			if (obj is NSView ctl && MacBase.GetHandler(obj) is IMacViewHandler handler)
			{
				var rect = ctl.ConvertRectToView(ctl.Bounds, null);
				return ctl.Window?.ConvertRectToScreen(rect) ?? CGRect.Empty;
			}
			return CGRect.Empty;
		}

		internal static IntPtr DoCommandBySelector_Selector = Selector.GetHandle("doCommandBySelector:");
		internal static MarshalDelegates.Action_IntPtr_IntPtr_IntPtr DoCommandBySelector_Delegate = DoCommandBySelector;
		static void DoCommandBySelector(IntPtr sender, IntPtr sel, IntPtr selector)
		{
			var obj = Runtime.GetNSObject(sender);
			
			if (obj != null && ObjCExtensions.SuperClassInstancesRespondsToSelector(obj, sel))
				Messaging.void_objc_msgSendSuper_IntPtr(obj.SuperHandle, sel, selector);
		}

		// The NSTextInputClient protocol is added to the *class*, not the instance, so every control of that type
		// conforms as soon as any single one of them handles the TextInput event.  The two overrides below report
		// the truth per instance so the system only treats the controls that actually handle text input as text
		// input - otherwise it adds things like AutoFill to the context menu of every control of that type.
		static bool HandlesTextInput(IntPtr sender)
		{
			return MacBase.GetHandler(Runtime.GetNSObject(sender)) is IMacViewHandler handler && handler.HandlesTextInput;
		}

		internal static IntPtr InputContext_Selector = Selector.GetHandle("inputContext");
		internal static MarshalDelegates.Func_IntPtr_IntPtr_IntPtr InputContext_Delegate = InputContext;
		static IntPtr InputContext(IntPtr sender, IntPtr sel)
		{
			if (!HandlesTextInput(sender))
				return IntPtr.Zero;

			var obj = Runtime.GetNSObject(sender);
			return Messaging.IntPtr_objc_msgSendSuper(obj.SuperHandle, sel);
		}

		internal static IntPtr ConformsToProtocol_Selector = Selector.GetHandle("conformsToProtocol:");
		internal static MarshalDelegates.Func_IntPtr_IntPtr_IntPtr_bool ConformsToProtocol_Delegate = ConformsToProtocol;
		static bool ConformsToProtocol(IntPtr sender, IntPtr sel, IntPtr protocol)
		{
			if (protocol == NSTextInputClientProtocol_Handle && !HandlesTextInput(sender))
				return false;

			var obj = Runtime.GetNSObject(sender);
			return Messaging.bool_objc_msgSendSuper_IntPtr(obj.SuperHandle, sel, protocol);
		}

		internal static IntPtr NSTextInputClientProtocol_Handle = ObjCExtensions.GetProtocolHandle("NSTextInputClient");

		static Selector selString = new Selector("string");
		static string GetText(IntPtr text)
		{
			if (text == IntPtr.Zero)
				return string.Empty;

			var obj = Runtime.GetNSObject(text);
			if (obj == null)
				return string.Empty;

			if (obj is NSString str)
				return str.ToString();

			if (obj.RespondsToSelector(selString))
			{
				var stringHandle = Messaging.IntPtr_objc_msgSend(obj.Handle, selString.Handle);
				return (string)Runtime.GetNSObject<NSString>(stringHandle) ?? string.Empty;
			}

			return obj.ToString() ?? string.Empty;
		}
	}

	partial class MacView<TControl, TWidget, TCallback>
	{
		public virtual NSView TextInputControl => EventControl;
		public bool EnsureTextInputImplemented(NSView view = null)
		{
			view = view ?? TextInputControl;

			// this control actually handles text input, as opposed to merely being an instance of a class that
			// had the NSTextInputClient protocol added to it by another control.  See MacViewTextInput.InputContext.
			HandlesTextInput = true;

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

			// the protocol above is added to the class, so keep the instances of it that don't handle text input
			// from being treated as a text input client
			AddMethod(MacViewTextInput.InputContext_Selector, MacViewTextInput.InputContext_Delegate, "@@:", view);
			AddMethod(MacViewTextInput.ConformsToProtocol_Selector, MacViewTextInput.ConformsToProtocol_Delegate, "B@:@", view);

			AddMethod(MacView.selInsertTextReplacementRange, MacView.TriggerTextInput_Delegate, "v@:@{NSRange=QQ}", view);
			return true;
		}

	}
}
