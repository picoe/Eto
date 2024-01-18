using Eto.Mac.Forms.Controls;
using Eto.Mac.Forms.Printing;
#if MACOS_NET
using NSDraggingInfo = AppKit.INSDraggingInfo;
#endif


namespace Eto.Mac.Forms
{
	class MouseDelegate : NSObject
	{
		WeakReference _widget;
		bool _entered;

		public IMacViewHandler Handler { get => (IMacViewHandler)_widget.Target; set => _widget = new WeakReference(value); }

		public static HashSet<MouseDelegate> EnteredControls = new HashSet<MouseDelegate>();

		[Export("mouseMoved:")]
		public void MouseMoved(NSEvent theEvent)
		{
			var h = Handler;
			if (h == null || !h.Enabled) return;
			h.Callback.OnMouseMove(h.Widget, MacConversions.GetMouseEvent(h, theEvent, false));
		}

		[Export("mouseEntered:")]
		public void MouseEntered(NSEvent theEvent)
		{
			// we could be entered already after using CaptureMouse()
			var h = Handler;
			if (h == null || !h.Enabled || _entered) return;
			_entered = true;
			// Debug.WriteLine($"MouseEnter: {h.Widget.GetType()}");
			h.Callback.OnMouseEnter(h.Widget, MacConversions.GetMouseEvent(h, theEvent, false));
			EnteredControls.Add(this);
		}

		[Export("cursorUpdate:")]
		public void CursorUpdate(NSEvent theEvent)
		{
		}

		[Export("mouseExited:")]
		public void MouseExited(NSEvent theEvent)
		{
			var h = Handler;
			if (h == null || !h.Enabled || !_entered) return;
			_entered = false;
			// Debug.WriteLine($"MouseLeave: {h.Widget.GetType()}");
			h.Callback.OnMouseLeave(h.Widget, MacConversions.GetMouseEvent(h, theEvent, false));
			EnteredControls.Remove(this);
		}

		[Export("scrollWheel:")]
		public void ScrollWheel(NSEvent theEvent)
		{
			var h = Handler;
			if (h == null) return;
			h.Callback.OnMouseWheel(h.Widget, MacConversions.GetMouseEvent(h, theEvent, true));
		}
		
		public void FireMouseLeaveIfNeeded(bool async)
		{
			var h = Handler;
			if (h == null || !_entered) return;
			_entered = false;
			var theEvent = NSApplication.SharedApplication.CurrentEvent;
			var args = MacConversions.GetMouseEvent(h, theEvent, false);
			if (async)
			{
				Application.Instance.AsyncInvoke(() =>
				{
					if (h.Widget.IsDisposed)
						return;
					h.Callback.OnMouseLeave(h.Widget, args);
				});
			}
			else
			{
				h.Callback.OnMouseLeave(h.Widget, args);
			}
		}

		public void FireMouseEnterIfNeeded(bool async)
		{
			var h = Handler;
			if (h == null || _entered) return;
			_entered = true;
			var theEvent = NSApplication.SharedApplication.CurrentEvent;
			var args = MacConversions.GetMouseEvent(h, theEvent, false);
			if (async)
			{
				Application.Instance.AsyncInvoke(() =>
				{
					if (h.Widget.IsDisposed)
						return;
					h.Callback.OnMouseEnter(h.Widget, args);
				});
			}
			else
			{
				h.Callback.OnMouseEnter(h.Widget, args);
			}
		}
		
	}

	public interface IMacViewHandler : IMacControlHandler
	{
		Size UserPreferredSize { get; }

		Control Widget { get; }

		Control.ICallback Callback { get; }

		Cursor CurrentCursor { get; }

		Color BackgroundColor { get; set; }

		Dictionary<IntPtr, Command> SystemActions { get; }

		void OnKeyDown(KeyEventArgs e);

		void OnKeyUp(KeyEventArgs e);

		void OnSizeChanged(EventArgs e);

		bool? ShouldHaveFocus { get; set; }
		int SuppressMouseEvents { get; set; }
		bool TextInputCancelled { get; set; }
		bool TextInputImplemented { get; }
		bool UseNSBoxBackgroundColor { get; set; }
		bool Enabled { get; set; }

		DragEventArgs GetDragEventArgs(NSDraggingInfo info, object customControl);

		void SetEnabled(bool parentEnabled);

		void SetAlignmentFrameSize(CGSize size);
		void SetAlignmentFrame(CGRect frame);
		CGRect GetAlignmentFrame();
		CGSize GetAlignmentSizeForSize(CGSize size);
		CGPoint GetAlignmentPointForFramePoint(CGPoint point);
		CGRect GetAlignmentRectForFrame(CGRect frame);
		bool OnAcceptsFirstMouse(NSEvent theEvent);
		bool TriggerMouseCallback(NSEvent theEvent = null, bool includeMouseDown = true);
		MouseEventArgs TriggerMouseDown(NSObject obj, IntPtr sel, NSEvent theEvent);
		MouseEventArgs TriggerMouseUp(NSObject obj, IntPtr sel, NSEvent theEvent);
		void UpdateTrackingAreas();
		void OnViewDidMoveToWindow();
		bool AutoAttachNative { get; set; }
		void FireMouseEnterIfNeeded();
		void FireMouseLeaveIfNeeded();
	}

	static partial class MacView
	{
		public static readonly object AutoSize_Key = new object();
		public static readonly object MinimumSize_Key = new object();
		public static readonly object MaximumSize_Key = new object();
		public static readonly object UserPreferredSize_Key = new object();
		public static readonly object NaturalAvailableSize_Key = new object();
		public static readonly object NaturalSize_Key = new object();
		public static readonly object NaturalSizeInfinity_Key = new object();
		public static readonly object Enabled_Key = new object();
		public static readonly object ActualEnabled_Key = new object();
		public static readonly object AcceptsFirstMouse_Key = new object();
		public static readonly object TextInputCancelled_Key = new object();
		public static readonly object TextInputImplemented_Key = new object();
		public static readonly object AutoAttachNative_Key = new object();
		public static readonly IntPtr selMouseDown = Selector.GetHandle("mouseDown:");
		public static readonly IntPtr selMouseUp = Selector.GetHandle("mouseUp:");
		public static readonly IntPtr selMouseDragged = Selector.GetHandle("mouseDragged:");
		public static readonly IntPtr selRightMouseDown = Selector.GetHandle("rightMouseDown:");
		public static readonly IntPtr selRightMouseUp = Selector.GetHandle("rightMouseUp:");
		public static readonly IntPtr selRightMouseDragged = Selector.GetHandle("rightMouseDragged:");
		public static readonly IntPtr selOtherMouseDown = Selector.GetHandle("otherMouseDown:");
		public static readonly IntPtr selOtherMouseUp = Selector.GetHandle("otherMouseUp:");
		public static readonly IntPtr selOtherMouseDragged = Selector.GetHandle("otherMouseDragged:");
		public static readonly IntPtr selScrollWheel = Selector.GetHandle("scrollWheel:");
		public static readonly IntPtr selFlagsChanged = Selector.GetHandle("flagsChanged:");
		public static readonly IntPtr selKeyDown = Selector.GetHandle("keyDown:");
		public static readonly IntPtr selKeyUp = Selector.GetHandle("keyUp:");
		public static readonly IntPtr selBecomeFirstResponder = Selector.GetHandle("becomeFirstResponder");
		public static readonly IntPtr selSetFrameSize = Selector.GetHandle("setFrameSize:");
		public static readonly IntPtr selResignFirstResponder = Selector.GetHandle("resignFirstResponder");
		public static readonly IntPtr selInsertTextReplacementRange = Selector.GetHandle("insertText:replacementRange:");
		public static readonly IntPtr selDraggingEntered = Selector.GetHandle("draggingEntered:");
		public static readonly IntPtr selDraggingExited = Selector.GetHandle("draggingExited:");
		public static readonly IntPtr selDraggingUpdated = Selector.GetHandle("draggingUpdated:");
		public static readonly IntPtr selPerformDragOperation = Selector.GetHandle("performDragOperation:");
		public static readonly object InitialFocusKey = new object();
		public static readonly object BackgroundColorKey = new object();
		public static readonly IntPtr selDrawRect = Selector.GetHandle("drawRect:");
		public static readonly IntPtr selUpdateLayer = Selector.GetHandle("updateLayer");
		public static readonly object ShouldHaveFocusKey = new object();
		public static readonly IntPtr selResetCursorRects = Selector.GetHandle("resetCursorRects");
		public static readonly object Cursor_Key = new object();
		public static readonly IntPtr selGetAction = Selector.GetHandle("action");
		public static readonly IntPtr selValidateUserInterfaceItem = Selector.GetHandle("validateUserInterfaceItem:");
		public static readonly IntPtr selCut = Selector.GetHandle("cut:");
		public static readonly IntPtr selCopy = Selector.GetHandle("copy:");
		public static readonly IntPtr selPaste = Selector.GetHandle("paste:");
		public static readonly IntPtr selSelectAll = Selector.GetHandle("selectAll:");
		public static readonly IntPtr selDelete = Selector.GetHandle("delete:");
		public static readonly IntPtr selUndo = Selector.GetHandle("undo:");
		public static readonly IntPtr selRedo = Selector.GetHandle("redo:");
		public static readonly IntPtr selPasteAsPlainText = Selector.GetHandle("pasteAsPlainText:");
		public static readonly IntPtr selPerformClose = Selector.GetHandle("performClose:");
		public static readonly IntPtr selPerformZoom = Selector.GetHandle("performZoom:");
		public static readonly IntPtr selArrangeInFront = Selector.GetHandle("arrangeInFront:");
		public static readonly IntPtr selPerformMiniaturize = Selector.GetHandle("performMiniaturize:");
		public static readonly IntPtr selUpdateTrackingAreas = Selector.GetHandle("updateTrackingAreas");
		public static readonly Dictionary<string, IntPtr> systemActionSelectors = new Dictionary<string, IntPtr>
		{
			{ "cut", selCut },
			{ "copy", selCopy },
			{ "paste", selPaste },
			{ "selectAll", selSelectAll },
			{ "delete", selDelete },
			{ "undo", selUndo },
			{ "redo", selRedo },
			{ "pasteAsPlainText", selPasteAsPlainText },
			{ "performClose", selPerformClose },
			{ "performZoom", selPerformZoom },
			{ "arrangeInFront", selArrangeInFront },
			{ "performMiniaturize", selPerformMiniaturize }
		};
		public static readonly object TabIndex_Key = new object();
		public static readonly object AllowDrop_Key = new object();
		public static readonly Selector selSetCanDrawSubviewsIntoLayer = new Selector("setCanDrawSubviewsIntoLayer:");
		public static readonly bool supportsCanDrawSubviewsIntoLayer = ObjCExtensions.InstancesRespondToSelector<NSView>("setCanDrawSubviewsIntoLayer:");
		public static readonly object UseAlignmentFrame_Key = new object();
		public static readonly object SuppressMouseEvents_Key = new object();
		public static readonly object SuppressMouseUp_Key = new object();
		public static readonly object UseMouseTrackingLoop_Key = new object();
		public static readonly object MouseTrackingRunLoopMode_Key = new object();
		public static readonly object UseNSBoxBackgroundColor_Key = new object();
		public static readonly IntPtr selSetDataProviderForTypes_Handle = Selector.GetHandle("setDataProvider:forTypes:");
		public static readonly IntPtr selInitWithPasteboardWriter_Handle = Selector.GetHandle("initWithPasteboardWriter:");
		public static readonly IntPtr selClass_Handle = Selector.GetHandle("class");
		public const string FlagsChangedEvent = "MacView.FlagsChangedEvent";

