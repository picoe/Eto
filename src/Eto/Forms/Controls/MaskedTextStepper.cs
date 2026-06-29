
namespace Eto.Forms;


/// <summary>
/// Masked text box that provides a value converted to/from text
/// </summary>
/// <remarks>
/// This is useful when the text can be converted to another type (e.g. DateTime, numeric, etc).
///
/// The <see cref="Provider"/> specified for the control is responsible for converting the value.
/// </remarks>
public class MaskedTextStepper<T> : MaskedTextStepper
{
	T _lastValue;
	/// <summary>
	/// Event to handle when the <see cref="Value"/> property changes
	/// </summary>
	public event EventHandler<EventArgs> ValueChanged;

	/// <inheritdoc/>
	override protected void OnTextChanged(EventArgs e)
	{
		base.OnTextChanged(e);

		var val = Value;
		if (!EqualityComparer<T>.Default.Equals(val, _lastValue))
		{
			OnValueChanged(EventArgs.Empty);
			_lastValue = val;
		}
	}

	/// <summary>
	/// Raises the <see cref="ValueChanged"/> event.
	/// </summary>
	/// <param name="e">Event arguments.</param>
	protected virtual void OnValueChanged(EventArgs e)
	{
		ValueChanged?.Invoke(this, e);
	}

	/// <summary>
	/// Gets or sets the provider for the text box
	/// </summary>
	/// <value>The provider.</value>
	public new IMaskedTextProvider<T> Provider
	{
		get { return base.Provider as IMaskedTextProvider<T>; }
		set { base.Provider = value; }
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="Forms.MaskedTextStepper{T}"/> class.
	/// </summary>
	public MaskedTextStepper()
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="Forms.MaskedTextStepper{T}"/> class with the specified masked text provider.
	/// </summary>
	/// <param name="provider">Masked text provider to format the mask.</param>
	public MaskedTextStepper(IMaskedTextProvider<T> provider)
		: base(provider)
	{
	}

	/// <summary>
	/// Gets or sets the translated value of the masked text.
	/// </summary>
	/// <value>The translated value.</value>
	public virtual T Value
	{
		get { return Provider != null ? Provider.Value : default(T); }
		set
		{
			if (Provider != null)
				Provider.Value = value;
			// if (HasFocus)
			CommitText();
		}
	}

	/// <summary>
	/// Gets a binding for the <see cref="Value"/> property.
	/// </summary>
	/// <value>The value binding.</value>
	public BindableBinding<MaskedTextStepper<T>, T> ValueBinding
	{
		get
		{
			return new BindableBinding<MaskedTextStepper<T>, T>(
				this,
				c => c.Value,
				(c, v) => c.Value = v,
				(c, eh) => c.ValueChanged += eh,
				(c, eh) => c.ValueChanged -= eh
			);
		}
	}
}

/// <summary>
/// Text box with masking capabilities.
/// </summary>
/// <remarks>
/// This uses the <see cref="IMaskedTextProvider"/> as its interface to the mask.
/// The mask can implement any format it wishes, including both fixed or variable length masks.
/// The MaskedTextStepper allows you to mask, or limit which characters can be entered in the text box with either a fixed, variable, or custom mask.
/// A fixed mask can be a phone number, postal code, or something that requires a specific format and can be created using the <see cref="FixedMaskedTextProvider"/>.
/// A variable mask limits which characters can be entered but is not limited to a fixed number of characters.
/// An implementation of a variable mask is the <see cref="NumericMaskedTextBox{T}"/> which allows you to enter only numeric values in a text box, and places the positive / negative symbol at the beginning regardless of where you type it.
/// </remarks>
[ContentProperty("Provider")]
public class MaskedTextStepper : TextStepper, IMaskedTextControl
{
	readonly MaskedTextBoxLogic _logic;

	string IMaskedTextControl.BaseText
	{
		get => base.Text;
		set => base.Text = value;
	}

	void IMaskedTextControl.UpdateText() => UpdateText();

	void IMaskedTextControl.CommitText() => CommitText();

	/// <summary>
	/// Gets or sets the masked text provider to specify the mask format.
	/// </summary>
	/// <value>The masked text provider.</value>
	public IMaskedTextProvider Provider
	{
		get => _logic.Provider;
		set => _logic.Provider = value;
	}

	/// <summary>
	/// Gets or sets the mode for insertion. Use <see cref="IsOverwrite"/> to determine the current mode.
	/// </summary>
	/// <value>The desired insert mode.</value>
	public InsertKeyMode InsertMode
	{
		get => _logic.InsertMode;
		set => _logic.InsertMode = value;
	}

	/// <summary>
	/// Gets a value indicating whether typing will overwrite text.
	/// </summary>
	/// <seealso cref="InsertMode"/>
	/// <value><c>true</c> if text will be overwritten; otherwise, <c>false</c> to insert text.</value>
	public bool IsOverwrite => _logic.IsOverwrite;

	/// <summary>
	/// Gets or sets a value indicating that the prompt characters should only be shown when the control has focus.
	/// </summary>
	/// <value><c>true</c> if to show the prompt only when focussed; otherwise, <c>false</c>.</value>
	[Obsolete("Since 2.5.1, Use ShowPromptMode instead")]
	public bool ShowPromptOnFocus
	{
		get => ShowPromptMode == ShowPromptMode.OnFocus;
		set => ShowPromptMode = value ? ShowPromptMode.OnFocus : ShowPromptMode.Always;
	}

