namespace Eto.Forms;

/// <summary>
/// Interface implemented by controls that delegate their masking behaviour to <see cref="MaskedTextBoxLogic"/>.
/// </summary>
/// <remarks>
/// This allows the shared masking implementation to read and write the underlying text of the control
/// (bypassing the masked <see cref="TextControl.Text"/> override) and to invoke the control's overridable
/// <see cref="UpdateText"/> and <see cref="CommitText"/> hooks so subclass overrides are honored.
/// </remarks>
interface IMaskedTextControl
{
	/// <summary>
	/// Gets or sets the underlying text of the control, bypassing any masked <see cref="TextControl.Text"/> override.
	/// </summary>
	string BaseText { get; set; }

	/// <summary>
	/// Invokes the control's overridable UpdateText method.
	/// </summary>
	void UpdateText();

	/// <summary>
	/// Invokes the control's overridable CommitText method.
	/// </summary>
	void CommitText();
}

/// <summary>
/// Shared implementation of the masking behaviour used by both <see cref="MaskedTextBox"/> and <see cref="MaskedTextStepper"/>.
/// </summary>
/// <remarks>
/// Both controls derive from <see cref="TextBox"/> via different paths (<see cref="MaskedTextStepper"/> through
/// <see cref="TextStepper"/>), so a common base class is not possible. Instead, each control owns an instance of this
/// class and forwards its event overrides and masking properties here, keeping a single source of truth for the logic.
/// </remarks>
class MaskedTextBoxLogic
{
	readonly TextBox _control;
	readonly IMaskedTextControl _owner;

	IMaskedTextProvider _provider;
	int _isUpdatingText;
	ShowPromptMode _showPromptMode;
	bool _showPlaceholderWhenEmpty = true;
	bool _autoSelectEditableRanges;

	static readonly object SupportsInsertKey = new object();
	static readonly object OverwriteModeKey = new object();

	/// <summary>
	/// Gets a cached value indicating the current platform supports getting the insert mode state.
	/// </summary>
	static bool SupportsInsert
	{
		// cache whether the platform supports the insert key for Keyboard.IsKeyLocked
		get => Platform.Instance.GetSharedProperty(SupportsInsertKey, () => Keyboard.SupportedLockKeys.Contains(Keys.Insert));
	}

