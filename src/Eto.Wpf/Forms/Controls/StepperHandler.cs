namespace Eto.Wpf.Forms.Controls
{
	public class StepperHandler : WpfControl<EtoButtonSpinner, Stepper, Stepper.ICallback>, Stepper.IHandler
	{
		public StepperHandler()
		{
			Control = new EtoButtonSpinner
			{
				ShowContentArea = false
			};
		}

		public StepperValidDirections ValidDirection
		{
			get { return Control.ValidSpinDirection.ToEto(); }
			set { Control.ValidSpinDirection = value.ToWpf(); }
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case Stepper.StepEvent:
					Control.Spin += (sender, e) => Callback.OnStep(Widget, new StepperEventArgs(e.Direction == SpinDirection.Increase ? StepperDirection.Up : StepperDirection.Down));
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}
	}
}
