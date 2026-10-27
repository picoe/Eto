#if GTKCORE
namespace Eto.GtkSharp.Forms.Controls
{
	interface IGtkGestureHandler : Gesture.IHandler
	{
		void AttachTo(Gtk.Widget widget);
		void Detach();
	}

	public abstract class GestureHandler<TControl, TWidget> : WidgetHandler<TControl, TWidget>, IGtkGestureHandler
		where TControl : Gtk.Gesture
		where TWidget : Gesture
	{
		bool _enabled = true;

		protected new Gesture.ICallback Callback => (Gesture.ICallback)((ICallbackSource)Widget).Callback;

		public bool Enabled
		{
			get => _enabled;
			set
			{
				_enabled = value;
				if (Control != null)
					Control.PropagationPhase = value ? Gtk.PropagationPhase.Bubble : Gtk.PropagationPhase.None;
			}
		}

		public void AttachTo(Gtk.Widget widget)
		{
			Detach();
			Control = CreateGesture(widget);
			Control.PropagationPhase = _enabled ? Gtk.PropagationPhase.Bubble : Gtk.PropagationPhase.None;
		}

		public void Detach()
		{
			if (Control != null)
			{
				Control.Dispose();
				Control = null;
			}
		}

		protected abstract TControl CreateGesture(Gtk.Widget widget);
	}

	public class MagnificationGestureHandler : GestureHandler<Gtk.GestureZoom, MagnificationGesture>, MagnificationGesture.IHandler
	{
		double _previousScale = 1.0;
		float _magnification;

		public float Magnification => _magnification;

		protected override Gtk.GestureZoom CreateGesture(Gtk.Widget widget)
		{
			var gesture = new Gtk.GestureZoom(widget);
			gesture.Begin += (sender, e) => _previousScale = gesture.ScaleDelta;
			gesture.ScaleChanged += (sender, e) =>
			{
				var current = gesture.ScaleDelta;
				_magnification = _previousScale > 0 ? (float)(current / _previousScale - 1.0) : 0f;
				_previousScale = current;
				Callback.OnActivated(Widget, EventArgs.Empty);
			};
			return gesture;
		}
	}

	public class RotationGestureHandler : GestureHandler<Gtk.GestureRotate, RotationGesture>, RotationGesture.IHandler
	{
		double _previousAngle;
		float _rotation;

		public float Rotation => _rotation;

		protected override Gtk.GestureRotate CreateGesture(Gtk.Widget widget)
		{
			var gesture = new Gtk.GestureRotate(widget);
			gesture.Begin += (sender, e) => _previousAngle = gesture.AngleDelta;
			gesture.AngleChanged += (sender, e) =>
			{
				// AngleDelta is the cumulative rotation in radians since the gesture began.
				// Gtk measures counter-clockwise; expose degrees clockwise to match other platforms.
				var current = gesture.AngleDelta;
				_rotation = (float)(-(current - _previousAngle) * 180.0 / Math.PI);
				_previousAngle = current;
				if (_rotation == 0f)
					return;
				Callback.OnActivated(Widget, EventArgs.Empty);
			};
			return gesture;
		}
	}

	public class PanGestureHandler : GestureHandler<Gtk.GestureDrag, PanGesture>, PanGesture.IHandler
	{
		PointF _translation;
		PointF _velocity;
		double _previousX;
		double _previousY;
		DateTime _previousTime;
		MouseButtons _mouseButtons = MouseButtons.Primary;

		public PointF Translation => _translation;
		public PointF Velocity => _velocity;
		public MouseButtons Buttons
		{
			get => _mouseButtons;
			set
			{
				if (value == MouseButtons.None)
					throw new ArgumentException($"{nameof(Buttons)} cannot be {nameof(MouseButtons.None)}.", nameof(value));
				_mouseButtons = value;
				if (Control != null)
					Control.Button = ToGtkButton(value);
			}
		}

