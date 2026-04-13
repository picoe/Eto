namespace Eto.Forms.ThemedControls;

/// <summary>
/// Themed implementation of the <see cref="NumericStepper"/> control using a <see cref="NumericMaskedTextStepper{T}"/>.
/// </summary>
public class ThemedNumericStepperHandler : ThemedControlHandler<NumericMaskedTextStepper<double>, NumericStepper, NumericStepper.ICallback>, NumericStepper.IHandler
{

	/// <summary>
	/// Initializes a new instance of the <see cref="ThemedNumericStepperHandler"/> class.
	/// </summary>
	public ThemedNumericStepperHandler()
	{
		Control = new NumericMaskedTextStepper<double>();
		Control.MaximumDecimalPlaces = 0;
		Control.ValueChanged += (sender, e) => Callback.OnValueChanged(Widget, EventArgs.Empty);
	}



	/// <inheritdoc/>
	public override void AttachEvent(string id)
	{
		switch (id)
		{
			default:
				base.AttachEvent(id);
				break;
		}
	}

	/// <inheritdoc/>
	public bool ReadOnly
	{
		get => Control.ReadOnly;
		set => Control.ReadOnly = value;
	}

	/// <inheritdoc/>
	public double Value
	{
		get => Control.Value;
		set => Control.Value = value;
	}

	/// <inheritdoc/>
	public double MinValue
	{
		get => Control.MinValue;
		set => Control.MinValue = value;
	}

	/// <inheritdoc/>
	public double MaxValue
	{
		get => Control.MaxValue;
		set => Control.MaxValue = value;
	}


	/// <inheritdoc/>
	public int DecimalPlaces
	{
		get => Control.DecimalPlaces;
		set => Control.DecimalPlaces = value;
	}

	/// <inheritdoc/>
	public double Increment
	{
		get => Control.Increment;
		set => Control.Increment = value;
	}

	/// <inheritdoc/>
	public Color TextColor
	{
		get => Control.TextColor;
		set => Control.TextColor = value;
	}

	/// <inheritdoc/>
	public int MaximumDecimalPlaces
	{
		get => Control.MaximumDecimalPlaces;
		set => Control.MaximumDecimalPlaces = value;
	}

	bool HasFormatString => !string.IsNullOrEmpty(FormatString);

	/// <inheritdoc/>
	public string FormatString
	{
		get => Control.FormatString;
		set => Control.FormatString = value;
	}

	/// <inheritdoc/>
	public CultureInfo CultureInfo
	{
		get => Control.Culture;
		set => Control.Culture = value;
	}

	/// <inheritdoc/>
	public bool Wrap
	{
		get => Control.Wrap;
		set => Control.Wrap = value;
	}
	
	/// <inheritdoc/>
	public TextAlignment TextAlignment
	{
		get => Control.TextAlignment;
		set => Control.TextAlignment = value;
	}

	/// <inheritdoc/>
	public Font Font
	{
		get => Control.Font;
		set => Control.Font = value;
	}


	/// <inheritdoc/>
	protected override Control KeyboardControl => Control;

	/// <inheritdoc/>
	public override void Focus() => Control.Focus();

	/// <inheritdoc/>
	public override bool HasFocus => base.HasFocus || Control.HasFocus;
}