	/// <summary>
	/// Gets or sets the mode for when the input prompts should be shown
	/// </summary>
	public ShowPromptMode ShowPromptMode
	{
		get => _logic.ShowPromptMode;
		set => _logic.ShowPromptMode = value;
	}

	/// <summary>
	/// Gets or sets a value indicating that the placeholder should be shown when the mask is empty and the control does
	/// not have focus.
	/// </summary>
	/// <value><c>true</c> to show the placeholder when its value is empty; otherwise, <c>false</c>.</value>
	[DefaultValue(true)]
	public bool ShowPlaceholderWhenEmpty
	{
		get => _logic.ShowPlaceholderWhenEmpty;
		set => _logic.ShowPlaceholderWhenEmpty = value;
	}

	/// <summary>
	/// Gets or sets a value indicating whether editable mask ranges should be selected automatically.
	/// </summary>
	/// <remarks>
	/// When enabled, the control selects contiguous editable regions from the mask and allows the user
	/// to move between them using the left and right arrow keys.
	/// </remarks>
	[DefaultValue(false)]
	public bool AutoSelectEditableRanges
	{
		get => _logic.AutoSelectEditableRanges;
		set => _logic.AutoSelectEditableRanges = value;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="MaskedTextStepper"/> class.
	/// </summary>
	public MaskedTextStepper()
	{
		_logic = new MaskedTextBoxLogic(this);
		HandleEvent(TextChangingEvent);
		HandleEvent(TextChangedEvent);
		HandleEvent(KeyDownEvent);
		HandleEvent(GotFocusEvent);
		HandleEvent(LostFocusEvent);
		HandleEvent(StepEvent);
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="MaskedTextStepper"/> class with the specified masked text provider.
	/// </summary>
	/// <param name="provider">Masked text provider to specify the format of the mask.</param>
	public MaskedTextStepper(IMaskedTextProvider provider)
		: this()
	{
		if (provider == null)
			throw new ArgumentNullException(nameof(provider));
		_logic.Provider = provider;
	}

	/// <summary>
	/// Updates the text to the display text from the provider.
	/// </summary>
	/// <remarks>
	/// Call this in a subclass when you want to update the text based on the state of the control.
	/// When the <see cref="IMaskedTextProvider.IsEmpty"/> is true, it will set the text to null to show the placeholder text.
	///
	/// Override this to perform other actions before or after the text of the control is updated.
	/// </remarks>
	protected virtual void UpdateText() => _logic.UpdateText();

	/// <summary>
	/// Commits the text to the provider and updates the display text.
	/// </summary>
	protected virtual void CommitText() => _logic.CommitText();

	/// <summary>
	/// Raises the <see cref="Control.LoadComplete"/> event.
	/// </summary>
	/// <param name="e">Event arguments</param>
	protected override void OnLoadComplete(EventArgs e)
	{
		base.OnLoadComplete(e);
		_logic.OnLoadComplete();
	}

	/// <summary>
	/// Raises the <see cref="Control.GotFocus"/> event.
	/// </summary>
	/// <param name="e">Event arguments</param>
	protected override void OnGotFocus(EventArgs e)
	{
		base.OnGotFocus(e);
		_logic.OnGotFocus();
	}

	/// <inheritdoc/>
	protected override void OnMouseUp(MouseEventArgs e)
	{
		base.OnMouseUp(e);
		_logic.OnMouseUp();
	}

	/// <summary>
	/// Raises the <see cref="Control.LostFocus"/> event.
	/// </summary>
	/// <param name="e">Event arguments</param>
	protected override void OnLostFocus(EventArgs e)
	{
		base.OnLostFocus(e);
		_logic.OnLostFocus();
	}

	/// <summary>
	/// Raises the <see cref="TextControl.TextChanged"/> event.
	/// </summary>
	/// <param name="e">Event arguments.</param>
	protected override void OnTextChanged(EventArgs e)
	{
		_logic.OnTextChanged();
		base.OnTextChanged(e);
	}

	/// <summary>
	/// Raises the <see cref="TextBox.TextChanging"/> event.
	/// </summary>
	/// <param name="e">Event arguments</param>
	protected override void OnTextChanging(TextChangingEventArgs e)
	{
		base.OnTextChanging(e);
		_logic.OnTextChanging(e);
	}

	/// <summary>
	/// Raises the <see cref="Control.KeyDown"/> event.
	/// </summary>
	/// <param name="e">Key event arguments</param>
	protected override void OnKeyDown(KeyEventArgs e)
	{
		base.OnKeyDown(e);
		_logic.OnKeyDown(e);
	}

	/// <summary>
	/// Gets or sets the text of the control including any mask characters.
	/// </summary>
	/// <value>The text content of the control.</value>
	public override string Text
	{
		get => _logic.Text;
		set => _logic.Text = value;
	}

	/// <summary>
	/// Gets a value indicating whether the mask is completed.
	/// </summary>
	/// <value><c>true</c> if mask completed; otherwise, <c>false</c>.</value>
	public bool MaskCompleted => _logic.MaskCompleted;

	/// <summary>
	/// Selects the editable range at the specified index.
	/// </summary>
	/// <param name="index">The index to select the editable range at.</param>
	/// <param name="forward">Indicates the direction to move the selection.</param>
	/// <param name="fallbackToFirstLast">Indicates whether to fallback to the first or last editable range if the index is out of bounds.</param>
	/// <returns><c>true</c> if an editable range was selected; otherwise, <c>false</c>.</returns>
	protected bool SelectEditableRangeAt(int index, bool forward, bool fallbackToFirstLast = true)
		=> _logic.SelectEditableRangeAt(index, forward, fallbackToFirstLast);
}