		// before 10.12, we have to call base.Layout() AFTER we do our layout otherwise it doesn't work correctly..
		// however, that causes (temporary) glitches when resizing especially with Scrollable >= 10.12
		public static readonly bool NewLayout = MacVersion.IsAtLeast(10, 12);
		
		internal static MarshalDelegates.Action_IntPtr_IntPtr TriggerUpdateTrackingAreas_Delegate = TriggerUpdateTrackingAreas;
		static void TriggerUpdateTrackingAreas(IntPtr sender, IntPtr sel)
		{
			var obj = Runtime.GetNSObject(sender);
			
			Messaging.void_objc_msgSendSuper(obj.SuperHandle, sel);
			
			if (MacBase.GetHandler(obj) is IMacViewHandler handler)
			{
				handler.UpdateTrackingAreas();
			}
		}
		
		internal static MarshalDelegates.Action_IntPtr_IntPtr_IntPtr TriggerMouseDragged_Delegate = TriggerMouseDragged;
		static void TriggerMouseDragged(IntPtr sender, IntPtr sel, IntPtr e)
		{
			var obj = Runtime.GetNSObject(sender);
			if (MacBase.GetHandler(obj) is IMacViewHandler handler)
			{
				var theEvent = Messaging.GetNSObject<NSEvent>(e);
				var args = MacConversions.GetMouseEvent(handler, theEvent, false);
				handler.Callback.OnMouseMove(handler.Widget, args);
				if (!args.Handled)
				{
					Messaging.void_objc_msgSendSuper_IntPtr(obj.SuperHandle, sel, e);
				}
			}
		}

		internal static MarshalDelegates.Action_IntPtr_IntPtr_CGSize SetFrameSize_Delegate = SetFrameSizeAction;
		static void SetFrameSizeAction(IntPtr sender, IntPtr sel, CGSize size)
		{
			var obj = Runtime.GetNSObject(sender);
			Messaging.void_objc_msgSendSuper_SizeF(obj.SuperHandle, sel, size);

			if (MacBase.GetHandler(obj) is IMacViewHandler handler)
			{
				handler.OnSizeChanged(EventArgs.Empty);
				handler.Callback.OnSizeChanged(handler.Widget, EventArgs.Empty);
			}
		}

		internal static MarshalDelegates.Action_IntPtr_IntPtr_IntPtr TriggerMouseDown_Delegate = TriggerMouseDown;
		static void TriggerMouseDown(IntPtr sender, IntPtr sel, IntPtr e)
		{
			var obj = Runtime.GetNSObject(sender);
			if (MacBase.GetHandler(obj) is IMacViewHandler handler)
			{
				var theEvent = Messaging.GetNSObject<NSEvent>(e);
				handler.TriggerMouseDown(obj, sel, theEvent);
			}
		}

		internal static MarshalDelegates.Action_IntPtr_IntPtr_IntPtr TriggerMouseUp_Delegate = TriggerMouseUp;
		static void TriggerMouseUp(IntPtr sender, IntPtr sel, IntPtr e)
		{
			var obj = Runtime.GetNSObject(sender);

			if (MacBase.GetHandler(obj) is IMacViewHandler handler && handler.Enabled)
			{
				var theEvent = Messaging.GetNSObject<NSEvent>(e);
				handler.TriggerMouseUp(obj, sel, theEvent);
			}
		}

		internal static MarshalDelegates.Action_IntPtr_IntPtr_IntPtr TriggerMouseWheel_Delegate = TriggerMouseWheel;
		static void TriggerMouseWheel(IntPtr sender, IntPtr sel, IntPtr e)
		{
			var obj = Runtime.GetNSObject(sender);
			if (MacBase.GetHandler(obj) is IMacViewHandler handler && handler.Enabled)
			{
				var theEvent = Messaging.GetNSObject<NSEvent>(e);
				var args = MacConversions.GetMouseEvent(handler, theEvent, true);
				if (!args.Delta.IsZero)
				{
					handler.Callback.OnMouseWheel(handler.Widget, args);
				}
				if (!args.Handled)
				{
					Messaging.void_objc_msgSendSuper_IntPtr(obj.SuperHandle, sel, e);
				}
			}
			else
				Messaging.void_objc_msgSendSuper_IntPtr(obj.SuperHandle, sel, e);
		}

		static int _interpretingKeyEvents;
		internal static MarshalDelegates.Action_IntPtr_IntPtr_IntPtr TriggerKeyDown_Delegate = TriggerKeyDown;
		static void TriggerKeyDown(IntPtr sender, IntPtr sel, IntPtr e)
		{
			var obj = Runtime.GetNSObject(sender);
			if (MacBase.GetHandler(obj) is IMacViewHandler handler)
			{
				var theEvent = Messaging.GetNSObject<NSEvent>(e);
				if (!MacEventView.KeyDown(handler.Widget, theEvent))
				{
					if (handler.TextInputImplemented && _interpretingKeyEvents == 0)
					{
						_interpretingKeyEvents++;
						// sending this twice for the same event actually makes it go to the same control.. 
						handler.TextInputControl.InterpretKeyEvents(new [] { theEvent });
						
						if (!handler.TextInputCancelled)
							Messaging.void_objc_msgSendSuper_IntPtr(obj.SuperHandle, sel, e);
							
						_interpretingKeyEvents--;
					}
					else
					{
						Messaging.void_objc_msgSendSuper_IntPtr(obj.SuperHandle, sel, e);
					}
				}
			}
		}

		internal static MarshalDelegates.Action_IntPtr_IntPtr_IntPtr TriggerFlagsChanged_Delegate = TriggerFlagsChanged;
		static void TriggerFlagsChanged(IntPtr sender, IntPtr sel, IntPtr e)
		{
			var obj = Runtime.GetNSObject(sender);
			if (MacBase.GetHandler(obj) is IMacViewHandler handler)
			{
				var theEvent = Messaging.GetNSObject<NSEvent>(e);
				if (!MacEventView.FlagsChanged(handler.Widget, theEvent))
				{
					Messaging.void_objc_msgSendSuper_IntPtr(obj.SuperHandle, sel, e);
				}
			}
		}

