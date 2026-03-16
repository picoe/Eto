namespace Eto.Wpf.Forms.Controls
{
	public class TextStepperHandler : TextBoxHandler<EtoButtonSpinner, TextStepper, TextStepper.ICallback>, TextStepper.IHandler
	{
		public TextStepperHandler()
		{
			Control = new EtoButtonSpinner
			{
				Handler = this,
				IsTabStop = false,
				Focusable = false,

				Content = new EtoWatermarkTextBox
				{
					MinHeight = 0,
					KeepWatermarkOnGotFocus = true,
					BorderThickness = new sw.Thickness(0),
					BorderBrush = null,
					Background = null,
					Padding = new sw.Thickness(0),
				},
			};
		}

		public override sw.FrameworkElement TabControl => WatermarkTextBox;

		public override string PlaceholderText
		{
			get { return WatermarkTextBox.Watermark as string; }
			set { WatermarkTextBox.Watermark = value; }
		}

		public StepperValidDirections ValidDirection
		{
			get { return Control.ValidSpinDirection.ToEto(); }
			set { Control.ValidSpinDirection = value.ToWpf(); }
		}

		public bool ShowStepper
		{
			get { return Control.ShowButtonSpinner; }
			set { Control.ShowButtonSpinner = value; }
		}

		public override Color TextColor
		{
			get { return TextBox.Foreground.ToEtoColor(); }
			set { TextBox.Foreground = value.ToWpfBrush(TextBox.Foreground); }
		}

		EtoWatermarkTextBox WatermarkTextBox => (EtoWatermarkTextBox)Control.Content;

		protected override swc.TextBox TextBox => (swc.TextBox)Control.Content;

		protected override swc.Control BorderControl => Control;

		protected override sw.FrameworkElement FocusControl => TextBox;

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case TextStepper.StepEvent:
					Control.Spin += (sender, e) =>
					{
						Callback.OnStep(Widget, new StepperEventArgs(e.Direction == SpinDirection.Increase ? StepperDirection.Up : StepperDirection.Down));
					};
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}
	}
}
