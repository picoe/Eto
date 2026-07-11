using Eto.Mac.Forms.Controls;
using Eto.Mac.Forms.Menu;


namespace Eto.Mac.Forms
{
	public class MacFieldEditor : NSTextView, IMacControl
	{
		public MacFieldEditor()
		{
			FieldEditor = true;
		}

		public MacFieldEditor(IntPtr handle)
			: base(handle)
		{
		}

		public IMacControl MacControl => WeakDelegate as IMacControl;
		public object Handler => MacControl?.WeakHandler?.Target;

		public IMacViewHandler MacViewHandler => Handler as IMacViewHandler;

		WeakReference IMacControl.WeakHandler
		{
			get => MacControl?.WeakHandler;
			set { }
		}

		public override void KeyDown(NSEvent theEvent)
		{
			var handler = Handler as IMacViewHandler;
			if (handler != null && MacEventView.KeyDown(handler.Widget, theEvent))
				return;

			base.KeyDown(theEvent);
		}

		public override void FlagsChanged(NSEvent theEvent)
		{
			var handler = Handler as IMacViewHandler;
			if (handler != null && MacEventView.FlagsChanged(handler.Widget, theEvent))
				return;

			base.FlagsChanged(theEvent);
		}

		void MouseDownEvent(NSEvent theEvent, Action<NSEvent> baseMethod)
		{
			var handler = Handler as IMacViewHandler;
			if (handler == null)
			{
				baseMethod(theEvent);
				return;
			}

			if (handler.Widget?.IsDisposed != false) return;

			if (handler.SuppressMouseEvents > 0)
			{
				// we can get called from a MouseDown from the owning object
				baseMethod(theEvent);
				return;
			}

			var args = MacConversions.GetMouseEvent(handler, theEvent, false);
			if (theEvent.ClickCount >= 2)
				handler.Callback.OnMouseDoubleClick(handler.Widget, args);

			if (!args.Handled)
			{
				handler.Callback.OnMouseDown(handler.Widget, args);
			}

			if (!args.Handled)
			{
				baseMethod(theEvent);

				// trigger mouse up here, if needed				
				handler.TriggerMouseCallback();
			}
		}

		bool MouseUpEvent(NSEvent theEvent)
		{
			var handler = Handler as IMacViewHandler;
			if (handler == null)
				return false;

			if (handler.Widget?.IsDisposed != false) return false;

			var args = MacConversions.GetMouseEvent(handler, theEvent, false);
			handler.Callback.OnMouseUp(handler.Widget, args);
			return args.Handled;
		}

		public override void MouseDown(NSEvent theEvent)
		{
			MouseDownEvent(theEvent, base.MouseDown);
		}

		public override void MouseUp(NSEvent theEvent)
		{
			if (!MouseUpEvent(theEvent))
				base.MouseUp(theEvent);
		}

		public override void RightMouseDown(NSEvent theEvent)
		{
			MouseDownEvent(theEvent, base.RightMouseDown);
		}

		public override void RightMouseUp(NSEvent theEvent)
		{
			if (!MouseUpEvent(theEvent))
				base.RightMouseUp(theEvent);
		}

		public override void OtherMouseDown(NSEvent theEvent)
		{
			MouseDownEvent(theEvent, base.OtherMouseDown);
		}

		public override void OtherMouseUp(NSEvent theEvent)
		{
			if (!MouseUpEvent(theEvent))
				base.OtherMouseUp(theEvent);
		}

		// Set by an action method (paste:/cut:) that is in progress, so the resulting
		// ShouldChangeText can report the semantic source instead of guessing from the event.
		TextChangeSource? pendingSource;

		// True while AppKit is updating the marked (provisional) text of an in-progress IME
		// composition. TextChanging is suppressed for those updates and only fired once the
		// composition commits (via insertText:), matching WPF/Windows behavior.
		bool settingMarkedText;

		// The text of an in-progress insertText: call. TextInput is raised from ShouldChangeText
		// (after TextChanging and before the edit is applied) rather than directly in the
		// insertText: override, so its ordering and cancellation match WPF: TextChanging fires
		// first, then TextInput, and either can cancel the edit.
		NSString pendingTextInput;
		bool pendingTextInputActive;

