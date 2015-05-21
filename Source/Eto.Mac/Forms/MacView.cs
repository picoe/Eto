using System;
using Eto.Drawing;
using Eto.Forms;
using Eto.Mac.Forms.Controls;
using System.Collections.Generic;
using Eto.Mac.Forms.Printing;

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
using CGSize = MonoMac.Foundation.NSSize;
using CGRect = MonoMac.Foundation.NSRect;
using CGPoint = MonoMac.Foundation.NSPoint;
using nfloat = System.Double;
using nint = System.Int64;
using nuint = System.UInt64;
#else
using CGSize = System.Drawing.SizeF;
using CGRect = System.Drawing.RectangleF;
using CGPoint = System.Drawing.PointF;
using nfloat = System.Single;
using nint = System.Int32;
using nuint = System.UInt32;
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
			Handler.Callback.OnMouseMove(Handler.Widget, MacConversions.GetMouseEvent(Handler.EventControl, theEvent, false));
		}

		[Export("mouseEntered:")]
		public void MouseEntered(NSEvent theEvent)
		{
			Handler.Callback.OnMouseEnter(Handler.Widget, MacConversions.GetMouseEvent(Handler.EventControl, theEvent, false));
		}

		[Export("cursorUpdate:")]
		public void CursorUpdate(NSEvent theEvent)
		{
		}

		[Export("mouseExited:")]
		public void MouseExited(NSEvent theEvent)
		{
			Handler.Callback.OnMouseLeave(Handler.Widget, MacConversions.GetMouseEvent(Handler.EventControl, theEvent, false));
		}

		[Export("scrollWheel:")]
		public void ScrollWheel(NSEvent theEvent)
		{
			Handler.Callback.OnMouseWheel(Handler.Widget, MacConversions.GetMouseEvent(Handler.EventControl, theEvent, true));
		}
	}

	public interface IMacViewHandler : IMacControlHandler
	{
		Size? PreferredSize { get; }

		Control Widget { get; }

		Control.ICallback Callback { get; }

		Cursor CurrentCursor { get; }

		void OnKeyDown(KeyEventArgs e);

		void OnSizeChanged(EventArgs e);

		NSObject CustomFieldEditor { get; }

		bool? ShouldHaveFocus { get; set; }
	}

	public abstract class MacView<TControl, TWidget, TCallback> : MacObject<TControl, TWidget, TCallback>, Control.IHandler, IMacViewHandler
		where TControl: NSObject
		where TWidget: Control
		where TCallback: Control.ICallback
	{
		bool mouseMove;
		NSTrackingArea tracking;
		NSTrackingAreaOptions mouseOptions;
		MouseDelegate mouseDelegate;

		public override IntPtr NativeHandle { get { return Control.Handle; } }

		Control.ICallback IMacViewHandler.Callback { get { return Callback; } }

		public abstract NSView ContainerControl { get; }

		public virtual NSView ContentControl { get { return ContainerControl; } }

		public virtual NSView EventControl { get { return ContainerControl; } }

		public virtual NSView FocusControl { get { return EventControl; } }

		static readonly object AutoSizeKey = new object();
		public virtual bool AutoSize
		{
			get { return Widget.Properties.Get<bool?>(AutoSizeKey) ?? true; }
			protected set { Widget.Properties[AutoSizeKey] = value; }
		}

		static readonly object MinimumSizeKey = new object();
		public virtual Size MinimumSize
		{
			get { return Widget.Properties.Get<Size?>(MinimumSizeKey) ?? Size.Empty; }
			set { Widget.Properties[MinimumSizeKey] = value; }
		}

		static readonly object MaximumSizeKey = new object();
		public virtual SizeF MaximumSize
		{
			get { return Widget.Properties.Get<SizeF?>(MaximumSizeKey) ?? SizeF.MaxValue; }
			set { Widget.Properties[MaximumSizeKey] = value; }
		}

		static readonly object PreferredSizeKey = new object();
		public Size? PreferredSize
		{
			get { return Widget.Properties.Get<Size?>(PreferredSizeKey); }
			set { Widget.Properties[PreferredSizeKey] = value; }
		}

		public virtual Size Size
		{
			get { return ContainerControl.Frame.Size.ToEtoSize(); }
			set
			{ 
				var oldSize = GetPreferredSize(Size.MaxValue);
				PreferredSize = value;

				var oldFrameSize = ContainerControl.Frame.Size;
				var newSize = oldFrameSize;
				if (value.Width >= 0)
					newSize.Width = value.Width;
				if (value.Height >= 0)
					newSize.Height = value.Height;

				// this doesn't get to our overridden method to handle the event (since it calls [super setFrameSize:]) so trigger event manually.
				ContainerControl.SetFrameSize(newSize);
				if (oldFrameSize != newSize)
					Callback.OnSizeChanged(Widget, EventArgs.Empty);

				AutoSize = value.Width == -1 && value.Height == -1;
				CreateTracking();
				LayoutIfNeeded(oldSize);
			}
		}

		static readonly object NaturalSizeKey = new object();
		protected SizeF? NaturalSize
		{
			get { return Widget.Properties.Get<SizeF?>(NaturalSizeKey); }
			set { Widget.Properties[NaturalSizeKey] = value; }
		}

		public virtual NSObject CustomFieldEditor { get { return null; } }

		protected virtual bool LayoutIfNeeded(SizeF? oldPreferredSize = null, bool force = false)
		{
			NaturalSize = null;
			if (Widget.Loaded)
			{
				var oldSize = oldPreferredSize ?? ContainerControl.Frame.Size.ToEtoSize();
				var newSize = GetPreferredSize(SizeF.MaxValue);
				if (newSize != oldSize || force)
				{
					var container = Widget.Parent.GetMacContainer();
					if (container != null)
						container.LayoutParent();
					return true;
				}
			}
			return false;
		}

		protected virtual SizeF GetNaturalSize(SizeF availableSize)
		{
			var naturalSize = NaturalSize;
			if (naturalSize != null)
				return naturalSize.Value;
			var control = Control as NSControl;
			if (control != null)
			{
				var size = (Widget.Loaded) ? (CGSize?)control.Frame.Size : null;
				control.SizeToFit();
				naturalSize = control.Frame.Size.ToEto();
				if (size != null)
					control.SetFrameSize(size.Value);
				NaturalSize = naturalSize;
				return naturalSize.Value;
			}
			return Size.Empty;
		}

		public virtual SizeF GetPreferredSize(SizeF availableSize)
		{
			var size = GetNaturalSize(availableSize);
			if (!AutoSize && PreferredSize != null)
			{
				var preferredSize = PreferredSize.Value;
				if (preferredSize.Width >= 0)
					size.Width = preferredSize.Width;
				if (preferredSize.Height >= 0)
					size.Height = preferredSize.Height;
			}
			return SizeF.Min(SizeF.Max(size, MinimumSize), MaximumSize);
		}

		public virtual Size PositionOffset { get { return Size.Empty; } }

		void CreateTracking()
		{
			if (!mouseMove)
				return;
			if (tracking != null)
				EventControl.RemoveTrackingArea(tracking);
			//Console.WriteLine ("Adding mouse tracking {0} for area {1}", this.Widget.GetType ().FullName, Control.Frame.Size);
			if (mouseDelegate == null)
				mouseDelegate = new MouseDelegate { Handler = this };
			var options = mouseOptions | NSTrackingAreaOptions.ActiveAlways | NSTrackingAreaOptions.EnabledDuringMouseDrag | NSTrackingAreaOptions.InVisibleRect;
			tracking = new NSTrackingArea(new CGRect(CGPoint.Empty, EventControl.Frame.Size), options, mouseDelegate, new NSDictionary());
			EventControl.AddTrackingArea(tracking);
		}

		public virtual void SetParent(Container parent)
		{
		}

		static readonly IntPtr selMouseDown = Selector.GetHandle("mouseDown:");
		static readonly IntPtr selMouseUp = Selector.GetHandle("mouseUp:");
		static readonly IntPtr selMouseDragged = Selector.GetHandle("mouseDragged:");
		static readonly IntPtr selRightMouseDown = Selector.GetHandle("rightMouseDown:");
		static readonly IntPtr selRightMouseUp = Selector.GetHandle("rightMouseUp:");
		static readonly IntPtr selRightMouseDragged = Selector.GetHandle("rightMouseDragged:");
		static readonly IntPtr selScrollWheel = Selector.GetHandle("scrollWheel:");
		static readonly IntPtr selKeyDown = Selector.GetHandle("keyDown:");
		static readonly IntPtr selKeyUp = Selector.GetHandle("keyUp:");
		static readonly IntPtr selBecomeFirstResponder = Selector.GetHandle("becomeFirstResponder");
		static readonly IntPtr selSetFrameSize = Selector.GetHandle("setFrameSize:");
		static readonly IntPtr selResignFirstResponder = Selector.GetHandle("resignFirstResponder");
		static readonly IntPtr selInsertText = Selector.GetHandle("insertText:");

		public override void AttachEvent(string id)
		{
			switch (id)
			{
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
					AddMethod(selMouseDragged, new Action<IntPtr, IntPtr, IntPtr>(TriggerMouseDragged), "v@:@");
					AddMethod(selRightMouseDragged, new Action<IntPtr, IntPtr, IntPtr>(TriggerMouseDragged), "v@:@");
					break;
				case Eto.Forms.Control.SizeChangedEvent:
					AddMethod(selSetFrameSize, new Action<IntPtr, IntPtr, CGSize>(SetFrameSizeAction), "v@:{CGSize=ff}", ContainerControl);
					break;
				case Eto.Forms.Control.MouseDownEvent:
					AddMethod(selMouseDown, new Action<IntPtr, IntPtr, IntPtr>(TriggerMouseDown), "v@:@");
					AddMethod(selRightMouseDown, new Action<IntPtr, IntPtr, IntPtr>(TriggerMouseDown), "v@:@");
					break;
				case Eto.Forms.Control.MouseUpEvent:
					AddMethod(selMouseUp, new Action<IntPtr, IntPtr, IntPtr>(TriggerMouseUp), "v@:@");
					AddMethod(selRightMouseUp, new Action<IntPtr, IntPtr, IntPtr>(TriggerMouseUp), "v@:@");
					break;
				case Eto.Forms.Control.MouseDoubleClickEvent:
					HandleEvent(Eto.Forms.Control.MouseDownEvent);
					break;
				case Eto.Forms.Control.MouseWheelEvent:
					AddMethod(selScrollWheel, new Action<IntPtr, IntPtr, IntPtr>(TriggerMouseWheel), "v@:@");
					break;
				case Eto.Forms.Control.KeyDownEvent:
					AddMethod(selKeyDown, new Action<IntPtr, IntPtr, IntPtr>(TriggerKeyDown), "v@:@");
					break;
				case Eto.Forms.Control.KeyUpEvent:
					AddMethod(selKeyUp, new Action<IntPtr, IntPtr, IntPtr>(TriggerKeyUp), "v@:@");
					break;
				case Eto.Forms.Control.LostFocusEvent:
					AddMethod(selResignFirstResponder, new Func<IntPtr, IntPtr, bool>(TriggerLostFocus), "B@:");
					break;
				case Eto.Forms.Control.GotFocusEvent:
					AddMethod(selBecomeFirstResponder, new Func<IntPtr, IntPtr, bool>(TriggerGotFocus), "B@:");
					break;
				case Eto.Forms.Control.ShownEvent:
				// TODO
					break;
				case Eto.Forms.Control.TextInputEvent:
					AddMethod(selInsertText, new Action<IntPtr, IntPtr, IntPtr>(TriggerTextInput), "v@:@");
					break;
				default:
					base.AttachEvent(id);
					break;

			}
		}

		protected static void TriggerTextInput(IntPtr sender, IntPtr sel, IntPtr textPtr)
		{
			var obj = Runtime.GetNSObject(sender);

			var handler = GetHandler(obj) as IMacViewHandler;
			if (handler != null)
			{
				var text = (string)Messaging.GetNSObject<NSString>(textPtr);
				handler.Callback.OnTextInput(handler.Widget, new TextInputEventArgs(text));
			}
			Messaging.void_objc_msgSendSuper_IntPtr(obj.SuperHandle, sel, textPtr);
		}

		static void SetFrameSizeAction(IntPtr sender, IntPtr sel, CGSize size)
		{
			var obj = Runtime.GetNSObject(sender);
			Messaging.void_objc_msgSendSuper_SizeF(obj.SuperHandle, sel, size);

			var handler = GetHandler(obj) as IMacViewHandler;
			if (handler != null)
			{
				handler.OnSizeChanged(EventArgs.Empty);
				handler.Callback.OnSizeChanged(handler.Widget, EventArgs.Empty);
			}
		}

		protected static bool TriggerGotFocus(IntPtr sender, IntPtr sel)
		{
			var obj = Runtime.GetNSObject(sender);
			var handler = GetHandler(obj) as IMacViewHandler;
			if (handler != null)
			{
				handler.ShouldHaveFocus = true;
				handler.Callback.OnGotFocus(handler.Widget, EventArgs.Empty);
				handler.ShouldHaveFocus = null;
				return Messaging.bool_objc_msgSendSuper(obj.SuperHandle, sel);
			}
			return false;
		}

		protected static bool TriggerLostFocus(IntPtr sender, IntPtr sel)
		{
			var obj = Runtime.GetNSObject(sender);
			var handler = GetHandler(obj) as IMacViewHandler;
			if (handler != null)
			{
				handler.ShouldHaveFocus = false;
				handler.Callback.OnLostFocus(handler.Widget, EventArgs.Empty);
				handler.ShouldHaveFocus = null;
				return Messaging.bool_objc_msgSendSuper(obj.SuperHandle, sel);
			}
			return false;
		}

		static void TriggerKeyDown(IntPtr sender, IntPtr sel, IntPtr e)
		{
			var obj = Runtime.GetNSObject(sender);
			var handler = GetHandler(obj) as IMacViewHandler;
			if (handler != null)
			{
				var theEvent = Messaging.GetNSObject<NSEvent>(e);
				if (!MacEventView.KeyDown(handler.Widget, theEvent))
				{
					Messaging.void_objc_msgSendSuper_IntPtr(obj.SuperHandle, sel, e);
				}
			}
		}

		static void TriggerKeyUp(IntPtr sender, IntPtr sel, IntPtr e)
		{
			var obj = Runtime.GetNSObject(sender);
			var handler = GetHandler(obj) as IMacViewHandler;
			if (handler != null)
			{
				var theEvent = Messaging.GetNSObject<NSEvent>(e);
				if (!MacEventView.KeyUp(handler.Widget, theEvent))
				{
					Messaging.void_objc_msgSendSuper_IntPtr(obj.SuperHandle, sel, e);
				}
			}
		}

		static void TriggerMouseDown(IntPtr sender, IntPtr sel, IntPtr e)
		{
			var obj = Runtime.GetNSObject(sender);
			var handler = GetHandler(obj) as IMacViewHandler;
			if (handler != null)
			{
				var theEvent = Messaging.GetNSObject<NSEvent>(e);
				var args = MacConversions.GetMouseEvent(handler.ContainerControl, theEvent, false);
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

		static void TriggerMouseUp(IntPtr sender, IntPtr sel, IntPtr e)
		{
			var obj = Runtime.GetNSObject(sender);
			var handler = GetHandler(obj) as IMacViewHandler;

			if (handler != null)
			{
				var theEvent = Messaging.GetNSObject<NSEvent>(e);
				var args = MacConversions.GetMouseEvent(handler.ContainerControl, theEvent, false);
				handler.Callback.OnMouseUp(handler.Widget, args);
				if (!args.Handled)
				{
					Messaging.void_objc_msgSendSuper_IntPtr(obj.SuperHandle, sel, e);
				}
			}
		}

		static void TriggerMouseDragged(IntPtr sender, IntPtr sel, IntPtr e)
		{
			var obj = Runtime.GetNSObject(sender);
			var handler = GetHandler(obj) as IMacViewHandler;
			if (handler != null)
			{
				var theEvent = Messaging.GetNSObject<NSEvent>(e);
				var args = MacConversions.GetMouseEvent(handler.ContainerControl, theEvent, false);
				handler.Callback.OnMouseMove(handler.Widget, args);
				if (!args.Handled)
				{
					Messaging.void_objc_msgSendSuper_IntPtr(obj.SuperHandle, sel, e);
				}
			}
		}

		static void TriggerMouseWheel(IntPtr sender, IntPtr sel, IntPtr e)
		{
			var obj = Runtime.GetNSObject(sender);
			var handler = GetHandler(obj) as IMacViewHandler;
			if (handler != null)
			{
				var theEvent = Messaging.GetNSObject<NSEvent>(e);
				var args = MacConversions.GetMouseEvent(handler.ContainerControl, theEvent, true);
				if (!args.Delta.IsZero)
				{
					handler.Callback.OnMouseWheel(handler.Widget, args);
					if (!args.Handled)
					{
						Messaging.void_objc_msgSendSuper_IntPtr(obj.SuperHandle, sel, e);
					}
				}
			}
		}

		public virtual void OnSizeChanged(EventArgs e)
		{
			CreateTracking();
		}

		public virtual void Invalidate()
		{
			ContainerControl.NeedsDisplay = true;
		}

		public virtual void Invalidate(Rectangle rect)
		{
			var region = rect.ToNS();
			region.Y = EventControl.Frame.Height - region.Y - region.Height;
			EventControl.SetNeedsDisplayInRect(region);
		}

		public void SuspendLayout()
		{
		}

		public void ResumeLayout()
		{
		}


		static readonly object InitialFocusKey = new object();

		bool InitialFocus
		{
			get { return Widget.Properties.Get<bool?>(InitialFocusKey) ?? false; }
			set { Widget.Properties[InitialFocusKey] = value ? (object)true : null; }
		}

		public virtual void Focus()
		{
			if (FocusControl.Window != null)
				FocusControl.Window.MakeFirstResponder(FocusControl);
			else
				InitialFocus = true;
		}

		static readonly object BackgroundColorKey = new object();
		public virtual Color BackgroundColor
		{
			get { return Widget.Properties.Get<Color?>(BackgroundColorKey) ?? Colors.Transparent; }
			set
			{
				Widget.Properties[BackgroundColorKey] = value;
				if (Widget.Loaded)
					SetBackgroundColor(value);
			}
		}

		protected virtual void SetBackgroundColor(Color? color)
		{
			if (color != null) {
				if (color.Value.A > 0) {
					ContainerControl.WantsLayer = true;
					var layer = ContainerControl.Layer;
					if (layer != null)
						layer.BackgroundColor = color.Value.ToCG();
				}
				else {
					ContainerControl.WantsLayer = false;
					var layer = ContainerControl.Layer;
					if (layer != null)
						layer.BackgroundColor = null;
				}
			}
		}

		public abstract bool Enabled { get; set; }

		static readonly object ShouldHaveFocusKey = new object();

		public bool? ShouldHaveFocus
		{
			get { return Widget.Properties.Get<bool?>(ShouldHaveFocusKey); }
			set { Widget.Properties[ShouldHaveFocusKey] = value; }
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
			get { return !ContentControl.Hidden; }
			set
			{ 
				if (ContentControl.Hidden == value)
				{
					var oldSize = GetPreferredSize(Size.MaxValue);
					ContentControl.Hidden = !value;
					LayoutIfNeeded(oldSize, true);
				}
			}
		}

		static readonly IntPtr selResetCursorRects = Selector.GetHandle("resetCursorRects");

		static void TriggerResetCursorRects(IntPtr sender, IntPtr sel)
		{
			var obj = Runtime.GetNSObject(sender);
			var handler = GetHandler(obj) as IMacViewHandler;
			if (handler != null)
			{
				var cursor = handler.CurrentCursor;
				if (cursor != null)
				{
					handler.EventControl.AddCursorRect(new CGRect(CGPoint.Empty, handler.EventControl.Frame.Size), cursor.ControlObject as NSCursor);
				}
			}
		}

		public virtual Cursor CurrentCursor
		{
			get { return Cursor; }
		}

		static readonly object CursorKey = new object();

		public virtual Cursor Cursor
		{
			get { return Widget.Properties.Get<Cursor>(CursorKey); }
			set {
				if (Cursor != value)
				{
					Widget.Properties[CursorKey] = value;
					AddMethod(selResetCursorRects, new Action<IntPtr, IntPtr>(TriggerResetCursorRects), "v@:");
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
		}

		public virtual void OnLoadComplete(EventArgs e)
		{
			if (InitialFocus && FocusControl.Window != null)
			{
				FocusControl.Window.MakeFirstResponder(FocusControl);
				InitialFocus = false;
			}
			SetBackgroundColor(Widget.Properties.Get<Color?>(BackgroundColorKey));
		}

		public virtual void OnUnLoad(EventArgs e)
		{
		}

		public virtual void OnKeyDown(KeyEventArgs e)
		{
			Callback.OnKeyDown(Widget, e);
		}

		Control IMacViewHandler.Widget { get { return Widget; } }

		public virtual PointF PointFromScreen(PointF point)
		{
			var sdpoint = point.ToNS();
			if (EventControl.Window != null)
			{
				sdpoint.Y = ContentControl.Window.Screen.Frame.Height - sdpoint.Y;
				sdpoint = ContentControl.Window.ConvertScreenToBase(sdpoint);
			}
			sdpoint = ContentControl.ConvertPointFromView(sdpoint, null);
			sdpoint.Y = ContentControl.Frame.Height - sdpoint.Y;
			return sdpoint.ToEto();
		}

		public virtual PointF PointToScreen(PointF point)
		{
			var sdpoint = point.ToNS();
			sdpoint.Y = ContentControl.Frame.Height - sdpoint.Y;
			sdpoint = ContentControl.ConvertPointToView(sdpoint, null);
			if (ContentControl.Window != null)
			{
				sdpoint = ContentControl.Window.ConvertBaseToScreen(sdpoint);
				sdpoint.Y = ContentControl.Window.Screen.Frame.Height - sdpoint.Y;
			}
			return sdpoint.ToEto();
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

		static void TriggerSystemAction(IntPtr sender, IntPtr sel, IntPtr e)
		{
			var control = Runtime.GetNSObject(sender);
			var handler = GetHandler(control) as MacView<TControl,TWidget,TCallback>;
			if (handler != null)
			{
				Command command;
				if (handler.systemActions != null && handler.systemActions.TryGetValue(sel, out command))
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

		static bool ValidateSystemUserInterfaceItem(IntPtr sender, IntPtr sel, IntPtr item)
		{
			var actionHandle = Messaging.IntPtr_objc_msgSend(item, selGetAction);

			var control = Runtime.GetNSObject(sender);
			var handler = GetHandler(control) as MacView<TControl,TWidget,TCallback>;
			if (handler != null)
			{
				Command command;
				if (handler.systemActions != null && actionHandle != IntPtr.Zero && handler.systemActions.TryGetValue(actionHandle, out command))
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

		Dictionary<IntPtr, Command> systemActions;
		static readonly IntPtr selGetAction = Selector.GetHandle("action");
		static readonly IntPtr selValidateUserInterfaceItem = Selector.GetHandle("validateUserInterfaceItem:");
		static readonly IntPtr selCut = Selector.GetHandle("cut:");
		static readonly IntPtr selCopy = Selector.GetHandle("copy:");
		static readonly IntPtr selPaste = Selector.GetHandle("paste:");
		static readonly IntPtr selSelectAll = Selector.GetHandle("selectAll:");
		static readonly IntPtr selDelete = Selector.GetHandle("delete:");
		static readonly IntPtr selUndo = Selector.GetHandle("undo:");
		static readonly IntPtr selRedo = Selector.GetHandle("redo:");
		static readonly IntPtr selPasteAsPlainText = Selector.GetHandle("pasteAsPlainText:");
		static readonly IntPtr selPerformClose = Selector.GetHandle("performClose:");
		static readonly IntPtr selPerformZoom = Selector.GetHandle("performZoom:");
		static readonly IntPtr selArrangeInFront = Selector.GetHandle("arrangeInFront:");
		static readonly IntPtr selPerformMiniaturize = Selector.GetHandle("performMiniaturize:");
		static readonly Dictionary<string, IntPtr> systemActionSelectors = new Dictionary<string, IntPtr>
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

		public virtual IEnumerable<string> SupportedPlatformCommands
		{
			get { return systemActionSelectors.Keys; }
		}

		public void MapPlatformCommand(string systemAction, Command command)
		{
			InnerMapPlatformCommand(systemAction, command, null);
		}

		protected virtual void InnerMapPlatformCommand(string systemAction, Command command, NSObject control)
		{
			IntPtr sel;
			if (systemActionSelectors.TryGetValue(systemAction, out sel))
			{
				if (systemActions == null)
				{
					systemActions = new Dictionary<IntPtr, Command>();
					AddMethod(selValidateUserInterfaceItem, new Func<IntPtr, IntPtr, IntPtr, bool>(ValidateSystemUserInterfaceItem), "B@:@", control);
				}
				AddMethod(sel, new Action<IntPtr, IntPtr, IntPtr>(TriggerSystemAction), "v@:@", control);
				systemActions[sel] = command;
			}
		}
	}
}

