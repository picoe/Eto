using System;
using System.Text.RegularExpressions;
using Eto.Forms;
using Eto.Drawing;
using Eto.GtkSharp.Drawing;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;

namespace Eto.GtkSharp.Forms
{
	public interface IGtkControl
	{
		Point CurrentLocation { get; set; }

		Size UserPreferredSize { get; }

		Gtk.Widget ContainerControl { get; }

		Color? SelectedBackgroundColor { get; }

		void SetBackgroundColor();

		void InvalidateMeasure();

#if GTK2
		void TriggerEnabled(bool oldEnabled, bool newEnabled, bool force = false);
#endif
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

	static class GtkControl
	{
		public static readonly object UserPreferredSize_Key = new object();
		public static readonly object ScrollAmount_Key = new object();
		public static readonly object DragInfo_Key = new object();
		public static readonly object DropSource_Key = new object();
		public static readonly object Font_Key = new object();
		public static readonly object TabIndex_Key = new object();
		public static readonly object Cursor_Key = new object();
		public static readonly object AllowDrop_Key = new object();
		public static uint? DefaultBorderWidth;
		
	}

	public abstract class GtkControl<TControl, TWidget, TCallback> : WidgetHandler<TControl, TWidget, TCallback>, Control.IHandler, IGtkControl
		where TControl : Gtk.Widget
		where TWidget : Control
		where TCallback : Control.ICallback
	{
		Size asize;
		bool mouseDownHandled;
#if GTK2
		Color? cachedBackgroundColor;
#else

		// In GTK the gesture types are handled by external classes for each type of gesture
		private static Gtk.GestureSwipe swipe;
		private static Gtk.GestureRotate rotate;
		private static Gtk.GestureZoom zoom;
		private static Gtk.GesturePan panv;
		private static Gtk.GesturePan panh;
		//private static Gtk.GestureLongPress longpress;
		private static Gtk.GestureMultiPress longpress;
		private static Gtk.GestureDrag drag;
		
#endif
		Color? backgroundColor;

		public float ScrollAmount
		{
			get => Widget.Properties.Get<float?>(GtkControl.ScrollAmount_Key) ?? 2f;
			set => Widget.Properties.Set(GtkControl.ScrollAmount_Key, value);
		}

		public override IntPtr NativeHandle { get { return Control.Handle; } }

		public Size UserPreferredSize
		{
			get => Widget.Properties.Get<Size?>(GtkControl.UserPreferredSize_Key) ?? new Size(-1, -1);
			set => Widget.Properties.Set(GtkControl.UserPreferredSize_Key, value);
		}

		public virtual Size DefaultSize { get { return new Size(-1, -1); } }

		public virtual Gtk.Widget ContainerControl => Control;

		public virtual Gtk.Widget EventControl => Control;

		public virtual Gtk.Widget ContainerContentControl => ContainerControl;

		public virtual Gtk.Widget BackgroundControl => ContainerContentControl;

		public virtual Gtk.Widget DragControl => EventControl;

		public virtual Point CurrentLocation { get; set; }

		public virtual Size Size
		{
			get => ContainerControl.Visible ? ContainerControl.Allocation.Size.ToEto() : UserPreferredSize;
			set
			{
				var userPreferredSize = UserPreferredSize;
				if (userPreferredSize == value)
					return;
				userPreferredSize = value;
				UserPreferredSize = userPreferredSize;
				if (userPreferredSize.Width == -1 || userPreferredSize.Height == -1)
				{
					var defSize = DefaultSize;
					if (userPreferredSize.Width == -1)
						userPreferredSize.Width = defSize.Width;
					if (userPreferredSize.Height == -1)
						userPreferredSize.Height = defSize.Height;
				}
				SetSize(userPreferredSize);
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

		protected void SetSize() => SetSize(DefaultSize);

		protected virtual void SetSize(Size size)
		{
			ContainerControl.SetSizeRequest(size.Width, size.Height);
			InvalidateMeasure();
		}

		public virtual bool Enabled
		{
			get
			{
#if GTK3
				return ContainerControl.IsSensitive;
#else
				return ContainerControl.Sensitive && Widget.VisualParent?.Enabled != false;
#endif
			}
			set
			{
#if GTK3
				ContainerControl.Sensitive = value;
#else
				TriggerEnabled(Enabled, value, true);
#endif
			}
		}

#if GTK2
		public virtual void TriggerEnabled(bool oldEnabled, bool newEnabled, bool force)
		{
			if (force && newEnabled != ContainerControl.Sensitive)
				ContainerControl.Sensitive = newEnabled;

			newEnabled = force ? Enabled : newEnabled;

			if (oldEnabled != newEnabled)
				Callback.OnEnabledChanged(Widget, EventArgs.Empty);
		}
#endif

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
			if (!Widget.IsSuspended)
				InvalidateMeasure();
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
			get { return ContainerControl.Visible; }
			set
			{
				ContainerControl.Visible = value;
				ContainerControl.NoShowAll = !value;
				if (value && Widget.Loaded)
				{
					ContainerControl.ShowAll();
				}
			}
		}

		public virtual void SetParent(Container oldParent, Container newParent)
		{
			if (newParent == null)
			{
#if GTK3
				ContainerControl.Unparent();
				var isSensitive = ContainerControl.Sensitive;
				if (ContainerControl.IsSensitive != isSensitive)
				{
					// HACK: Gtk3 does not appear to properly propagate the INSENSITIVE state flag when removed from a parent
					ContainerControl.Sensitive = !isSensitive;
					ContainerControl.Sensitive = isSensitive;
				}
#else
				// when removed from a parent that wasn't enabled, trigger the change if needed!
				if (oldParent?.Enabled == false)
				{
					TriggerEnabled(false, true, false);
				}
#endif
			}
		}

		protected override void Initialize()
		{
			base.Initialize();
			DragControl.DragDataReceived += Connector.HandleDragDataReceived;
			DragControl.DragDataGet += Connector.HandleDragDataGet;
			HandleEvent(Eto.Forms.Control.SizeChangedEvent);
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

		protected virtual void RealizedSetup()
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
#if GTK3
				case Eto.Forms.Control.SwipeGestureEvent:
					EventControl.AddEvents((int)Gdk.EventMask.TouchpadGestureMask);
					swipe = new Gtk.GestureSwipe(Control);
					swipe.PropagationPhase = Gtk.PropagationPhase.Bubble;
					swipe.Swipe += Connector.HandleSwipeGestureEvent;
					break;
				case Eto.Forms.Control.RotateGestureEvent:
					EventControl.AddEvents((int)Gdk.EventMask.TouchpadGestureMask);
					rotate = new Gtk.GestureRotate(Control);
					rotate.PropagationPhase = Gtk.PropagationPhase.Bubble;
					rotate.AngleChanged += Connector.HandleRotateGestureEvent;
					break;
				case Eto.Forms.Control.PanHGestureEvent:
					EventControl.AddEvents((int)Gdk.EventMask.TouchpadGestureMask);
					panh = new Gtk.GesturePan(Control,Gtk.Orientation.Horizontal);
					panh.PropagationPhase = Gtk.PropagationPhase.Bubble;
					panh.Pan += Connector.HandlePanHGestureEvent;
					//pan.TouchOnly = true;
					break;
				case Eto.Forms.Control.PanVGestureEvent:
					EventControl.AddEvents((int)Gdk.EventMask.TouchpadGestureMask);
					panv = new Gtk.GesturePan(Control,Gtk.Orientation.Vertical);
					panv.PropagationPhase = Gtk.PropagationPhase.Bubble;
					//pan.TouchOnly = true;
					panv.Pan += Connector.HandlePanVGestureEvent;
					break;

				case Eto.Forms.Control.LongPressGestureEvent:
					EventControl.AddEvents((int)Gdk.EventMask.TouchpadGestureMask);
					longpress = new Gtk.GestureMultiPress(Control);
					
					longpress.PropagationPhase = Gtk.PropagationPhase.Bubble;
					longpress.Begin += Connector.HandleLongPressBeginGestureEvent;
					longpress.Pressed += Connector.HandleLongPressGestureEvent;
					longpress.End += Connector.HandleLongPressDoneGestureEvent;
					longpress.Cancel += Connector.HandleLongPressCancelGestureEvent;
					longpress.TouchOnly = true;
					break;
				case Eto.Forms.Control.ZoomGestureEvent:
					EventControl.AddEvents((int)Gdk.EventMask.TouchpadGestureMask);
					zoom = new Gtk.GestureZoom(Control);
					zoom.PropagationPhase = Gtk.PropagationPhase.Bubble;
					zoom.ScaleChanged += Connector.HandleZoomGestureEvent;					
					break;
				case Eto.Forms.Control.DragGestureEvent:
					EventControl.AddEvents((int)Gdk.EventMask.TouchpadGestureMask);
					drag = new Gtk.GestureDrag(Control);
					drag.PropagationPhase = Gtk.PropagationPhase.Bubble;
					drag.DragBegin += Connector.HandleDragStartGestureEvent;
					drag.Update += Connector.HandleDragGestureEvent;
					drag.End += Connector.HandleDragDoneGestureEvent;
					break;
				
#endif
				case Eto.Forms.Control.GotFocusEvent:
					EventControl.AddEvents((int)Gdk.EventMask.FocusChangeMask);
					EventControl.FocusInEvent += Connector.FocusInEvent;
					break;
				case Eto.Forms.Control.LostFocusEvent:
					EventControl.AddEvents((int)Gdk.EventMask.FocusChangeMask);
					EventControl.FocusOutEvent += Connector.FocusOutEvent;
					break;
				case Eto.Forms.Control.ShownEvent:
					EventControl.Mapped += Connector.MappedEvent;
					break;
				case Eto.Forms.Control.DragDropEvent:
					DragControl.DragDrop += Connector.HandleDragDrop;
					break;
				case Eto.Forms.Control.DragOverEvent:
					DragControl.DragMotion += Connector.HandleDragMotion;
					HandleEvent(Eto.Forms.Control.DragLeaveEvent);
					break;

				case Eto.Forms.Control.DragEnterEvent:
					// no enter event in gtk, so we fire it on the first DragMotion event and reset after it leaves
					HandleEvent(Eto.Forms.Control.DragOverEvent);
					break;
				case Eto.Forms.Control.DragLeaveEvent:
					HandleEvent(Eto.Forms.Control.DragOverEvent);
					DragControl.DragLeave += Connector.HandleDragLeave;
					break;
				case Eto.Forms.Control.EnabledChangedEvent:
#if GTK3
					ContainerControl.StateFlagsChanged += Connector.HandleStateFlagsChangedForEnabled;
#endif
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
			DragEffects? _dragEnterEffects;
			protected DragEventArgs DragArgs { get; private set; }

			new GtkControl<TControl, TWidget, TCallback> Handler { get { return (GtkControl<TControl, TWidget, TCallback>)base.Handler; } }

			public void HandleScrollEvent(object o, Gtk.ScrollEventArgs args)
			{
				var h = Handler;
				if (h == null)
					return;
				var p = new PointF((float)args.Event.X, (float)args.Event.Y);
				Keys modifiers = args.Event.State.ToEtoKey();
				MouseButtons buttons = args.Event.State.ToEtoMouseButtons();
				SizeF delta;

				switch (args.Event.Direction)
				{
					case Gdk.ScrollDirection.Down:
						delta = new SizeF(0f, -h.ScrollAmount);
						break;
					case Gdk.ScrollDirection.Left:
						delta = new SizeF(h.ScrollAmount, 0f);
						break;
					case Gdk.ScrollDirection.Right:
						delta = new SizeF(-h.ScrollAmount, 0f);
						break;
					case Gdk.ScrollDirection.Up:
						delta = new SizeF(0f, h.ScrollAmount);
						break;
#if GTKCORE
					case Gdk.ScrollDirection.Smooth:
						delta = new SizeF((float)args.Event.DeltaX, (float)args.Event.DeltaY);
						break;
#endif
					default:
						throw new NotSupportedException();
				}

				h.Callback.OnMouseWheel(h.Widget, new MouseEventArgs(buttons, modifiers, p, delta));
			}

			[GLib.ConnectBefore]
			public void HandleControlLeaveNotifyEvent(object o, Gtk.LeaveNotifyEventArgs args)
			{
				// ignore child events
				if (args.Event.Detail == Gdk.NotifyType.Inferior)
					return;
				var p = new PointF((float)args.Event.X, (float)args.Event.Y);
				Keys modifiers = args.Event.State.ToEtoKey();
				MouseButtons buttons = MouseButtons.None;

				Handler.Callback.OnMouseLeave(Handler.Widget, new MouseEventArgs(buttons, modifiers, p));
			}

			[GLib.ConnectBefore]
			public void HandleControlEnterNotifyEvent(object o, Gtk.EnterNotifyEventArgs args)
			{
				// ignore child events
				if (args.Event.Detail == Gdk.NotifyType.Inferior)
					return;
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

			[GLib.ConnectBefore]
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
			
	#if GTK3
			[GLib.ConnectBefore]
			public void HandleSwipeGestureEvent(object o, Gtk.SwipeArgs args)
			{
				var handler = Handler;
				if (handler == null)
					return;

				var e = new SwipeGestureEventArgs(args.VelocityX, args.VelocityY);
				if (e != null)
				{
					handler.Callback.OnSwipe(Handler.Widget, e);
					args.RetVal = e.Handled;
				}
			}

			[GLib.ConnectBefore]
			public void HandleRotateGestureEvent(object o, Gtk.AngleChangedArgs args)
			{
				var handler = Handler;
				if (handler == null)
					return;

				// could be rotate.AngleDelta if event args is not right
				var e = new RotateGestureEventArgs(rotate.AngleDelta);
				//var e = new RotateGestureEventArgs(args.Angle);
				if (e != null)
				{
					handler.Callback.OnRotate(Handler.Widget, e);
					args.RetVal = e.Handled;
				}
			}

			[GLib.ConnectBefore]
			public void HandlePanHGestureEvent(object o, Gtk.PanArgs args)
			{
				var handler = Handler;
				if (handler == null)
					return;



				PanDirection Dir = PanDirection.Left;

				switch (args.Direction)
				{
					case Gtk.PanDirection.Up:
						Dir = PanDirection.Up;
						break;
					case Gtk.PanDirection.Down:
						Dir = PanDirection.Down;
						break;
					case Gtk.PanDirection.Left:
						Dir = PanDirection.Left;
						break;
					case Gtk.PanDirection.Right:
						Dir = PanDirection.Right;
						
						break;
					default:
						break;
				}
				var e = new PanGestureEventArgs(true, 1, Dir, args.Offset);
				if (e != null)
				{
					handler.Callback.OnPanH(Handler.Widget, e);
					args.RetVal = e.Handled;
				}
			}


			[GLib.ConnectBefore]
			public void HandlePanVGestureEvent(object o, Gtk.PanArgs args)
			{
				var handler = Handler;
				if (handler == null)
					return;

				bool vpan = false;

				PanDirection Dir = PanDirection.Left;

				switch (args.Direction) {
				case Gtk.PanDirection.Up:
					Dir = PanDirection.Up;
					vpan = true;
					break;
				case Gtk.PanDirection.Down:
					Dir = PanDirection.Down;
					vpan = true;
					break;
				case Gtk.PanDirection.Left:
					Dir = PanDirection.Left;
					vpan = false;
					break;
				case Gtk.PanDirection.Right:
					Dir = PanDirection.Right;
					vpan = false;
					break;
				default:
					break;
				}
				// could be rotate.AngleDelta if event args is not right
				var e = new PanGestureEventArgs(true, 1, Dir, args.Offset);
				if (e != null)
				{
					handler.Callback.OnPanV(Handler.Widget, e);
					args.RetVal = e.Handled;
				}
			}

			[GLib.ConnectBefore]
			public void HandleLongPressBeginGestureEvent(object o, Gtk.BeginArgs args)
			{
				var handler = Handler;
				if (handler == null)
					return;

				double X, Y;
				longpress.GetBoundingBoxCenter(out X, out Y);
				

				var e = new LongPressGestureEventArgs(false, (int)longpress.NPoints, X, Y);
				if (e != null)
				{
					handler.Callback.OnLongPress(Handler.Widget, e);
					args.RetVal = e.Handled;
				}
			}


			[GLib.ConnectBefore]
			public void HandleLongPressGestureEvent(object o, Gtk.PressedArgs args)
			{
				var handler = Handler;
				if (handler == null)
					return;

				//double X, Y;
				//longpress.GetBoundingBoxCenter (out X, out Y);

				//var e = new LongPressGestureEventArgs (true, (int)longpress.NPoints, X, Y);
				var e = new LongPressGestureEventArgs(true, args.NPress, args.X, args.Y);
				if (e != null)
				{
					handler.Callback.OnLongPress(Handler.Widget, e);
					args.RetVal = e.Handled;
				}
			}

			[GLib.ConnectBefore]
			public void HandleLongPressDoneGestureEvent(object o, Gtk.EndArgs args)
			{
				var handler = Handler;
				if (handler == null)
					return;

				var e = new LongPressGestureEventArgs(false, 0,0,0);
				if (e != null)
				{
					handler.Callback.OnLongPress(Handler.Widget, e);
					args.RetVal = e.Handled;
				}
			}

			
			[GLib.ConnectBefore]
			public void HandleLongPressCancelGestureEvent(object o, Gtk.CancelArgs args)
			{
				var handler = Handler;
				if (handler == null)
					return;
								
				var e = new LongPressGestureEventArgs(false, 0, 0, 0);
				if (e != null)
				{
					handler.Callback.OnLongPress(Handler.Widget, e);
					args.RetVal = e.Handled;
				}
			}

			[GLib.ConnectBefore]
			public void HandleZoomGestureEvent(object o, Gtk.ScaleChangedArgs args)
			{
				var handler = Handler;
				if (handler == null)
					return;

				// could be rotate.AngleDelta if event args is not right
				var e = new ZoomGestureEventArgs(args.Scale);
				if (e != null)
				{
					handler.Callback.OnZoomExpand(Handler.Widget, e);
					args.RetVal = e.Handled;
				}
			}
			
			
			[GLib.ConnectBefore]
			public void HandleDragStartGestureEvent(object o, Gtk.DragBeginArgs args)
			{
				var handler = Handler;
				if (handler == null)
					return;
				
				// capture the 
				drag.GetStartPoint(out double X, out double Y);

				var e = new DragGestureEventArgs (true, (int)drag.NPoints, X ,Y, 0, 0);
				if (e != null)
				{
					handler.Callback.OnDragGesture(Handler.Widget, e);
					args.RetVal = e.Handled;
				}
			}

			
			[GLib.ConnectBefore]
			public void HandleDragGestureEvent(object o, Gtk.UpdateArgs args)
			{
				var handler = Handler;
				if (handler == null)
					return;

				drag.GetStartPoint(out double Xstart, out double Ystart);
				drag.GetBoundingBoxCenter (out double X, out double Y);
				
				// usually we will want a relative delta here
				double deltaX = X - Xstart;
				double deltaY = Y - Ystart;
				
				
				var e = new DragGestureEventArgs (true, (int)drag.NPoints, Xstart, Ystart, deltaX, deltaY);
				if (e != null)
				{
					handler.Callback.OnDragGesture(Handler.Widget, e);
					args.RetVal = e.Handled;
				}
			}

			[GLib.ConnectBefore]
			public void HandleDragDoneGestureEvent(object o, Gtk.EndArgs args)
			{
				var handler = Handler;
				if (handler == null)
					return;

				var e = new DragGestureEventArgs(false, 0,0,0, 0, 0);
				if (e != null)
				{
					handler.Callback.OnDragGesture(Handler.Widget, e);
					args.RetVal = e.Handled;
				}
			}

#endif
		
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

			public void HandleControlRealized(object sender, EventArgs e)
			{
				Handler.RealizedSetup();
				Handler.Control.Realized -= HandleControlRealized;
			}

			public virtual void MappedEvent(object sender, EventArgs e)
			{
				Handler.Callback.OnShown(Handler.Widget, EventArgs.Empty);
			}

			protected virtual DragEventArgs GetDragEventArgs(Gdk.DragContext context, PointF? location, uint time = 0, object controlObject = null)
			{
				var widget = Gtk.Drag.GetSourceWidget(context);
				var source = widget?.Data[GtkControl.DropSource_Key] as Eto.Forms.Control;

#if GTK2
				var action = context.Action;
#else
				var action = context.SelectedAction;
#endif

				var data = new DataObject(new DataObjectHandler(Handler.DragControl, context, time));
				if (location == null)
					location = Handler.PointFromScreen(Mouse.Position);

				return new DragEventArgs(source, data, action.ToEto(), location.Value, Keyboard.Modifiers, Mouse.Buttons, controlObject);
			}

			[GLib.ConnectBefore]
			public virtual void HandleDragDataReceived(object o, Gtk.DragDataReceivedArgs args)
			{
				var h = DragArgs?.Data.Handler as DataObjectHandler;
				h?.SetDataReceived(args);

				args.RetVal = true;
			}

			[GLib.ConnectBefore]
			public virtual void HandleDragDataGet(object o, Gtk.DragDataGetArgs args)
			{
				var data = Handler?.DragInfo?.Data.Handler as DataObjectHandler;
				if (data != null)
				{
					data.Apply(args.SelectionData);
					args.RetVal = true;
				}
			}

			[GLib.ConnectBefore]
			public virtual void HandleDragDrop(object o, Gtk.DragDropArgs args)
			{
				DragArgs = GetDragEventArgs(args.Context, new PointF(args.X, args.Y), args.Time);
				Handler.Callback.OnDragDrop(Handler.Widget, DragArgs);
				Gtk.Drag.Finish(args.Context, true, DragArgs.Effects.HasFlag(DragEffects.Move), args.Time);
				DragArgs = null;
				args.RetVal = true;
			}



			[GLib.ConnectBefore]
			public virtual void HandleDragMotion(object o, Gtk.DragMotionArgs args)
			{
				DragArgs = GetDragEventArgs(args.Context, new PointF(args.X, args.Y), args.Time);

				if (_dragEnterEffects == null)
				{
					Handler.Callback.OnDragEnter(Handler.Widget, DragArgs);
					_dragEnterEffects = DragArgs.Effects;
				}
				else
				{
					DragArgs.Effects = _dragEnterEffects.Value;
					Handler.Callback.OnDragOver(Handler.Widget, DragArgs);
				}

				Gdk.Drag.Status(args.Context, DragArgs.Effects.ToGdk(), args.Time);

				args.RetVal = true;
			}

			public virtual void HandleDragLeave(object o, Gtk.DragLeaveArgs args)
			{
				// use old args in case of a drop so we use the last motion args to determine position of drop for TreeGridView/GridView.
				DragArgs = DragArgs ?? GetDragEventArgs(args.Context, Handler.PointFromScreen(Mouse.Position), args.Time);
				Handler.Callback.OnDragLeave(Handler.Widget, DragArgs);
				_dragEnterEffects = null;
				Eto.Forms.Application.Instance.AsyncInvoke(() => DragArgs = null);
			}

#if GTK3
			public virtual void HandleStateFlagsChangedForEnabled(object o, Gtk.StateFlagsChangedArgs args)
			{
				var h = Handler;
				if (h == null)
					return;
				var wasSensitive = args.PreviousStateFlags.HasFlag(Gtk.StateFlags.Insensitive);
				var isSensitive = h.ContainerControl.StateFlags.HasFlag(Gtk.StateFlags.Insensitive);
				if (wasSensitive != isSensitive)
				{
					h.Callback.OnEnabledChanged(h.Widget, EventArgs.Empty);
				}
			}
#endif
		}

		protected virtual Gtk.Widget FontControl
		{
			get { return Control; }
		}

		public virtual Font Font
		{
			get => Widget.Properties.Get<Font>(GtkControl.Font_Key) ?? Widget.Properties.Get(GtkControl.Font_Key, FontControl.GetFont().ToEto());
			set
			{
				if (Widget.Properties.TrySet(GtkControl.Font_Key, value))
				{
					FontControl.SetFont(value.ToPango());
				}
			}
		}

		public Cursor Cursor
		{
			get { return Widget.Properties.Get<Cursor>(GtkControl.Cursor_Key); }
			set
			{
				Widget.Properties.Set(GtkControl.Cursor_Key, value, () =>
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
			var gdkWindow = EventControl.GetWindow();
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
			var gdkWindow = EventControl.GetWindow();
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
				if (GtkControl.DefaultBorderWidth == null)
					GtkControl.DefaultBorderWidth = container.BorderWidth;
				container.BorderWidth = value ? GtkControl.DefaultBorderWidth.Value : 0;
			}
		}

		public int TabIndex
		{
			get { return Widget.Properties.Get<int>(GtkControl.TabIndex_Key, int.MaxValue); }
			set { Widget.Properties.Set(GtkControl.TabIndex_Key, value, int.MaxValue); }
		}

		public virtual IEnumerable<Control> VisualControls => Enumerable.Empty<Control>();


		public bool AllowDrop
		{
			get { return Widget.Properties.Get(GtkControl.AllowDrop_Key, false); }
			set
			{
				if (value != AllowDrop)
				{
					if (value)
						Gtk.Drag.DestSet(DragControl, 0, null, 0); // accept all types?  can't find docs about this..
					else
						Gtk.Drag.DestUnset(DragControl);
					Widget.Properties.Set(GtkControl.AllowDrop_Key, value);
				}
			}
		}

		public void DoDragDrop(DataObject data, DragEffects allowedEffects, Image image, PointF cursorOffset)
		{
			var targets = (data.Handler as DataObjectHandler)?.GetTargets();

			DragInfo = new DragInfoObject { Data = data, AllowedEffects = allowedEffects };

			DragControl.Data[GtkControl.DropSource_Key] = Widget;

#if GTKCORE
			var context = Gtk.Drag.BeginWithCoordinates(DragControl, targets, allowedEffects.ToGdk(), 1, Gtk.Application.CurrentEvent, -1, -1);
#else
			var context = Gtk.Drag.Begin(DragControl, targets, allowedEffects.ToGdk(), 1, Gtk.Application.CurrentEvent);
#endif
			if (image != null)
				Gtk.Drag.SetIconPixbuf(context, image.ToGdk(), (int)cursorOffset.X, (int)cursorOffset.Y);

		}

		public Window GetNativeParentWindow() => (Control.Toplevel as Gtk.Window).ToEtoWindow();

		public virtual SizeF GetPreferredSize(SizeF availableSize)
		{
			if (!ContainerControl.IsRealized)
			{
				ContainerControl.Realize();
				ContainerControl.ShowAll();
			}
#if GTK3
			var requestMode = ContainerControl.RequestMode;
			var preferred = UserPreferredSize;
			var size = new Size(availableSize.Width >= float.MaxValue ? int.MaxValue : (int)availableSize.Width, availableSize.Height >= float.MaxValue ? int.MaxValue : (int)availableSize.Height);
			if (requestMode == Gtk.SizeRequestMode.HeightForWidth && preferred.Height < 0)
			{
				int width;
				int natural_width;
				if (preferred.Width >= 0)
					width = natural_width = preferred.Width;
				else
				{
					ContainerControl.GetPreferredWidth(out var minimum_width, out natural_width);
					width = Math.Max(minimum_width, Math.Min(size.Width, natural_width));
				}
				ContainerControl.GetPreferredHeightForWidth(width, out var minimum_height, out var natural_height);
				return new SizeF(natural_width, natural_height);
			}
			else if (requestMode == Gtk.SizeRequestMode.WidthForHeight && preferred.Width < 0)
			{
				int height;
				int natural_height;
				if (preferred.Height >= 0)
					height = natural_height = preferred.Height;
				else
				{
					ContainerControl.GetPreferredHeight(out var minimum_height, out natural_height);
					height = Math.Max(minimum_height, Math.Min(size.Height, natural_height));
				}
				ContainerControl.GetPreferredHeightForWidth(height, out var minimum_width, out var natural_width);
				return new SizeF(natural_width, natural_height);
			}
			ContainerControl.GetPreferredSize(out var minimum, out var natural);
			if (preferred.Width >= 0)
				natural.Width = preferred.Width;
			if (preferred.Height >= 0)
				natural.Height = preferred.Height;
			return new SizeF(natural.Width, natural.Height);
#else
			var size = ContainerControl.SizeRequest();
			return new SizeF(size.Width, size.Height);
#endif
		}

		class DragInfoObject
		{
			public DataObject Data { get; set; }
			public DragEffects AllowedEffects { get; set; }
		}

		DragInfoObject DragInfo
		{
			get { return Widget.Properties.Get<DragInfoObject>(GtkControl.DragInfo_Key); }
			set { Widget.Properties.Set(GtkControl.DragInfo_Key, value); }
		}

		public virtual void InvalidateMeasure()
		{
			if (Widget.Loaded && !Widget.IsSuspended)
			{
				Widget.VisualParent?.GetGtkControlHandler()?.InvalidateMeasure();
			}
		}

		public void Print()
		{
			// ContainerControl.Print
		}
	}
}
