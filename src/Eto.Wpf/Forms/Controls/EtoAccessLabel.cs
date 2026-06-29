using swd = System.Windows.Documents;

namespace Eto.Wpf.Forms.Controls;

public class EtoAccessLabel : swc.Label
{
	string _text;
	bool _useMnemonic = true;
	bool _enableMnemonic = true;
	bool _alwaysShowMnemonic;
	string _accessKey;
	sw.TextAlignment _textAlignment;
	sw.TextDecorationCollection _textDecorations;
	sw.TextWrapping _textWrapping;
	public EtoAccessLabel()
	{
		Padding = new sw.Thickness(0);
		HorizontalContentAlignment = sw.HorizontalAlignment.Stretch;
		VerticalContentAlignment = sw.VerticalAlignment.Center;
	}
	
	public sw.TextAlignment TextAlignment
	{
		get => _textAlignment;
		set
		{
			_textAlignment = value;
			if (Content is swc.TextBlock textBlock)
			{
				textBlock.TextAlignment = value;
			}
			else if (Content is swc.AccessText accessText)
			{
				accessText.TextAlignment = value;
			}
		}
	}
	
	public sw.TextDecorationCollection TextDecorations
	{
		get => _textDecorations;
		set
		{
			_textDecorations = value;
			if (Content is swc.TextBlock textBlock)
			{
				textBlock.TextDecorations = value;
			}
			else if (Content is swc.AccessText accessText)
			{
				accessText.TextDecorations = value;
			}
		}
	}
	
	protected override void OnAccessKey(swi.AccessKeyEventArgs e)
	{
		// move focus to the next control after the label
		var tRequest = new swi.TraversalRequest(swi.FocusNavigationDirection.Next);
		MoveFocus(tRequest);
	}
	
	public bool UseMnemonic
	{
		get => _useMnemonic;
		set
		{
			if (_useMnemonic == value) return;
			_useMnemonic = value;
			CreateContent();
		}
	}

	public bool AlwaysShowMnemonic
	{
		get => _alwaysShowMnemonic;
		set
		{
			if (_alwaysShowMnemonic == value) return;
			_alwaysShowMnemonic = value;
			CreateContent();
		}
	}

	public bool EnableMnemonic
	{
		get => _enableMnemonic;
		set
		{
			if (_enableMnemonic == value) return;
			_enableMnemonic = value;
			CreateContent();
		}
	}

	public string Text
	{
		get => _text;
		set
		{
			if (_text == value) return;
			_text = value;
			CreateContent();
		}
	}

	public sw.TextWrapping TextWrapping
	{
		get => _textWrapping;
		set
		{
			_textWrapping = value;
			if (Content is swc.TextBlock textBlock)
			{
				textBlock.TextWrapping = value;
			}
			else if (Content is swc.AccessText accessText)
			{
				accessText.TextWrapping = value;
			}
		}
	}

	static swc.TextBlock UnderlineCharacter(string text, int indexToUnderline)
	{
		var textBlock = new swc.TextBlock();

		if (indexToUnderline > 0)
		{
			textBlock.Inlines.Add(new swd.Run(text.Substring(0, indexToUnderline)));
		}

		if (indexToUnderline >= 0 && indexToUnderline < text.Length)
		{
			var underline = new swd.Underline(new swd.Run(text[indexToUnderline].ToString()));
			textBlock.Inlines.Add(underline);
		}

		if (indexToUnderline + 1 < text.Length)
		{
			textBlock.Inlines.Add(new swd.Run(text.Substring(indexToUnderline + 1)));
		}

		return textBlock;
	}

	void CreateContent()
	{
		if (_accessKey != null)
		{
			swi.AccessKeyManager.Unregister(_accessKey, this);
			_accessKey = null;
		}

		if (string.IsNullOrEmpty(_text))
		{
			Content = null;
			return;
		}

		if (!UseMnemonic)
		{
			if (Content is swc.TextBlock tb)
				tb.Text = _text;
			else
				Content = new swc.TextBlock { Text = _text, TextWrapping = _textWrapping, TextAlignment = _textAlignment, TextDecorations = _textDecorations };
			return;
		}

		if (AlwaysShowMnemonic || EnableMnemonic)
		{
			var mnemonicText = _text.Replace("_", "__");
			Match match = Conversions.PlatformMnemonic.Match(mnemonicText);
			if (match.Success)
			{
				var sb = new StringBuilder(mnemonicText);
				sb[match.Index] = '_';
				char? ch = match.Index < sb.Length - 1 ? sb[match.Index + 1] : null;
				sb.Replace("&&", "&");
				if (AlwaysShowMnemonic)
				{
					sb.Remove(match.Index, 1);
					var textBlock = UnderlineCharacter(sb.ToString(), match.Index);
					textBlock.TextWrapping = _textWrapping;
					textBlock.TextAlignment = _textAlignment;
					textBlock.TextDecorations = _textDecorations;
					Content = textBlock;

					_accessKey = ch.ToString();
					if (EnableMnemonic)
						swi.AccessKeyManager.Register(_accessKey, this);
				}
				else
				{
					if (Content is swc.AccessText accessText)
						accessText.Text = sb.ToString();
					else
						Content = new swc.AccessText { Text = sb.ToString(), TextWrapping = _textWrapping, TextAlignment = _textAlignment, TextDecorations = _textDecorations };
				}
				return;
			}
		}

		var text = _text.Replace("&&", "&");
		if (Content is swc.TextBlock tb2)
			tb2.Text = text;
		else
			Content = new swc.TextBlock { Text = text, TextWrapping = _textWrapping, TextAlignment = _textAlignment, TextDecorations = _textDecorations };
	}

	protected override sw.Size MeasureOverride(sw.Size constraint)
	{
		var size = base.MeasureOverride(constraint);
		 
		 // add a little extra width to avoid WPF bug wrapping with Display (GDI) font rendering in some DPI settings (e.g. 150%, 175%)
		var textFormattingMode = swm.TextOptions.GetTextFormattingMode(this);
		if (textFormattingMode == swm.TextFormattingMode.Display)
			size.Width += 1;
		return size;
	}



}
