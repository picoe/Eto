using System;
using MonoMac.AppKit;
using Eto.Drawing;
using Eto.Forms;
using MonoMac.CoreGraphics;
using MonoMac.Foundation;
using Eto.Platform.Mac.Drawing;
using MonoMac.ObjCRuntime;
using SD = System.Drawing;
using Eto.Platform.Mac.Forms.Controls;
using System.Collections.Generic;
using Eto.Platform.Mac.Forms.Printing;

namespace Eto.Platform.Mac.Forms
{
	class MouseDelegate : NSObject
	{
		public NSView View { get; set; }
	
		public Control Widget { get; set; }

		[Export("mouseMoved:")]
		public void MouseMoved (NSEvent theEvent)
		{
			Widget.OnMouseMove (Conversions.GetMouseEvent (View, theEvent, false));
		}
		
		[Export("mouseEntered:")]
		public void MouseEntered (NSEvent theEvent)
		{
			Widget.OnMouseEnter (Conversions.GetMouseEvent (View, theEvent, false));
		}

		[Export("cursorUpdate:")]
		public void CursorUpdate (NSEvent theEvent)
		{
		}
	
		[Export("mouseExited:")]
		public void MouseExited (NSEvent theEvent)
		{
			Widget.OnMouseLeave (Conversions.GetMouseEvent (View, theEvent, false));
		}

		[Export("scrollWheel:")]
		public void ScrollWheel (NSEvent theEvent)
		{
			Widget.OnMouseWheel (Conversions.GetMouseEvent (View, theEvent, true));
		}
	}
	
	public interface IMacAutoSizing
	{
		bool AutoSize { get; }
		
		Size GetPreferredSize (Size availableSize);
	}

	public interface IMacViewHandler : IMacAutoSizing
	{
		Size PositionOffset { get; }
		
		Size? PreferredSize { get; }
		
		Size MinimumSize { get; set; }
		
		Control Widget { get; }
		
		Cursor Cursor { get; set; }

		bool IsEventHandled (string eventName);

		void PostKeyDown (KeyEventArgs e);
	}
	
	public interface IMacContainerControl
	{
		NSView ContainerControl { get; }

		NSView ContentControl { get; }

		NSView EventControl { get; }
	}

