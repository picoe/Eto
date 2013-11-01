using System;
using MonoMac.AppKit;
using Eto.Drawing;
using Eto.Forms;
using MonoMac.Foundation;
using MonoMac.ObjCRuntime;
using SD = System.Drawing;
using Eto.Platform.Mac.Forms.Controls;
using System.Collections.Generic;
using Eto.Platform.Mac.Forms.Printing;
using MonoMac.CoreAnimation;

namespace Eto.Platform.Mac.Forms
{
	class MouseDelegate : NSObject
	{
		WeakReference view;

		public NSView View { get { return (NSView)view.Target; } set { view = new WeakReference(value); } }

		WeakReference widget;

		public Control Widget { get { return (Control)widget.Target; } set { widget = new WeakReference(value); } }

		[Export("mouseMoved:")]
		public void MouseMoved(NSEvent theEvent)
		{
			Widget.OnMouseMove(Conversions.GetMouseEvent(View, theEvent, false));
		}

		[Export("mouseEntered:")]
		public void MouseEntered(NSEvent theEvent)
		{
			Widget.OnMouseEnter(Conversions.GetMouseEvent(View, theEvent, false));
		}

		[Export("cursorUpdate:")]
		public void CursorUpdate(NSEvent theEvent)
		{
		}

		[Export("mouseExited:")]
		public void MouseExited(NSEvent theEvent)
		{
			Widget.OnMouseLeave(Conversions.GetMouseEvent(View, theEvent, false));
		}

		[Export("scrollWheel:")]
		public void ScrollWheel(NSEvent theEvent)
		{
			Widget.OnMouseWheel(Conversions.GetMouseEvent(View, theEvent, true));
		}
	}

	public interface IMacViewHandler : IMacAutoSizing
	{
		NSView ContainerControl { get; }

		Size PositionOffset { get; }

		Size? PreferredSize { get; }

		Size MinimumSize { get; set; }

		Control Widget { get; }

		Cursor Cursor { get; set; }

		bool IsEventHandled(string eventName);

		void PostKeyDown(KeyEventArgs e);

		void OnSizeChanged(EventArgs e);
	}

