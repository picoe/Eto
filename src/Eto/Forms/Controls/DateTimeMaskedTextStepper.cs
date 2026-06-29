namespace Eto.Forms;

/// <summary>
/// Masked text stepper used to edit date and/or time values with a fixed culture-aware format.
/// </summary>
/// <remarks>
/// The date/time masking, parsing, and stepping logic is provided by <see cref="DateTimeMaskedTextProvider"/>.
/// </remarks>
public class DateTimeMaskedTextStepper : MaskedTextStepper<DateTime?>
{
	readonly DateTimeMaskedTextProvider provider;

	/// <summary>
	/// Initializes a new instance of the <see cref="DateTimeMaskedTextStepper"/> class.
	/// </summary>
	public DateTimeMaskedTextStepper()
	{
		ShowPromptMode = ShowPromptMode.OnFocus;
		InsertMode = InsertKeyMode.Overwrite;
		AutoSelectEditableRanges = true;
		AutoSelectMode = AutoSelectMode.Never;
		ShowPlaceholderWhenEmpty = true;
		provider = new DateTimeMaskedTextProvider();
		Provider = provider;
		ValidDirection = StepperValidDirections.Both;
		HandleEvent(StepEvent);
		HandleEvent(TextChangedEvent);
		HandleEvent(LostFocusEvent);
	}

	/// <summary>
	/// Gets or sets the minimum allowed date/time value.
	/// </summary>
	public DateTime MinDate
	{
		get => provider.MinDate;
		set
		{
			provider.MinDate = value;
			UpdateText();
		}
	}

	/// <summary>
	/// Gets or sets the maximum allowed date/time value.
	/// </summary>
	public DateTime MaxDate
	{
		get => provider.MaxDate;
		set
		{
			provider.MaxDate = value;
			UpdateText();
		}
	}

	/// <summary>
	/// Gets or sets the prompt character used for editing.
	/// </summary>
	public char PromptChar
	{
		get => provider.PromptChar;
		set
		{
			provider.PromptChar = value;
			UpdateText();
		}
	}

	/// <summary>
	/// Gets or sets the visible date/time editing mode.
	/// </summary>
	public DateTimePickerMode Mode
	{
		get => provider.Mode;
		set
		{
			provider.Mode = value;
			UpdateText();
		}
	}

	void UpdateValidDirection()
	{
		ValidDirection = ReadOnly || !Enabled ? StepperValidDirections.None : StepperValidDirections.Both;
	}

	/// <inheritdoc/>
	protected override void OnReadOnlyChanged(EventArgs e)
	{
		base.OnReadOnlyChanged(e);
		UpdateValidDirection();
	}

	/// <inheritdoc/>
	protected override void OnStep(StepperEventArgs e)
	{
		base.OnStep(e);
		if (ReadOnly || !Enabled)
			return;

		var delta = e.Direction == StepperDirection.Up ? 1 : -1;
		Value = provider.GetStepValue(CaretIndex, delta, out var segmentStart);
		SelectEditableRangeAt(segmentStart, forward: true);
	}
}