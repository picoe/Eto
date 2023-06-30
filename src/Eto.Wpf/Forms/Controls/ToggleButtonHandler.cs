namespace Eto.Wpf.Forms.Controls
{
	public class EtoToggleButton : swcp.ToggleButton, IEtoWpfControl
	{
		public IWpfFrameworkElement Handler { get; set; }

		protected override sw.Size MeasureOverride(sw.Size constraint)
		{
			return Handler?.MeasureOverride(constraint, base.MeasureOverride) ?? base.MeasureOverride(constraint);
		}
	}

	public class ToggleButtonHandler : ButtonHandler<EtoToggleButton, ToggleButton, ToggleButton.ICallback>, ToggleButton.IHandler
	{
		protected override EtoToggleButton CreateControl() => new EtoToggleButton { Handler = this };

		protected override Size GetDefaultMinimumSize() => new Size(23, 23);

		public bool Checked
		{
			get => Control.IsChecked ?? false;
			set => Control.IsChecked = value;
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case ToggleButton.CheckedChangedEvent:
					AttachPropertyChanged(EtoToggleButton.IsCheckedProperty, OnCheckedChanged);
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		private void OnCheckedChanged(object sender, sw.DependencyPropertyChangedEventArgs e)
		{
			Callback.OnCheckedChanged(Widget, EventArgs.Empty);
		}
	}
}