	public abstract class MacView<TControl, TWidget> : MacObject<TControl, TWidget>, IControl, IMacViewHandler, IMacContainerControl
		where TControl: NSResponder
		where TWidget: Control
	{
		bool focus;
		bool mouseMove;
		NSTrackingArea tracking;
		NSTrackingAreaOptions mouseOptions;
		MouseDelegate mouseDelegate;
		Cursor cursor;
		SizeF? naturalSize;

		public abstract NSView ContainerControl { get; }

		public virtual NSView ContentControl { get { return ContainerControl; } }

		public virtual NSView EventControl { get { return ContainerControl; } }

		public virtual bool AutoSize { get; protected set; }

		public virtual Size MinimumSize { get; set; }

		public virtual SizeF MaximumSize { get; set; }

		public Size? PreferredSize { get; set; }

		public virtual Size Size
		{
			get { return ContainerControl.Frame.Size.ToEtoSize(); }
			set
			{ 
				var oldSize = GetPreferredSize(Size.MaxValue);
				PreferredSize = value;

				var newSize = ContainerControl.Frame.Size;
				if (value.Width >= 0)
					newSize.Width = value.Width;
				if (value.Height >= 0)
					newSize.Height = value.Height;
				ContainerControl.SetFrameSize(newSize);

				AutoSize = value.Width == -1 && value.Height == -1;
				CreateTracking();
				LayoutIfNeeded(oldSize);
			}
		}

		protected SizeF? NaturalSize
		{
			get { return naturalSize; }
			set { naturalSize = value; }
		}

		protected virtual bool LayoutIfNeeded(SizeF? oldPreferredSize = null, bool force = false)
		{
			naturalSize = null;
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

		protected MacView()
		{
			AutoSize = true;
			MaximumSize = SizeF.MaxValue;
		}

		protected virtual SizeF GetNaturalSize(SizeF availableSize)
		{
			if (naturalSize != null)
				return naturalSize.Value;
			var control = Control as NSControl;
			if (control != null)
			{
				SD.SizeF? size = (Widget.Loaded) ? (SD.SizeF?)control.Frame.Size : null;
				control.SizeToFit();
				naturalSize = control.Frame.Size.ToEto();
				if (size != null)
					control.SetFrameSize(size.Value);
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
				mouseDelegate = new MouseDelegate { Widget = Widget, View = EventControl };
			var options = mouseOptions | NSTrackingAreaOptions.ActiveAlways | NSTrackingAreaOptions.EnabledDuringMouseDrag | NSTrackingAreaOptions.InVisibleRect;
			tracking = new NSTrackingArea(new SD.RectangleF(SD.PointF.Empty, EventControl.Frame.Size), options, mouseDelegate, new NSDictionary());
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
		static readonly IntPtr selResignFirstResponder = Selector.GetHandle("resignFirstResponder");
		static readonly IntPtr selSetFrameSize = Selector.GetHandle("setFrameSize:");

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
					AddMethod(selSetFrameSize, new Action<IntPtr, IntPtr, SD.SizeF>(SetFrameSizeAction), "v@:{CGSize=ff}", ContainerControl);
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
				default:
					base.AttachEvent(id);
					break;

			}
		}

		static void SetFrameSizeAction(IntPtr sender, IntPtr sel, SD.SizeF size)
		{
			var obj = Runtime.GetNSObject(sender);
			Messaging.void_objc_msgSendSuper_SizeF(obj.SuperHandle, sel, size);

			var handler = GetHandler(obj) as IMacViewHandler;
			if (handler != null)
			{
				handler.OnSizeChanged(EventArgs.Empty);
				handler.Widget.OnSizeChanged(EventArgs.Empty);
			}
		}

		static bool TriggerGotFocus(IntPtr sender, IntPtr sel)
		{
			var obj = Runtime.GetNSObject(sender);
			var handler = GetHandler(obj) as IMacViewHandler;
			if (handler != null)
			{
				handler.Widget.OnGotFocus(EventArgs.Empty);
				return Messaging.bool_objc_msgSendSuper(obj.SuperHandle, sel);
			}
			return false;
		}

		static bool TriggerLostFocus(IntPtr sender, IntPtr sel)
		{
			var obj = Runtime.GetNSObject(sender);
			var handler = GetHandler(obj) as IMacViewHandler;
			if (handler != null)
			{
				handler.Widget.OnLostFocus(EventArgs.Empty);
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
				var theEvent = new NSEvent(e);
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
				var theEvent = new NSEvent(e);
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
				var theEvent = new NSEvent(e);
				var args = Conversions.GetMouseEvent(handler.ContainerControl, theEvent, false);
				if (theEvent.ClickCount >= 2)
					handler.Widget.OnMouseDoubleClick(args);
			
				if (!args.Handled)
				{
					handler.Widget.OnMouseDown(args);
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
				var theEvent = new NSEvent(e);
				var args = Conversions.GetMouseEvent(handler.ContainerControl, theEvent, false);
				handler.Widget.OnMouseUp(args);
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
				var theEvent = new NSEvent(e);
				var args = Conversions.GetMouseEvent(handler.ContainerControl, theEvent, false);
				handler.Widget.OnMouseMove(args);
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
				var theEvent = new NSEvent(e);
				var args = Conversions.GetMouseEvent(handler.ContainerControl, theEvent, true);
				if (!args.Delta.IsZero)
				{
					handler.Widget.OnMouseWheel(args);
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
			var region = rect.ToSDRectangleF();
			region.Y = EventControl.Frame.Height - region.Y - region.Height;
			EventControl.SetNeedsDisplayInRect(region);
		}

		public void SuspendLayout()
		{
		}

		public void ResumeLayout()
		{
		}

		public virtual void Focus()
		{
			if (EventControl.Window != null)
				EventControl.Window.MakeFirstResponder(EventControl);
			else
				focus = true;
		}

		Color? backgroundColor;
		public virtual Color BackgroundColor
		{
			get { return backgroundColor ?? Colors.Transparent; }
			set
			{
				if (Widget.Loaded)
				{
					if (value.A > 0)
					{
						ContainerControl.WantsLayer = true;
						var layer = ContainerControl.Layer;
						if (layer != null)
							layer.BackgroundColor = value.ToCGColor();
					}
					else
					{
						ContainerControl.WantsLayer = false;
						var layer = ContainerControl.Layer;
						if (layer != null)
							layer.BackgroundColor = null;
					}
				}
				backgroundColor = value;
			}
		}

		public abstract bool Enabled { get; set; }

		public virtual bool HasFocus
		{
			get
			{
				return EventControl.Window != null && EventControl.Window.FirstResponder == Control;
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

		public virtual Cursor Cursor
		{
			get { return cursor; }
			set { cursor = value; }
		}

		public string ToolTip
		{
			get { return ContentControl.ToolTip; }
			set { ContentControl.ToolTip = value; }
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
			if (focus && EventControl.Window != null)
				EventControl.Window.MakeFirstResponder(Control);
			if (backgroundColor != null)
				BackgroundColor = backgroundColor.Value;
		}

		public virtual void OnUnLoad(EventArgs e)
		{
		}

		public virtual void PostKeyDown(KeyEventArgs e)
		{
		}

		Control IMacViewHandler.Widget { get { return Widget; } }

		public virtual PointF PointFromScreen(PointF point)
		{
			var sdpoint = point.ToSD();
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
			var sdpoint = point.ToSD();
			sdpoint.Y = ContentControl.Frame.Height - sdpoint.Y;
			sdpoint = ContentControl.ConvertPointToView(sdpoint, null);
			if (ContentControl.Window != null)
			{
				sdpoint = ContentControl.Window.ConvertBaseToScreen(sdpoint);
				sdpoint.Y = ContentControl.Window.Screen.Frame.Height - sdpoint.Y;
			}
			return sdpoint.ToEto();
		}

		Point IControl.Location
		{
			get { return ContentControl.Frame.Location.ToEtoPoint(); }
		}

		static void TriggerSystemAction(IntPtr sender, IntPtr sel, IntPtr e)
		{
			var control = Runtime.GetNSObject(sender);
			var handler = GetHandler(control) as MacView<TControl,TWidget>;
			if (handler != null)
			{
				BaseAction action;
				if (handler.systemActions != null && handler.systemActions.TryGetValue(sel, out action))
				{
					action.Activate();
				}
			}
		}

		static bool ValidateSystemMenuAction(IntPtr sender, IntPtr sel, IntPtr item)
		{
			var menuItem = new NSMenuItem(item);
			
			var control = Runtime.GetNSObject(sender);
			var handler = GetHandler(control) as MacView<TControl,TWidget>;
			if (handler != null)
			{
				BaseAction action;
				if (handler.systemActions != null && menuItem.Action != null && handler.systemActions.TryGetValue(sel, out action))
				{
					if (action != null)
						return action.Enabled;
				}
			}
			return false;
		}

		static bool ValidateSystemToolbarAction(IntPtr sender, IntPtr sel, IntPtr item)
		{
			var toolbarItem = new NSToolbarItem(item);
			
			var control = Runtime.GetNSObject(sender);
			var handler = GetHandler(control) as MacView<TControl,TWidget>;
			if (handler != null)
			{
				BaseAction action;
				if (handler.systemActions != null && toolbarItem.Action != null && handler.systemActions.TryGetValue(sel, out action))
				{
					if (action != null)
						return action.Enabled;
				}
			}
			return false;
		}

		Dictionary<IntPtr, BaseAction> systemActions;
		static readonly IntPtr selValidateMenuItem = Selector.GetHandle("validateMenuItem:");
		static readonly IntPtr selValidateToolbarItem = Selector.GetHandle("validateToolbarItem:");
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

		public virtual void MapPlatformAction(string systemAction, BaseAction action)
		{
			IntPtr sel;
			if (systemActionSelectors.TryGetValue(systemAction, out sel))
			{
				if (systemActions == null)
				{
					systemActions = new Dictionary<IntPtr, BaseAction>();
					AddMethod(selValidateMenuItem, new Func<IntPtr, IntPtr, IntPtr, bool>(ValidateSystemMenuAction), "B@:@");
					AddMethod(selValidateToolbarItem, new Func<IntPtr, IntPtr, IntPtr, bool>(ValidateSystemToolbarAction), "B@:@");
				}
				AddMethod(sel, new Action<IntPtr, IntPtr, IntPtr>(TriggerSystemAction), "v@:@");
				systemActions[sel] = action;
			}
		}
	}
}

