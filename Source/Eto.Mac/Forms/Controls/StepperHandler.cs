﻿using System;
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

		static object ValidDirection_Key = new object();

		public StepperValidDirections ValidDirection
		{
			get { return Widget.Properties.Get(ValidDirection_Key, StepperValidDirections.Both); }
			set { Widget.Properties.Set(ValidDirection_Key, value, UpdateState, StepperValidDirections.Both); }
		}

		void UpdateState()
		{
			switch (ValidDirection)
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
			Control.Enabled = Enabled && ValidDirection != StepperValidDirections.None;
		}

		StepperDirection? GetDirection()
		{
			switch (ValidDirection)
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

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case Stepper.StepEvent:
					Control.Activated += (sender, e) =>
					{
						var dir = GetDirection();
						if (dir != null)
							Callback.OnStep(Widget, new StepperEventArgs(dir.Value));
					};
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}
	}
}
