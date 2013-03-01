using System;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Collections;
using Eto.Forms;
using Eto.Drawing;
using Eto.Platform.GtkSharp.Drawing;
using System.Collections.Generic;
using GLib;
using System.Text;

namespace Eto.Platform.GtkSharp
{
	public interface IGtkControl
	{
		Point Location { get; set; }

		Gtk.Widget ContainerControl { get; }
	}

	public static class GtkControlExtensions
	{
		public static Gtk.Widget GetContainerWidget (this Control control)
		{
			if (control == null)
				return null;
			var containerHandler = control.Handler as IGtkControl;
			if (containerHandler != null)
				return containerHandler.ContainerControl;
			return control.ControlObject as Gtk.Widget;
		}
	}

	public abstract class GtkControl<T, W> : WidgetHandler<T, W>, IControl, IGtkControl
		where T: Gtk.Widget
		where W: Control
	{
		Font font;
		Size size;
		Size asize;
		Point location;
		bool mouseDownHandled;
		Cursor cursor;
		Color? originalBackgroundColor;
		Color? backgroundColor;

		public static float SCROLL_AMOUNT = 2f;

		public GtkControl ()
		{
			size = Size.Empty;
		}

		public virtual Gtk.Widget ContainerControl
		{
			get { return Control; }
		}

		public virtual Gtk.Widget EventControl
		{
			get { return Control; }
		}

		public static string StringToMnuemonic (string label)
		{
			if (label == null)
				return string.Empty;
			label = label.Replace ("_", "__");
			var match = Regex.Match (label, @"(?<=([^&](?:[&]{2})*)|^)[&](?![&])");
			if (match.Success) {
				var sb = new StringBuilder (label);
				sb[match.Index] = '_';
				sb.Replace ("&&", "&");
				return sb.ToString ();
			}
			return label.Replace ("&&", "&");
		}

		public static string MnuemonicToString (string label)
		{
			if (label == null)
				return null;
			var match = Regex.Match (label, @"(?<=([^_](?:[_]{2})*)|^)[_](?![_])");
			if (match.Success) {
				var sb = new StringBuilder (label);
				sb[match.Index] = '&';
				sb.Replace ("__", "_");
				return sb.ToString ();
			}
			label = label.Replace ("__", "_");
			return label;
		}

		public virtual Point Location {
			get { return location; }
			set { location = value; }
		}

		public virtual Size Size {
			get {
				if (ContainerControl.Visible)
					return ContainerControl.Allocation.Size.ToEto ();
				else
					return size; 
			}
			set {
				if (size != value) {
					size = value;
					var alloc = Control.Allocation;
					alloc.Size = value.ToGdk ();
					ContainerControl.SetSizeRequest (size.Width, size.Height);
				}
			}
		}

		public virtual bool Enabled {
			get { return Control.Sensitive; }
			set {
				Control.Sensitive = value;
					
			}
		}

		public virtual string Text {
			get { return Control.Name; }
			set { Control.Name = value; }
		}

		public void Invalidate ()
		{
			Control.QueueDraw ();
		}

		public void Invalidate (Rectangle rect)
		{
			Control.QueueDrawArea (rect.X, rect.Y, rect.Width, rect.Height);
		}

		Color GetOriginalBackgroundColor ()
		{
			if (originalBackgroundColor == null)
				originalBackgroundColor = ContainerControl.Style.Background (Gtk.StateType.Normal).ToEto ();
			return originalBackgroundColor.Value;
		}

		public virtual Color BackgroundColor {
			get {
				return backgroundColor ?? GetOriginalBackgroundColor ();
			}
			set {
				if (backgroundColor != value) {
					backgroundColor = value;
					var col = Color.Blend (GetOriginalBackgroundColor (), value);
					var eb = ContainerControl as Gtk.EventBox;
					if (eb != null && value.A > 0) eb.VisibleWindow = true;
					ContainerControl.ModifyBg (Gtk.StateType.Normal, col.ToGdk ());
				}
			}
		}

		public void SuspendLayout ()
		{
		}

		public void ResumeLayout ()
		{
		}

		public virtual void Focus ()
		{
			Control.GrabFocus ();
		}

		public bool HasFocus {
			get { return Control.HasFocus; }
		}

		public bool Visible {
			get { return Control.Visible; }
			set { 
				Control.Visible = value;
				if (!value)
					Control.NoShowAll = true;
				if (value && Widget.Loaded) {
					Control.NoShowAll = false;
					Control.ShowAll ();
				}
			}
		}

		public virtual void SetLayout (Layout parent)
		{
		}

		public virtual void SetParentLayout (Layout parent)
		{
		}

		public virtual void SetParent (Control parent)
		{
		}

		public virtual void OnPreLoad (EventArgs e)
		{
		}
		
		public virtual void OnLoad (EventArgs e)
		{
		}

		public virtual void OnLoadComplete (EventArgs e)
		{
			if (cursor != null) {
				Control.Realized += HandleControlRealized;
			}
		}

		public virtual void OnUnLoad (EventArgs e)
		{
		}

		void HandleControlRealized (object sender, EventArgs e)
		{
			if (cursor != null)
				Control.GdkWindow.Cursor = cursor.ControlObject as Gdk.Cursor;
			Control.Realized -= HandleControlRealized;
		}
		
		public override void AttachEvent (string handler)
		{
			switch (handler) {
			case Eto.Forms.Control.KeyDownEvent:
				EventControl.AddEvents ((int)Gdk.EventMask.KeyPressMask);
				EventControl.KeyPressEvent += HandleKeyPressEvent;
				break;
			case Eto.Forms.Control.KeyUpEvent:
				EventControl.AddEvents ((int)Gdk.EventMask.KeyReleaseMask);
				EventControl.KeyReleaseEvent += HandleKeyReleaseEvent;
				break;
			case Eto.Forms.Control.SizeChangedEvent:
				EventControl.AddEvents ((int)Gdk.EventMask.StructureMask);
				EventControl.SizeAllocated += GtkControlObject_SizeAllocated;
				break;
			case Eto.Forms.Control.MouseDoubleClickEvent:
			case Eto.Forms.Control.MouseDownEvent:
				if (!mouseDownHandled) {
					EventControl.AddEvents ((int)Gdk.EventMask.ButtonPressMask);
					EventControl.AddEvents ((int)Gdk.EventMask.ButtonReleaseMask);
					EventControl.ButtonPressEvent += GtkControlObject_ButtonPressEvent;
					mouseDownHandled = true;
				}
				break;
			case Eto.Forms.Control.MouseUpEvent:
				EventControl.AddEvents ((int)Gdk.EventMask.ButtonReleaseMask);
				EventControl.ButtonReleaseEvent += GtkControlObject_ButtonReleaseEvent;
				break;
			case Eto.Forms.Control.MouseEnterEvent:
				EventControl.AddEvents ((int)Gdk.EventMask.EnterNotifyMask);
				EventControl.EnterNotifyEvent += HandleControlEnterNotifyEvent;
				break;
			case Eto.Forms.Control.MouseLeaveEvent:
				EventControl.AddEvents ((int)Gdk.EventMask.LeaveNotifyMask);
				EventControl.LeaveNotifyEvent += HandleControlLeaveNotifyEvent;
				break;
			case Eto.Forms.Control.MouseMoveEvent:
				EventControl.AddEvents ((int)Gdk.EventMask.ButtonMotionMask);
				EventControl.AddEvents ((int)Gdk.EventMask.PointerMotionMask);
					//GtkControlObject.Events |= Gdk.EventMask.PointerMotionHintMask;
				EventControl.MotionNotifyEvent += GtkControlObject_MotionNotifyEvent;
				break;
			case Eto.Forms.Control.MouseWheelEvent:
				EventControl.AddEvents ((int)Gdk.EventMask.ScrollMask);
				EventControl.ScrollEvent += HandleScrollEvent;
				break;
			case Eto.Forms.Control.GotFocusEvent:
				EventControl.AddEvents ((int)Gdk.EventMask.FocusChangeMask);
				EventControl.FocusInEvent += delegate {
					Widget.OnGotFocus (EventArgs.Empty);
				};
				break;
			case Eto.Forms.Control.LostFocusEvent:
				EventControl.AddEvents ((int)Gdk.EventMask.FocusChangeMask);
				EventControl.FocusOutEvent += delegate {
					Widget.OnLostFocus (EventArgs.Empty);
				};
				break;
			case Eto.Forms.Control.ShownEvent:
				EventControl.AddEvents ((int)Gdk.EventMask.VisibilityNotifyMask);
				EventControl.VisibilityNotifyEvent += (o, args) => {
					if (args.Event.State == Gdk.VisibilityState.FullyObscured)
						Widget.OnShown (EventArgs.Empty);
				};
				break;
			default:
				base.AttachEvent (handler);
				return;
			}
		}

		void HandleScrollEvent (object o, Gtk.ScrollEventArgs args)
		{
			Point p = new Point (Convert.ToInt32 (args.Event.X), Convert.ToInt32 (args.Event.Y));
			Key modifiers = GetKeyModifiers (args.Event.State);
			MouseButtons buttons = GetButtonModifiers (args.Event.State);
			SizeF delta;

			switch (args.Event.Direction) {
			case Gdk.ScrollDirection.Down:
				delta = new SizeF (0f, -SCROLL_AMOUNT);
				break;
			case Gdk.ScrollDirection.Left:
				delta = new SizeF (SCROLL_AMOUNT, 0f);
				break;
			case Gdk.ScrollDirection.Right:
				delta = new SizeF (-SCROLL_AMOUNT, 0f);
				break;
			case Gdk.ScrollDirection.Up:
				delta = new SizeF (0f, SCROLL_AMOUNT);
				break;
			default:
				throw new NotSupportedException ();
			}

			Widget.OnMouseWheel (new MouseEventArgs (buttons, modifiers, p, delta));
		}

		void HandleControlLeaveNotifyEvent (object o, Gtk.LeaveNotifyEventArgs args)
		{
			Point p = new Point (Convert.ToInt32 (args.Event.X), Convert.ToInt32 (args.Event.Y));
			Key modifiers = GetKeyModifiers (args.Event.State);
			MouseButtons buttons = MouseButtons.None;
			
			Widget.OnMouseLeave (new MouseEventArgs (buttons, modifiers, p));
		}

		void HandleControlEnterNotifyEvent (object o, Gtk.EnterNotifyEventArgs args)
		{
			Point p = new Point (Convert.ToInt32 (args.Event.X), Convert.ToInt32 (args.Event.Y));
			Key modifiers = GetKeyModifiers (args.Event.State);
			MouseButtons buttons = MouseButtons.None;
			
			Widget.OnMouseEnter (new MouseEventArgs (buttons, modifiers, p));
		}

		private Key GetKeyModifiers (Gdk.ModifierType state)
		{
			Key modifiers = Key.None;
			if ((state & Gdk.ModifierType.ShiftMask) != 0)
				modifiers |= Key.Shift;
			if ((state & Gdk.ModifierType.ControlMask) != 0)
				modifiers |= Key.Control;
			if ((state & Gdk.ModifierType.Mod1Mask) != 0)
				modifiers |= Key.Alt;
			return modifiers;
		}
		
		MouseButtons GetButtons (Gdk.EventButton ev)
		{
			switch (ev.Button) {
			case 1:
				return MouseButtons.Primary;
			case 2:
				return MouseButtons.Middle;
			case 3:
				return MouseButtons.Alternate;
			default:
				return MouseButtons.None;
			}
		}

		private MouseButtons GetButtonModifiers (Gdk.ModifierType state)
		{
			MouseButtons buttons = MouseButtons.None;
			if ((state & Gdk.ModifierType.Button1Mask) != 0)
				buttons |= MouseButtons.Primary;
			if ((state & Gdk.ModifierType.Button2Mask) != 0)
				buttons |= MouseButtons.Alternate;
			if ((state & Gdk.ModifierType.Button3Mask) != 0)
				buttons |= MouseButtons.Middle;
			return buttons;
		}

		private void GtkControlObject_MotionNotifyEvent (System.Object o, Gtk.MotionNotifyEventArgs args)
		{
			Point p = new Point (Convert.ToInt32 (args.Event.X), Convert.ToInt32 (args.Event.Y));
			Key modifiers = GetKeyModifiers (args.Event.State);
			MouseButtons buttons = GetButtonModifiers (args.Event.State);
			
			Widget.OnMouseMove (new MouseEventArgs (buttons, modifiers, p));
			
			/*int x,y;
			GtkControlObject.GetPointer(out x, out y);
			p = new Point(x, y);*/
		}

		private void GtkControlObject_ButtonReleaseEvent (object o, Gtk.ButtonReleaseEventArgs args)
		{
			Point p = new Point (Convert.ToInt32 (args.Event.X), Convert.ToInt32 (args.Event.Y));
			Key modifiers = GetKeyModifiers (args.Event.State);
			MouseButtons buttons = GetButtons (args.Event);
			
			Widget.OnMouseUp (new MouseEventArgs (buttons, modifiers, p));
		}

		private void GtkControlObject_ButtonPressEvent (object sender, Gtk.ButtonPressEventArgs args)
		{
			Point p = new Point (Convert.ToInt32 (args.Event.X), Convert.ToInt32 (args.Event.Y));
			Key modifiers = GetKeyModifiers (args.Event.State);
			MouseButtons buttons = GetButtons (args.Event);
			if (Control.CanFocus && !Control.HasFocus)
				Control.GrabFocus ();
			if (args.Event.Type == Gdk.EventType.ButtonPress) {
				Widget.OnMouseDown (new MouseEventArgs (buttons, modifiers, p));
			} else if (args.Event.Type == Gdk.EventType.TwoButtonPress) {
				Widget.OnMouseDoubleClick (new MouseEventArgs (buttons, modifiers, p));
			}
		}
		
		private void GtkControlObject_SizeAllocated (object o, Gtk.SizeAllocatedArgs args)
		{
			if (asize != args.Allocation.Size.ToEto ()) {
				// only call when the size has actually changed, gtk likes to call anyway!!  grr.
				this.asize = args.Allocation.Size.ToEto ();
				Widget.OnSizeChanged (EventArgs.Empty);
			}
		}
		
		[ConnectBefore]
		void HandleKeyPressEvent (object o, Gtk.KeyPressEventArgs args)
		{
			var e = args.Event.ToEto ();
			if (e != null) {
				Widget.OnKeyDown (e);
				if (e.Handled)
					args.RetVal = true;
			}
		}

		void HandleKeyReleaseEvent (object o, Gtk.KeyReleaseEventArgs args)
		{
			var e = args.Event.ToEto ();
			if (e != null) {
				Widget.OnKeyUp (e);
				if (e.Handled)
					args.RetVal = true;
			}
		}
		

		protected virtual Gtk.Widget FontControl
		{
			get { return Control; }
		}
		
		public virtual Font Font {
			get {
				if (font == null)
					font = new Font (Widget.Generator, new FontHandler (FontControl));
				return font;
			}
			set {
				font = value;
				if (font != null)
					FontControl.ModifyFont (font.ControlObject as Pango.FontDescription);
				else
					FontControl.ModifyFont (null);
			}
		}
				
		public Cursor Cursor {
			get { return cursor; }
			set {
				cursor = value;
				if (Control.GdkWindow != null) {
					if (cursor != null)
						Control.GdkWindow.Cursor = cursor.ControlObject as Gdk.Cursor;
					else
						Control.GdkWindow.Cursor = null;
				}
			}
		}
		
		public string ToolTip {
			get { return Control.TooltipText; }
			set { Control.TooltipText = value; }
		}
		
		public virtual void MapPlatformAction (string systemAction, BaseAction action)
		{
		}
	}
}
