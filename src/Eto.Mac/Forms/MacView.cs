using System;
using Eto.Drawing;
using Eto.Forms;
using Eto.Mac.Forms.Controls;
using System.Collections.Generic;
using Eto.Mac.Forms.Printing;
using System.Linq;
using System.Runtime.InteropServices;

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

namespace Eto.Mac.Forms
{
	class MouseDelegate : NSObject
	{
		WeakReference widget;

		public IMacViewHandler Handler { get { return (IMacViewHandler)widget.Target; } set { widget = new WeakReference(value); } }

		[Export("mouseMoved:")]
		public void MouseMoved(NSEvent theEvent)
		{
			Handler.Callback.OnMouseMove(Handler.Widget, MacConversions.GetMouseEvent(Handler, theEvent, false));
		}

		[Export("mouseEntered:")]
		public void MouseEntered(NSEvent theEvent)
		{
			Handler.Callback.OnMouseEnter(Handler.Widget, MacConversions.GetMouseEvent(Handler, theEvent, false));
		}

		[Export("cursorUpdate:")]
		public void CursorUpdate(NSEvent theEvent)
		{
		}

		[Export("mouseExited:")]
		public void MouseExited(NSEvent theEvent)
		{
			Handler.Callback.OnMouseLeave(Handler.Widget, MacConversions.GetMouseEvent(Handler, theEvent, false));
		}

		[Export("scrollWheel:")]
		public void ScrollWheel(NSEvent theEvent)
		{
			Handler.Callback.OnMouseWheel(Handler.Widget, MacConversions.GetMouseEvent(Handler, theEvent, true));
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

		DragEventArgs GetDragEventArgs(NSDraggingInfo info, object customControl);

		void SetEnabled(bool parentEnabled);

		void SetAlignmentFrameSize(CGSize size);
		void SetAlignmentFrame(CGRect frame);
		CGRect GetAlignmentFrame();
		CGSize GetAlignmentSizeForSize(CGSize size);
		CGPoint GetAlignmentPointForFramePoint(CGPoint point);
		CGRect GetAlignmentRectForFrame(CGRect frame);
	}

	static class MacView
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
		public static readonly IntPtr selInsertText = Selector.GetHandle("insertText:");
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
		public static readonly IntPtr selSetDataProviderForTypes_Handle = Selector.GetHandle("setDataProvider:forTypes:");
		public static readonly IntPtr selInitWithPasteboardWriter_Handle = Selector.GetHandle("initWithPasteboardWriter:");
		public const string FlagsChangedEvent = "MacView.FlagsChangedEvent";

		// before 10.12, we have to call base.Layout() AFTER we do our layout otherwise it doesn't work correctly..
		// however, that causes (temporary) glitches when resizing especially with Scrollable >= 10.12
		public static readonly bool NewLayout = MacVersion.IsAtLeast(10, 12);

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
				var args = MacConversions.GetMouseEvent(handler, theEvent, false);
				if (theEvent.ClickCount >= 2)
					handler.Callback.OnMouseDoubleClick(handler.Widget, args);

