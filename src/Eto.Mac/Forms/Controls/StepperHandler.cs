using System;
using Eto.Forms;
using Eto.Drawing;


namespace Eto.Mac.Forms.Controls
{
	public class StepperHandler : MacControl<NSStepper, Stepper, Stepper.ICallback>, Stepper.IHandler
	{
		public class EtoStepper : NSStepper, IMacControl
		{
			public WeakReference WeakHandler { get; set; }

			public EtoStepper()
			{
				MinValue = 0;
				MaxValue = 2;
			}
		}

		protected override bool DefaultUseAlignmentFrame => true;

		protected override NSStepper CreateControl() => new EtoStepper();

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
			SetEnabled();
		}

		protected override bool ControlEnabled
		{
			get => base.ControlEnabled;
			set => base.ControlEnabled = value && ValidDirection != StepperValidDirections.None;
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
					Control.Activated += Control_Activated;
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		static void Control_Activated(object sender, EventArgs e)
		{
			var h = GetHandler(sender) as StepperHandler;
			if (h == null)
				return;
			var dir = h.GetDirection();
			if (dir != null)
				h.Callback.OnStep(h.Widget, new StepperEventArgs(dir.Value));
		}
	}
}