		internal static MarshalDelegates.Action_IntPtr_IntPtr_IntPtr TriggerKeyUp_Delegate = TriggerKeyUp;
		static void TriggerKeyUp(IntPtr sender, IntPtr sel, IntPtr e)
		{
			var obj = Runtime.GetNSObject(sender);
			var handler = MacBase.GetHandler(obj) as IMacViewHandler;
			if (handler != null)
			{
				var theEvent = Messaging.GetNSObject<NSEvent>(e);
				if (!MacEventView.KeyUp(handler.Widget, theEvent))
				{
					Messaging.void_objc_msgSendSuper_IntPtr(obj.SuperHandle, sel, e);
				}
			}
		}

		internal static MarshalDelegates.Func_IntPtr_IntPtr_IntPtr_bool TriggerPerformDragOperation_Delegate = TriggerPerformDragOperation;
		static bool TriggerPerformDragOperation(IntPtr sender, IntPtr sel, IntPtr draggingInfoPtr)
		{
			var handler = MacBase.GetHandler(Runtime.GetNSObject(sender)) as IMacViewHandler;
			var e = handler?.GetDragEventArgs(Runtime.GetINativeObject<NSDraggingInfo>(draggingInfoPtr, false), null);
			if (e != null)
			{
				handler.Callback.OnDragLeave(handler.Widget, e);
				handler.Callback.OnDragDrop(handler.Widget, e);
				return true;
			}
			return false;
		}

		internal static MarshalDelegates.Func_IntPtr_IntPtr_IntPtr_NSDragOperation TriggerDraggingUpdated_Delegate = TriggerDraggingUpdated;
		static NSDragOperation TriggerDraggingUpdated(IntPtr sender, IntPtr sel, IntPtr draggingInfoPtr)
		{
			var obj = Runtime.GetNSObject(sender);
			var effect = (NSDragOperation)Messaging.IntPtr_objc_msgSendSuper_IntPtr(obj.SuperHandle, sel, draggingInfoPtr);
			var handler = MacBase.GetHandler(Runtime.GetNSObject(sender)) as IMacViewHandler;
			var e = handler?.GetDragEventArgs(Runtime.GetINativeObject<NSDraggingInfo>(draggingInfoPtr, false), null);
			if (e != null)
			{

				e.Effects = effect.ToEto();
				handler.Callback.OnDragOver(handler.Widget, e);
				if (e.AllowedEffects.HasFlag(e.Effects))
					effect = e.Effects.ToNS();
			}
			return effect;
		}

		internal static MarshalDelegates.Func_IntPtr_IntPtr_IntPtr_NSDragOperation TriggerDraggingEntered_Delegate = TriggerDraggingEntered;
		static NSDragOperation TriggerDraggingEntered(IntPtr sender, IntPtr sel, IntPtr draggingInfoPtr)
		{
			var obj = Runtime.GetNSObject(sender);
			var effect = (NSDragOperation)Messaging.IntPtr_objc_msgSendSuper_IntPtr(obj.SuperHandle, sel, draggingInfoPtr);
			var handler = MacBase.GetHandler(Runtime.GetNSObject(sender)) as IMacViewHandler;
			var draggingInfo = Runtime.GetINativeObject<NSDraggingInfo>(draggingInfoPtr, false);
			var e = handler?.GetDragEventArgs(draggingInfo, null);
			if (e != null)
			{
				e.Effects = effect.ToEto();
				handler.Callback.OnDragEnter(handler.Widget, e);
				if (e.AllowedEffects.HasFlag(e.Effects))
					effect = e.Effects.ToNS();
			}
			return effect;
		}

		internal static MarshalDelegates.Action_IntPtr_IntPtr_IntPtr TriggerDraggingExited_Delegate = TriggerDraggingExited;
		static void TriggerDraggingExited(IntPtr sender, IntPtr sel, IntPtr draggingInfoPtr)
		{
			var obj = Runtime.GetNSObject(sender);
			var handler = MacBase.GetHandler(Runtime.GetNSObject(sender)) as IMacViewHandler;
			var e = handler?.GetDragEventArgs(Runtime.GetINativeObject<NSDraggingInfo>(draggingInfoPtr, false), null);
			if (e != null)
			{
				handler.Callback.OnDragLeave(handler.Widget, e);
			}
			Messaging.void_objc_msgSendSuper_IntPtr(obj.SuperHandle, sel, draggingInfoPtr);
		}
		

		internal static MarshalDelegates.Action_IntPtr_IntPtr_IntPtr_NSRange TriggerTextInput_Delegate = TriggerTextInput;
		static void TriggerTextInput(IntPtr sender, IntPtr sel, IntPtr textPtr, NSRange range)
		{
			var obj = Runtime.GetNSObject(sender);

			if (MacBase.GetHandler(obj) is IMacViewHandler handler)
			{
				handler.TextInputCancelled = false;
				var text = (string)Messaging.GetNSObject<NSString>(textPtr);
				var args = new TextInputEventArgs(text);
				handler.Callback.OnTextInput(handler.Widget, args);
				if (args.Cancel)
				{
					handler.TextInputCancelled = true;
					return;
				}
			}
			
			if (ObjCExtensions.SuperClassInstancesRespondsToSelector(obj, sel))
				Messaging.void_objc_msgSendSuper_IntPtr_NSRange(obj.SuperHandle, sel, textPtr, range);
		}


		internal static MarshalDelegates.Func_IntPtr_IntPtr_bool TriggerGotFocus_Delegate = TriggerGotFocus;
		static bool TriggerGotFocus(IntPtr sender, IntPtr sel)
		{
			var obj = Runtime.GetNSObject(sender);
			if (MacBase.GetHandler(obj) is IMacViewHandler handler)
			{
				handler.ShouldHaveFocus = true;
				var result = Messaging.bool_objc_msgSendSuper(obj.SuperHandle, sel);
				handler.Callback.OnGotFocus(handler.Widget, EventArgs.Empty);
				handler.ShouldHaveFocus = null;
				return result;
			}
			return false;
		}

		internal static MarshalDelegates.Func_IntPtr_IntPtr_bool TriggerLostFocus_Delegate = TriggerLostFocus;
		static bool TriggerLostFocus(IntPtr sender, IntPtr sel)
		{
			var obj = Runtime.GetNSObject(sender);
			if (MacBase.GetHandler(obj) is IMacViewHandler handler)
			{
				handler.ShouldHaveFocus = false;
				var result = Messaging.bool_objc_msgSendSuper(obj.SuperHandle, sel);
				handler.Callback.OnLostFocus(handler.Widget, EventArgs.Empty);
				handler.ShouldHaveFocus = null;
				return result;
			}
			return false;
		}

		internal static MarshalDelegates.Action_IntPtr_IntPtr_CGRect DrawBackgroundRect_Delegate = DrawBackgroundRect;
		static void DrawBackgroundRect(IntPtr sender, IntPtr sel, CGRect rect)
		{
			var control = Runtime.GetNSObject(sender);
			if (MacBase.GetHandler(control) is IMacViewHandler handler)
			{
				var col = handler.BackgroundColor;
				if (col.A > 0)
				{
					var context = NSGraphicsContext.CurrentContext.GraphicsPort;
					col.ToNSUI().SetFill();
					var bounds = handler.GetAlignmentRectForFrame(handler.ContainerControl.Bounds);
					context.FillRect(bounds);
				}
			}
			Messaging.void_objc_msgSendSuper_CGRect(control.SuperHandle, sel, rect);
		}

		static void UpdateLayerWithBackground(IntPtr sender, IntPtr sel)
		{
			var control = Runtime.GetNSObject<NSView>(sender);
			if (MacBase.GetHandler(control) is IMacViewHandler handler)
			{
				control.Layer.BackgroundColor = handler.BackgroundColor.ToCG();
			}
			Messaging.void_objc_msgSendSuper(control.SuperHandle, sel);
		}

		internal static MarshalDelegates.Action_IntPtr_IntPtr TriggerResetCursorRects_Delegate = TriggerResetCursorRects;
		static void TriggerResetCursorRects(IntPtr sender, IntPtr sel)
		{
			var obj = Runtime.GetNSObject(sender);
			var handler = MacBase.GetHandler(obj) as IMacViewHandler;
			if (handler != null)
			{
				var cursor = handler.CurrentCursor;
				if (cursor != null)
				{
					handler.EventControl.AddCursorRect(new CGRect(CGPoint.Empty, handler.EventControl.Frame.Size), cursor.ControlObject as NSCursor);
				}
			}
		}

		internal static MarshalDelegates.Action_IntPtr_IntPtr_IntPtr TriggerSystemAction_Delegate = TriggerSystemAction;
		static void TriggerSystemAction(IntPtr sender, IntPtr sel, IntPtr e)
		{
			var control = Runtime.GetNSObject(sender);
			if (MacBase.GetHandler(control) is IMacViewHandler handler)
			{
				if (handler.SystemActions != null && handler.SystemActions.TryGetValue(sel, out var command))
				{
					if (command != null)
					{
						command.Execute();
						return;
					}
				}
			}
			Messaging.void_objc_msgSendSuper_IntPtr(control.SuperHandle, sel, e);
		}

