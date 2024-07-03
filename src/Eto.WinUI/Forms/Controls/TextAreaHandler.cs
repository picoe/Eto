using CommunityToolkit.WinUI;

namespace Eto.WinUI.Forms.Controls;

public class EtoTextBox : muc.TextBox
{
	public IWinUIFrameworkElement Handler { get; set; }

	protected override wf.Size MeasureOverride(wf.Size availableSize)
	{
		return Handler?.MeasureOverride(availableSize, base.MeasureOverride) ?? base.MeasureOverride(availableSize);
	}
}
public class TextAreaHandler : WinUIControl<muc.TextBox, TextArea, TextArea.ICallback>, TextArea.IHandler
{
	public bool ReadOnly
	{
		get => Control.IsReadOnly;
		set => Control.IsReadOnly = value;
	}
	public string Text
	{
		get => Control.Text;
		set => Control.Text = value;
	}
	public bool Wrap
	{
		get => Control.TextWrapping != mux.TextWrapping.NoWrap;
		set => Control.TextWrapping = value ? mux.TextWrapping.Wrap : mux.TextWrapping.NoWrap;
	}
	public string SelectedText
	{
		get => Control.SelectedText;
		set => Control.SelectedText = value;
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
	public int CaretIndex
	{
		get => Control.SelectionStart;
		set
		{
			Control.SelectionStart = value;
			Control.SelectionLength = 0;
		}
	}
	public bool AcceptsTab { get; set; }
	public bool AcceptsReturn
	{
		get => Control.AcceptsReturn;
		set => Control.AcceptsReturn = value;
	}
	public TextReplacements TextReplacements { get; set; }
	public TextReplacements SupportedTextReplacements { get; }
	public TextAlignment TextAlignment
	{
		get => Control.TextAlignment.ToEto();
		set => Control.TextAlignment = value.ToWinUI();
	}
	public bool SpellCheck
	{
		get => Control.IsSpellCheckEnabled;
		set => Control.IsSpellCheckEnabled = value;
	}
	public bool SpellCheckIsSupported => true;
	public BorderType Border { get; set; }
	public int TextLength => Control.Text.Length;

	protected override muc.TextBox CreateControl() => new EtoTextBox { Handler = this };

	protected override void Initialize()
	{
		base.Initialize();
		Control.AcceptsReturn = true;
	}

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

	public void Append(string text, bool scrollToCursor)
	{
		Control.Text += text;
		if (scrollToCursor)
		{
			ScrollToEnd();
		}
	}

	public void SelectAll() => Control.SelectAll();

	public void ScrollTo(Range<int> range)
	{
		throw new NotImplementedException();
	}

	muc.ScrollViewer ScrollViewer => Control.FindDescendant<muc.ScrollViewer>();

	public void ScrollToStart()
	{
		ScrollViewer?.ChangeView(null, 0, null);
	}

	public void ScrollToEnd()
	{
		ScrollViewer?.ChangeView(null, ScrollViewer.ExtentHeight, null);
	}
}
