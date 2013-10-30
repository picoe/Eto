using System;
using System.Text.RegularExpressions;
using Eto.Forms;
using Eto.Drawing;
using Eto.Platform.GtkSharp.Drawing;
using GLib;
using System.Text;

namespace Eto.Platform.GtkSharp
{
	public interface IGtkControl
	{
		Point CurrentLocation { get; set; }

		Gtk.Widget ContainerControl { get; }

		Color? SelectedBackgroundColor { get; }

		void SetBackgroundColor();
	}

	public static class GtkControlExtensions
	{
		public static Gtk.Widget GetContainerWidget(this Control control)
		{
			if (control == null)
				return null;
			var containerHandler = control.Handler as IGtkControl;
			if (containerHandler != null)
				return containerHandler.ContainerControl;
			var controlObject = control.ControlObject as Control;
			if (controlObject != null)
				return controlObject.GetContainerWidget();
			return control.ControlObject as Gtk.Widget;
		}

		public static IGtkControl GetGtkControlHandler(this Control control)
		{
			if (control == null)
				return null;
			var containerHandler = control.Handler as IGtkControl;
			if (containerHandler != null)
				return containerHandler;
			var controlObject = control.ControlObject as Control;
			if (controlObject != null)
				return controlObject.GetGtkControlHandler();
			return null;
		}
	}

