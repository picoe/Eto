using System.Runtime.CompilerServices;

namespace Eto.Mac.Forms.Controls
{
	interface IMacGestureHandler : Gesture.IHandler
	{
		Gesture Gesture { get; }
		NSGestureRecognizer Recognizer { get; }
	}

	interface IMacGestureActivationHandler
	{
		void OnActivated(NSGestureRecognizer recognizer);
	}

	interface IMacScrollWheelGestureHandler
	{
		Gesture Gesture { get; }
		bool OnScrollWheel(NSEvent theEvent);
	}

	class GestureRecognizerTarget : NSObject
	{
		WeakReference handler;

		public static readonly Selector ActivatedSelector = new Selector("activated:");

		public GestureRecognizerTarget(IMacGestureActivationHandler handler)
		{
			this.handler = new WeakReference(handler);
		}

		[Export("activated:")]
		public void Activated(NSGestureRecognizer recognizer)
		{
			(handler.Target as IMacGestureActivationHandler)?.OnActivated(recognizer);
		}
	}

	class EtoGestureRecognizerDelegate : NSGestureRecognizerDelegate
	{
		public static readonly EtoGestureRecognizerDelegate Instance = new EtoGestureRecognizerDelegate();
		readonly ConditionalWeakTable<NSGestureRecognizer, IMacGestureHandler> handlers = new ConditionalWeakTable<NSGestureRecognizer, IMacGestureHandler>();

		public void Register(NSGestureRecognizer recognizer, IMacGestureHandler handler)
		{
			handlers.Remove(recognizer);
			handlers.Add(recognizer, handler);
		}

		public override bool ShouldRecognizeSimultaneously(NSGestureRecognizer gestureRecognizer, NSGestureRecognizer otherGestureRecognizer)
		{
			return gestureRecognizer != null
				&& otherGestureRecognizer != null
				&& handlers.TryGetValue(gestureRecognizer, out var gestureHandler)
				&& handlers.TryGetValue(otherGestureRecognizer, out var otherGestureHandler)
				&& gestureHandler.Gesture.CanRecognizeSimultaneouslyWith(otherGestureHandler.Gesture);
		}
	}

	public abstract class GestureHandler<TControl, TWidget> : WidgetHandler<TControl, TWidget>, IMacGestureHandler, IMacGestureActivationHandler
		where TControl : NSGestureRecognizer
		where TWidget : Gesture
	{
		GestureRecognizerTarget target;

		protected new Gesture.ICallback Callback => (Gesture.ICallback)((ICallbackSource)Widget).Callback;

		protected NSObject Target => target ??= new GestureRecognizerTarget(this);

		public bool Enabled
		{
			get => Control.Enabled;
			set => Control.Enabled = value;
		}

		protected override void Initialize()
		{
			base.Initialize();
			Control.Target = Target;
			Control.Action = GestureRecognizerTarget.ActivatedSelector;
			Control.Delegate = EtoGestureRecognizerDelegate.Instance;
			EtoGestureRecognizerDelegate.Instance.Register(Control, this);
		}

		Gesture IMacGestureHandler.Gesture => Widget;

		NSGestureRecognizer IMacGestureHandler.Recognizer => Control;

		protected virtual void OnActivated(NSGestureRecognizer recognizer)
		{
			Callback.OnActivated(Widget, EventArgs.Empty);
		}

		void IMacGestureActivationHandler.OnActivated(NSGestureRecognizer recognizer) => OnActivated(recognizer);
	}

	public class MagnificationGestureHandler : GestureHandler<NSMagnificationGestureRecognizer, MagnificationGesture>, MagnificationGesture.IHandler
	{
		public float Magnification => (float)Control.Magnification;

		protected override NSMagnificationGestureRecognizer CreateControl()
		{
			return new NSMagnificationGestureRecognizer();
		}

		protected override void OnActivated(NSGestureRecognizer recognizer)
		{
			base.OnActivated(recognizer);
			if (Control.State == NSGestureRecognizerState.Changed)
			{
				Control.Magnification = 0; // reset magnification so we get the delta each time
			}
			
		}
	}

	public class RotationGestureHandler : GestureHandler<NSRotationGestureRecognizer, RotationGesture>, RotationGesture.IHandler
	{
		// NSRotationGestureRecognizer reports radians counter-clockwise; expose degrees clockwise.
		public float Rotation => (float)(-Control.Rotation * 180.0 / Math.PI);

