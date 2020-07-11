using System;
using System.Text.RegularExpressions;
using Eto.Forms;
using Eto.Drawing;
using Eto.GtkSharp.Drawing;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.IO;
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
	}
}