		internal static MarshalDelegates.Func_IntPtr_IntPtr_IntPtr_bool ValidateSystemUserInterfaceItem_Delegate = ValidateSystemUserInterfaceItem;

		static bool ValidateSystemUserInterfaceItem(IntPtr sender, IntPtr sel, IntPtr item)
		{
			var actionHandle = Messaging.IntPtr_objc_msgSend(item, MacView.selGetAction);

			var control = Runtime.GetNSObject(sender);
			var handler = MacBase.GetHandler(control) as IMacViewHandler;
			if (handler != null)
			{
				Command command;
				if (handler.SystemActions != null && actionHandle != IntPtr.Zero && handler.SystemActions.TryGetValue(actionHandle, out command))
				{
					if (command != null)
						return command.Enabled;
				}
			}
			
			return
				ObjCExtensions.SuperClassInstancesRespondsToSelector(sender, sel)
				&& Messaging.bool_objc_msgSendSuper_IntPtr(control.SuperHandle, sel, item);
		}

		/// <summary>
		/// Flag used to determine if we're in/going to use a mouse tracking event loop.
		/// If during a MouseDown event we do something that buries the MouseUp event from happening,
		/// such as showing a context menu or dialog, this must be set to false.
		/// </summary>
		public static bool InMouseTrackingLoop;

		public static IMacViewHandler CapturedControl;

		public static IntPtr selViewDidMoveToWindow = Selector.GetHandle("viewDidMoveToWindow");

		internal static MarshalDelegates.Action_IntPtr_IntPtr TriggerViewDidMoveToWindow_Delegate = TriggerViewDidMoveToWindow;
		static void TriggerViewDidMoveToWindow(IntPtr sender, IntPtr sel)
		{
			var obj = Runtime.GetNSObject(sender);
			
			Messaging.void_objc_msgSendSuper(obj.SuperHandle, sel);
			
			if (MacBase.GetHandler(obj) is IMacViewHandler handler)
			{
				handler.OnViewDidMoveToWindow();
			}
		}
	}

	public abstract partial class MacView<TControl, TWidget, TCallback> : MacObject<TControl, TWidget, TCallback>, Control.IHandler, IMacViewHandler
		where TControl : NSObject
		where TWidget : Control
		where TCallback : Control.ICallback
	{
		bool mouseMove;
		NSTrackingArea tracking;
		NSTrackingAreaOptions mouseOptions;
		MouseDelegate mouseDelegate;

		public override IntPtr NativeHandle { get { return Control.Handle; } }

		Control.ICallback IMacViewHandler.Callback { get { return Callback; } }

		public abstract NSView ContainerControl { get; }

		public virtual NSView ContentControl => ContainerControl;

		public virtual NSView DragControl => ContainerControl;

		public virtual NSView EventControl => ContainerControl;

		public virtual NSView FocusControl => EventControl;

		public virtual IEnumerable<Control> VisualControls => Enumerable.Empty<Control>();

		protected virtual Size DefaultMinimumSize => Size.Empty;

		public virtual Size MinimumSize
		{
			get { return Widget.Properties.Get<Size?>(MacView.MinimumSize_Key) ?? DefaultMinimumSize; }
			set { Widget.Properties[MacView.MinimumSize_Key] = value; InvalidateMeasure(); }
		}

		public virtual SizeF MaximumSize
		{
			get { return Widget.Properties.Get<SizeF?>(MacView.MaximumSize_Key) ?? SizeF.MaxValue; }
			set { Widget.Properties[MacView.MaximumSize_Key] = value; InvalidateMeasure(); }
		}

		public Size UserPreferredSize
		{
			get => Widget.Properties.Get<Size?>(MacView.UserPreferredSize_Key) ?? new Size(-1, -1);
			set
			{
				if (Widget.Properties.TrySet(MacView.UserPreferredSize_Key, value))
					OnUserPreferredSizeChanged();
			}
		}

		protected virtual void OnUserPreferredSizeChanged()
		{
		}

		public virtual Size Size
		{
			get
			{
				if (!Widget.Loaded)
					return UserPreferredSize;
				return GetAlignmentFrame().Size.ToEtoSize();
			}
			set
			{
				var preferredSize = UserPreferredSize;
				if (preferredSize == value)
					return;
				UserPreferredSize = value;

				if (!Widget.Loaded)
				{
					Callback.OnSizeChanged(Widget, EventArgs.Empty);
					return;
				}

				var oldFrame = GetAlignmentFrame();
				var newFrame = oldFrame;
				if (value.Width >= 0)
					newFrame.Width = value.Width;
				if (value.Height >= 0)
					newFrame.Height = value.Height;

				// this doesn't get to our overridden method to handle the event (since it calls [super setFrameSize:]) so trigger event manually.
				SetAlignmentFrameSize(newFrame.Size);
				if (oldFrame.Size != newFrame.Size)
					Callback.OnSizeChanged(Widget, EventArgs.Empty);

				InvalidateMeasure();
			}
		}

		public virtual int Width
		{
			get => Size.Width;
			set => Size = new Size(value, UserPreferredSize.Height);
		}

		public virtual int Height
		{
			get => Size.Height;
			set => Size = new Size(UserPreferredSize.Width, value);
		}

		protected Size? NaturalAvailableSize
		{
			get { return Widget.Properties.Get<Size?>(MacView.NaturalAvailableSize_Key); }
			set { Widget.Properties[MacView.NaturalAvailableSize_Key] = value; }
		}

		protected SizeF? NaturalSize
		{
			get { return Widget.Properties.Get<SizeF?>(MacView.NaturalSize_Key); }
			set { Widget.Properties[MacView.NaturalSize_Key] = value; }
		}

		protected SizeF? NaturalSizeInfinity
		{
			get { return Widget.Properties.Get<SizeF?>(MacView.NaturalSizeInfinity_Key); }
			set { Widget.Properties[MacView.NaturalSizeInfinity_Key] = value; }
		}
		
		public virtual void InvalidateMeasure()
		{
			NaturalSize = null;
			NaturalSizeInfinity = null;

			if (!Widget.Loaded || Widget.IsSuspended)
				return;

			Widget.VisualParent.GetMacControl()?.InvalidateMeasure();
		}

		protected virtual SizeF GetNaturalSize(SizeF availableSize)
		{
			var naturalSize = NaturalSize;
			if (naturalSize != null)
				return naturalSize.Value;
			if (ContainerControl is NSView control)
			{
				naturalSize = GetAlignmentSizeForSize(control.FittingSize).ToEto();

				NaturalSize = naturalSize;
				return naturalSize.Value;
			}
			return Size.Empty;
		}

		public virtual SizeF GetPreferredSize(SizeF availableSize)
		{
			SizeF size;
			var preferredSize = UserPreferredSize;
			// only get natural size if the size isn't explicitly set.
			if (preferredSize.Width == -1 || preferredSize.Height == -1)
			{
				if (preferredSize.Width >= 0)
					availableSize.Width = preferredSize.Width;
				if (preferredSize.Height >= 0)
					availableSize.Height = preferredSize.Height;
				size = GetNaturalSize(availableSize);
			}
			else
				size = SizeF.Empty;

			if (preferredSize.Width >= 0)
				size.Width = preferredSize.Width;
			if (preferredSize.Height >= 0)
				size.Height = preferredSize.Height;

			size =  SizeF.Min(SizeF.Max(size, MinimumSize), MaximumSize);

			return size;
		}
		
		public virtual void UpdateTrackingAreas()
		{
			if (!mouseMove)
				return;
			if (tracking != null)
			{
				EventControl.RemoveTrackingArea(tracking);
				tracking = null;
			}
			//Console.WriteLine ("Adding mouse tracking {0} for area {1}", this.Widget.GetType ().FullName, Control.Frame.Size);
			var frame = new CGRect(CGPoint.Empty, EventControl.Frame.Size);
			if (!frame.IsEmpty)
			{
				if (mouseDelegate == null)
					mouseDelegate = new MouseDelegate { Handler = this };

				frame = GetAlignmentRectForFrame(frame);

				var options = mouseOptions | NSTrackingAreaOptions.ActiveAlways | NSTrackingAreaOptions.EnabledDuringMouseDrag;

				tracking = new NSTrackingArea(frame, options, mouseDelegate, null);
				EventControl.AddTrackingArea(tracking);

				// when scrolling we need to fire the events manually
				if (Widget.Loaded)
				{
					var mousePosition = PointFromScreen(Mouse.Position);
					if (frame.Contains(mousePosition.ToNS()))
						mouseDelegate.FireMouseEnterIfNeeded(false);
					else
						mouseDelegate.FireMouseLeaveIfNeeded(false);
				}
			}
		}

		public virtual void SetParent(Container oldParent, Container newParent)
		{
			SetEnabled(ParentEnabled);
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case Eto.Forms.Control.EnabledChangedEvent:
					// handled manually
					break;
				case Eto.Forms.Control.MouseEnterEvent:
					HandleEvent(Eto.Forms.Control.MouseLeaveEvent);
					break;
				case Eto.Forms.Control.MouseLeaveEvent:
					mouseOptions |= NSTrackingAreaOptions.MouseEnteredAndExited;
					mouseMove = true;
					AddMethod(MacView.selUpdateTrackingAreas, MacView.TriggerUpdateTrackingAreas_Delegate, "v@:@", EventControl);
					EventControl.UpdateTrackingAreas();
					break;
				case Eto.Forms.Control.MouseMoveEvent:
					mouseOptions |= NSTrackingAreaOptions.MouseMoved;
					mouseMove = true;
					AddMethod(MacView.selUpdateTrackingAreas, MacView.TriggerUpdateTrackingAreas_Delegate, "v@:@", EventControl);
					EventControl.UpdateTrackingAreas();
					AddMethod(MacView.selMouseDragged, MacView.TriggerMouseDragged_Delegate, "v@:@");
					AddMethod(MacView.selRightMouseDragged, MacView.TriggerMouseDragged_Delegate, "v@:@");
					AddMethod(MacView.selOtherMouseDragged, MacView.TriggerMouseDragged_Delegate, "v@:@");
					break;
				case Eto.Forms.Control.SizeChangedEvent:
					AddMethod(MacView.selSetFrameSize, MacView.SetFrameSize_Delegate, EtoEnvironment.Is64BitProcess ? "v@:{CGSize=dd}" : "v@:{CGSize=ff}", ContainerControl);
					break;
				case Eto.Forms.Control.MouseDownEvent:
					AddMethod(MacView.selMouseDown, MacView.TriggerMouseDown_Delegate, "v@:@");
					AddMethod(MacView.selRightMouseDown, MacView.TriggerMouseDown_Delegate, "v@:@");
					AddMethod(MacView.selOtherMouseDown, MacView.TriggerMouseDown_Delegate, "v@:@");
					break;
				case Eto.Forms.Control.MouseUpEvent:
					AddMethod(MacView.selMouseUp, MacView.TriggerMouseUp_Delegate, "v@:@");
					AddMethod(MacView.selRightMouseUp, MacView.TriggerMouseUp_Delegate, "v@:@");
					AddMethod(MacView.selOtherMouseUp, MacView.TriggerMouseUp_Delegate, "v@:@");
					break;
				case Eto.Forms.Control.MouseDoubleClickEvent:
					HandleEvent(Eto.Forms.Control.MouseDownEvent);
					break;
				case Eto.Forms.Control.MouseWheelEvent:
					AddMethod(MacView.selScrollWheel, MacView.TriggerMouseWheel_Delegate, "v@:@");
					break;
				case Eto.Forms.Control.KeyDownEvent:
					AddMethod(MacView.selKeyDown, MacView.TriggerKeyDown_Delegate, "v@:@");
					HandleEvent(MacView.FlagsChangedEvent);
					break;
				case Eto.Forms.Control.KeyUpEvent:
					AddMethod(MacView.selKeyUp, MacView.TriggerKeyUp_Delegate, "v@:@");
					HandleEvent(MacView.FlagsChangedEvent);
					break;
				case MacView.FlagsChangedEvent:
					AddMethod(MacView.selFlagsChanged, MacView.TriggerFlagsChanged_Delegate, "v@:@");
					break;
				case Eto.Forms.Control.LostFocusEvent:
					AddMethod(MacView.selResignFirstResponder, MacView.TriggerLostFocus_Delegate, "B@:");
					break;
				case Eto.Forms.Control.GotFocusEvent:
					AddMethod(MacView.selBecomeFirstResponder, MacView.TriggerGotFocus_Delegate, "B@:");
					break;
				case Eto.Forms.Control.ShownEvent:
					// TODO
					break;
				case Eto.Forms.Control.TextInputEvent:
					if (EnsureTextInputImplemented())
					{
						HandleEvent(Eto.Forms.Control.KeyDownEvent);
					}
					break;

				case Eto.Forms.Control.DragDropEvent:
					AddMethod(MacView.selPerformDragOperation, MacView.TriggerPerformDragOperation_Delegate, "b@:@", DragControl);
					break;
				case Eto.Forms.Control.DragOverEvent:
					AddMethod(MacView.selDraggingUpdated, MacView.TriggerDraggingUpdated_Delegate, "i@:@", DragControl);
					break;
				case Eto.Forms.Control.DragEnterEvent:
					AddMethod(MacView.selDraggingEntered, MacView.TriggerDraggingEntered_Delegate, "i@:@", DragControl);
					break;
				case Eto.Forms.Control.DragLeaveEvent:
					AddMethod(MacView.selDraggingExited, MacView.TriggerDraggingExited_Delegate, "v@:@", DragControl);
					break;
				case Eto.Forms.Control.DragEndEvent:
					// handled in EtoDragSource, TreeGridViewHandler.EtoDragSource, and GridViewHandler.EtoDragSource
					break;
				default:
					base.AttachEvent(id);
					break;

			}
		}

