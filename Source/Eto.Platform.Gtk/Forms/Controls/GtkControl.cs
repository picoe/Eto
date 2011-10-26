using System;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Collections;
using Eto.Forms;
using Eto.Drawing;
using Eto.Platform.GtkSharp.Drawing;
using System.Collections.Generic;
using GLib;

namespace Eto.Platform.GtkSharp
{
	public interface IGtkControl
	{
		Point Location { get; set; }
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

		public GtkControl ()
		{
			size = Size.Empty;
		}

		public static string StringToMnuemonic (string label)
		{
			//label = label.Replace("_", "__");
			label = label.Replace ("&", "_");
			return label;
		}

		public static string MnuemonicToString (string label)
		{
			label = label.Replace ("_", "&");
			//label = label.Replace("__", "_");
			return label;
		}

		public virtual Point Location {
			get { return location; }
			set { location = value; }
		}


		public virtual Size Size {
			get { 
				if (Control.Visible) 
					return Generator.Convert(Control.Allocation.Size);
				else
					return size; 
			}
			set {
				if (size != value) {
					size = value;
					var alloc = Control.Allocation;
					alloc.Size = Generator.Convert (value);
					//Control.Allocation = alloc;
					Control.SetSizeRequest (size.Width, size.Height);
				}
			}
		}

		public virtual bool Enabled {
			get { return Control.Sensitive; }
			set {
				Control.Sensitive = value;
					
			}
		}

		public string Id { get; set; }

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

		public virtual Color BackgroundColor {
			get { return Generator.Convert (Control.Style.Background (Gtk.StateType.Normal)); }
			set { Control.ModifyBg (Gtk.StateType.Normal, Generator.Convert (value)); }
		}

		public Graphics CreateGraphics ()
		{
			return new Graphics (Widget.Generator, new GraphicsHandler (Control, Control.GdkWindow, Control.Style.BlackGC));
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
		}
		
		public override void AttachEvent (string handler)
		{
			switch (handler) {
			case Eto.Forms.Control.KeyDownEvent:
				Control.AddEvents((int)Gdk.EventMask.KeyPressMask);
				Control.KeyPressEvent += GtkControlObject_KeyPressEvent;
				break;
			case Eto.Forms.Control.SizeChangedEvent:
				Control.AddEvents((int)Gdk.EventMask.StructureMask);
				Control.SizeAllocated += GtkControlObject_SizeAllocated;
				break;
			case Eto.Forms.Control.MouseDoubleClickEvent:
			case Eto.Forms.Control.MouseDownEvent:
				if (!mouseDownHandled) {
					Control.AddEvents((int)Gdk.EventMask.ButtonPressMask);
					Control.AddEvents((int)Gdk.EventMask.ButtonReleaseMask);
					Control.ButtonPressEvent += GtkControlObject_ButtonPressEvent;
					mouseDownHandled = true;
				}
				break;
			case Eto.Forms.Control.MouseUpEvent:
				Control.AddEvents((int)Gdk.EventMask.ButtonReleaseMask);
				Control.ButtonReleaseEvent += GtkControlObject_ButtonReleaseEvent;
				break;
			case Eto.Forms.Control.MouseEnterEvent:
				Control.AddEvents((int)Gdk.EventMask.EnterNotifyMask);
				Control.EnterNotifyEvent += HandleControlEnterNotifyEvent;
				break;
			case Eto.Forms.Control.MouseLeaveEvent:
				Control.AddEvents((int)Gdk.EventMask.LeaveNotifyMask);
				Control.LeaveNotifyEvent += HandleControlLeaveNotifyEvent;
				break;
			case Eto.Forms.Control.MouseMoveEvent:
				Control.AddEvents((int)Gdk.EventMask.ButtonMotionMask);
				Control.AddEvents((int)Gdk.EventMask.PointerMotionMask);
					//GtkControlObject.Events |= Gdk.EventMask.PointerMotionHintMask;
				Control.MotionNotifyEvent += GtkControlObject_MotionNotifyEvent;
				break;
			case Eto.Forms.Control.GotFocusEvent:
				Control.AddEvents((int)Gdk.EventMask.FocusChangeMask);
				Control.FocusInEvent += delegate { Widget.OnGotFocus(EventArgs.Empty); };
				break;
			case Eto.Forms.Control.LostFocusEvent:
				Control.AddEvents((int)Gdk.EventMask.FocusChangeMask);
				Control.FocusOutEvent += delegate { Widget.OnLostFocus(EventArgs.Empty); };
				break;
			default:
				base.AttachEvent(handler);
				return;
			}
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
			switch (ev.Button)
			{
			case 1: return MouseButtons.Primary;
			case 2: return MouseButtons.Middle;
			case 3: return MouseButtons.Alternate;
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
			}
			else if (args.Event.Type == Gdk.EventType.TwoButtonPress) {
				Widget.OnMouseDoubleClick (new MouseEventArgs (buttons, modifiers, p));
			}
		}
		
		
		private void GtkControlObject_SizeAllocated (object o, Gtk.SizeAllocatedArgs args)
		{
			if (asize != Generator.Convert (args.Allocation.Size)) {
				// only call when the size has actually changed, gtk likes to call anyway!!  grr.
				this.asize = Generator.Convert (args.Allocation.Size);
				Widget.OnSizeChanged (EventArgs.Empty);
			}
		}
		
		[ConnectBefore]
		void GtkControlObject_KeyPressEvent (object o, Gtk.KeyPressEventArgs args)
		{
			Key key = Key.None;
			key |= KeyMap.Convert (args.Event.Key);
			Gdk.ModifierType state = args.Event.State;
			key |= KeyMap.Convert (state);
			//args.Event. = false; //.RetVal = false;

			//Console.WriteLine("{0} | {1} | {2} | {3}", args.Event.Key.ToString(), args.Event.State.ToString(), this.Widget.ToString(), key.ToString());
			if (key != Key.None) {
				KeyPressEventArgs kpea;
				Key modifiers = (key & Key.ModifierMask);
				if (args.Event.KeyValue <= 128 && ((modifiers & ~Key.Shift) == 0))
					kpea = new KeyPressEventArgs (key, (char)args.Event.KeyValue);
				else
					kpea = new KeyPressEventArgs (key);
				Widget.OnKeyDown (kpea);
				if (kpea.Handled)
					args.RetVal = true;
			} else if (args.Event.KeyValue <= 128) {
				KeyPressEventArgs kpea;
				kpea = new KeyPressEventArgs (key, (char)args.Event.KeyValue);
				Widget.OnKeyDown (kpea);
				if (kpea.Handled)
					args.RetVal = true;
			}
		}
		
		public Font Font {
			get {
				return font;
			}
			set {
				font = value;
				if (font != null)
					Control.ModifyFont (font.ControlObject as Pango.FontDescription);
				else
					Control.ModifyFont (null);
			}
		}
		
	}
}
