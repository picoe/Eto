using System;
using Eto.Drawing;
using Eto.Forms;
using Eto.Mac.Forms.Controls;
using System.Collections.Generic;
using Eto.Mac.Forms.Printing;
using System.Linq;

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

		public virtual NSView EventControl { get { return ContainerControl; } }

		public virtual NSView FocusControl { get { return EventControl; } }

		public virtual IEnumerable<Control> VisualControls => Enumerable.Empty<Control>();

		static readonly object AutoSize_Key = new object();
		public virtual bool AutoSize
		{
			get { return Widget.Properties.Get<bool>(AutoSize_Key, true); }
			protected set { Widget.Properties.Set(AutoSize_Key, value, true); }
		}

		protected virtual Size DefaultMinimumSize
		{
			get { return Size.Empty; }
		}

		static readonly object MinimumSize_Key = new object();
		public virtual Size MinimumSize
		{
			get { return Widget.Properties.Get<Size?>(MinimumSize_Key) ?? DefaultMinimumSize; }
			set { Widget.Properties[MinimumSize_Key] = value; NaturalSize = null; }
		}

		static readonly object MaximumSize_Key = new object();
		public virtual SizeF MaximumSize
		{
			get { return Widget.Properties.Get<SizeF?>(MaximumSize_Key) ?? SizeF.MaxValue; }
			set { Widget.Properties[MaximumSize_Key] = value; }
		}

		static readonly object PreferredSize_Key = new object();
		public Size? PreferredSize
		{
			get { return Widget.Properties.Get<Size?>(PreferredSize_Key); }
			set { Widget.Properties[PreferredSize_Key] = value; }
		}

		public virtual Size Size
		{
			get
			{
				if (!Widget.Loaded)
					return PreferredSize ?? new Size(-1, -1);
				return ContainerControl.Frame.Size.ToEtoSize();
			}
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

		static readonly object NaturalSize_Key = new object();
		protected SizeF? NaturalSize
		{
			get { return Widget.Properties.Get<SizeF?>(NaturalSize_Key); }
			set { Widget.Properties[NaturalSize_Key] = value; }
		}

		public virtual NSObject CustomFieldEditor { get { return null; } }

		protected virtual bool LayoutIfNeeded(SizeF? oldPreferredSize = null, bool force = false)
		{
			NaturalSize = null;
			if (Widget.Loaded)
			{
				var oldSize = oldPreferredSize ?? ContainerControl.Frame.Size.ToEtoSize();
				var newSize = GetPreferredSize(Size.MaxValue);
				if (newSize != oldSize || force)
				{
					var container = Widget.VisualParent.GetMacContainer();
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
			SizeF size;
			if (PreferredSize != null)
			{
				var preferredSize = PreferredSize.Value;
				// only get natural size if the size isn't explicitly set.
				if (preferredSize.Width == -1 || preferredSize.Height == -1)
					size = GetNaturalSize(availableSize);
				else
					size = SizeF.Empty;

				if (preferredSize.Width >= 0)
					size.Width = preferredSize.Width;
				if (preferredSize.Height >= 0)
					size.Height = preferredSize.Height;
			}
			else
				size = GetNaturalSize(availableSize);
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
					AddMethod(selSetFrameSize, new Action<IntPtr, IntPtr, CGSize>(SetFrameSizeAction), EtoEnvironment.Is64BitProcess ? "v@:{CGSize=dd}" : "v@:{CGSize=ff}", ContainerControl);
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
				var args = new TextInputEventArgs(text);
				handler.Callback.OnTextInput(handler.Widget, args);
				if (args.Cancel)
					return;
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
				var result = Messaging.bool_objc_msgSendSuper(obj.SuperHandle, sel);
				handler.Callback.OnGotFocus(handler.Widget, EventArgs.Empty);
				handler.ShouldHaveFocus = null;
				return result;
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
				var result = Messaging.bool_objc_msgSendSuper(obj.SuperHandle, sel);
				handler.Callback.OnLostFocus(handler.Widget, EventArgs.Empty);
				handler.ShouldHaveFocus = null;
				return result;
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
				Callback.OnMouseUp(Widget, MacConversions.GetMouseEvent(ContainerControl, evt, false));
			}
			if (evt.Type == NSEventType.LeftMouseDragged || evt.Type == NSEventType.RightMouseDragged || evt.Type == NSEventType.OtherMouseDragged)
			{
				Callback.OnMouseMove(Widget, MacConversions.GetMouseEvent(ContainerControl, evt, false));
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
			if (!Widget.IsSuspended && Widget.Loaded)
				LayoutIfNeeded();
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

		static IntPtr selDrawRect = Selector.GetHandle("drawRect:");

		static void DrawBackgroundRect(IntPtr sender, IntPtr sel, CGRect rect)
		{
			var control = Runtime.GetNSObject(sender);
			var handler = GetHandler(control) as MacView<TControl, TWidget, TCallback>;
			if (handler != null)
			{
				var col = handler.BackgroundColor;
				if (col.A > 0)
				{
					var context = NSGraphicsContext.CurrentContext.GraphicsPort;
					context.SetFillColor(col.ToCG());
					context.FillRect(rect);
				}
			}
			Messaging.void_objc_msgSendSuper_CGRect(control.SuperHandle, sel, rect);
		}

		protected virtual void SetBackgroundColor(Color? color)
		{
			if (color != null)
			{
				if (color.Value.A > 0)
				{
					AddMethod(selDrawRect, new Action<IntPtr, IntPtr, CGRect>(DrawBackgroundRect), EtoEnvironment.Is64BitProcess ? "v@:{CGRect dddd}" : "v@:{CGRect ffff}", ContainerControl);
				}
				ContainerControl.SetNeedsDisplay();
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
			get { return !ContainerControl.Hidden; }
			set
			{ 
				if (ContainerControl.Hidden == value)
				{
					var oldSize = GetPreferredSize(Size.MaxValue);
					ContainerControl.Hidden = !value;
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

		static readonly object Cursor_Key = new object();

		public virtual Cursor Cursor
		{
			get { return Widget.Properties.Get<Cursor>(Cursor_Key); }
			set {
				if (Cursor != value)
				{
					Widget.Properties[Cursor_Key] = value;
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
			var bg = Widget.Properties.Get<Color?>(BackgroundColorKey);
			if (bg != null)
				SetBackgroundColor(bg);
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
			if (!ContentControl.IsFlipped)
				sdpoint.Y = ContentControl.Frame.Height - sdpoint.Y;
			return sdpoint.ToEto();
		}

		public virtual PointF PointToScreen(PointF point)
		{
			var sdpoint = point.ToNS();
			if (!ContentControl.IsFlipped)
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

		static readonly object TabIndex_Key = new object();

		public int TabIndex
		{
			get { return Widget.Properties.Get<int>(TabIndex_Key, int.MaxValue); }
			set { Widget.Properties.Set(TabIndex_Key, value, int.MaxValue); }
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
	}
}