		protected override Gtk.GestureDrag CreateGesture(Gtk.Widget widget)
		{
			var gesture = new Gtk.GestureDrag(widget);
			gesture.Button = ToGtkButton(_mouseButtons);
			gesture.DragBegin += (sender, e) =>
			{
				_previousX = 0;
				_previousY = 0;
				_previousTime = DateTime.UtcNow;
			};
			gesture.DragUpdate += (sender, e) =>
			{
				if (!IsMouseButtonMatched())
					return;

				gesture.GetOffset(out var x, out var y);
				var now = DateTime.UtcNow;
				var dx = x - _previousX;
				var dy = y - _previousY;
				var dt = (now - _previousTime).TotalSeconds;
				_translation = new PointF((float)dx, (float)dy);
				_velocity = dt > 0 ? new PointF((float)(dx / dt), (float)(dy / dt)) : PointF.Empty;
				_previousX = x;
				_previousY = y;
				_previousTime = now;
				Callback.OnActivated(Widget, EventArgs.Empty);
			};
			return gesture;
		}

		bool IsMouseButtonMatched()
		{
			return (Mouse.Buttons & _mouseButtons) == _mouseButtons;
		}

		static uint ToGtkButton(MouseButtons buttons)
		{
			switch (buttons)
			{
				case MouseButtons.Primary:
					return 1;
				case MouseButtons.Middle:
					return 2;
				case MouseButtons.Alternate:
					return 3;
				default:
					return 0;
			}
		}
	}

	public class ScrollGestureHandler : WidgetHandler<object, ScrollGesture>, ScrollGesture.IHandler, IGtkGestureHandler
	{
		Gtk.Widget _widget;
		bool _enabled = true;
		DateTime _previousTime;
		SizeF _delta;
		PointF _velocity;

		protected new Gesture.ICallback Callback => (Gesture.ICallback)((ICallbackSource)Widget).Callback;

		public bool Enabled
		{
			get => _enabled;
			set => _enabled = value;
		}

		public SizeF Delta => _delta;
		public PointF Velocity => _velocity;

		// GDK applies natural scrolling to the deltas without saying that it did, so there is no way to tell
		public bool IsDirectionInverted => false;

		public void AttachTo(Gtk.Widget widget)
		{
			Detach();
			_widget = widget;
			_widget.AddEvents((int)Gdk.EventMask.ScrollMask);
			_widget.ScrollEvent += HandleScrollEvent;
		}

		public void Detach()
		{
			if (_widget == null)
				return;
			_widget.ScrollEvent -= HandleScrollEvent;
			_widget = null;
		}

		void HandleScrollEvent(object o, Gtk.ScrollEventArgs args)
		{
			if (!Enabled)
				return;

			var delta = GetDelta(args);
			if (delta.IsZero)
				return;

			var now = DateTime.UtcNow;
			var dt = _previousTime == default ? 0 : (now - _previousTime).TotalSeconds;
			_previousTime = now;

			_delta = delta;
			_velocity = dt > 0 ? new PointF((float)(delta.Width / dt), (float)(delta.Height / dt)) : PointF.Empty;
			Callback.OnActivated(Widget, EventArgs.Empty);
		}

		SizeF GetDelta(Gtk.ScrollEventArgs args)
		{
			var scrollAmount = Widget.WheelScrollAmount;

			switch (args.Event.Direction)
			{
				case Gdk.ScrollDirection.Down:
					return new SizeF(0f, -scrollAmount);
				case Gdk.ScrollDirection.Left:
					return new SizeF(scrollAmount, 0f);
				case Gdk.ScrollDirection.Right:
					return new SizeF(-scrollAmount, 0f);
				case Gdk.ScrollDirection.Up:
					return new SizeF(0f, scrollAmount);
				case Gdk.ScrollDirection.Smooth:
					return new SizeF((float)args.Event.DeltaX * scrollAmount, (float)args.Event.DeltaY * scrollAmount);
				default:
					return SizeF.Empty;
			}
		}
	}
}
#endif
