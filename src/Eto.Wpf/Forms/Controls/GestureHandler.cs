namespace Eto.Wpf.Forms.Controls
{
	interface IWpfGestureHandler : Gesture.IHandler
	{
		Gesture Gesture { get; }
		bool CanHandleDelta(swi.ManipulationDeltaEventArgs e);
		bool CanHandleWheel(SizeF delta);
		void AttachTo(sw.UIElement element);
		void Detach();
	}

	public abstract class GestureHandler<TWidget> : WidgetHandler<object, TWidget>, IWpfGestureHandler
		where TWidget : Gesture
	{
		sw.UIElement _element;
		bool _enabled = true;

		protected new Gesture.ICallback Callback => (Gesture.ICallback)((ICallbackSource)Widget).Callback;

		public bool Enabled
		{
			get => _enabled;
			set => _enabled = value;
		}

		Gesture IWpfGestureHandler.Gesture => Widget;

		protected sw.UIElement Element => _element;

		public void AttachTo(sw.UIElement element)
		{
			Detach();
			_element = element;
			_element.IsManipulationEnabled = true;
			_element.ManipulationStarting += OnManipulationStarting;
			_element.ManipulationDelta += OnManipulationDelta;
			OnAttached(_element);
		}

		public void Detach()
		{
			if (_element == null)
				return;
			OnDetaching(_element);
			_element.ManipulationStarting -= OnManipulationStarting;
			_element.ManipulationDelta -= OnManipulationDelta;
			_element = null;
		}

		protected virtual void OnAttached(sw.UIElement element)
		{
		}

		protected virtual void OnDetaching(sw.UIElement element)
		{
		}

		void OnManipulationStarting(object sender, swi.ManipulationStartingEventArgs e)
		{
			e.ManipulationContainer = _element;
			e.Mode = swi.ManipulationModes.All;
		}

		void OnManipulationDelta(object sender, swi.ManipulationDeltaEventArgs e)
		{
			if (!_enabled || !CanRecognize(e))
				return;
			HandleDelta(e);
		}

		bool CanRecognize(swi.ManipulationDeltaEventArgs e)
		{
			var control = Widget.Control;
			if (control == null)
				return true;

			foreach (var gesture in control.Gestures)
			{
				if (((IHandlerSource)gesture).Handler is IWpfGestureHandler handler && handler.CanHandleDelta(e))
					return handler.Gesture == Widget || handler.Gesture.CanRecognizeSimultaneouslyWith(Widget);
			}
			return true;
		}

		bool IWpfGestureHandler.CanHandleDelta(swi.ManipulationDeltaEventArgs e)
		{
			return _enabled && CanHandleDelta(e);
		}

		bool IWpfGestureHandler.CanHandleWheel(SizeF delta)
		{
			return _enabled && CanHandleWheel(delta);
		}

		protected abstract bool CanHandleDelta(swi.ManipulationDeltaEventArgs e);

		protected abstract void HandleDelta(swi.ManipulationDeltaEventArgs e);

		protected virtual bool CanHandleWheel(SizeF delta) => false;

		/// <summary>
		/// Checks whether this gesture is allowed to activate from a wheel event given the other gestures attached to the control.
		/// </summary>
		protected bool CanRecognizeWheel(SizeF delta)
		{
			var control = Widget.Control;
			if (control == null)
				return true;

			foreach (var gesture in control.Gestures)
			{
				if (((IHandlerSource)gesture).Handler is IWpfGestureHandler handler && handler.CanHandleWheel(delta))
					return handler.Gesture == Widget || handler.Gesture.CanRecognizeSimultaneouslyWith(Widget);
			}
			return true;
		}
	}

	public class MagnificationGestureHandler : GestureHandler<MagnificationGesture>, MagnificationGesture.IHandler
	{
		const int WM_MOUSEWHEEL = 0x020A;
		const int MK_CONTROL = 0x0008;

		float _magnification;
		bool _enableWheelMagnification = true;
		float _wheelMagnificationStep = 0.1f;
		swin.HwndSource _source;
		bool _hooked;

		public float Magnification => _magnification;

		/// <summary>
		/// Gets or sets a value indicating whether Ctrl+MouseWheel events activate the gesture. Defaults to true.
		/// </summary>
		/// <remarks>
		/// On Windows, precision-touchpad pinch gestures are synthesized by the OS as Ctrl+<c>WM_MOUSEWHEEL</c>
		/// with the delta quantized to <c>WHEEL_DELTA</c> (120) chunks for apps that haven't opted into pointer input.
		/// Plain WPF apps fall into that bucket, so pinch will feel coarser than in Edge/Chromium even though both
		/// paths come through the same hook here. Regular wheel events (no Ctrl) are ignored and left to
		/// <see cref="ScrollGesture"/>.
		/// </remarks>
		public bool EnableWheelMagnification
		{
			get => _enableWheelMagnification;
			set
			{
				if (_enableWheelMagnification == value)
					return;
				_enableWheelMagnification = value;
				UpdateHook();
			}
		}

		/// <summary>
		/// Gets or sets the magnification factor applied per <c>WHEEL_DELTA</c> (120) of wheel movement when handling Ctrl+Wheel. Defaults to 0.1 (10% per detent).
		/// </summary>
		public float WheelMagnificationStep
		{
			get => _wheelMagnificationStep;
			set => _wheelMagnificationStep = value;
		}

		protected override bool CanHandleDelta(swi.ManipulationDeltaEventArgs e)
		{
			var scale = e.DeltaManipulation.Scale;
			var factor = (scale.X + scale.Y) / 2.0;
			return factor > 0 && !double.IsNaN(factor) && factor != 1.0;
		}

		protected override void HandleDelta(swi.ManipulationDeltaEventArgs e)
		{
			var scale = e.DeltaManipulation.Scale;
			var factor = (scale.X + scale.Y) / 2.0;
			if (factor <= 0 || double.IsNaN(factor))
				return;
			_magnification = (float)(factor - 1.0);
			if (_magnification == 0f)
				return;
			Callback.OnActivated(Widget, EventArgs.Empty);
		}

		protected override bool CanHandleWheel(SizeF delta)
		{
			return _enableWheelMagnification && !delta.IsZero && IsZoomModifierActive();
		}

		protected override void OnAttached(sw.UIElement element)
		{
			if (element is sw.FrameworkElement fe)
			{
				fe.Loaded += OnLoaded;
				fe.Unloaded += OnUnloaded;
			}
			UpdateHook();
		}

		protected override void OnDetaching(sw.UIElement element)
		{
			if (element is sw.FrameworkElement fe)
			{
				fe.Loaded -= OnLoaded;
				fe.Unloaded -= OnUnloaded;
			}
			RemoveHook();
		}

		void OnLoaded(object sender, sw.RoutedEventArgs e) => UpdateHook();
		void OnUnloaded(object sender, sw.RoutedEventArgs e) => RemoveHook();

		void UpdateHook()
		{
			if (_enableWheelMagnification && Element != null)
				AddHook();
			else
				RemoveHook();
		}

		void AddHook()
		{
			if (_hooked || Element == null)
				return;
			_source = sw.PresentationSource.FromVisual(Element) as swin.HwndSource;
			if (_source == null)
				return;
			_source.AddHook(HwndHook);
			_hooked = true;
		}

		void RemoveHook()
		{
			if (!_hooked || _source == null)
				return;
			_source.RemoveHook(HwndHook);
			_source = null;
			_hooked = false;
		}

		IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			if (msg != WM_MOUSEWHEEL || !Enabled || !_enableWheelMagnification)
				return IntPtr.Zero;
			if (Element?.IsMouseOver != true)
				return IntPtr.Zero;

			// wParam: HIWORD = signed delta, LOWORD = modifier flags (MK_CONTROL etc.).
			var wParamLong = (long)wParam;
			if (((int)wParamLong & MK_CONTROL) == 0)
				return IntPtr.Zero;

			var delta = (short)((wParamLong >> 16) & 0xFFFF);
			if (delta == 0)
				return IntPtr.Zero;

			var normalized = delta / WpfConversions.WheelDelta;
			if (!CanRecognizeWheel(new SizeF(0, normalized)))
				return IntPtr.Zero;

			var step = normalized * _wheelMagnificationStep;
			if (step == 0f)
				return IntPtr.Zero;

			_magnification = step;
			Callback.OnActivated(Widget, EventArgs.Empty);
			// Suppress WPF wheel processing for this Ctrl+Wheel message so parent ScrollViewers don't also scroll.
			handled = true;
			return IntPtr.Zero;
		}

		static bool IsZoomModifierActive()
		{
			return (swi.Keyboard.Modifiers & swi.ModifierKeys.Control) == swi.ModifierKeys.Control;
		}
	}

	public class RotationGestureHandler : GestureHandler<RotationGesture>, RotationGesture.IHandler
	{
		float _rotation;

		public float Rotation => _rotation;

		protected override bool CanHandleDelta(swi.ManipulationDeltaEventArgs e)
		{
			var rotation = e.DeltaManipulation.Rotation;
			return rotation != 0 && !double.IsNaN(rotation);
		}

		protected override void HandleDelta(swi.ManipulationDeltaEventArgs e)
		{
			// DeltaManipulation.Rotation is the per-delta rotation in degrees, clockwise positive.
			var rotation = e.DeltaManipulation.Rotation;
			if (rotation == 0 || double.IsNaN(rotation))
				return;
			_rotation = (float)rotation;
			Callback.OnActivated(Widget, EventArgs.Empty);
		}
	}

	public class PanGestureHandler : GestureHandler<PanGesture>, PanGesture.IHandler
	{
		PointF _translation;
		PointF _velocity;
		bool _enableMousePan = true;
		MouseButtons _mouseButtons = MouseButtons.Primary;
		bool _mousePanHooked;
		bool _mousePanActive;
		sw.Point _lastMousePosition;
		DateTime _lastMouseTime;

		public PointF Translation => _translation;
		public PointF Velocity => _velocity;

		/// <summary>
		/// Gets or sets a value indicating whether the gesture should also activate from mouse button down/move/up events on the attached element.
		/// </summary>
		/// <remarks>
		/// When enabled, pressing the buttons specified by <see cref="Buttons"/> on the element captures the mouse and reports translation/velocity on each move until one of the buttons is released.
		/// This is independent of the touch/trackpad manipulation events that always drive the gesture.
		/// </remarks>
		public bool EnableMousePan
		{
			get => _enableMousePan;
			set
			{
				if (_enableMousePan == value)
					return;
				_enableMousePan = value;
				UpdateMouseHooks();
			}
		}

		/// <summary>
		/// Gets or sets the mouse buttons that activate the gesture when <see cref="EnableMousePan"/> is true. Defaults to <see cref="MouseButtons.Primary"/>.
		/// </summary>
		/// <remarks>
		/// All specified buttons must be held down for the pan to activate, and releasing any one of them ends the pan.
		/// </remarks>
		public MouseButtons Buttons
		{
			get => _mouseButtons;
			set
			{
				if (_mouseButtons == value)
					return;
				_mouseButtons = value;
				CancelMousePan();
			}
		}

		protected override bool CanHandleDelta(swi.ManipulationDeltaEventArgs e)
		{
			var t = e.DeltaManipulation.Translation;
			return t.X != 0 || t.Y != 0;
		}

		protected override void HandleDelta(swi.ManipulationDeltaEventArgs e)
		{
			var t = e.DeltaManipulation.Translation;
			if (t.X == 0 && t.Y == 0)
				return;
			_translation = new PointF((float)t.X, (float)t.Y);
			var v = e.Velocities.LinearVelocity;
			_velocity = new PointF((float)(v.X * 1000.0), (float)(v.Y * 1000.0));
			Callback.OnActivated(Widget, EventArgs.Empty);
		}

		protected override void OnAttached(sw.UIElement element)
		{
			UpdateMouseHooks();
		}

		protected override void OnDetaching(sw.UIElement element)
		{
			DetachMouseEvents(element);
		}

		void UpdateMouseHooks()
		{
			var element = Element;
			if (_enableMousePan && element != null)
				AttachMouseEvents(element);
			else if (element != null)
				DetachMouseEvents(element);
		}

		void AttachMouseEvents(sw.UIElement element)
		{
			if (_mousePanHooked)
				return;
			element.PreviewMouseDown += OnMouseDown;
			element.PreviewMouseMove += OnMouseMove;
			element.PreviewMouseUp += OnMouseUp;
			element.LostMouseCapture += OnLostMouseCapture;
			_mousePanHooked = true;
		}

		void DetachMouseEvents(sw.UIElement element)
		{
			if (!_mousePanHooked)
				return;
			element.PreviewMouseDown -= OnMouseDown;
			element.PreviewMouseMove -= OnMouseMove;
			element.PreviewMouseUp -= OnMouseUp;
			element.LostMouseCapture -= OnLostMouseCapture;
			_mousePanHooked = false;
			CancelMousePan();
		}

		void CancelMousePan()
		{
			if (!_mousePanActive)
				return;
			_mousePanActive = false;
			Element?.ReleaseMouseCapture();
		}

		void OnMouseDown(object sender, swi.MouseButtonEventArgs e)
		{
			if (!Enabled || _mousePanActive)
				return;
			// Only start once the button that completes the required set is pressed, and all required buttons are down.
			if ((GetChangedButton(e) & _mouseButtons) == 0)
				return;
			if ((GetPressedButtons(e) & _mouseButtons) != _mouseButtons)
				return;
			var element = Element;
			if (element == null)
				return;
			_mousePanActive = true;
			_lastMousePosition = e.GetPosition(element);
			_lastMouseTime = DateTime.UtcNow;
			element.CaptureMouse();
		}

		void OnMouseMove(object sender, swi.MouseEventArgs e)
		{
			if (!Enabled || !_mousePanActive)
				return;
			var element = Element;
			if (element == null)
				return;
			var position = e.GetPosition(element);
			var dx = position.X - _lastMousePosition.X;
			var dy = position.Y - _lastMousePosition.Y;
			var now = DateTime.UtcNow;
			var dt = (now - _lastMouseTime).TotalSeconds;
			_lastMousePosition = position;
			_lastMouseTime = now;
			if (dx == 0 && dy == 0)
				return;
			_translation = new PointF((float)dx, (float)dy);
			_velocity = dt > 0 ? new PointF((float)(dx / dt), (float)(dy / dt)) : PointF.Empty;
			Callback.OnActivated(Widget, EventArgs.Empty);
		}

		void OnMouseUp(object sender, swi.MouseButtonEventArgs e)
		{
			// Releasing any one of the required buttons breaks the "all down" requirement and ends the pan.
			if (!_mousePanActive || (GetChangedButton(e) & _mouseButtons) == 0)
				return;
			_mousePanActive = false;
			Element?.ReleaseMouseCapture();
		}

		void OnLostMouseCapture(object sender, swi.MouseEventArgs e)
		{
			_mousePanActive = false;
		}

		static MouseButtons GetChangedButton(swi.MouseButtonEventArgs e)
		{
			switch (e.ChangedButton)
			{
				case swi.MouseButton.Left:
					return MouseButtons.Primary;
				case swi.MouseButton.Right:
					return MouseButtons.Alternate;
				case swi.MouseButton.Middle:
					return MouseButtons.Middle;
				default:
					return MouseButtons.None;
			}
		}

		static MouseButtons GetPressedButtons(swi.MouseEventArgs e)
		{
			var buttons = MouseButtons.None;
			if (e.LeftButton == swi.MouseButtonState.Pressed)
				buttons |= MouseButtons.Primary;
			if (e.RightButton == swi.MouseButtonState.Pressed)
				buttons |= MouseButtons.Alternate;
			if (e.MiddleButton == swi.MouseButtonState.Pressed)
				buttons |= MouseButtons.Middle;
			return buttons;
		}
	}

	public class ScrollGestureHandler : WidgetHandler<object, ScrollGesture>, ScrollGesture.IHandler, IWpfGestureHandler
	{
		const int WM_MOUSEHWHEEL = 0x020E;

		sw.UIElement _element;
		swin.HwndSource _source;
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

		Gesture IWpfGestureHandler.Gesture => Widget;

		public SizeF Delta => _delta;
		public PointF Velocity => _velocity;

		public void AttachTo(sw.UIElement element)
		{
			Detach();
			_element = element;
			_element.PreviewMouseWheel += OnPreviewMouseWheel;
			if (_element is sw.FrameworkElement frameworkElement)
			{
				frameworkElement.Loaded += OnLoaded;
				frameworkElement.Unloaded += OnUnloaded;
			}
			AddHook();
		}

		public void Detach()
		{
			if (_element == null)
				return;
			_element.PreviewMouseWheel -= OnPreviewMouseWheel;
			if (_element is sw.FrameworkElement frameworkElement)
			{
				frameworkElement.Loaded -= OnLoaded;
				frameworkElement.Unloaded -= OnUnloaded;
			}
			RemoveHook();
			_element = null;
		}

		void OnLoaded(object sender, sw.RoutedEventArgs e)
		{
			AddHook();
		}

		void OnUnloaded(object sender, sw.RoutedEventArgs e)
		{
			RemoveHook();
		}

		void AddHook()
		{
			if (_element == null || _source != null)
				return;
			_source = sw.PresentationSource.FromVisual(_element) as swin.HwndSource;
			_source?.AddHook(HwndHook);
		}

		void RemoveHook()
		{
			if (_source == null)
				return;
			_source.RemoveHook(HwndHook);
			_source = null;
		}

		IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			if (msg == WM_MOUSEHWHEEL && Enabled && _element?.IsMouseOver == true)
				ActivateIfAllowed(new SizeF(-GetWheelDelta(wParam) / WpfConversions.WheelDelta * Widget.WheelScrollAmount, 0));
			return IntPtr.Zero;
		}

		void OnPreviewMouseWheel(object sender, swi.MouseWheelEventArgs e)
		{
			if (Enabled)
				ActivateIfAllowed(new SizeF(0, (float)e.Delta / WpfConversions.WheelDelta * Widget.WheelScrollAmount));
		}

		void ActivateIfAllowed(SizeF delta)
		{
			if (!CanRecognize(delta))
				return;
			Activate(delta);
		}

		bool CanRecognize(SizeF delta)
		{
			var control = Widget.Control;
			if (control == null)
				return true;

			foreach (var gesture in control.Gestures)
			{
				if (((IHandlerSource)gesture).Handler is IWpfGestureHandler handler && handler.CanHandleWheel(delta))
					return handler.Gesture == Widget || handler.Gesture.CanRecognizeSimultaneouslyWith(Widget);
			}
			return true;
		}

		void Activate(SizeF delta)
		{
			if (delta.IsZero)
				return;

			var now = DateTime.UtcNow;
			var dt = _previousTime == default ? 0 : (now - _previousTime).TotalSeconds;
			_previousTime = now;

			_delta = delta;
			_velocity = dt > 0 ? new PointF((float)(delta.Width / dt), (float)(delta.Height / dt)) : PointF.Empty;
			Callback.OnActivated(Widget, EventArgs.Empty);
		}

		static int GetWheelDelta(IntPtr wParam)
		{
			return (short)(((long)wParam >> 16) & 0xFFFF);
		}

		bool IWpfGestureHandler.CanHandleDelta(swi.ManipulationDeltaEventArgs e)
		{
			return false;
		}

		bool IWpfGestureHandler.CanHandleWheel(SizeF delta)
		{
			// Defer Ctrl+Wheel to gestures that claim it (e.g. MagnificationGesture for precision touchpad pinches).
			if ((swi.Keyboard.Modifiers & swi.ModifierKeys.Control) == swi.ModifierKeys.Control)
				return false;
			return Enabled && !delta.IsZero;
		}
	}
}