	/// <summary>
	/// If the platform doesn't support global insert/overwrite mode, this stores an application-wide state of the insert mode
	/// </summary>
	static bool ManualOverwriteMode
	{
		get => Platform.Instance.GetSharedProperty(OverwriteModeKey, () => false);
		set => Platform.Instance.SetSharedProperty(OverwriteModeKey, value);
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="MaskedTextBoxLogic"/> class for the specified control.
	/// </summary>
	/// <param name="control">The control to provide masking behaviour for. Must implement <see cref="IMaskedTextControl"/>.</param>
	public MaskedTextBoxLogic(TextBox control)
	{
		_control = control;
		_owner = (IMaskedTextControl)control;
	}

	/// <summary>
	/// Gets or sets the masked text provider to specify the mask format.
	/// </summary>
	public IMaskedTextProvider Provider
	{
		get => _provider;
		set
		{
			if (!ReferenceEquals(value, _provider))
			{
				var oldProvider = _provider;
				_provider = value;
				if (_provider != null && oldProvider != null)
					_provider.Text = oldProvider.Text;
				_owner.UpdateText();
			}
		}
	}

	/// <summary>
	/// Gets or sets the mode for insertion. Use <see cref="IsOverwrite"/> to determine the current mode.
	/// </summary>
	public InsertKeyMode InsertMode { get; set; } = InsertKeyMode.Insert;

	/// <summary>
	/// Gets a value indicating whether typing will overwrite text.
	/// </summary>
	public bool IsOverwrite
	{
		get
		{
			if (InsertMode == InsertKeyMode.Overwrite)
				return true;
			var overwrite = SupportsInsert ? Keyboard.IsKeyLocked(Keys.Insert) : ManualOverwriteMode;
			return InsertMode == InsertKeyMode.Toggle && overwrite;
		}
	}

	/// <summary>
	/// Gets or sets the mode for when the input prompts should be shown.
	/// </summary>
	public ShowPromptMode ShowPromptMode
	{
		get => _showPromptMode;
		set
		{
			if (_showPromptMode != value)
			{
				_showPromptMode = value;
				_owner.UpdateText();
			}
		}
	}

	/// <summary>
	/// Gets or sets a value indicating that the placeholder should be shown when the mask is empty and the control does
	/// not have focus.
	/// </summary>
	public bool ShowPlaceholderWhenEmpty
	{
		get => _showPlaceholderWhenEmpty;
		set
		{
			if (_showPlaceholderWhenEmpty != value)
			{
				_showPlaceholderWhenEmpty = value;
				_owner.UpdateText();
			}
		}
	}

	/// <summary>
	/// Gets or sets a value indicating whether editable mask ranges should be selected automatically.
	/// </summary>
	public bool AutoSelectEditableRanges
	{
		get => _autoSelectEditableRanges;
		set
		{
			if (_autoSelectEditableRanges != value)
			{
				_autoSelectEditableRanges = value;
				if (value && _control.HasFocus)
					SelectEditableRangeAt(_control.CaretIndex, forward: true);
			}
		}
	}

	/// <summary>
	/// Gets a value indicating whether the mask is completed.
	/// </summary>
	public bool MaskCompleted => _provider?.MaskCompleted == true;

	/// <summary>
	/// Gets or sets the text of the control including any mask characters.
	/// </summary>
	public string Text
	{
		get => _provider != null ? _provider.Text : _owner.BaseText;
		set
		{
			if (_provider != null)
			{
				_provider.Text = value;
				_owner.UpdateText();
			}
			else
				_owner.BaseText = value;
		}
	}

	/// <summary>
	/// Updates the text to the display text from the provider.
	/// </summary>
	public void UpdateText()
	{
		if (_provider == null)
			return;
		var hasFocus = _control.HasFocus;
		if (!hasFocus)
			_provider.CommitText();
		_isUpdatingText++;
		if (!hasFocus && _showPlaceholderWhenEmpty && _provider.IsEmpty)
			_owner.BaseText = null;
		else if ((hasFocus && _showPromptMode == ShowPromptMode.OnFocus) || _showPromptMode == ShowPromptMode.Always)
			_owner.BaseText = _provider.DisplayText;
		else
			_owner.BaseText = _provider.Text;
		_isUpdatingText--;
	}

	/// <summary>
	/// Commits the text to the provider and updates the display text.
	/// </summary>
	public void CommitText()
	{
		_provider?.CommitText();
		_owner.UpdateText();
	}

	/// <summary>
	/// Handles the control's LoadComplete event.
	/// </summary>
	public void OnLoadComplete() => _owner.UpdateText();

	/// <summary>
	/// Handles the control's GotFocus event.
	/// </summary>
	public void OnGotFocus()
	{
		if (_showPromptMode == ShowPromptMode.OnFocus || _showPlaceholderWhenEmpty)
		{
			_owner.UpdateText();
			_control.CaretIndex = 0;
		}
		if (_autoSelectEditableRanges && _control.Selection.Length() == 0)
			SelectEditableRangeAt(_control.CaretIndex, forward: true);
	}

	/// <summary>
	/// Handles the control's MouseUp event.
	/// </summary>
	public void OnMouseUp()
	{
		if (_autoSelectEditableRanges && _control.Selection.Length() == 0)
			SelectEditableRangeAt(_control.CaretIndex, forward: true);
	}

	/// <summary>
	/// Handles the control's LostFocus event.
	/// </summary>
	public void OnLostFocus() => _owner.CommitText();

	/// <summary>
	/// Handles the control's TextChanged event. Call before the base TextChanged is raised.
	/// </summary>
	public void OnTextChanged()
	{
		// handle undo/redo and drag/drop which doesn't always get a TextChanging event.
		if (_isUpdatingText == 0 && _provider != null)
		{
			_provider.Text = _owner.BaseText;
			_owner.UpdateText();
		}
	}

	/// <summary>
	/// Handles the control's TextChanging event. Call after the base TextChanging is raised.
	/// </summary>
	public void OnTextChanging(TextChangingEventArgs e)
	{
		if (e.Cancel || _control.ReadOnly || !_control.Enabled || !e.FromUser)
			return;
		var sel = e.Range;
		var pos = sel.Start;
		var len = sel.Length();
		var overwrite = IsOverwrite;
		if (_provider == null)
		{
			// with no provider, still have some functionality
			if (e.Text.Length > 0)
			{
				if (overwrite && len == 0)
				{
					var text = Text;
					if (sel.Start < text.Length)
						text = text.Remove(sel.Start, Math.Min(text.Length - sel.Start, e.Text.Length));
					text = text.Insert(sel.Start, e.Text);
					Text = text;
				}
				else
					_control.SelectedText = e.Text;
				_control.CaretIndex = pos + e.Text.Length;
				e.Cancel = true;
			}
			return;
		}

		if (len > 0)
		{
			var tempPos = pos;
			if (overwrite)
				_provider.Clear(ref tempPos, len, true);
			else
				_provider.Delete(ref tempPos, len, true);
		}

		foreach (char ch in e.Text)
		{
			if (overwrite)
				_provider.Replace(ch, ref pos);
			else
				_provider.Insert(ch, ref pos);
		}

		_owner.UpdateText();
		_control.CaretIndex = pos;
		e.Cancel = true;
	}

	/// <summary>
	/// Handles the control's KeyDown event. Call after the base KeyDown is raised.
	/// </summary>
	public void OnKeyDown(KeyEventArgs e)
	{
		if (e.Handled || _control.ReadOnly || !_control.Enabled)
			return;
		switch (e.KeyData)
		{
			case Keys.Left:
				if (_autoSelectEditableRanges)
				{
					if (MoveEditableRangeSelection(forward: false, fallbackToFirstLast: true))
					{
						var selection = _control.Selection;
						_owner.CommitText();
						e.Handled = true;
						_control.Selection = selection;
					}
				}
				break;
			case Keys.Right:
				if (_autoSelectEditableRanges)
				{
					if (MoveEditableRangeSelection(forward: true, fallbackToFirstLast: true))
					{
						var selection = _control.Selection;
						_owner.CommitText();
						_control.Selection = selection;
						e.Handled = true;
					}
				}
				break;
			case Keys.Tab:
			case Keys.Tab | Keys.Shift:
				if (_autoSelectEditableRanges)
				{
					if (MoveEditableRangeSelection(forward: !e.Shift, fallbackToFirstLast: false))
					{
						var selection = _control.Selection;
						_owner.CommitText();
						e.Handled = true;
						_control.Selection = selection;
					}
				}
				break;
			case Keys.Delete:
			case Keys.Backspace:
				if (_provider == null)
					return;
				// override default delete/backspace behaviour so we can skip past literals
				var sel = _control.Selection;
				var pos = sel.Start;
				var len = sel.Length();
				var forward = len > 0 || e.KeyData == Keys.Delete;
				len = Math.Max(1, len);
				bool changed;
				if (IsOverwrite)
					changed = _provider.Clear(ref pos, len, forward);
				else
					changed = _provider.Delete(ref pos, len, forward);

				if (changed)
				{
					Text = _provider.DisplayText;
					if (_autoSelectEditableRanges)
						SelectEditableRangeAt(pos, forward: e.KeyData == Keys.Delete);
					else
						_control.CaretIndex = pos;
				}
				e.Handled = true;
				break;
			case Keys.Insert:
				if (!SupportsInsert && InsertMode == InsertKeyMode.Toggle)
				{
					ManualOverwriteMode = !ManualOverwriteMode;
					e.Handled = true;
				}
				break;
		}
	}

	bool MoveEditableRangeSelection(bool forward, bool fallbackToFirstLast)
	{
		if (_provider == null)
			return false;

		var selection = _control.Selection;
		// get current editable range, and move to the next one in the direction specified

		int index;
		if (selection.Length() == 0 && TryGetEditableRange(_control.CaretIndex, out var range))
			index = forward ? range.End + 1 : range.Start - 1;
		else
			index = forward ? selection.End + 1 : selection.Start - 1;

		return SelectEditableRangeAt(index, forward, fallbackToFirstLast);
	}

	/// <summary>
	/// Selects the editable range at the specified index.
	/// </summary>
	/// <param name="index">The index to select the editable range at.</param>
	/// <param name="forward">Indicates the direction to move the selection.</param>
	/// <param name="fallbackToFirstLast">Indicates whether to fallback to the first or last editable range if the index is out of bounds.</param>
	/// <returns><c>true</c> if an editable range was selected; otherwise, <c>false</c>.</returns>
	public bool SelectEditableRangeAt(int index, bool forward, bool fallbackToFirstLast = true)
	{
		if (!TryGetEditableRange(index, forward, fallbackToFirstLast, out var range))
			return false;

		_control.Selection = range;
		return true;
	}

	bool TryGetEditableRange(int index, out Range<int> range)
	{
		range = default;
		if (_provider?.EditPositions == null)
			return false;

		var groups = GetEditableRanges().ToList();
		if (groups.Count == 0)
			return false;
		foreach (var editableRange in groups)
		{
			if (index >= editableRange.Start && index <= editableRange.End)
			{
				range = editableRange;
				return true;
			}
		}
		return false;
	}

	bool TryGetEditableRange(int index, bool forward, bool fallbackToFirstLast, out Range<int> range)
	{
		range = default;
		if (_provider?.EditPositions == null)
			return false;

		var groups = GetEditableRanges().ToList();
		if (groups.Count == 0)
			return false;

		if (forward)
		{
			foreach (var editableRange in groups)
			{
				if (index <= editableRange.End)
				{
					range = editableRange;
					return true;
				}
			}
			if (fallbackToFirstLast)
			{
				range = groups[groups.Count - 1];
				return true;
			}
			return false;
		}

		for (var i = groups.Count - 1; i >= 0; i--)
		{
			var editableRange = groups[i];
			if (index >= editableRange.Start)
			{
				range = editableRange;
				return true;
			}
		}

		if (fallbackToFirstLast)
		{
			range = groups[0];
			return true;
		}

		return false;
	}

	IEnumerable<Range<int>> GetEditableRanges()
	{
		if (_provider?.EditPositions == null)
			yield break;

		var started = false;
		var start = 0;
		var end = 0;

		foreach (var position in _provider.EditPositions.OrderBy(r => r))
		{
			if (!started)
			{
				start = end = position;
				started = true;
			}
			else if (position == end + 1)
			{
				end = position;
			}
			else
			{
				yield return new Range<int>(start, end);
				start = end = position;
			}
		}

		if (started)
			yield return new Range<int>(start, end);
	}
}
