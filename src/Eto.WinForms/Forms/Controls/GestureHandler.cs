namespace Eto.WinForms.Forms.Controls
{
	interface IWinFormsGestureHandler : Gesture.IHandler
	{
		Gesture Gesture { get; }
		bool CanProcessGesture(ref Win32.GESTUREINFO info);
		bool CanProcessWheel(SizeF delta);
		void ProcessGesture(ref Win32.GESTUREINFO info, float scale);
		void ProcessWheel(SizeF delta);
		void Reset();
		void AttachTo(swf.Control control);
		void Detach();
	}

	class WinFormsGestureCoordinator : swf.NativeWindow, IDisposable
	{
		readonly swf.Control _control;
		readonly List<IWinFormsGestureHandler> _handlers = new List<IWinFormsGestureHandler>();
		bool _configured;

		public WinFormsGestureCoordinator(swf.Control control)
		{
			_control = control;
			if (_control.IsHandleCreated)
				OnHandleCreated(_control, EventArgs.Empty);
			_control.HandleCreated += OnHandleCreated;
			_control.HandleDestroyed += OnHandleDestroyed;
		}

		public int Count => _handlers.Count;

		public void Register(IWinFormsGestureHandler handler)
		{
			_handlers.Add(handler);
			handler.AttachTo(_control);
			ConfigureGestures();
		}

		public void Unregister(IWinFormsGestureHandler handler)
		{
			if (_handlers.Remove(handler))
				handler.Detach();
		}

		void OnHandleCreated(object sender, EventArgs e)
		{
			if (Handle == IntPtr.Zero)
				AssignHandle(_control.Handle);
			ConfigureGestures();
		}

		void OnHandleDestroyed(object sender, EventArgs e)
		{
			_configured = false;
			ReleaseHandle();
			foreach (var handler in _handlers)
				handler.Reset();
		}

		void ConfigureGestures()
		{
			if (_configured || !_control.IsHandleCreated)
				return;
			var configs = new[]
			{
				new Win32.GESTURECONFIG { dwID = 0, dwWant = Win32.GC_ALLGESTURES, dwBlock = 0 }
			};
			if (Win32.SetGestureConfig(_control.Handle, 0, configs.Length, configs, Marshal.SizeOf(typeof(Win32.GESTURECONFIG))))
				_configured = true;
		}

		protected override void WndProc(ref swf.Message m)
		{
			if (m.Msg == (int)Win32.WM.GESTURE && _handlers.Count > 0)
			{
				var info = new Win32.GESTUREINFO { cbSize = Marshal.SizeOf(typeof(Win32.GESTUREINFO)) };
				if (Win32.GetGestureInfo(m.LParam, ref info))
				{
					var scale = GetScaleFactor();
					if (info.dwID == Win32.GID_END)
					{
						for (int i = 0; i < _handlers.Count; i++)
							_handlers[i].ProcessGesture(ref info, scale);
					}
					else
					{
						ProcessRecognizingHandlers(handler => handler.CanProcessGesture(ref info), handler => handler.ProcessGesture(ref info, scale));
					}
				}
			}
			else if ((m.Msg == (int)Win32.WM.MOUSEWHEEL || m.Msg == (int)Win32.WM.MOUSEHWHEEL) && _handlers.Count > 0)
			{
				var wheelDelta = Win32.GetWheelDeltaWParam(m.WParam) / WinConversions.WheelDelta;
				var delta = m.Msg == (int)Win32.WM.MOUSEHWHEEL ? new SizeF(-wheelDelta, 0) : new SizeF(0, wheelDelta);
				ProcessRecognizingHandlers(handler => handler.CanProcessWheel(delta), handler => handler.ProcessWheel(delta));
			}
			base.WndProc(ref m);
		}

		void ProcessRecognizingHandlers(Func<IWinFormsGestureHandler, bool> canProcess, Action<IWinFormsGestureHandler> process)
		{
			var firstHandler = _handlers.FirstOrDefault(canProcess);
			if (firstHandler == null)
				return;

			for (int i = 0; i < _handlers.Count; i++)
			{
				var handler = _handlers[i];
				if (!canProcess(handler))
					continue;
				if (handler == firstHandler || firstHandler.Gesture.CanRecognizeSimultaneouslyWith(handler.Gesture))
					process(handler);
			}
		}

		float GetScaleFactor()
		{
			if (Win32.PerMonitorDpiSupported && _control.IsHandleCreated)
			{
				var dpi = Win32.GetDpiForWindow(_control.Handle);
				if (dpi > 0)
					return dpi / 96f;
			}
			return Win32.SystemDpi;
		}

		public void Dispose()
		{
			foreach (var handler in _handlers)
				handler.Detach();
			_handlers.Clear();
			_control.HandleCreated -= OnHandleCreated;
			_control.HandleDestroyed -= OnHandleDestroyed;
			ReleaseHandle();
		}
	}

	public abstract class GestureHandler<TWidget> : WidgetHandler<object, TWidget>, IWinFormsGestureHandler
		where TWidget : Gesture
	{
		bool _enabled = true;

		protected new Gesture.ICallback Callback => (Gesture.ICallback)((ICallbackSource)Widget).Callback;

		public bool Enabled
		{
			get => _enabled;
			set => _enabled = value;
		}

		Gesture IWinFormsGestureHandler.Gesture => Widget;

		void IWinFormsGestureHandler.ProcessGesture(ref Win32.GESTUREINFO info, float scale)
		{
			if (!_enabled)
			{
				Reset();
				return;
			}
			HandleGesture(ref info, scale);
		}

		bool IWinFormsGestureHandler.CanProcessGesture(ref Win32.GESTUREINFO info)
		{
			return _enabled && CanHandleGesture(ref info);
		}

		void IWinFormsGestureHandler.ProcessWheel(SizeF delta)
		{
			if (_enabled)
				HandleWheel(delta);
		}

		bool IWinFormsGestureHandler.CanProcessWheel(SizeF delta)
		{
			return _enabled && CanHandleWheel(delta);
		}

		public virtual void Reset()
		{
		}

		void IWinFormsGestureHandler.AttachTo(swf.Control control) => OnAttached(control);

		void IWinFormsGestureHandler.Detach() => OnDetached();

		protected virtual void OnAttached(swf.Control control)
		{
		}

		protected virtual void OnDetached()
		{
		}

		internal virtual bool CanHandleGesture(ref Win32.GESTUREINFO info) => false;

		internal abstract void HandleGesture(ref Win32.GESTUREINFO info, float scale);

		internal virtual bool CanHandleWheel(SizeF delta) => false;

		internal virtual void HandleWheel(SizeF delta)
		{
		}
	}

	public class MagnificationGestureHandler : GestureHandler<MagnificationGesture>, MagnificationGesture.IHandler
	{
		long _previousDistance;
		bool _active;
		float _magnification;

		public float Magnification => _magnification;

		public override void Reset()
		{
			_active = false;
			_previousDistance = 0;
		}

		internal override void HandleGesture(ref Win32.GESTUREINFO info, float scale)
		{
			if (info.dwID != Win32.GID_ZOOM)
			{
				if (info.dwID == Win32.GID_END)
					Reset();
				return;
			}

			if ((info.dwFlags & Win32.GF_BEGIN) != 0)
			{
				_previousDistance = info.ullArguments;
				_active = _previousDistance > 0;
				return;
			}

			if (!_active || _previousDistance <= 0 || info.ullArguments <= 0)
			{
				_previousDistance = info.ullArguments;
				_active = info.ullArguments > 0;
				return;
			}

			_magnification = (float)((double)info.ullArguments / _previousDistance - 1.0);
			_previousDistance = info.ullArguments;
			if (_magnification == 0f)
				return;
			Callback.OnActivated(Widget, EventArgs.Empty);
		}

		internal override bool CanHandleGesture(ref Win32.GESTUREINFO info) => info.dwID == Win32.GID_ZOOM;
	}

	public class RotationGestureHandler : GestureHandler<RotationGesture>, RotationGesture.IHandler
	{
		double _previousAngle;
		bool _active;
		float _rotation;

		public float Rotation => _rotation;

		public override void Reset()
		{
			_active = false;
			_previousAngle = 0;
		}

		// Win32 encodes the cumulative rotation angle in the low word of ullArguments.
		// GID_ROTATE_ANGLE_FROM_ARGUMENT: counter-clockwise radians, 0 at gesture begin.
		static double DecodeAngle(long arguments)
		{
			var arg = (double)(ushort)(arguments & 0xFFFF);
			return arg / 65535.0 * 4.0 * Math.PI - 2.0 * Math.PI;
		}

		internal override void HandleGesture(ref Win32.GESTUREINFO info, float scale)
		{
			if (info.dwID != Win32.GID_ROTATE)
			{
				if (info.dwID == Win32.GID_END)
					Reset();
				return;
			}

			if ((info.dwFlags & Win32.GF_BEGIN) != 0)
			{
				_previousAngle = DecodeAngle(info.ullArguments);
				_active = true;
				return;
			}

			var angle = DecodeAngle(info.ullArguments);
			if (!_active)
			{
				_previousAngle = angle;
				_active = true;
				return;
			}

			// Negate to expose degrees clockwise to match other platforms.
			_rotation = (float)(-(angle - _previousAngle) * 180.0 / Math.PI);
			_previousAngle = angle;
			if (_rotation == 0f)
				return;
			Callback.OnActivated(Widget, EventArgs.Empty);
		}

		internal override bool CanHandleGesture(ref Win32.GESTUREINFO info) => info.dwID == Win32.GID_ROTATE;
	}

	public class PanGestureHandler : GestureHandler<PanGesture>, PanGesture.IHandler
	{
		swf.Control _control;
		short _previousX;
		short _previousY;
		DateTime _previousTime;
		bool _active;
		PointF _translation;
		PointF _velocity;
		MouseButtons _mouseButtons = MouseButtons.Primary;
		bool _mouseActive;
		PointF _previousMousePosition;
		DateTime _previousMouseTime;

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
			}
		}

		public override void Reset()
		{
			_active = false;
			EndMousePan();
		}

		internal override void HandleGesture(ref Win32.GESTUREINFO info, float scale)
		{
			if (info.dwID != Win32.GID_PAN)
			{
				if (info.dwID == Win32.GID_END)
					Reset();
				return;
			}

			if ((info.dwFlags & Win32.GF_BEGIN) != 0)
			{
				_previousX = info.ptsLocation.x;
				_previousY = info.ptsLocation.y;
				_previousTime = DateTime.UtcNow;
				_active = true;
				return;
			}

			if (!_active)
			{
				_previousX = info.ptsLocation.x;
				_previousY = info.ptsLocation.y;
				_previousTime = DateTime.UtcNow;
				_active = true;
				return;
			}

			var dx = info.ptsLocation.x - _previousX;
			var dy = info.ptsLocation.y - _previousY;
			var now = DateTime.UtcNow;
			var dt = (now - _previousTime).TotalSeconds;
			_previousX = info.ptsLocation.x;
			_previousY = info.ptsLocation.y;
			_previousTime = now;

			if (dx == 0 && dy == 0)
				return;

			var invScale = scale > 0 ? 1f / scale : 1f;
			var logicalDx = dx * invScale;
			var logicalDy = dy * invScale;
			_translation = new PointF(logicalDx, logicalDy);
			_velocity = dt > 0 ? new PointF((float)(logicalDx / dt), (float)(logicalDy / dt)) : PointF.Empty;
			Callback.OnActivated(Widget, EventArgs.Empty);
		}

		internal override bool CanHandleGesture(ref Win32.GESTUREINFO info)
		{
			return info.dwID == Win32.GID_PAN;
		}

		protected override void OnAttached(swf.Control control)
		{
			_control = control;
			_control.MouseDown += OnMouseDown;
			_control.MouseMove += OnMouseMove;
			_control.MouseUp += OnMouseUp;
			_control.MouseCaptureChanged += OnMouseCaptureChanged;
		}

		protected override void OnDetached()
		{
			if (_control == null)
				return;
			_control.MouseDown -= OnMouseDown;
			_control.MouseMove -= OnMouseMove;
			_control.MouseUp -= OnMouseUp;
			_control.MouseCaptureChanged -= OnMouseCaptureChanged;
			EndMousePan();
			_control = null;
		}

		void OnMouseDown(object sender, swf.MouseEventArgs e)
		{
			if (!Enabled || !AreMouseButtonsPressed())
				return;

			_mouseActive = true;
			_previousMousePosition = e.ToEto(_control).Location;
			_previousMouseTime = DateTime.UtcNow;
			_control.Capture = true;
		}

		void OnMouseMove(object sender, swf.MouseEventArgs e)
		{
			if (!_mouseActive)
				return;

			if (!Enabled || !AreMouseButtonsPressed())
			{
				EndMousePan();
				return;
			}

			var position = e.ToEto(_control).Location;
			var now = DateTime.UtcNow;
			var translation = position - _previousMousePosition;
			var dt = (now - _previousMouseTime).TotalSeconds;

			_previousMousePosition = position;
			_previousMouseTime = now;

			if (translation.X == 0 && translation.Y == 0)
				return;

			_translation = translation;
			_velocity = dt > 0 ? new PointF((float)(translation.X / dt), (float)(translation.Y / dt)) : PointF.Empty;
			Callback.OnActivated(Widget, EventArgs.Empty);
		}

		void OnMouseUp(object sender, swf.MouseEventArgs e)
		{
			if (_mouseActive && !AreMouseButtonsPressed())
				EndMousePan();
		}

		void OnMouseCaptureChanged(object sender, EventArgs e)
		{
			if (_control?.Capture == false)
				_mouseActive = false;
		}

		void EndMousePan()
		{
			_mouseActive = false;
			if (_control?.Capture == true)
				_control.Capture = false;
		}

		bool AreMouseButtonsPressed()
		{
			var buttons = swf.Control.MouseButtons.ToEto();
			return (buttons & _mouseButtons) == _mouseButtons;
		}
	}

	public class ScrollGestureHandler : GestureHandler<ScrollGesture>, ScrollGesture.IHandler
	{
		DateTime _previousTime;
		SizeF _delta;
		PointF _velocity;

		public SizeF Delta => _delta;
		public PointF Velocity => _velocity;

		// Windows applies a reversed scrolling direction to the deltas without saying that it did, so there is no way to tell
		public bool IsDirectionInverted => false;

		public override void Reset()
		{
			_previousTime = default;
			_velocity = PointF.Empty;
		}

		internal override void HandleGesture(ref Win32.GESTUREINFO info, float scale)
		{
			if (info.dwID == Win32.GID_END)
				Reset();
		}

		internal override void HandleWheel(SizeF delta)
		{
			if (delta.IsZero)
				return;

			delta = new SizeF(delta.Width * Widget.WheelScrollAmount, delta.Height * Widget.WheelScrollAmount);

			var now = DateTime.UtcNow;
			var dt = _previousTime == default ? 0 : (now - _previousTime).TotalSeconds;
			_previousTime = now;

			_delta = delta;
			_velocity = dt > 0 ? new PointF((float)(delta.Width / dt), (float)(delta.Height / dt)) : PointF.Empty;
			Callback.OnActivated(Widget, EventArgs.Empty);
		}

		internal override bool CanHandleWheel(SizeF delta) => !delta.IsZero;
	}
}
