namespace Eto.WinUI.Forms.Controls;

public class TextBoxHandler : WinUIControl<muc.TextBox, TextBox, TextBox.ICallback>, TextBox.IHandler, SearchBox.IHandler
{
	int TextLength => Control.Text?.Length ?? 0;

	public bool ReadOnly
	{
		get => Control.IsReadOnly;
		set => Control.IsReadOnly = value;
	}
	public int MaxLength
	{
		get => Control.MaxLength;
		set => Control.MaxLength = value;
	}
	public string PlaceholderText
	{
		get => Control.PlaceholderText;
		set => Control.PlaceholderText = value;
	}
	public int CaretIndex
	{
		get => Control.SelectionStart;
		set
		{
			Control.SelectionStart = value;
			Control.SelectionLength = 0;
		}
	}
	public Range<int> Selection
	{
		get => Eto.Forms.Range.FromLength(Control.SelectionStart, Control.SelectionLength);
		set
		{
			Control.SelectionStart = value.Start;
			Control.SelectionLength = value.Length();
		}
	}
	public bool ShowBorder
	{
		get => !Control.BorderThickness.ToEto().IsZero;
		set => Control.BorderThickness = new mux.Thickness(value ? 1 : 0);
	}
	public TextAlignment TextAlignment
	{
		get => Control.TextAlignment.ToEto();
		set => Control.TextAlignment = value.ToWinUI();
	}
	public AutoSelectMode AutoSelectMode { get; set; }
	public string Text
	{
		get => Control.Text;
		set => Control.Text = value;
	}
	public bool AlwaysShowSelection
	{
		get => Control.SelectionHighlightColorWhenNotFocused != null;
		set => Control.SelectionHighlightColorWhenNotFocused = value ? Control.SelectionHighlightColor : null;
	}

	public int GetCharacterIndex(PointF location)
	{
		var length = TextLength;
		if (length == 0)
			return 0;

		if (location.X <= GetInsertionX(0))
			return 0;

		if (location.X >= GetInsertionX(length))
			return length;

		var low = 0;
		var high = length;
		while (low < high)
		{
			var mid = (low + high) / 2;
			if (location.X <= GetInsertionX(mid))
				high = mid;
			else
				low = mid + 1;
		}

		var right = low;
		var left = Math.Max(0, right - 1);
		return Math.Abs(location.X - GetInsertionX(left)) <= Math.Abs(GetInsertionX(right) - location.X) ? left : right;
	}

	public void SelectAll() => Control.SelectAll();

	double GetInsertionX(int index)
	{
		if (index <= 0)
			return Control.GetRectFromCharacterIndex(0, false).Left;
		if (index >= TextLength)
			return Control.GetRectFromCharacterIndex(TextLength - 1, true).Right;
		return Control.GetRectFromCharacterIndex(index, false).Left;
	}

	protected override muc.TextBox CreateControl() => new EtoTextBox { Handler = this };

	public override void AttachEvent(string id)
	{
		switch (id)
		{
			case TextBox.TextChangedEvent:
				Control.TextChanged += Control_TextChanged;
				break;
			default:
				base.AttachEvent(id);
				break;
		}
	}

	private void Control_TextChanged(object sender, muc.TextChangedEventArgs e)
	{
		Callback.OnTextChanged(Widget, EventArgs.Empty);
	}
}