		protected override NSRotationGestureRecognizer CreateControl()
		{
			return new NSRotationGestureRecognizer();
		}

		protected override void OnActivated(NSGestureRecognizer recognizer)
		{
			base.OnActivated(recognizer);
			if (Control.State == NSGestureRecognizerState.Changed)
				Control.Rotation = 0; // reset so we get the delta each time
		}
	}

	public class PanGestureHandler : GestureHandler<NSPanGestureRecognizer, PanGesture>, PanGesture.IHandler
	{
		MouseButtons _mouseButtons = MouseButtons.Primary;

		NSView View => Control.View;

		public PointF Translation => ToEtoVector(Control.TranslationInView(View), View);

		public PointF Velocity => ToEtoVector(Control.VelocityInView(View), View);
		public MouseButtons Buttons
		{
			get => _mouseButtons;
			set
			{
				if (value == MouseButtons.None)
					throw new ArgumentException($"{nameof(Buttons)} cannot be {nameof(MouseButtons.None)}.", nameof(value));
				_mouseButtons = value;
				ApplyButtonMask();
			}
		}

		protected override NSPanGestureRecognizer CreateControl()
		{
			return new NSPanGestureRecognizer();
		}

		protected override void Initialize()
		{
			base.Initialize();
			ApplyButtonMask();
		}

		protected override void OnActivated(NSGestureRecognizer recognizer)
		{
			if (!IsMouseButtonMatched())
			{
				Control.SetTranslation(CGPoint.Empty, View);
				return;
			}

			base.OnActivated(recognizer);
			if (Control.State == NSGestureRecognizerState.Changed)
				Control.SetTranslation(CGPoint.Empty, View);
		}

		bool IsMouseButtonMatched()
		{
			return (Mouse.Buttons & _mouseButtons) == _mouseButtons;
		}

		void ApplyButtonMask()
		{
			Control.ButtonMask = ToButtonMask(_mouseButtons);
		}

		static nuint ToButtonMask(MouseButtons buttons)
		{
			nuint mask = 0;
			if ((buttons & MouseButtons.Primary) != 0)
				mask |= 1;
			if ((buttons & MouseButtons.Alternate) != 0)
				mask |= 2;
			if ((buttons & MouseButtons.Middle) != 0)
				mask |= 4;
			return mask;
		}

		static PointF ToEtoVector(CGPoint vector, NSView view)
		{
			var result = vector.ToEto();
			if (view?.IsFlipped == false)
				result.Y = -result.Y;
			return result;
		}
	}

	public class ScrollGestureHandler : WidgetHandler<object, ScrollGesture>, ScrollGesture.IHandler, IMacScrollWheelGestureHandler
	{
		bool enabled = true;
		SizeF delta;
		PointF velocity;
		double lastScrollTimestamp;

		protected new Gesture.ICallback Callback => (Gesture.ICallback)((ICallbackSource)Widget).Callback;

		public bool Enabled
		{
			get => enabled;
			set => enabled = value;
		}

		public SizeF Delta => delta;

		public PointF Velocity => velocity;

		Gesture IMacScrollWheelGestureHandler.Gesture => Widget;

		public bool OnScrollWheel(NSEvent theEvent)
		{
			if (!Enabled)
				return false;

			delta = theEvent.HasPreciseScrollingDeltas
				? new SizeF((float)theEvent.ScrollingDeltaX, (float)theEvent.ScrollingDeltaY)
				: new SizeF((float)theEvent.DeltaX * ScrollGesture.DefaultWheelScrollAmount / Widget.WheelScrollAmount, (float)theEvent.DeltaY * ScrollGesture.DefaultWheelScrollAmount / Widget.WheelScrollAmount);

			var timestamp = theEvent.Timestamp;
			var phase = theEvent.Phase;
			if (phase == NSEventPhase.Began || lastScrollTimestamp <= 0 || timestamp <= lastScrollTimestamp)
				velocity = PointF.Empty;
			else
			{
				var elapsed = timestamp - lastScrollTimestamp;
				velocity = new PointF((float)(delta.Width / elapsed), (float)(delta.Height / elapsed));
			}
			lastScrollTimestamp = timestamp;

			if (!delta.IsZero)
				Callback.OnActivated(Widget, EventArgs.Empty);

			if (phase == NSEventPhase.Ended || phase == NSEventPhase.Cancelled)
			{
				lastScrollTimestamp = 0;
				velocity = PointF.Empty;
			}
			return true;
		}
	}
}