		public override bool ShouldChangeText(NSRange affectedCharRange, string replacementString)
		{
			// Provisional marked-text updates during an IME composition are not committed changes
			// and can't be meaningfully validated/cancelled, so don't report them. TextChanging is
			// fired once the composition commits, which arrives here through insertText: instead.
			var handler = settingMarkedText ? null : Handler as IMacTextBoxHandler;
			if (handler != null)
			{
				var source = GetChangeSource();
				pendingSource = null;
				var args = new TextChangingEventArgs(replacementString, affectedCharRange.ToEto(), source);
				handler.Callback.OnTextChanging(handler.Widget, args);
				if (args.Cancel)
				{
					// TextChanging rejected the edit, so don't raise the paired TextInput (matches WPF).
					pendingTextInputActive = false;
					return false;
				}
			}

			// TextInput fires after TextChanging but still before the edit is applied, so it can cancel.
			if (pendingTextInputActive && FireTextInput())
				return false;

			return base.ShouldChangeText(affectedCharRange, replacementString);
		}

		public override void SetMarkedText(NSObject text, NSRange selectedRange, NSRange replacementRange)
		{
			settingMarkedText = true;
			try
			{
				base.SetMarkedText(text, selectedRange, replacementRange);
			}
			finally
			{
				settingMarkedText = false;
			}
		}

		// AppKit funnels every edit (typing, paste, cut, delete, composition) through the single
		// ShouldChangeText delegate, so the semantic source is derived here: an action method
		// (paste:/cut:) sets it explicitly, otherwise it's inferred from the composition state
		// and finally the current event as a coarse fallback.
		TextChangeSource GetChangeSource()
		{
			if (pendingSource != null)
				return pendingSource.Value;

			// Commit of an input method / composition session (e.g. dead keys, CJK input). The
			// provisional marked-text updates are suppressed in ShouldChangeText; this fires for the
			// committing insertText:, at which point the marked text is still present.
			if (HasMarkedText)
				return TextChangeSource.Composition;

			// Typing and keyboard deletions (Delete/Backspace/word deletes) route through a key-down
			// event; whether text is being inserted or deleted is derivable from the empty replacement
			// string. Anything else (drag/drop, dictation, services) that doesn't go through an action
			// method is reported as unknown.
			var currentEvent = NSApplication.SharedApplication.CurrentEvent;
			if (currentEvent != null && currentEvent.Type == NSEventType.KeyDown)
				return TextChangeSource.Keyboard;

			return TextChangeSource.Unknown;
		}

		public override void Paste(NSObject sender)
		{
			pendingSource = TextChangeSource.Paste;
			base.Paste(sender);
			pendingSource = null;
		}

		public override void PasteAsPlainText(NSObject sender)
		{
			pendingSource = TextChangeSource.Paste;
			base.PasteAsPlainText(sender);
			pendingSource = null;
		}

		public override void Cut(NSObject sender)
		{
			pendingSource = TextChangeSource.Cut;
			base.Cut(sender);
			pendingSource = null;
		}

		[Export("menuForEvent:")]
		public NSMenu OnMenuForEvent(NSEvent theEvent)
		{
			var handler = MacViewHandler;
			if (handler != null)
			{
				var nativeMenu = (handler.Widget.ContextMenu?.Handler as ContextMenuHandler)?.Control;
				return nativeMenu;
			}
			return null;
		}


		public override bool ResignFirstResponder()
		{
			var handler = Handler as IMacViewHandler;
			if (handler != null && handler.ShouldHaveFocus == null)
			{
				handler.ShouldHaveFocus = false;
				// for some reason calling base.ResignFirstResponder calls this method again???
				var result = base.ResignFirstResponder();
				handler.Callback.OnLostFocus(handler.Widget, EventArgs.Empty);
				handler.ShouldHaveFocus = null;
				return result;
			}
			return base.ResignFirstResponder();
		}

		public override void InsertText(NSObject text, NSRange replacementRange)
		{
			// Defer TextInput to ShouldChangeText (which base.InsertText routes through) so it fires
			// after TextChanging. Only plain NSString input raises TextInput, as before.
			pendingTextInput = text as NSString;
			pendingTextInputActive = pendingTextInput != null;
			try
			{
				base.InsertText(text, replacementRange);

				// Fallback: if the insert didn't route through ShouldChangeText, still deliver TextInput.
				if (pendingTextInputActive)
					FireTextInput();
			}
			finally
			{
				pendingTextInputActive = false;
				pendingTextInput = null;
			}
		}

		// Raises the Control-level TextInput event for the pending insertText: text, returning true
		// if it was cancelled (so the edit should be rejected).
		bool FireTextInput()
		{
			pendingTextInputActive = false;
			var handler = Handler as IMacViewHandler;
			if (handler == null || pendingTextInput == null)
				return false;
			var args = new TextInputEventArgs(pendingTextInput);
			handler.Callback.OnTextInput(handler.Widget, args);
			return args.Cancel;
		}
	}
}