				if (!args.Handled)
				{
					handler.Callback.OnMouseDown(handler.Widget, args);
				}
				if (!args.Handled)
				{
					Messaging.void_objc_msgSendSuper_IntPtr(obj.SuperHandle, sel, e);
				}
			}
		}

		internal static MarshalDelegates.Action_IntPtr_IntPtr_IntPtr TriggerMouseUp_Delegate = TriggerMouseUp;
		static void TriggerMouseUp(IntPtr sender, IntPtr sel, IntPtr e)
		{
			var obj = Runtime.GetNSObject(sender);

			if (MacBase.GetHandler(obj) is IMacViewHandler handler)
			{
				var theEvent = Messaging.GetNSObject<NSEvent>(e);
				var args = MacConversions.GetMouseEvent(handler, theEvent, false);
				handler.Callback.OnMouseUp(handler.Widget, args);
				if (!args.Handled)
				{
					Messaging.void_objc_msgSendSuper_IntPtr(obj.SuperHandle, sel, e);
				}
			}
		}

		internal static MarshalDelegates.Action_IntPtr_IntPtr_IntPtr TriggerMouseWheel_Delegate = TriggerMouseWheel;
		static void TriggerMouseWheel(IntPtr sender, IntPtr sel, IntPtr e)
		{
			var obj = Runtime.GetNSObject(sender);
			if (MacBase.GetHandler(obj) is IMacViewHandler handler)
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

		internal static MarshalDelegates.Action_IntPtr_IntPtr_IntPtr TriggerKeyDown_Delegate = TriggerKeyDown;
		static void TriggerKeyDown(IntPtr sender, IntPtr sel, IntPtr e)
		{
			var obj = Runtime.GetNSObject(sender);
			if (MacBase.GetHandler(obj) is IMacViewHandler handler)
			{
				var theEvent = Messaging.GetNSObject<NSEvent>(e);
				if (!MacEventView.KeyDown(handler.Widget, theEvent))
				{
					Messaging.void_objc_msgSendSuper_IntPtr(obj.SuperHandle, sel, e);
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
			var e = handler?.GetDragEventArgs(Runtime.GetNSObject<NSDraggingInfo>(draggingInfoPtr), null);
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
			var e = handler?.GetDragEventArgs(Runtime.GetNSObject<NSDraggingInfo>(draggingInfoPtr), null);
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
			var draggingInfo = Runtime.GetNSObject<NSDraggingInfo>(draggingInfoPtr);
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
			var e = handler?.GetDragEventArgs(Runtime.GetNSObject<NSDraggingInfo>(draggingInfoPtr), null);
			if (e != null)
			{
				handler.Callback.OnDragLeave(handler.Widget, e);
			}
			Messaging.void_objc_msgSendSuper_IntPtr(obj.SuperHandle, sel, draggingInfoPtr);
		}

		internal static MarshalDelegates.Action_IntPtr_IntPtr_IntPtr TriggerTextInput_Delegate = TriggerTextInput;
		static void TriggerTextInput(IntPtr sender, IntPtr sel, IntPtr textPtr)
		{
			var obj = Runtime.GetNSObject(sender);

			if (MacBase.GetHandler(obj) is IMacViewHandler handler)
			{
				var text = (string)Messaging.GetNSObject<NSString>(textPtr);
				var args = new TextInputEventArgs(text);
				handler.Callback.OnTextInput(handler.Widget, args);
				if (args.Cancel)
					return;
			}
			Messaging.void_objc_msgSendSuper_IntPtr(obj.SuperHandle, sel, textPtr);
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
			var objClass = ObjCExtensions.object_getClass(sender);

			if (objClass == IntPtr.Zero)
				return false;

			var superClass = ObjCExtensions.class_getSuperclass(objClass);
			return
				superClass != IntPtr.Zero
				&& ObjCExtensions.ClassInstancesRespondToSelector(superClass, sel)
				&& Messaging.bool_objc_msgSendSuper_IntPtr(control.SuperHandle, sel, item);
		}

	}

	public abstract class MacView<TControl, TWidget, TCallback> : MacObject<TControl, TWidget, TCallback>, Control.IHandler, IMacViewHandler
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

		public virtual NSView ContentControl { get { return ContainerControl; } }

		public virtual NSView DragControl => ContainerControl;

		public virtual NSView EventControl { get { return ContainerControl; } }

		public virtual NSView FocusControl { get { return EventControl; } }

		public virtual IEnumerable<Control> VisualControls => Enumerable.Empty<Control>();

		public virtual bool AutoSize
		{
			get { return Widget.Properties.Get<bool>(MacView.AutoSize_Key, true); }
			protected set { Widget.Properties.Set(MacView.AutoSize_Key, value, true); }
		}

		protected virtual Size DefaultMinimumSize
		{
			get { return Size.Empty; }
		}

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
					SetAutoSize();
			}
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

				CreateTracking();
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

		protected virtual void SetAutoSize()
		{
			var userPreferredSize = UserPreferredSize;
			AutoSize = userPreferredSize.Width == -1 || userPreferredSize.Height == -1;
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

			if (!Widget.Loaded)
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

		void CreateTracking()
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
				if (!UseAlignmentFrame)
					options |= NSTrackingAreaOptions.InVisibleRect;

				tracking = new NSTrackingArea(frame, options, mouseDelegate, null);
				EventControl.AddTrackingArea(tracking);
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
					HandleEvent(Eto.Forms.Control.SizeChangedEvent);
					CreateTracking();
					break;
				case Eto.Forms.Control.MouseMoveEvent:
					mouseOptions |= NSTrackingAreaOptions.MouseMoved;
					mouseMove = true;
					HandleEvent(Eto.Forms.Control.SizeChangedEvent);
					CreateTracking();
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
					AddMethod(MacView.selInsertText, MacView.TriggerTextInput_Delegate, "v@:@");
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
		protected void TriggerMouseCallback()
		{
			// trigger mouse up event since it's buried by cocoa
			var evt = NSApplication.SharedApplication.CurrentEvent;
			if (evt == null)
				return;
			if (evt.Type == NSEventType.LeftMouseUp || evt.Type == NSEventType.RightMouseUp || evt.Type == NSEventType.OtherMouseUp)
			{
				Callback.OnMouseUp(Widget, MacConversions.GetMouseEvent(this, evt, false));
			}
			if (evt.Type == NSEventType.LeftMouseDragged || evt.Type == NSEventType.RightMouseDragged || evt.Type == NSEventType.OtherMouseDragged)
			{
				Callback.OnMouseMove(Widget, MacConversions.GetMouseEvent(this, evt, false));
			}
		}

		
		public virtual void OnSizeChanged(EventArgs e)
		{
			CreateTracking();
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

		public virtual Color BackgroundColor
		{
			get { return Widget.Properties.Get<Color?>(MacView.BackgroundColorKey) ?? Colors.Transparent; }
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

		protected virtual bool UseNSBoxBackgroundColor => true;

		protected virtual void SetBackgroundColor(Color? color)
		{
			if (color != null)
			{
				if (UseNSBoxBackgroundColor && ContainerControl is NSBox box)
				{
					// use NSBox to fill instead to have better dark mode support
					// e.g. background color is tinted by system automatically.
					box.FillColor = color.Value.ToNSUI();
					box.Transparent = color.Value.A <= 0;
					return;
				}

				if (!drawRectAdded && color.Value.A > 0)
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
				return ShouldHaveFocus ?? (FocusControl.Window != null && FocusControl.Window.FirstResponder == Control);
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

		public void Print(PrintSettings settings)
		{
			var op = NSPrintOperation.FromView(EventControl);
			if (settings != null)
				op.PrintInfo = ((PrintSettingsHandler)settings.Handler).Control;
			op.ShowsPrintPanel = false;
			op.RunOperation();
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
		}

		public virtual void OnKeyDown(KeyEventArgs e) => Callback.OnKeyDown(Widget, e);
		public virtual void OnKeyUp(KeyEventArgs e) => Callback.OnKeyUp(Widget, e);

		Control IMacViewHandler.Widget { get { return Widget; } }

		public virtual PointF PointFromScreen(PointF point)
		{
			var pt = point.ToNS();
			var view = ContainerControl;
			var window = view.Window;
			if (window != null)
			{
				// macOS has flipped co-ordinates starting at the bottom left of the main screen,
				// so we flip to make 0,0 top left
				var mainFrame = NSScreen.Screens[0].Frame;
				pt.Y = mainFrame.Height - pt.Y;
				pt = window.ConvertScreenToBase(pt);
			}
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
			var window = view.Window;
			if (window != null)
			{
				pt = window.ConvertBaseToScreen(pt);
				// macOS has flipped co-ordinates starting at the bottom left of the main screen,
				// so we flip to make 0,0 top left
				var mainFrame = NSScreen.Screens[0].Frame;
				pt.Y = mainFrame.Height - pt.Y;
			}
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

		public bool AllowDrop
		{
			get { return Widget.Properties.Get<bool>(MacView.AllowDrop_Key); }
			set
			{
				Widget.Properties.Set(MacView.AllowDrop_Key, value);
				if (value)
					DragControl.RegisterForDraggedTypes(new string[] { UTType.Item });
				else
					DragControl.UnregisterDraggedTypes();
			}
		}

		public virtual void DoDragDrop(DataObject data, DragEffects allowedAction, Image image, PointF origin)
		{
			var handler = data.Handler as IDataObjectHandler;

			var source = new EtoDragSource { AllowedOperation = allowedAction.ToNS(), SourceView = ContainerControl };

			NSDraggingItem[] draggingItems = null;
			if (image != null)
			{
				var pasteboardItem = new NSPasteboardItem();
				// item needs to have data, but we don't want to supply a standard UTI
				const string utdragimage = "eto.dragimage";
				pasteboardItem.SetStringForType(string.Empty, utdragimage);
				// custom types need to be registered when using an NSPasteboardItem..
				ContainerControl.RegisterForDraggedTypes(new string[] { utdragimage });
#if XAMMAC2
				var draggingItem = new NSDraggingItem(pasteboardItem);
#else
				var draggingItem = new NSDraggingItem(NSObjectFlag.Empty);
				Messaging.bool_objc_msgSend_IntPtr(draggingItem.Handle, MacView.selInitWithPasteboardWriter_Handle, pasteboardItem.Handle);
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

			var session = ContainerControl.BeginDraggingSession(draggingItems, NSApplication.SharedApplication.CurrentEvent, source);
			handler.Apply(session.DraggingPasteboard);

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
	}
}

