namespace Eto.WinUI.Forms.Controls;

public class TextBoxHandler : WinUIControl<muc.TextBox, TextBox, TextBox.ICallback>, TextBox.IHandler, SearchBox.IHandler
{
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

	public void SelectAll() => Control.SelectAll();

	protected override muc.TextBox CreateControl() => new();

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
