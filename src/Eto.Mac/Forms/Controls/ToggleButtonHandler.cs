namespace Eto.Mac.Forms.Controls
{
	public class ToggleButtonHandler : ButtonHandler<ToggleButton, ToggleButton.ICallback>, ToggleButton.IHandler
	{
		static Size s_defaultMinimumSize;

		protected override Size DefaultMinimumSize => s_defaultMinimumSize;

		protected override NSButtonType DefaultButtonType => NSButtonType.PushOnPushOff;

		static ToggleButtonHandler()
		{
			// store the normal size for a rounded button, so we can determine what style to give it based on actual size
			var b = new EtoButton(NSButtonType.PushOnPushOff);
			s_defaultMinimumSize = b.GetAlignmentRectForFrame(new CGRect(CGPoint.Empty, b.FittingSize)).Size.ToEtoSize();
		}

		protected override void SetImagePosition()
		{
			if (UserPreferredSize.Width == -1)
			{
				var position = ImagePosition.ToNS();
				if (string.IsNullOrEmpty(Text))
					position = NSCellImagePosition.ImageOnly;
				Control.ImagePosition = position;
				InvalidateMeasure();
			}
			else
				base.SetImagePosition();
		}

		public bool Checked
		{
			get => Control.State == NSCellStateValue.On;
			set
			{
				if (value != Checked)
				{
					Control.State = value ? NSCellStateValue.On : NSCellStateValue.Off;
					Callback.OnCheckedChanged(Widget, EventArgs.Empty);
				}
			}
		}

		protected override void OnActivated()
		{
			TriggerMouseCallback();
			Callback.OnCheckedChanged(Widget, EventArgs.Empty);
			Callback.OnClick(Widget, EventArgs.Empty);
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case ToggleButton.CheckedChangedEvent:
					// handled intrinsically
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}
	}
}