	public abstract class GtkControl<T, W> : WidgetHandler<T, W>, IControl, IGtkControl
		where T: Gtk.Widget
		where W: Control
	{
		Font font;
		Size size;
		Size asize;
		bool mouseDownHandled;
		Cursor cursor;
		Color? cachedBackgroundColor;
		Color? backgroundColor;
		public static float ScrollAmount = 2f;

		protected GtkControl()
		{
			size = Size.Empty;
		}

		public virtual Size DefaultSize { get { return new Size(-1, -1); } }

		public virtual Gtk.Widget ContainerControl
		{
			get { return Control; }
		}

		public virtual Gtk.Widget EventControl
		{
			get { return Control; }
		}

		public virtual Gtk.Widget ContainerContentControl
		{
			get { return ContainerControl; }
		}

		public static string StringToMnuemonic(string label)
		{
			if (label == null)
				return string.Empty;
			label = label.Replace("_", "__");
			var match = Regex.Match(label, @"(?<=([^&](?:[&]{2})*)|^)[&](?![&])");
			if (match.Success)
			{
				var sb = new StringBuilder(label);
				sb[match.Index] = '_';
				sb.Replace("&&", "&");
				return sb.ToString();
			}
			return label.Replace("&&", "&");
		}

		public static string MnuemonicToString(string label)
		{
			if (label == null)
				return null;
			var match = Regex.Match(label, @"(?<=([^_](?:[_]{2})*)|^)[_](?![_])");
			if (match.Success)
			{
				var sb = new StringBuilder(label);
				sb[match.Index] = '&';
				sb.Replace("__", "_");
				return sb.ToString();
			}
			label = label.Replace("__", "_");
			return label;
		}

		public virtual Point CurrentLocation { get; set; }

		public virtual Size Size
		{
			get
			{
				return ContainerControl.Visible ? ContainerControl.Allocation.Size.ToEto() : size; 
			}
			set
			{
				if (size != value)
				{
					size = value;
					if (size.Width == -1 || size.Height == -1)
					{
						var defSize = DefaultSize;
						if (size.Width == -1)
							size.Width = defSize.Width;
						if (size.Height == -1)
							size.Height = defSize.Height;
					}
					var alloc = Control.Allocation;
					alloc.Size = value.ToGdk();
					ContainerControl.SetSizeRequest(size.Width, size.Height);
				}
			}
		}

		public virtual bool Enabled
		{
			get { return Control.Sensitive; }
			set
			{
				Control.Sensitive = value;
					
			}
		}

		public virtual string Text
		{
			get { return Control.Name; }
			set { Control.Name = value; }
		}

		public void Invalidate()
		{
			Control.QueueDraw();
		}

		public void Invalidate(Rectangle rect)
		{
			Control.QueueDrawArea(rect.X, rect.Y, rect.Width, rect.Height);
		}

		protected virtual bool IsTransparentControl { get { return true; } }

		protected virtual Color DefaultBackgroundColor
		{
			get { return ContainerContentControl.Style.Background(Gtk.StateType.Normal).ToEto(); }
		}

		public virtual Color? SelectedBackgroundColor
		{
			get
			{
				Color? col;
				if (cachedBackgroundColor != null)
					return cachedBackgroundColor.Value;
				if (IsTransparentControl)
				{
					var parent = Widget.Parent.GetGtkControlHandler();
					col = parent != null ? parent.SelectedBackgroundColor : DefaultBackgroundColor;
				}
				else
					col = DefaultBackgroundColor;
				if (backgroundColor != null)
				{
					col = col != null ? Color.Blend(col.Value, backgroundColor.Value) : backgroundColor;
				}
				cachedBackgroundColor = col;
				return col;
			}
		}

		public virtual void SetBackgroundColor()
		{
			cachedBackgroundColor = null;
			SetBackgroundColor(SelectedBackgroundColor);
		}

		protected virtual void SetBackgroundColor(Color? color)
		{
			if (color != null)
				ContainerContentControl.ModifyBg(Gtk.StateType.Normal, color.Value.ToGdk());
			else
				ContainerContentControl.ModifyBg(Gtk.StateType.Normal);
		}


		public virtual Color BackgroundColor
		{
			get
			{
				return backgroundColor ?? SelectedBackgroundColor ?? Colors.Transparent;
			}
			set
			{
				if (backgroundColor != value)
				{
					backgroundColor = value;
					SetBackgroundColor();
				}
			}
		}

		public void SuspendLayout()
		{
		}

		public void ResumeLayout()
		{
		}

		public virtual void Focus()
		{
			Control.GrabFocus();
		}

		public bool HasFocus
		{
			get { return Control.HasFocus; }
		}

		public bool Visible
		{
			get { return Control.Visible; }
			set
			{ 
				Control.Visible = value;
				if (!value)
					Control.NoShowAll = true;
				if (value && Widget.Loaded)
				{
					Control.NoShowAll = false;
					Control.ShowAll();
				}
			}
		}

		public virtual void SetParent(Container parent)
		{
			if (parent == null)
			{
				if (ContainerControl.Parent != null)
					((Gtk.Container)ContainerControl.Parent).Remove(ContainerControl);
			}
		}

		public virtual void OnPreLoad(EventArgs e)
		{
		}

		public virtual void OnLoad(EventArgs e)
		{
		}

		public virtual void OnLoadComplete(EventArgs e)
		{
			if (!Control.IsRealized)
				Control.Realized += HandleControlRealized;
			else
				RealizedSetup();
		}

		public virtual void OnUnLoad(EventArgs e)
		{
		}

		void RealizedSetup()
		{
			if (cursor != null)
				Control.GdkWindow.Cursor = cursor.ControlObject as Gdk.Cursor;
			SetBackgroundColor();
		}

		void HandleControlRealized(object sender, EventArgs e)
		{
			RealizedSetup();
			Control.Realized -= HandleControlRealized;
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case Eto.Forms.Control.KeyDownEvent:
					EventControl.AddEvents((int)Gdk.EventMask.KeyPressMask);
					EventControl.KeyPressEvent += HandleKeyPressEvent;
					break;
				case Eto.Forms.Control.KeyUpEvent:
					EventControl.AddEvents((int)Gdk.EventMask.KeyReleaseMask);
					EventControl.KeyReleaseEvent += HandleKeyReleaseEvent;
					break;
				case Eto.Forms.Control.SizeChangedEvent:
					EventControl.AddEvents((int)Gdk.EventMask.StructureMask);
					EventControl.SizeAllocated += GtkControlObject_SizeAllocated;
					break;
				case Eto.Forms.Control.MouseDoubleClickEvent:
				case Eto.Forms.Control.MouseDownEvent:
					if (!mouseDownHandled)
					{
						EventControl.AddEvents((int)Gdk.EventMask.ButtonPressMask);
						EventControl.AddEvents((int)Gdk.EventMask.ButtonReleaseMask);
						EventControl.ButtonPressEvent += GtkControlObject_ButtonPressEvent;
						mouseDownHandled = true;
					}
					break;
				case Eto.Forms.Control.MouseUpEvent:
					EventControl.AddEvents((int)Gdk.EventMask.ButtonReleaseMask);
					EventControl.ButtonReleaseEvent += GtkControlObject_ButtonReleaseEvent;
					break;
				case Eto.Forms.Control.MouseEnterEvent:
					EventControl.AddEvents((int)Gdk.EventMask.EnterNotifyMask);
					EventControl.EnterNotifyEvent += HandleControlEnterNotifyEvent;
					break;
				case Eto.Forms.Control.MouseLeaveEvent:
					EventControl.AddEvents((int)Gdk.EventMask.LeaveNotifyMask);
					EventControl.LeaveNotifyEvent += HandleControlLeaveNotifyEvent;
					break;
				case Eto.Forms.Control.MouseMoveEvent:
					EventControl.AddEvents((int)Gdk.EventMask.ButtonMotionMask);
					EventControl.AddEvents((int)Gdk.EventMask.PointerMotionMask);
					//GtkControlObject.Events |= Gdk.EventMask.PointerMotionHintMask;
					EventControl.MotionNotifyEvent += GtkControlObject_MotionNotifyEvent;
					break;
				case Eto.Forms.Control.MouseWheelEvent:
					EventControl.AddEvents((int)Gdk.EventMask.ScrollMask);
					EventControl.ScrollEvent += HandleScrollEvent;
					break;
				case Eto.Forms.Control.GotFocusEvent:
					EventControl.AddEvents((int)Gdk.EventMask.FocusChangeMask);
					EventControl.FocusInEvent += delegate
					{
						Widget.OnGotFocus(EventArgs.Empty);
					};
					break;
				case Eto.Forms.Control.LostFocusEvent:
					EventControl.AddEvents((int)Gdk.EventMask.FocusChangeMask);
					EventControl.FocusOutEvent += delegate
					{
						Widget.OnLostFocus(EventArgs.Empty);
					};
					break;
				case Eto.Forms.Control.ShownEvent:
					EventControl.AddEvents((int)Gdk.EventMask.VisibilityNotifyMask);
					EventControl.VisibilityNotifyEvent += (o, args) => {
						if (args.Event.State == Gdk.VisibilityState.FullyObscured)
							Widget.OnShown(EventArgs.Empty);
					};
					break;
				default:
					base.AttachEvent(id);
					return;
			}
		}