		public DragEventArgs GetDragEventArgs(NSDraggingInfo info, object customControl)
		{
			var source = info.DraggingSource as EtoDragSource;
			var sourceView = source?.SourceView ?? info.DraggingSource as NSView;
			var sourceHandler = GetHandler(sourceView) as IMacViewHandler;
			var data = info.DraggingPasteboard.ToEto();
			var location = ContainerControl.ConvertPointFromView(info.DraggingLocation, null);
			if (!ContentControl.IsFlipped)
				location.Y = ContentControl.Frame.Height - location.Y;
			return new DragEventArgs(sourceHandler?.Widget, data, info.DraggingSourceOperationMask.ToEto(), location.ToEto(), Keyboard.Modifiers, Mouse.Buttons, customControl);
		}

		/// <summary>
		/// Triggers a mouse callback from a different event. 
		/// e.g. when an NSButton is clicked it is triggered from a mouse up event.
		/// </summary>
		public bool TriggerMouseCallback(NSEvent evt = null, bool includeMouseDown = true)
		{
			// trigger mouse up event since it's buried by cocoa
			evt ??= NSApplication.SharedApplication.CurrentEvent;
			if (evt == null)
				return false;
			switch (evt.Type)
			{
				case NSEventType.LeftMouseDown:
				case NSEventType.RightMouseDown:
				case NSEventType.OtherMouseDown:
				{
					if (!includeMouseDown)
						return false;
					var args = MacConversions.GetMouseEvent(this, evt, false);
					Callback.OnMouseDown(Widget, args);
					return args.Handled;
				}
				case NSEventType.LeftMouseUp:
				case NSEventType.RightMouseUp:
				case NSEventType.OtherMouseUp:
				{
					var args = MacConversions.GetMouseEvent(this, evt, false);
					Callback.OnMouseUp(Widget, args);
					SuppressMouseTriggerCallback = true;
					MacView.CapturedControl = null;
					return args.Handled;
				}
				case NSEventType.LeftMouseDragged:
				case NSEventType.RightMouseDragged:
				case NSEventType.OtherMouseDragged:
				{
					var args = MacConversions.GetMouseEvent(this, evt, false);
					Callback.OnMouseMove(Widget, args);
					return args.Handled;
				}
			}
			return false;
		}

		
		public virtual void OnSizeChanged(EventArgs e)
		{
		}

		public virtual void Invalidate(bool invalidateChildren)
		{
			ContainerControl.NeedsDisplay = true;
		}

		public virtual void Invalidate(Rectangle rect, bool invalidateChildren)
		{
			var region = rect.ToNS();
			region.Y = EventControl.Frame.Height - region.Y - region.Height;
			EventControl.SetNeedsDisplayInRect(region);
		}

		public virtual void SuspendLayout()
		{
		}

		public virtual void ResumeLayout()
		{
			InvalidateMeasure();
		}


		bool InitialFocus
		{
			get { return Widget.Properties.Get<bool?>(MacView.InitialFocusKey) ?? false; }
			set { Widget.Properties[MacView.InitialFocusKey] = value ? (object)true : null; }
		}

		public virtual void Focus()
		{
			if (FocusControl.Window != null)
				FocusControl.Window.MakeFirstResponder(FocusControl);
			else
				InitialFocus = true;
		}

		protected bool HasBackgroundColor => Widget.Properties.Get<Color?>(MacView.BackgroundColorKey) != null;

		protected virtual Color DefaultBackgroundColor => ColorizeCell?.Color ?? Colors.Transparent;

		public virtual Color BackgroundColor
		{
			get { return Widget.Properties.Get<Color?>(MacView.BackgroundColorKey) ?? DefaultBackgroundColor; }
			set
			{
				if (value != BackgroundColor)
				{
					Widget.Properties[MacView.BackgroundColorKey] = value;
					SetBackgroundColor(value);
				}
			}
		}

		bool drawRectAdded;

