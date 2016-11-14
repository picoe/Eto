using System;
using Eto.Forms;
using Eto.Drawing;

#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
#if Mac64
using nfloat = System.Double;
using nint = System.Int64;
using nuint = System.UInt64;
#else
using nfloat = System.Single;
using nint = System.Int32;
using nuint = System.UInt32;
#endif
#if SDCOMPAT
using CGSize = System.Drawing.SizeF;
using CGRect = System.Drawing.RectangleF;
using CGPoint = System.Drawing.PointF;
#endif
#endif

namespace Eto.Mac.Forms.Controls
{
	public class MacViewHandler2<TWidget, TControl> : WidgetHandler2<TWidget, TControl>, IMacControlHandler2
		where TWidget : Control
		where TControl : NSView, new()
	{
		public virtual NSView GetContainerControl(Widget widget)
		{
			return GetControl((TWidget)widget);
		}

		public SizeF GetPreferredSize(Widget widget, SizeF availableSize)
		{
			return SizeF.Empty;
		}
	}

	public class StepperHandler : MacControl<NSStepper, Stepper, Stepper.ICallback>, Stepper.IHandler
	{
		public class EtoStepper : NSStepper, IMacControl
		{
			public WeakReference WeakHandler { get; set; }
		}

		public StepperHandler()
		{
			Control = new EtoStepper
			{
				WeakHandler = new WeakReference(this),
				MinValue = 0,
				MaxValue = 2
			};
		}

		void UpdateState()
		{
			switch (Widget.ValidDirection)
			{
				case StepperValidDirections.Both:
					Control.ValueWraps = true;
					Control.MaxValue = 2;
					Control.IntValue = 0;
					break;
				case StepperValidDirections.Up:
					Control.ValueWraps = false;
					Control.MaxValue = 1;
					Control.IntValue = 0;
					break;
				case StepperValidDirections.Down:
					Control.ValueWraps = false;
					Control.MaxValue = 1;
					Control.IntValue = 1;
					break;
				case StepperValidDirections.None:
					Control.ValueWraps = false;
					Control.MaxValue = 0;
					break;
			}
			SetStepperEnabled();
		}

		static object Enabled_Key = new object();

		public override bool Enabled
		{
			get { return Widget.Properties.Get(Enabled_Key, true); }
			set { Widget.Properties.Set(Enabled_Key, value, SetStepperEnabled, true); }
		}

		void SetStepperEnabled()
		{
			Control.Enabled = Enabled && Widget.ValidDirection != StepperValidDirections.None;
		}

		StepperDirection? GetDirection()
		{
			switch (Widget.ValidDirection)
			{
				case StepperValidDirections.Both:
					var dir = Control.IntValue == 1 ? StepperDirection.Up : StepperDirection.Down;
					Control.IntValue = 0;
					return dir;
				case StepperValidDirections.Up:
					if (Control.IntValue == 1)
					{
						Control.IntValue = 0;
						return StepperDirection.Up;
					}
					break;
				case StepperValidDirections.Down:
					if (Control.IntValue == 0)
					{
						Control.IntValue = 1;
						return StepperDirection.Down;
					}
					break;
			}
			return null;
		}

		public override void Initialize(Stepper widget)
		{
			base.Initialize(widget);
			GetControl(widget).SizeToFit();
		}

		public override Action<Stepper, object> SetProperty(object property)
		{
			if (property == Stepper.ValidDirectionProperty)
				return SetValidDirection;

			return base.SetProperty(property);
		}

		static void SetValidDirection(Stepper c, object value)
		{
			(c.Handler as StepperHandler)?.UpdateState();
		}

		public override Action<Stepper> GetEvent(object evt)
		{
			if (evt == Stepper.StepEvent)
				return AttachStepEvent;

			return base.GetEvent(evt);
		}

		static void AttachStepEvent(Stepper c)
		{
			var h = c.Handler as StepperHandler;
			h.Control.Activated += (sender, e) =>
			{
				var dir = h.GetDirection();
				if (dir != null)
					Stepper.StepEvent.Raise(c, new StepperEventArgs(dir.Value));
			};
		}
	}
}