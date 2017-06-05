using System;
using System.Text.RegularExpressions;
using Eto.Forms;
using Eto.Drawing;
using Eto.GtkSharp.Drawing;
using GLib;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace Eto.GtkSharp.Forms
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
			return controlObject != null ? controlObject.GetGtkControlHandler() : null;
		}
	}

	public abstract class GtkControl<TControl, TWidget, TCallback> : WidgetHandler<TControl, TWidget, TCallback>, Control.IHandler, IGtkControl
		where TControl: Gtk.Widget
		where TWidget: Control
		where TCallback: Control.ICallback
	{
		Size size;
		Size asize;
		bool mouseDownHandled;
#if GTK2
		Color? cachedBackgroundColor;
#endif
		Color? backgroundColor;
		public static float ScrollAmount = 2f;

		public override IntPtr NativeHandle { get { return Control.Handle; } }

		protected GtkControl()
		{
			size = new Size(-1, -1);
		}

		public Size PreferredSize { get { return size; } }

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

		public virtual Gtk.Widget BackgroundControl
		{
			get { return ContainerContentControl; }
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

		public void Invalidate(bool invalidateChildren)
		{
			Control.QueueDraw();
		}

		public void Invalidate(Rectangle rect, bool invalidateChildren)
		{
			Control.QueueDrawArea(rect.X, rect.Y, rect.Width, rect.Height);
		}

		protected virtual bool IsTransparentControl { get { return true; } }

		protected virtual Color DefaultBackgroundColor
		{
			get { return ContainerContentControl.GetBackground(); }
		}

		public virtual Color? SelectedBackgroundColor
		{
			get
			{
				#if GTK3
				return backgroundColor;
				#else
				Color? col;
				if (cachedBackgroundColor != null)
					return cachedBackgroundColor.Value;
				if (IsTransparentControl)
				{
					var parent = Widget.VisualParent.GetGtkControlHandler();
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
				#endif
			}
		}

		public virtual void SetBackgroundColor()
		{
#if GTK2
			cachedBackgroundColor = null;
#endif
			SetBackgroundColor(SelectedBackgroundColor);
		}

		protected virtual void SetBackgroundColor(Color? color)
		{
			if (color != null)
			{
				BackgroundControl.SetBackground(color.Value);
			}
			else
			{
				BackgroundControl.ClearBackground();
			}
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

		public void Focus()
		{
			if (Widget.Loaded)
				GrabFocus();
			else
				Widget.LoadComplete += Widget_SetFocus;
		}

		protected virtual void GrabFocus()
		{
			Control.GrabFocus();
		}

		void Widget_SetFocus(object sender, EventArgs e)
		{
			Widget.LoadComplete -= Widget_SetFocus;
			Eto.Forms.Application.Instance.AsyncInvoke(GrabFocus);
		}

		public bool HasFocus
		{
			get { return Control.HasFocus; }
		}

		public virtual bool Visible
		{
			get { return Control.Visible; }
			set
			{ 
				Control.Visible = value;
				Control.NoShowAll = !value;
				if (value && Widget.Loaded)
				{
					Control.ShowAll();
				}
			}
		}

		public virtual void SetParent(Container parent)
		{
			/*if (parent == null)
			{
				if (ContainerControl.Parent != null)
					((Gtk.Container)ContainerControl.Parent).Remove(ContainerControl);
			}*/
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
				Control.Realized += Connector.HandleControlRealized;
			else
				RealizedSetup();
		}

		public virtual void OnUnLoad(EventArgs e)
		{
		}

		void RealizedSetup()
		{
			if (Cursor != null)
				Control.GetWindow().Cursor = Cursor.ControlObject as Gdk.Cursor;
			SetBackgroundColor();
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case Eto.Forms.Control.KeyDownEvent:
					EventControl.AddEvents((int)Gdk.EventMask.KeyPressMask);
					EventControl.KeyPressEvent += Connector.HandleKeyPressEvent;
					break;
				case Eto.Forms.Control.TextInputEvent:
					HandleEvent(Eto.Forms.Control.KeyDownEvent);
					break;
				case Eto.Forms.Control.KeyUpEvent:
					EventControl.AddEvents((int)Gdk.EventMask.KeyReleaseMask);
					EventControl.KeyReleaseEvent += Connector.HandleKeyReleaseEvent;
					break;
				case Eto.Forms.Control.SizeChangedEvent:
					EventControl.AddEvents((int)Gdk.EventMask.StructureMask);
					EventControl.SizeAllocated += Connector.HandleSizeAllocated;
					break;
				case Eto.Forms.Control.MouseDoubleClickEvent:
				case Eto.Forms.Control.MouseDownEvent:
					if (!mouseDownHandled)
					{
						EventControl.AddEvents((int)Gdk.EventMask.ButtonPressMask);
						EventControl.AddEvents((int)Gdk.EventMask.ButtonReleaseMask);
						EventControl.ButtonPressEvent += Connector.HandleButtonPressEvent;
						mouseDownHandled = true;
					}
					break;
				case Eto.Forms.Control.MouseUpEvent:
					EventControl.AddEvents((int)Gdk.EventMask.ButtonReleaseMask);
					EventControl.ButtonReleaseEvent += Connector.HandleButtonReleaseEvent;
					break;
				case Eto.Forms.Control.MouseEnterEvent:
					EventControl.AddEvents((int)Gdk.EventMask.EnterNotifyMask);
					EventControl.EnterNotifyEvent += Connector.HandleControlEnterNotifyEvent;
					break;
				case Eto.Forms.Control.MouseLeaveEvent:
					EventControl.AddEvents((int)Gdk.EventMask.LeaveNotifyMask);
					EventControl.LeaveNotifyEvent += Connector.HandleControlLeaveNotifyEvent;
					break;
				case Eto.Forms.Control.MouseMoveEvent:
					EventControl.AddEvents((int)Gdk.EventMask.ButtonMotionMask);
					EventControl.AddEvents((int)Gdk.EventMask.PointerMotionMask);
					//GtkControlObject.Events |= Gdk.EventMask.PointerMotionHintMask;
					EventControl.MotionNotifyEvent += Connector.HandleMotionNotifyEvent;
					break;
				case Eto.Forms.Control.MouseWheelEvent:
					EventControl.AddEvents((int)Gdk.EventMask.ScrollMask);
					EventControl.ScrollEvent += Connector.HandleScrollEvent;
					break;
				case Eto.Forms.Control.GotFocusEvent:
					EventControl.AddEvents((int)Gdk.EventMask.FocusChangeMask);
					EventControl.FocusInEvent += Connector.FocusInEvent;
					break;
				case Eto.Forms.Control.LostFocusEvent:
					EventControl.AddEvents((int)Gdk.EventMask.FocusChangeMask);
					EventControl.FocusOutEvent += Connector.FocusOutEvent;
					break;
				case Eto.Forms.Control.ShownEvent:
					EventControl.AddEvents((int)Gdk.EventMask.VisibilityNotifyMask);
					EventControl.VisibilityNotifyEvent += Connector.VisibilityNotifyEvent;
					break;
				default:
					base.AttachEvent(id);
					return;
			}
		}

		protected new GtkControlConnector Connector { get { return (GtkControlConnector)base.Connector; } }

		protected override WeakConnector CreateConnector()
		{
			return new GtkControlConnector();
		}

		/// <summary>
		/// Connector for events to keep a weak reference to allow gtk controls to be garbage collected when no longer referenced
		/// </summary>
		protected class GtkControlConnector : WeakConnector
		{
			new GtkControl<TControl, TWidget, TCallback> Handler { get { return (GtkControl<TControl, TWidget, TCallback>)base.Handler; } }

			public void HandleScrollEvent(object o, Gtk.ScrollEventArgs args)
			{
				var p = new PointF((float)args.Event.X, (float)args.Event.Y);
				Keys modifiers = args.Event.State.ToEtoKey();
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

				Handler.Callback.OnMouseWheel(Handler.Widget, new MouseEventArgs(buttons, modifiers, p, delta));
			}

			[GLib.ConnectBefore]
			public void HandleControlLeaveNotifyEvent(object o, Gtk.LeaveNotifyEventArgs args)
			{
				var p = new PointF((float)args.Event.X, (float)args.Event.Y);
				Keys modifiers = args.Event.State.ToEtoKey();
				MouseButtons buttons = MouseButtons.None;

				Handler.Callback.OnMouseLeave(Handler.Widget, new MouseEventArgs(buttons, modifiers, p));
			}

			[GLib.ConnectBefore]
			public void HandleControlEnterNotifyEvent(object o, Gtk.EnterNotifyEventArgs args)
			{
				var p = new PointF((float)args.Event.X, (float)args.Event.Y);
				Keys modifiers = args.Event.State.ToEtoKey();
				MouseButtons buttons = MouseButtons.None;

				Handler.Callback.OnMouseEnter(Handler.Widget, new MouseEventArgs(buttons, modifiers, p));
			}

			[GLib.ConnectBefore]
			public void HandleMotionNotifyEvent(System.Object o, Gtk.MotionNotifyEventArgs args)
			{
				var p = new PointF((float)args.Event.X, (float)args.Event.Y);
				Keys modifiers = args.Event.State.ToEtoKey();
				MouseButtons buttons = args.Event.State.ToEtoMouseButtons();

				Handler.Callback.OnMouseMove(Handler.Widget, new MouseEventArgs(buttons, modifiers, p));
			}

			[GLib.ConnectBefore]
			public void HandleButtonReleaseEvent(object o, Gtk.ButtonReleaseEventArgs args)
			{
				args.Event.ToEtoLocation();
				var p = new PointF((float)args.Event.X, (float)args.Event.Y);
				Keys modifiers = args.Event.State.ToEtoKey();
				MouseButtons buttons = args.Event.ToEtoMouseButtons();

				var mouseArgs = new MouseEventArgs(buttons, modifiers, p);
				Handler.Callback.OnMouseUp(Handler.Widget, mouseArgs);
				args.RetVal = mouseArgs.Handled;
			}

			[GLib.ConnectBefore]
			public void HandleButtonPressEvent(object sender, Gtk.ButtonPressEventArgs args)
			{
				var p = new PointF((float)args.Event.X, (float)args.Event.Y);
				Keys modifiers = args.Event.State.ToEtoKey();
				MouseButtons buttons = args.Event.ToEtoMouseButtons();
				var mouseArgs = new MouseEventArgs(buttons, modifiers, p);
				if (args.Event.Type == Gdk.EventType.ButtonPress)
				{
					Handler.Callback.OnMouseDown(Handler.Widget, mouseArgs);
				}
				else if (args.Event.Type == Gdk.EventType.TwoButtonPress)
				{
					Handler.Callback.OnMouseDoubleClick(Handler.Widget, mouseArgs);
				}
				if (!mouseArgs.Handled && Handler.EventControl.CanFocus && !Handler.EventControl.HasFocus)
					Handler.EventControl.GrabFocus();
				if (args.RetVal != null && (bool)args.RetVal == true)
					return;
				args.RetVal = mouseArgs.Handled;
			}

			public void HandleSizeAllocated(object o, Gtk.SizeAllocatedArgs args)
			{
				if (Handler.asize != args.Allocation.Size.ToEto())
				{
					// only call when the size has actually changed, gtk likes to call anyway!!  grr.
					Handler.asize = args.Allocation.Size.ToEto();
					Handler.Callback.OnSizeChanged(Handler.Widget, EventArgs.Empty);
				}
			}

			Gtk.IMContext context;
			bool commitHandled;

			Gtk.IMContext Context
			{
				get
				{
					if (context != null)
						return context; 
					context = new Gtk.IMContextSimple();

					context.Commit += (o, args) => 
					{
						var handler = Handler;
						if (handler == null || string.IsNullOrEmpty(args.Str))
							return;

						var tia = new TextInputEventArgs(args.Str);
						handler.Callback.OnTextInput(handler.Widget, tia);
						commitHandled = tia.Cancel;
						context.Reset();
					};
					return context;
				}
			}

			[ConnectBefore]
			public void HandleKeyPressEvent(object o, Gtk.KeyPressEventArgs args)
			{
				var handler = Handler;
				if (handler == null)
					return;

				var e = args.Event.ToEto();
				if (e != null)
				{
					handler.Callback.OnKeyDown(Handler.Widget, e);
					args.RetVal = e.Handled;
				}

				if (e == null || !e.Handled)
				{
					commitHandled = false;
					if (Context.FilterKeypress(args.Event))
					{
						args.RetVal = commitHandled;
					}
				}
			}

			public void HandleKeyReleaseEvent(object o, Gtk.KeyReleaseEventArgs args)
			{
				var handler = Handler;
				if (handler == null)
					return;
				var e = args.Event.ToEto();
				if (e != null)
				{
					handler.Callback.OnKeyUp(handler.Widget, e);
					args.RetVal = e.Handled;
				}
			}

			public virtual void FocusInEvent(object o, Gtk.FocusInEventArgs args)
			{
				var handler = Handler;
				if (handler == null)
					return;
				handler.Callback.OnGotFocus(handler.Widget, EventArgs.Empty);
			}

			public virtual void FocusOutEvent(object o, Gtk.FocusOutEventArgs args)
			{
				// Handler can be null here after window is closed
				var handler = Handler;
				if (handler != null)
					handler.Callback.OnLostFocus(Handler.Widget, EventArgs.Empty);
			}

			public void VisibilityNotifyEvent(object o, Gtk.VisibilityNotifyEventArgs args)
			{
				if (args.Event.State == Gdk.VisibilityState.FullyObscured)
					Handler.Callback.OnShown(Handler.Widget, EventArgs.Empty);
			}

			public void HandleControlRealized(object sender, EventArgs e)
			{
				Handler.RealizedSetup();
				Handler.Control.Realized -= HandleControlRealized;
			}
		}

		protected virtual Gtk.Widget FontControl
		{
			get { return Control; }
		}

		static readonly object Font_Key = new object();

		public virtual Font Font
		{
			get { return Widget.Properties.Get<Font>(Font_Key, () => new Font(new FontHandler(FontControl))); }
			set { Widget.Properties.Set(Font_Key, value, () => FontControl.SetFont(value.ToPango())); }
		}

		static readonly object Cursor_Key = new object();

		public Cursor Cursor
		{
			get { return Widget.Properties.Get<Cursor>(Cursor_Key); }
			set
			{
				Widget.Properties.Set(Cursor_Key, value, () =>
				{
					var gdkWindow = Control.GetWindow();
					if (gdkWindow != null)
					{
						gdkWindow.Cursor = value != null ? value.ControlObject as Gdk.Cursor : null;
					}
				});
			}
		}

		public string ToolTip
		{
			get { return Control.TooltipText; }
			set { Control.TooltipText = value; }
		}

		public virtual IEnumerable<string> SupportedPlatformCommands
		{
			get { return Enumerable.Empty<string>(); }
		}

		public virtual void MapPlatformCommand(string systemAction, Command action)
		{
		}

		public PointF PointFromScreen(PointF point)
		{
			var gdkWindow = Control.GetWindow();
			if (gdkWindow != null)
			{
				int x, y;
				gdkWindow.GetOrigin(out x, out y);
				return new PointF(point.X - x, point.Y - y);
			}
			return point;
		}

		public PointF PointToScreen(PointF point)
		{
			var gdkWindow = Control.GetWindow();
			if (gdkWindow != null)
			{
				int x, y;
				gdkWindow.GetOrigin(out x, out y);
				return new PointF(point.X + x, point.Y + y);
			}
			return point;
		}

		public Point Location
		{
			get { return Control.Allocation.Location.ToEto(); }
		}

		static uint? DefaultBorderWidth;

		public virtual bool ShowBorder
		{
			get
			{
				var container = Control as Gtk.Container;
				return container != null && container.BorderWidth > 0;
			}
			set
			{
				var container = Control as Gtk.Container;
				if (container == null)
					return;
				if (DefaultBorderWidth == null)
					DefaultBorderWidth = container.BorderWidth;
				container.BorderWidth = value ? DefaultBorderWidth.Value : 0;
			}
		}

		static readonly object TabIndex_Key = new object();

		public int TabIndex
		{
			get { return Widget.Properties.Get<int>(TabIndex_Key, int.MaxValue); }
			set { Widget.Properties.Set(TabIndex_Key, value, int.MaxValue); }
		}

		public virtual IEnumerable<Control> VisualControls => Enumerable.Empty<Control>();
	}
}