		public bool UseNSBoxBackgroundColor
		{
			get => Widget.Properties.Get<bool>(MacView.UseNSBoxBackgroundColor_Key, DefaultUseNSBoxBackgroundColor);
			set => Widget.Properties.Set(MacView.UseNSBoxBackgroundColor_Key, value, DefaultUseNSBoxBackgroundColor);
		}

		protected virtual bool DefaultUseNSBoxBackgroundColor => true;
		
		protected virtual IColorizeCell ColorizeCell => null;
		
		protected virtual bool UseColorizeCellWithAlphaOnly => false;

		protected void SetBackgroundColor() => SetBackgroundColor(Widget.Properties.Get<Color?>(MacView.BackgroundColorKey));
		
		protected virtual void SetBackgroundColor(Color? color)
		{
			if (color != null)
			{
				var c = color.Value;
				var colorizeCell = ColorizeCell;
				if (colorizeCell != null)
				{
					colorizeCell.Color = !UseColorizeCellWithAlphaOnly || c.A < 1 ? color : null;
					ContainerControl.SetNeedsDisplay();
					return;
				}

				if (UseNSBoxBackgroundColor && ContainerControl is NSBox box)
				{
					// use NSBox to fill instead to have better dark mode support
					// e.g. background color is tinted by system automatically.
					box.FillColor = c.ToNSUI();
					box.Transparent = c.A <= 0;
					return;
				}

				if (!drawRectAdded && c.A > 0)
				{
					//AddMethod(MacView.selUpdateLayer, new Action<IntPtr, IntPtr>(UpdateLayerWithBackground), EtoEnvironment.Is64BitProcess ? "v@:{CGRect=dddd}" : "v@:{CGRect=ffff}", ContainerControl);
					//ContainerControl.WantsLayer = true;
					if (AddMethod(MacView.selDrawRect, MacView.DrawBackgroundRect_Delegate, EtoEnvironment.Is64BitProcess ? "v@:{CGRect=dddd}" : "v@:{CGRect=ffff}", ContainerControl))
					{
						// need this to actually use drawRect:, which is determined when the object is created
						if (MacView.supportsCanDrawSubviewsIntoLayer)
							ContainerControl.CanDrawSubviewsIntoLayer = true;
					}

					drawRectAdded = true;
				}

				ContainerControl.SetNeedsDisplay();
			}
		}

		public virtual bool Enabled
		{
			get => ControlEnabled;
			set => SetEnabled(ParentEnabled, value);
		}

		protected bool WantsEnabled => Widget.Properties.Get<bool?>(MacView.Enabled_Key) ?? true;

		protected bool ParentEnabled => Widget.VisualParent?.Enabled != false;

		public void SetEnabled() => SetEnabled(ParentEnabled);

		public void SetEnabled(bool parentEnabled) => SetEnabled(parentEnabled, null);

		void SetEnabled(bool parentEnabled, bool? newValue)
		{
			var controlEnabled = ControlEnabled;

			var enabled = Widget.Properties.Get<bool?>(MacView.Enabled_Key);
			if (enabled == null || newValue != null)
				Widget.Properties.Set<bool?>(MacView.Enabled_Key, enabled = newValue ?? controlEnabled);

			var newEnabled = parentEnabled && enabled.Value;
			if (controlEnabled != newEnabled)
			{
				ControlEnabled = newEnabled;
				Callback.OnEnabledChanged(Widget, EventArgs.Empty);

				if (!newEnabled)
					mouseDelegate?.FireMouseLeaveIfNeeded(true);
			}
		}

		protected virtual bool ControlEnabled
		{
			get => Widget.Properties.Get<bool>(MacView.ActualEnabled_Key, true);
			set => Widget.Properties.Set<bool>(MacView.ActualEnabled_Key, value, true);
		}

		public bool? ShouldHaveFocus
		{
			get { return Widget.Properties.Get<bool?>(MacView.ShouldHaveFocusKey); }
			set { Widget.Properties[MacView.ShouldHaveFocusKey] = value; }
		}

		public virtual bool HasFocus
		{
			get
			{
				return ShouldHaveFocus ?? (FocusControl.Window?.FirstResponder == Control && FocusControl.Window?.IsKeyWindow == true);
			}
		}

		public virtual bool Visible
		{
			get { return !ContainerControl.Hidden; }
			set
			{
				if (ContainerControl.Hidden == value)
				{
					ContainerControl.Hidden = !value;
					InvalidateMeasure();
					if (Widget.Loaded && value)
						FireOnShown();
				}
			}
		}
		
		public void Print()
		{
			MacView.InMouseTrackingLoop = false;
			PrintSettingsHandler.SetDefaults(NSPrintInfo.SharedPrintInfo);
			ContainerControl.Print(ContainerControl);
		}

		public virtual Cursor CurrentCursor
		{
			get { return Cursor; }
		}

		public virtual Cursor Cursor
		{
			get { return Widget.Properties.Get<Cursor>(MacView.Cursor_Key); }
			set
			{
				if (Cursor != value)
				{
					bool needsMethod = !Widget.Properties.ContainsKey(MacView.Cursor_Key);
					Widget.Properties[MacView.Cursor_Key] = value;
					if (needsMethod)
						AddMethod(MacView.selResetCursorRects, MacView.TriggerResetCursorRects_Delegate, "v@:");

					EventControl.Window?.InvalidateCursorRectsForView(EventControl);
				}
			}
		}

		public string ToolTip
		{
			get { return ContentControl.ToolTip; }
			set { ContentControl.ToolTip = value ?? string.Empty; }
		}

		public virtual void OnPreLoad(EventArgs e)
		{
		}

		public virtual void OnLoad(EventArgs e)
		{
			if (Widget.VisualParent?.Loaded != false && !(Widget is Window))
			{
				// adding dynamically or loading without a parent (e.g. embedding into a native app)
				AsyncQueue.Add(FireOnShown);
			}
			if (Widget.IsAttached)
			{
				SetAlignmentFrameSize(GetPreferredSize(SizeF.PositiveInfinity).ToNS());
			}
		}

		public virtual void OnLoadComplete(EventArgs e)
		{
			if (InitialFocus && FocusControl.Window != null)
			{
				FocusControl.Window.MakeFirstResponder(FocusControl);
				InitialFocus = false;
			}
		}

		public virtual void OnUnLoad(EventArgs e)
		{
			mouseDelegate?.FireMouseLeaveIfNeeded(false);
		}

		public virtual void OnKeyDown(KeyEventArgs e) => Callback.OnKeyDown(Widget, e);
		public virtual void OnKeyUp(KeyEventArgs e) => Callback.OnKeyUp(Widget, e);

		Control IMacViewHandler.Widget { get { return Widget; } }

		public virtual PointF PointFromScreen(PointF point)
		{
			var pt = point.ToNS();
			var view = ContainerControl;
			
			// macOS has flipped co-ordinates starting at the bottom left of the main screen,
			// so we flip to make 0,0 top left
			var mainFrame = NSScreen.Screens[0].Frame;
			pt.Y = mainFrame.Height - pt.Y;
			var window = view.Window ?? NSApplication.SharedApplication.CurrentEvent?.Window;
			if (window != null)
				pt = window.ConvertScreenToBase(pt);

			pt = view.ConvertPointFromView(pt, null);
			if (!view.IsFlipped)
				pt.Y = view.Frame.Height - pt.Y;
			pt = GetAlignmentPointForFramePoint(pt);
			return pt.ToEto();
		}

		public virtual PointF PointToScreen(PointF point)
		{
			var pt = point.ToNS();
			pt = GetFramePointForAlignmentPoint(pt);
			var view = ContainerControl;
			if (!view.IsFlipped)
				pt.Y = view.Frame.Height - pt.Y;
			pt = view.ConvertPointToView(pt, null);
			var window = view.Window ?? NSApplication.SharedApplication.CurrentEvent?.Window;
			if (window != null)
				pt = window.ConvertBaseToScreen(pt);

			var mainFrame = NSScreen.Screens[0].Frame;
			pt.Y = mainFrame.Height - pt.Y;
			return pt.ToEto();
		}

		Point Control.IHandler.Location
		{
			get
			{
				var frame = ContentControl.Frame;
				var location = frame.Location;
				var super = ContentControl.Superview;
				if (super == null || super.IsFlipped)
					return location.ToEtoPoint();
				return new Point((int)location.X, (int)(super.Frame.Height - location.Y - frame.Height));
			}
		}

		Dictionary<IntPtr, Command> systemActions;

		Dictionary<IntPtr, Command> IMacViewHandler.SystemActions => systemActions;

		public virtual IEnumerable<string> SupportedPlatformCommands
		{
			get { return MacView.systemActionSelectors.Keys; }
		}

		public int TabIndex
		{
			get { return Widget.Properties.Get<int>(MacView.TabIndex_Key, int.MaxValue); }
			set { Widget.Properties.Set(MacView.TabIndex_Key, value, int.MaxValue); }
		}