		void HandleScrollEvent(object o, Gtk.ScrollEventArgs args)
		{
			var p = new PointF((float)args.Event.X, (float)args.Event.Y);
			Key modifiers = args.Event.State.ToEtoKey();
			MouseButtons buttons = args.Event.State.ToEtoMouseButtons();
			SizeF delta;

			switch (args.Event.Direction)
			{
				case Gdk.ScrollDirection.Down:
					delta = new SizeF(0f, -ScrollAmount);
					break;
				case Gdk.ScrollDirection.Left:
					delta = new SizeF(ScrollAmount, 0f);
					break;
				case Gdk.ScrollDirection.Right:
					delta = new SizeF(-ScrollAmount, 0f);
					break;
				case Gdk.ScrollDirection.Up:
					delta = new SizeF(0f, ScrollAmount);
					break;
				default:
					throw new NotSupportedException();
			}

			Widget.OnMouseWheel(new MouseEventArgs(buttons, modifiers, p, delta));
		}

		void HandleControlLeaveNotifyEvent(object o, Gtk.LeaveNotifyEventArgs args)
		{
			var p = new PointF((float)args.Event.X, (float)args.Event.Y);
			Key modifiers = args.Event.State.ToEtoKey();
			MouseButtons buttons = MouseButtons.None;
			
			Widget.OnMouseLeave(new MouseEventArgs(buttons, modifiers, p));
		}

		void HandleControlEnterNotifyEvent(object o, Gtk.EnterNotifyEventArgs args)
		{
			var p = new PointF((float)args.Event.X, (float)args.Event.Y);
			Key modifiers = args.Event.State.ToEtoKey();
			MouseButtons buttons = MouseButtons.None;
			
			Widget.OnMouseEnter(new MouseEventArgs(buttons, modifiers, p));
		}

