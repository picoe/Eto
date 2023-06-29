namespace Eto.Wpf.Forms.Controls
{
	public class StepperHandler : WpfControl<xwt.ButtonSpinner, Stepper, Stepper.ICallback>, Stepper.IHandler
	{
		double originalWidth;
		swc.Grid gridContent;

		public StepperHandler()
		{
			Control = new xwt.ButtonSpinner();
			Control.BorderThickness = new sw.Thickness(0);
			Control.Padding = new sw.Thickness(0);
			Control.Loaded += Control_Loaded;
		}

		void Control_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
			// when the control's size is set, grow the buttons instead of showing the entry area
			var parentGrid = Control.FindChild<swc.Grid>(string.Empty);
			if (parentGrid == null)
				return;
			parentGrid.ColumnDefinitions[0].Width = new sw.GridLength(0);
			parentGrid.ColumnDefinitions[1].Width = new sw.GridLength(1, sw.GridUnitType.Star);

			gridContent = parentGrid.FindChild<swc.Grid>("gridContent");
			if (gridContent == null)
				return;
			gridContent.MinWidth = originalWidth = double.IsNaN(gridContent.Width) ? 0 : gridContent.Width;
			gridContent.Width = double.NaN;
		}

		protected override void SetSize()
		{
			base.SetSize();
			if (gridContent != null)
			{
				gridContent.MinWidth = double.IsNaN(UserPreferredSize.Width) ? originalWidth : 0;
			}
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
					Control.Spin += (sender, e) => Callback.OnStep(Widget, new StepperEventArgs(e.Direction == Xceed.Wpf.Toolkit.SpinDirection.Increase ? StepperDirection.Up : StepperDirection.Down));
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}
	}
}