		public void MapPlatformCommand(string systemAction, Command command)
		{
			InnerMapPlatformCommand(systemAction, command, null);
		}

		protected virtual void InnerMapPlatformCommand(string systemAction, Command command, NSObject control)
		{
			IntPtr sel;
			if (MacView.systemActionSelectors.TryGetValue(systemAction, out sel))
			{
				if (systemActions == null)
				{
					systemActions = new Dictionary<IntPtr, Command>();
					AddMethod(MacView.selValidateUserInterfaceItem, MacView.ValidateSystemUserInterfaceItem_Delegate, "B@:@", control);
				}
				AddMethod(sel, MacView.TriggerSystemAction_Delegate, "v@:@", control);
				systemActions[sel] = command;
			}
		}

		public virtual void RecalculateKeyViewLoop(ref NSView last)
		{
			foreach (var child in Widget.VisualControls.OrderBy(c => c.TabIndex))
			{
				var handler = child.GetMacControl();
				if (handler != null)
				{
					handler.RecalculateKeyViewLoop(ref last);
					if (last != null)
						last.NextKeyView = handler.FocusControl;
					last = handler.FocusControl;
				}
			}
		}

		static void FireOnShown(Control control)
		{
			if (control.IsDisposed || !control.Visible)
				return;

			// don't use GetMacViewHandler() extension, as that will trigger OnShown for themed controls, which will
			// trigger Shown multiple times for the same themed control
			var handler = control.Handler as IMacViewHandler;
			handler?.Callback.OnShown(control, EventArgs.Empty);

			foreach (var ctl in control.VisualControls)
			{
				FireOnShown(ctl);
			}
		}

		protected virtual void FireOnShown() => FireOnShown(Widget);

		static readonly string[] s_dropTypes = { UTType.Item, UTType.Content };

		public bool AllowDrop
		{
			get => Widget.Properties.Get<bool>(MacView.AllowDrop_Key);
			set
			{
				if (Widget.Properties.TrySet(MacView.AllowDrop_Key, value))
				{
					if (value)
						DragControl.RegisterForDraggedTypes(s_dropTypes);
					else
						DragControl.UnregisterDraggedTypes();
				}
			}
		}

		static string s_etoDragItemType;
		static string s_etoDragImageType;

		protected void SetupDragPasteboard(NSPasteboard pasteboard)
		{
			// add a UTType.Item base type so dragging works regardless of what was put in the DataObject.
			if (s_etoDragItemType == null)
				s_etoDragItemType = UTType.CreatePreferredIdentifier(UTType.TagClassNSPboardType, "eto.dragitem", UTType.Item);

			// don't send an empty string, some code can't handle null data.
			pasteboard.SetStringForType(".", s_etoDragItemType);
		}

		public virtual void DoDragDrop(DataObject data, DragEffects allowedAction, Image image, PointF origin)
		{
			var handler = data.Handler as IDataObjectHandler;

			var source = new EtoDragSource { AllowedOperation = allowedAction.ToNS(), SourceView = ContainerControl, Handler = this, Data = data };

			NSDraggingItem[] draggingItems = null;
			if (image != null)
			{
				var pasteboardItem = new NSPasteboardItem();

				// register custom UTI for the drag image
				if (s_etoDragImageType == null)
					s_etoDragImageType = UTType.CreatePreferredIdentifier(UTType.TagClassNSPboardType, "eto.dragimage", UTType.Image);

				pasteboardItem.SetStringForType(string.Empty, s_etoDragImageType);
#if MONOMAC
				var draggingItem = new NSDraggingItem(NSObjectFlag.Empty);
				Messaging.bool_objc_msgSend_IntPtr(draggingItem.Handle, MacView.selInitWithPasteboardWriter_Handle, pasteboardItem.Handle);
#else
				var draggingItem = new NSDraggingItem(pasteboardItem);
#endif

				var mouseLocation = PointFromScreen(Mouse.Position);
				var loc = mouseLocation - origin;
				if (!ContainerControl.IsFlipped)
					loc.Y = (float)ContainerControl.Frame.Height - loc.Y - image.Height;

				draggingItem.SetDraggingFrame(new CGRect(loc.X, loc.Y, image.Width, image.Height), image.ToNS());

				draggingItems = new NSDraggingItem[] { draggingItem };
			}

			if (draggingItems == null)
				draggingItems = new NSDraggingItem[0];

			// stop mouse capture, if any
			MacView.InMouseTrackingLoop = false;
			
			var session = ContainerControl.BeginDraggingSession(draggingItems, NSApplication.SharedApplication.CurrentEvent, source);
			handler.Apply(session.DraggingPasteboard);

			SetupDragPasteboard(session.DraggingPasteboard);

			// TODO: block until drag is complete?
		}

		public Window GetNativeParentWindow()
		{
			var window = ContainerControl.Window;


			return window.ToEtoWindow();
		}

		protected virtual bool DefaultUseAlignmentFrame => false;

		public bool UseAlignmentFrame
		{
			get => Widget.Properties.Get<bool?>(MacView.UseAlignmentFrame_Key) ?? DefaultUseAlignmentFrame;
			set => Widget.Properties.Set<bool?>(MacView.UseAlignmentFrame_Key, value);
		}

		public int SuppressMouseEvents
		{
			get => Widget.Properties.Get<int>(MacView.SuppressMouseEvents_Key);
			set => Widget.Properties.Set<int>(MacView.SuppressMouseEvents_Key, value);
		}

		bool SuppressMouseUp
		{
			get => Widget.Properties.Get<bool>(MacView.SuppressMouseUp_Key);
			set => Widget.Properties.Set<bool>(MacView.SuppressMouseUp_Key, value);
		}

		public static bool SuppressMouseTriggerCallback { get; set; }
		
		/// <summary>
		/// Value to indicate that a mouse tracking loop should be used when a MouseDown is handled by user code.
		/// Defaults to true.
		/// </summary>
		public bool UseMouseTrackingLoop
		{
			get => Widget.Properties.Get<bool>(MacView.UseMouseTrackingLoop_Key, true);
			set => Widget.Properties.Set<bool>(MacView.UseMouseTrackingLoop_Key, value, true);
		}

		public NSRunLoopMode MouseTrackingRunLoopMode
		{
			get => Widget.Properties.Get<NSRunLoopMode>(MacView.MouseTrackingRunLoopMode_Key, NSRunLoopMode.Default);
			set => Widget.Properties.Set<NSRunLoopMode>(MacView.MouseTrackingRunLoopMode_Key, value, NSRunLoopMode.Default);
		}

		public void SetAlignmentFrameSize(CGSize size)
		{
			if (UseAlignmentFrame)
				size = GetFrameForAlignmentRect(new CGRect(CGPoint.Empty, size)).Size;

			ContainerControl.SetFrameSize(size);
		}

		public virtual void SetAlignmentFrame(CGRect frame) => ContainerControl.Frame = GetFrameForAlignmentRect(frame);

		public virtual CGRect GetFrameForAlignmentRect(CGRect frame)
		{
			if (!UseAlignmentFrame)
				return frame;

			return ContainerControl.GetFrameForAlignmentRect(frame);
		}

		public virtual CGRect GetAlignmentRectForFrame(CGRect frame)
		{
			if (!UseAlignmentFrame)
				return frame;

			return ContainerControl.GetAlignmentRectForFrame(frame);
		}

		public CGRect GetAlignmentFrame() => GetAlignmentRectForFrame(ContainerControl.Frame);

		public CGSize GetAlignmentSizeForSize(CGSize size) => GetAlignmentRectForFrame(new CGRect(CGPoint.Empty, size)).Size;

		public CGPoint GetAlignmentPointForFramePoint(CGPoint point)
		{
			if (!UseAlignmentFrame)
				return point;
			var view = ContainerControl;
			var frame = view.Frame;
			var alignment = view.GetAlignmentRectForFrame(new CGRect(CGPoint.Empty, frame.Size));
			point.X -= alignment.X;
			if (view.IsFlipped)
				point.Y += alignment.Y;
			else
				point.Y -= alignment.Y;
			return point;
		}
		public CGPoint GetFramePointForAlignmentPoint(CGPoint point)
		{
			if (!UseAlignmentFrame)
				return point;
			var view = ContainerControl;
			var frame = view.Frame;
			var alignment = view.GetAlignmentRectForFrame(new CGRect(CGPoint.Empty, frame.Size));
			point.X += alignment.X;
			if (view.IsFlipped)
				point.Y -= alignment.Y;
			else
				point.Y += alignment.Y;
			return point;
		}

		
		/// <summary>
		/// Provides an event to specify that this control should trigger an initial MouseDown event when clicked on an inactive window
		/// </summary>
		/// <remarks>
		/// On macOS, controls don't usually respond to clicks when initially clicking on an inactive window so the user can focus
		/// that window without ill effects.  However, in some cases you may want the user to do this, so this event allows you to
		/// specify if that should be allowed by setting <see cref="MouseEventArgs.Handled"/> to true.
		/// </remarks>
		public event EventHandler<MouseEventArgs> AcceptsFirstMouse
		{
			add => Widget.Properties.AddEvent(MacView.AcceptsFirstMouse_Key, value);
			remove => Widget.Properties.RemoveEvent(MacView.AcceptsFirstMouse_Key, value);
		}