	public abstract class MacView<T, W> : MacObject<T, W>, IControl, IMacViewHandler, IMacContainerControl
		where T: NSResponder
		where W: Control
	{
		bool focus;
		NSTrackingArea tracking;
		bool mouseMove;
		NSTrackingAreaOptions mouseOptions;
		MouseDelegate mouseDelegate;
		Cursor cursor;
		Size? oldFrameSize;
		Size? naturalSize;
		
		public abstract NSView ContainerControl { get; }

		public virtual NSView ContentControl { get { return ContainerControl; } }

		public virtual NSView EventControl { get { return ContainerControl; } }
		
		public virtual bool AutoSize { get; protected set; }

		public virtual Size Size {
			get { return ContainerControl.Frame.Size.ToEtoSize (); }
			set { 
				var oldSize = GetPreferredSize (Size.MaxValue);
				this.PreferredSize = value;
				Conversions.SetSizeWithAuto (ContainerControl, value);
				this.AutoSize = false;
				CreateTracking ();
				LayoutIfNeeded (oldSize);
			}
		}
		
		protected virtual bool LayoutIfNeeded (Size? oldPreferredSize = null, bool force = false)
		{
			naturalSize = null;
			if (Widget.Loaded) {
				var oldSize = oldPreferredSize ?? ContainerControl.Frame.Size.ToEtoSize ();
				var newSize = GetPreferredSize (Size.MaxValue);
				if (newSize != oldSize || force) {
					var container = Widget.Parent.GetMacContainer();
					if (container != null)
						container.LayoutParent (true);
					return true;
				}
			}
			return false;
		}

		public virtual Size MinimumSize {
			get;
			set;
		}
		
		public virtual Size? MaximumSize {
			get;
			set;
		}
		
		public Size? PreferredSize {
			get;
			set;
		}
		
		public MacView ()
		{
			this.AutoSize = true;
		}
		
		protected virtual Size GetNaturalSize (Size availableSize)
		{
			if (naturalSize != null) 
				return naturalSize.Value;
			var control = Control as NSControl;
			if (control != null) {
				SD.SizeF? size = (Widget.Loaded) ? (SD.SizeF?)control.Frame.Size : null;
				control.SizeToFit ();
				naturalSize = control.Frame.Size.ToEtoSize ();
				if (size != null)
					control.SetFrameSize (size.Value);
				return naturalSize.Value;
			}
			return Size.Empty;
		}
		
		public virtual Size GetPreferredSize (Size availableSize)
		{
			var size = GetNaturalSize (availableSize);
			if (!AutoSize && PreferredSize != null) {
				var preferredSize = PreferredSize.Value;
				if (preferredSize.Width >= 0)
					size.Width = preferredSize.Width;
				if (preferredSize.Height >= 0)
					size.Height = preferredSize.Height;
			}
			if (MinimumSize != Size.Empty)
				size = Size.Max (size, MinimumSize);
			if (MaximumSize != null)
				size = Size.Min (size, MaximumSize.Value);
			return size;
		}
		
		public virtual Size PositionOffset { get { return Size.Empty; } }
		
		void CreateTracking ()
		{
			if (!mouseMove)
				return;
			if (tracking != null)
				EventControl.RemoveTrackingArea (tracking);
			//Console.WriteLine ("Adding mouse tracking {0} for area {1}", this.Widget.GetType ().FullName, Control.Frame.Size);
			if (mouseDelegate == null)
				mouseDelegate = new MouseDelegate{ Widget = this.Widget, View = EventControl };
			tracking = new NSTrackingArea (new SD.RectangleF (new SD.PointF (0, 0), EventControl.Frame.Size), 
				NSTrackingAreaOptions.ActiveAlways | mouseOptions | NSTrackingAreaOptions.EnabledDuringMouseDrag | NSTrackingAreaOptions.InVisibleRect, 
			    mouseDelegate, 
				new NSDictionary ());
			EventControl.AddTrackingArea (tracking);
		}

		public virtual void SetParent (Container parent)
		{
		}

		static Selector selMouseDown = new Selector ("mouseDown:");
		static Selector selMouseUp = new Selector ("mouseUp:");
		static Selector selMouseDragged = new Selector ("mouseDragged:");
		static Selector selRightMouseDown = new Selector ("rightMouseDown:");
		static Selector selRightMouseUp = new Selector ("rightMouseUp:");
		static Selector selRightMouseDragged = new Selector ("rightMouseDragged:");
		static Selector selScrollWheel = new Selector ("scrollWheel:");
		static Selector selKeyDown = new Selector ("keyDown:");
		static Selector selKeyUp = new Selector ("keyUp:");
		static Selector selBecomeFirstResponder = new Selector ("becomeFirstResponder");
		static Selector selResignFirstResponder = new Selector ("resignFirstResponder");
		
		public override void AttachEvent (string handler)
		{
			switch (handler) {
			case Eto.Forms.Control.MouseEnterEvent:
				HandleEvent (Eto.Forms.Control.MouseLeaveEvent);
				break;
			case Eto.Forms.Control.MouseLeaveEvent:
				mouseOptions |= NSTrackingAreaOptions.MouseEnteredAndExited;
				mouseMove = true;
				HandleEvent (Eto.Forms.Control.SizeChangedEvent);
				CreateTracking ();
				break;
			case Eto.Forms.Control.MouseMoveEvent:
				mouseOptions |= NSTrackingAreaOptions.MouseMoved;
				mouseMove = true;
				HandleEvent (Eto.Forms.Control.SizeChangedEvent);
				CreateTracking ();
				AddMethod (selMouseDragged, new Action<IntPtr, IntPtr, IntPtr> (TriggerMouseDragged), "v@:@");
				AddMethod (selRightMouseDragged, new Action<IntPtr, IntPtr, IntPtr> (TriggerMouseDragged), "v@:@");
				break;
			case Eto.Forms.Control.SizeChangedEvent:
				ContainerControl.PostsFrameChangedNotifications = true;
				this.AddObserver (NSView.NSViewFrameDidChangeNotification, e => {
					var w = (Control)e.Widget;
					var h = ((MacView<T, W>)(e.Widget.Handler));
					var oldFrameSize = h.oldFrameSize;
					h.OnSizeChanged (EventArgs.Empty);
					var newSize = h.Size;
					if (oldFrameSize == null || oldFrameSize.Value != newSize) {
						w.OnSizeChanged (EventArgs.Empty);
						h.oldFrameSize = newSize;
					}
				});
				break;
			case Eto.Forms.Control.MouseDownEvent:
				AddMethod (selMouseDown, new Action<IntPtr, IntPtr, IntPtr> (TriggerMouseDown), "v@:@");
				AddMethod (selRightMouseDown, new Action<IntPtr, IntPtr, IntPtr> (TriggerMouseDown), "v@:@");
				break;
			case Eto.Forms.Control.MouseUpEvent:
				AddMethod (selMouseUp, new Action<IntPtr, IntPtr, IntPtr> (TriggerMouseUp), "v@:@");
				AddMethod (selRightMouseUp, new Action<IntPtr, IntPtr, IntPtr> (TriggerMouseUp), "v@:@");
				break;
			case Eto.Forms.Control.MouseDoubleClickEvent:
				HandleEvent (Eto.Forms.Control.MouseDownEvent);
				break;
			case Eto.Forms.Control.MouseWheelEvent:
				AddMethod (selScrollWheel, new Action<IntPtr, IntPtr, IntPtr> (TriggerMouseWheel), "v@:@");
				break;
			case Eto.Forms.Control.KeyDownEvent:
				AddMethod (selKeyDown, new Action<IntPtr, IntPtr, IntPtr> (TriggerKeyDown), "v@:@");
				break;
			case Eto.Forms.Control.KeyUpEvent:
				AddMethod (selKeyUp, new Action<IntPtr, IntPtr, IntPtr> (TriggerKeyUp), "v@:@");
				break;
			case Eto.Forms.Control.LostFocusEvent:
				AddMethod (selResignFirstResponder, new Func<IntPtr, IntPtr, bool> (TriggerLostFocus), "B@:");
				break;
			case Eto.Forms.Control.GotFocusEvent:
				AddMethod (selBecomeFirstResponder, new Func<IntPtr, IntPtr, bool> (TriggerGotFocus), "B@:");
				break;
			case Eto.Forms.Control.ShownEvent:
				// TODO
				break;
			default:
				base.AttachEvent (handler);
				break;

			}
		}

		static bool TriggerGotFocus (IntPtr sender, IntPtr sel)
		{
			var obj = Runtime.GetNSObject (sender);
			var handler = (MacView<T,W>)((IMacControl)obj).Handler;
			handler.Widget.OnGotFocus (EventArgs.Empty);
			return Messaging.bool_objc_msgSendSuper (obj.SuperHandle, sel);
		}

		static bool TriggerLostFocus (IntPtr sender, IntPtr sel)
		{
			var obj = Runtime.GetNSObject (sender);
			var handler = (MacView<T,W>)((IMacControl)obj).Handler;
			handler.Widget.OnLostFocus (EventArgs.Empty);
			return Messaging.bool_objc_msgSendSuper (obj.SuperHandle, sel);
		}

		static void TriggerKeyDown (IntPtr sender, IntPtr sel, IntPtr e)
		{
			var obj = Runtime.GetNSObject (sender);
			var handler = (MacView<T,W>)((IMacControl)obj).Handler;
			var theEvent = new NSEvent (e);
			if (!MacEventView.KeyDown (handler.Widget, theEvent)) {
				Messaging.void_objc_msgSendSuper_IntPtr (obj.SuperHandle, sel, e);
			}
		}
		
		static void TriggerKeyUp (IntPtr sender, IntPtr sel, IntPtr e)
		{
			var obj = Runtime.GetNSObject (sender);
			var handler = (MacView<T,W>)((IMacControl)obj).Handler;
			var theEvent = new NSEvent (e);
			if (!MacEventView.KeyUp (handler.Widget, theEvent)) {
				Messaging.void_objc_msgSendSuper_IntPtr (obj.SuperHandle, sel, e);
			}
		}
		
		static void TriggerMouseDown (IntPtr sender, IntPtr sel, IntPtr e)
		{
			var obj = Runtime.GetNSObject (sender);
			var handler = (MacView<T,W>)((IMacControl)obj).Handler;
			var theEvent = new NSEvent (e);
			var args = Conversions.GetMouseEvent ((NSView)obj, theEvent, false);
			if (theEvent.ClickCount >= 2)
				handler.Widget.OnMouseDoubleClick (args);
			
			if (!args.Handled) {
				handler.Widget.OnMouseDown (args);
			}
			if (!args.Handled) {
				Messaging.void_objc_msgSendSuper_IntPtr (obj.SuperHandle, sel, e);
			}
		}

		static void TriggerMouseUp (IntPtr sender, IntPtr sel, IntPtr e)
		{
			var obj = Runtime.GetNSObject (sender);
			var handler = (MacView<T,W>)((IMacControl)obj).Handler;

			var theEvent = new NSEvent (e);
			var args = Conversions.GetMouseEvent ((NSView)obj, theEvent, false);
			handler.Widget.OnMouseUp (args);
			if (!args.Handled) {
				Messaging.void_objc_msgSendSuper_IntPtr (obj.SuperHandle, sel, e);
			}
		}

		static void TriggerMouseDragged (IntPtr sender, IntPtr sel, IntPtr e)
		{
			var obj = Runtime.GetNSObject (sender);
			var handler = (MacView<T,W>)((IMacControl)obj).Handler;
			
			var theEvent = new NSEvent (e);
			var args = Conversions.GetMouseEvent ((NSView)obj, theEvent, false);
			handler.Widget.OnMouseMove (args);
			if (!args.Handled) {
				Messaging.void_objc_msgSendSuper_IntPtr (obj.SuperHandle, sel, e);
			}
		}

		static void TriggerMouseWheel (IntPtr sender, IntPtr sel, IntPtr e)
		{
			var obj = Runtime.GetNSObject (sender);
			var handler = (MacView<T,W>)((IMacControl)obj).Handler;
			
			var theEvent = new NSEvent (e);
			var args = Conversions.GetMouseEvent ((NSView)obj, theEvent, true);
			if (!args.Delta.IsZero) {
				handler.Widget.OnMouseWheel (args);
				if (!args.Handled) {
					Messaging.void_objc_msgSendSuper_IntPtr (obj.SuperHandle, sel, e);
				}
			}
		}

		protected virtual void OnSizeChanged (EventArgs e)
		{
			CreateTracking ();
		}
		
		public virtual void Invalidate ()
		{
			ContainerControl.NeedsDisplay = true;
		}

		public virtual void Invalidate (Rectangle rect)
		{
			var region = rect.ToSDRectangleF ();
			region.Y = EventControl.Frame.Height - region.Y - region.Height;
			EventControl.SetNeedsDisplayInRect (region);
		}

		public void SuspendLayout ()
		{
		}

		public void ResumeLayout ()
		{
		}

		public virtual void Focus ()
		{
			if (EventControl.Window != null)
				EventControl.Window.MakeFirstResponder (EventControl);
			else
				focus = true;
		}

		Color? backgroundColor;
		public virtual Color BackgroundColor
		{
			get { return backgroundColor ?? Colors.Transparent; }
			set
			{
				if (value.A > 0) {
					if (!EventControl.WantsLayer)
						EventControl.WantsLayer = true;
					EventControl.Layer.BackgroundColor = value.ToCGColor ();
				} else {
					EventControl.WantsLayer = false;
					if (EventControl.Layer != null)
						EventControl.Layer.BackgroundColor = null;
				}
				backgroundColor = value;
			}
		}



		public abstract bool Enabled { get; set; }

		public virtual bool HasFocus {
			get {
				return EventControl.Window != null && EventControl.Window.FirstResponder == Control;
			}
		}

		public virtual bool Visible {
			get { return !ContentControl.Hidden; }
			set { 
				if (ContentControl.Hidden == value) {
					var oldSize = this.GetPreferredSize (Size.MaxValue);
					ContentControl.Hidden = !value;
					LayoutIfNeeded (oldSize, true);
				}
			}
		}
		
		public virtual Cursor Cursor {
			get { return cursor; }
			set { cursor = value; }
		}
		
		public string ToolTip {
			get { return ContentControl.ToolTip; }
			set { ContentControl.ToolTip = value; }
		}

		public void Print (PrintSettings settings)
		{
			var op = NSPrintOperation.FromView(EventControl);
			if (settings != null)
				op.PrintInfo = ((PrintSettingsHandler)settings.Handler).Control;
			op.ShowsPrintPanel = false;
			op.RunOperation ();
		}
		
		public virtual void OnPreLoad (EventArgs e)
		{
		}
		
		public virtual void OnLoad (EventArgs e)
		{
		}
		
		public virtual void OnLoadComplete (EventArgs e)
		{
			if (focus && EventControl.Window != null)
				EventControl.Window.MakeFirstResponder (Control);
		}

		public virtual void OnUnLoad (EventArgs e)
		{
		}

		public virtual void PostKeyDown (KeyEventArgs e)
		{
		}

		Control IMacViewHandler.Widget { get { return this.Widget; } }

        public virtual PointF PointFromScreen (PointF point)
        {
			var sdpoint = point.ToSD ();
			if (EventControl.Window != null) {
				sdpoint.Y = ContentControl.Window.Screen.Frame.Height - sdpoint.Y;
				sdpoint = ContentControl.Window.ConvertScreenToBase (sdpoint);
			}
			sdpoint = ContentControl.ConvertPointFromView (sdpoint, null);
			sdpoint.Y = ContentControl.Frame.Height - sdpoint.Y;
			return Platform.Conversions.ToEto (sdpoint);
		}

        public virtual PointF PointToScreen (PointF point)
        {
			var sdpoint = point.ToSD ();
			sdpoint.Y = ContentControl.Frame.Height - sdpoint.Y;
			sdpoint = ContentControl.ConvertPointToView (sdpoint, null);
			if (ContentControl.Window != null) {
				sdpoint = ContentControl.Window.ConvertBaseToScreen (sdpoint);
				sdpoint.Y = ContentControl.Window.Screen.Frame.Height - sdpoint.Y;
			}
			return Platform.Conversions.ToEto (sdpoint);
        }

        Point IControl.Location
        {
			get { return Platform.Conversions.ToEtoPoint (ContentControl.Frame.Location); }
        }

		static void TriggerSystemAction (IntPtr sender, IntPtr sel, IntPtr e)
		{
			var selector = new Selector (sel);
			
			var control = Runtime.GetNSObject (sender);
			var handler = (MacView<T,W>)((IMacControl)control).Handler;
			BaseAction action;
			if (handler.systemActions != null && handler.systemActions.TryGetValue (selector.Name, out action)) {
				action.Activate ();
			}
		}
		
		static bool ValidateSystemMenuAction (IntPtr sender, IntPtr sel, IntPtr item)
		{
			var menuItem = new NSMenuItem (item);
			
			var control = Runtime.GetNSObject (sender);
			var macControl = control as IMacControl;
			if (macControl != null) {
				var handler = (MacView<T,W>)macControl.Handler;
				BaseAction action;
				if (handler.systemActions != null && menuItem.Action != null && handler.systemActions.TryGetValue (menuItem.Action.Name, out action)) {
					if (action != null)
						return action.Enabled;
				}
			}
			return false;
		}

		static bool ValidateSystemToolbarAction (IntPtr sender, IntPtr sel, IntPtr item)
		{
			var toolbarItem = new NSToolbarItem (item);
			
			var control = Runtime.GetNSObject (sender);
			var handler = (MacView<T,W>)((IMacControl)control).Handler;
			BaseAction action;
			if (handler.systemActions != null && toolbarItem.Action != null && handler.systemActions.TryGetValue (toolbarItem.Action.Name, out action)) {
				if (action != null)
					return action.Enabled;
			}
			return false;
		}
		
		Dictionary<string, BaseAction> systemActions;
		static Selector selValidateMenuItem = new Selector ("validateMenuItem:");
		static Selector selValidateToolbarItem = new Selector ("validateToolbarItem:");
		static Selector selCut = new Selector ("cut:");
		static Selector selCopy = new Selector ("copy:");
		static Selector selPaste = new Selector ("paste:");
		static Selector selSelectAll = new Selector ("selectAll:");
		static Selector selDelete = new Selector ("delete:");
		static Selector selUndo = new Selector ("undo:");
		static Selector selRedo = new Selector ("redo:");
		static Selector selPasteAsPlainText = new Selector ("pasteAsPlainText:");
		static Selector selPerformClose = new Selector ("performClose:");
		static Selector selPerformZoom = new Selector ("performZoom:");
		static Selector selArrangeInFront = new Selector ("arrangeInFront:");
		static Selector selPerformMiniaturize = new Selector ("performMiniaturize:");
		static Dictionary<string, Selector> systemActionSelectors = new Dictionary<string, Selector> ()
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

		public virtual void MapPlatformAction (string systemAction, BaseAction action)
		{
			Selector sel;
			if (systemActionSelectors.TryGetValue (systemAction, out sel)) {
				if (sel != null) {
					if (systemActions == null) {
						systemActions = new Dictionary<string, BaseAction> ();
						AddMethod (selValidateMenuItem, new Func<IntPtr, IntPtr, IntPtr, bool> (ValidateSystemMenuAction), "B@:@");
						AddMethod (selValidateToolbarItem, new Func<IntPtr, IntPtr, IntPtr, bool> (ValidateSystemToolbarAction), "B@:@");
					}
					AddMethod (sel, new Action<IntPtr, IntPtr, IntPtr> (TriggerSystemAction), "v@:@");
					systemActions [sel.Name] = action;
				}
			}
		}
    }
}