		void GtkControlObject_MotionNotifyEvent(System.Object o, Gtk.MotionNotifyEventArgs args)
		{
			var p = new PointF((float)args.Event.X, (float)args.Event.Y);
			Key modifiers = args.Event.State.ToEtoKey();
			MouseButtons buttons = args.Event.State.ToEtoMouseButtons();
			
			Widget.OnMouseMove(new MouseEventArgs(buttons, modifiers, p));
			
			/*int x,y;
			GtkControlObject.GetPointer(out x, out y);
			p = new Point(x, y);*/
		}

		void GtkControlObject_ButtonReleaseEvent(object o, Gtk.ButtonReleaseEventArgs args)
		{
			var p = new PointF((float)args.Event.X, (float)args.Event.Y);
			Key modifiers = args.Event.State.ToEtoKey();
			MouseButtons buttons = args.Event.ToEtoMouseButtons();
			
			Widget.OnMouseUp(new MouseEventArgs(buttons, modifiers, p));
		}

		void GtkControlObject_ButtonPressEvent(object sender, Gtk.ButtonPressEventArgs args)
		{
			var p = new PointF((float)args.Event.X, (float)args.Event.Y);
			Key modifiers = args.Event.State.ToEtoKey();
			MouseButtons buttons = args.Event.ToEtoMouseButtons();
			if (Control.CanFocus && !Control.HasFocus)
				Control.GrabFocus();
			if (args.Event.Type == Gdk.EventType.ButtonPress)
			{
				Widget.OnMouseDown(new MouseEventArgs(buttons, modifiers, p));
			}
			else if (args.Event.Type == Gdk.EventType.TwoButtonPress)
			{
				Widget.OnMouseDoubleClick(new MouseEventArgs(buttons, modifiers, p));
			}
		}

		void GtkControlObject_SizeAllocated(object o, Gtk.SizeAllocatedArgs args)
		{
			if (asize != args.Allocation.Size.ToEto())
			{
				// only call when the size has actually changed, gtk likes to call anyway!!  grr.
				asize = args.Allocation.Size.ToEto();
				Widget.OnSizeChanged(EventArgs.Empty);
			}
		}

		[ConnectBefore]
		void HandleKeyPressEvent(object o, Gtk.KeyPressEventArgs args)
		{
			var e = args.Event.ToEto();
			if (e != null)
			{
				Widget.OnKeyDown(e);
				if (e.Handled)
					args.RetVal = true;
			}
		}

		void HandleKeyReleaseEvent(object o, Gtk.KeyReleaseEventArgs args)
		{
			var e = args.Event.ToEto();
			if (e != null)
			{
				Widget.OnKeyUp(e);
				if (e.Handled)
					args.RetVal = true;
			}
		}

		protected virtual Gtk.Widget FontControl
		{
			get { return Control; }
		}

		public virtual Font Font
		{
			get
			{
				if (font == null)
					font = new Font(Widget.Generator, new FontHandler(FontControl));
				return font;
			}
			set
			{
				font = value;
				if (font == null)
					FontControl.ModifyFont(null);
				else
				{
					var handler = (FontHandler)font.Handler;
					FontControl.ModifyFont(handler.Control);
				}
			}
		}

		public Cursor Cursor
		{
			get { return cursor; }
			set
			{
				cursor = value;
				if (Control.GdkWindow != null)
				{
					if (cursor != null)
						Control.GdkWindow.Cursor = cursor.ControlObject as Gdk.Cursor;
					else
						Control.GdkWindow.Cursor = null;
				}
			}
		}

		public string ToolTip
		{
			get { return Control.TooltipText; }
			set { Control.TooltipText = value; }
		}

		public virtual void MapPlatformAction(string systemAction, BaseAction action)
		{
		}

		public PointF PointFromScreen(PointF point)
		{
			if (Control.GdkWindow != null)
			{
				int x, y;
				Control.GdkWindow.GetOrigin(out x, out y);
				return new PointF(point.X - x, point.Y - y);
			}
			return point;
		}

		public PointF PointToScreen(PointF point)
		{
			if (Control.GdkWindow != null)
			{
				int x, y;
				Control.GdkWindow.GetOrigin(out x, out y);
				return new PointF(point.X + x, point.Y + y);
			}
			return point;
		}

		public Point Location
		{
			get { return Control.Allocation.Location.ToEto(); }
		}
	}
}