		protected virtual bool OnAcceptsFirstMouse(NSEvent theEvent)
		{
			if (!Widget.Properties.ContainsKey(MacView.AcceptsFirstMouse_Key))
			{
				if (ContainerControl.Window is NSPanel)
					return Application.Instance.IsActive;
				return false;
			}

			var args = MacConversions.GetMouseEvent(this, theEvent, false);
			Widget.Properties.TriggerEvent(MacView.AcceptsFirstMouse_Key, this, args);
			return args.Handled;
		}

		bool IMacViewHandler.OnAcceptsFirstMouse(NSEvent theEvent) => OnAcceptsFirstMouse(theEvent);

		public virtual MouseEventArgs TriggerMouseDown(NSObject obj, IntPtr sel, NSEvent theEvent)
		{
			if (!Enabled)
				return null;

			var args = MacConversions.GetMouseEvent(this, theEvent, false);

			// Ensure we're actually in the control's bounds
			if (!new RectangleF(Size).Contains(args.Location))
			{
				// This can happen e.g. when double clicking outside of the control after a ContextMenu is shown during MouseUp.
				Messaging.void_objc_msgSendSuper_IntPtr(obj.SuperHandle, sel, theEvent.Handle);
				SuppressMouseUp = true;
				return null;
			}

			SuppressMouseUp = false; // MouseDown is called, so we are expecting a MouseUp

			// Flag that we are going to use a mouse tracking loop
			// if this is set to false during the OnMouseDown/OnMouseDoubleClick, then we won't
			// do a mouse tracking loop.  This is needed since the mouse up event gets buried when 
			// showing context menus, dialogs, etc.
			MacView.InMouseTrackingLoop = true;
				
			if (theEvent.ClickCount >= 2)
				Callback.OnMouseDoubleClick(Widget, args);
			
			if (!args.Handled)
			{
				Callback.OnMouseDown(Widget, args);
			}
			if (!args.Handled && sel != IntPtr.Zero)
			{
				SuppressMouseTriggerCallback = false;
				SuppressMouseEvents++;
				Messaging.void_objc_msgSendSuper_IntPtr(obj.SuperHandle, sel, theEvent.Handle);
				SuppressMouseEvents--;
				
				// some controls use event loops until mouse up, so we need to trigger the mouse up here.
				if (!SuppressMouseTriggerCallback)
					TriggerMouseCallback(theEvent, includeMouseDown: false);
			}
			else if (UseMouseTrackingLoop && MacView.InMouseTrackingLoop)
			{
				// do a mouse tracking loop to enforce capture always
				// e.g. if a child control that you started to click + drag on is removed then all future events
				// to the parent are no longer forwarded.
				// See MouseTests.EventsFromParentShouldWorkWhenChildRemoved
				DoMouseTrackingLoop(true);
			}
			MacView.InMouseTrackingLoop = false;
			return args;
		}

		private void DoMouseTrackingLoop(bool autoRelease)
		{
			var app = NSApplication.SharedApplication;
			MacView.CapturedControl = this;
			bool continueLoop;
			// Console.WriteLine("Entered MouseTrackingLoop");
			do
			{
				var evt = app.NextEvent(NSEventMask.AnyEvent, NSDate.DistantFuture, MouseTrackingRunLoopMode, true);

				switch (evt.Type)
				{
					case NSEventType.LeftMouseUp:
					case NSEventType.RightMouseUp:
					case NSEventType.OtherMouseUp:
						TriggerMouseCallback(evt);
						if (autoRelease)
						{
							MacView.InMouseTrackingLoop = false;
							MacView.CapturedControl = null;
						}
						break;
					case NSEventType.LeftMouseDragged:
					case NSEventType.RightMouseDragged:
					case NSEventType.OtherMouseDragged:
					case NSEventType.LeftMouseDown:
					case NSEventType.RightMouseDown:
					case NSEventType.OtherMouseDown:
						TriggerMouseCallback(evt);
						break;
					default:
						// not a mouse event, send it along.
						app.SendEvent(evt);
						break;
				}
				continueLoop = autoRelease ? MacView.InMouseTrackingLoop : CaptureLoopEnabled;
			}
			while (continueLoop);
			// Console.WriteLine("Exited MouseTrackingLoop");
		}

		public virtual MouseEventArgs TriggerMouseUp(NSObject obj, IntPtr sel, NSEvent theEvent)
		{
			var args = MacConversions.GetMouseEvent(this, theEvent, false);
			if (!SuppressMouseUp)
			{
				Callback.OnMouseUp(Widget, args);
				SuppressMouseUp = false;
			}

			if (!args.Handled)
			{
				Messaging.void_objc_msgSendSuper_IntPtr(obj.SuperHandle, sel, theEvent.Handle);
			}
			return args;
		}
		
		bool IMacViewHandler.TextInputCancelled
		{
			get => Widget.Properties.Get<bool>(MacView.TextInputCancelled_Key);
			set => Widget.Properties.Set(MacView.TextInputCancelled_Key, value);
		}
		
		public bool TextInputImplemented
		{
			get => Widget.Properties.Get<bool>(MacView.TextInputImplemented_Key);
			private set => Widget.Properties.Set(MacView.TextInputImplemented_Key, value);
		}
		
		public virtual void UpdateLayout()
		{
			ContainerControl?.Window?.LayoutIfNeeded();
		}
		
		public bool AutoAttachNative
		{
			get => Widget.Properties.Get<bool>(MacView.AutoAttachNative_Key);
			set
			{
				if (Widget.Properties.TrySet(MacView.AutoAttachNative_Key, value) && value)
				{
					// ensure method is added to the container control's class
					AddMethod(MacView.selViewDidMoveToWindow, MacView.TriggerViewDidMoveToWindow_Delegate, "v@:@", ContainerControl);
				}
			}
		}

		public virtual void OnViewDidMoveToWindow()
		{
			if (!AutoAttachNative)
				return;

			// ensure load/unload get called appropriately.
			if (ContainerControl.Window == null)
				Widget.DetachNative();
			else
				Widget.AttachNative();
		}

		public bool IsMouseCaptured => MacView.CapturedControl == this;

		public bool CaptureMouse()
		{
			if (!Widget.Loaded || !Widget.Visible)
				return false;

			// already captured?
			if (MacView.CapturedControl == this && CaptureLoopEnabled)
				return true;
			
			// ensure we release capture of any previous control
			if (MacView.CapturedControl != this)
				MacView.CapturedControl?.Widget.ReleaseMouseCapture();

			MacView.CapturedControl = this;
			MacView.InMouseTrackingLoop = false;
			// Do this asynchronously as this is not a blocking API
			Application.Instance.AsyncInvoke(DoMouseCaptureLoop);
			return true;
		}

		public void FireMouseEnterIfNeeded() => mouseDelegate?.FireMouseEnterIfNeeded(false);
		public void FireMouseLeaveIfNeeded() => mouseDelegate?.FireMouseLeaveIfNeeded(false);

		private void DoMouseCaptureLoop()
		{
			// fire mouse leave of current control(s)
			var enteredControls = MouseDelegate.EnteredControls.ToList();
			var parentControls = Widget.Parents.Select(r => r.Handler).OfType<IMacViewHandler>().ToList();
			foreach (var ctl in enteredControls)
			{
				if (ctl.Handler == this || parentControls.Contains(ctl.Handler))
					continue;
				ctl.FireMouseLeaveIfNeeded(false);
			}
			foreach (var parent in parentControls)
			{
				parent.FireMouseEnterIfNeeded();
			}
			FireMouseEnterIfNeeded();
			CaptureLoopEnabled = true;
			DoMouseTrackingLoop(false);
			var mousePosition = Mouse.Position;

			if (!Widget.RectangleToScreen(new RectangleF(Widget.Size)).Contains(mousePosition))
				FireMouseLeaveIfNeeded();
				
			foreach (var parent in parentControls)
			{
				if (!parent.Widget.RectangleToScreen(new RectangleF(parent.Widget.Size)).Contains(mousePosition))
					parent.FireMouseLeaveIfNeeded();
			}
			// fire mouse enter of previous control if still in bounds
			foreach (var ctl in enteredControls)
			{
				if (ctl.Handler == this)
					continue;
				var widget = ctl.Handler?.Widget;
				if (widget != null && widget.RectangleToScreen(new RectangleF(widget.Size)).Contains(mousePosition))
					ctl.FireMouseEnterIfNeeded(false);
			}
		}

		bool CaptureLoopEnabled;

		public void ReleaseMouseCapture()
		{
			CaptureLoopEnabled = false;
			MacView.CapturedControl = null;
		}
	}
}

