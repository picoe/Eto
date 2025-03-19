using Eto.Mac.Drawing;

namespace Eto.Mac.Forms.Controls
{
	class MacMnemonicString
	{
		string _text;
		private bool _alwaysShowMnemonic;
		NSMutableAttributedString _str;
		NSMutableParagraphStyle _paragraphStyle;
		Color? _textColor;
		Font _font;
		bool _useMnemonic = true;

		public string Text
		{
			get => _text;
			set
			{
				_text = value;
				_str = null;
			}
		}

		public bool UseMnemonic
		{
			get => _useMnemonic;
			set
			{
				_useMnemonic = value;
				_str = null;
			}
		}
		
		public bool AlwaysShowMnemonic
		{
			get => _alwaysShowMnemonic;
			set
			{
				_alwaysShowMnemonic = value;
				_str = null;
			}
		}


		public Color? TextColor
		{
			get => _textColor;
			set
			{
				_textColor = value;
				_str = null;
			}
		}

		public Font Font
		{
			get => _font;
			set
			{
				_font = value;
				_str = null;
			}
		}

		private NSMutableParagraphStyle ParagraphStyle => _paragraphStyle ??= new();

		public NSAttributedString AttributedString
		{
			get
			{
				if (_str == null)
				{
					Build();
				}
				return _str;
			}
		}

		public WrapMode Wrap
		{
			get => _paragraphStyle?.LineBreakMode switch
			{
				NSLineBreakMode.Clipping => WrapMode.None,
				NSLineBreakMode.ByWordWrapping => WrapMode.Word,
				_ => WrapMode.Character
			};
			set
			{
				ParagraphStyle.LineBreakMode = value switch
				{
					WrapMode.None => NSLineBreakMode.Clipping,
					WrapMode.Word => NSLineBreakMode.ByWordWrapping,
					WrapMode.Character => NSLineBreakMode.CharWrapping,
					_ => throw new NotSupportedException()
				};
				_str = null;
			}
		}
		
		public TextAlignment Alignment
		{
			get => _paragraphStyle?.Alignment.ToEto() ?? TextAlignment.Left;
			set
			{
				ParagraphStyle.Alignment = value.ToNS();
				_str = null;
			}
		}

		private void Build()
		{
			var text = _text ?? string.Empty;

			int mnemonicIndex = -1;
			if (UseMnemonic)
			{
				// find mnemonic starting with '&'
				var match = Regex.Match(text, @"(?<=([^&](?:[&]{2})*)|^)[&](?![&])");
				if (match.Success)
				{
					var val = text.Remove(match.Index, match.Length).Replace("&&", "&");

					var matches = Regex.Matches(text, @"[&][&]");
					var prefixCount = matches.Cast<Match>().Count(r => r.Index < match.Index);
					
					if (AlwaysShowMnemonic)
						mnemonicIndex = match.Index - prefixCount;
					text = val;
				}
				else
					text = text.Replace("&&", "&");
			}

			_str = new NSMutableAttributedString(text);
			var strRange = new NSRange(0, _str.Length);
			if (_paragraphStyle != null)
			{
				_str.AddAttribute(NSStringAttributeKey.ParagraphStyle, _paragraphStyle.Copy(), strRange);
			}
			if (_font != null)
			{
				var attr = new NSMutableDictionary();
				_font.Apply(attr);
				_str.AddAttributes(attr, strRange);
			}
			if (mnemonicIndex >= 0 && mnemonicIndex < _str.Length)
			{
				var range = new NSRange(mnemonicIndex, 1);
				_str.AddAttribute(NSStringAttributeKey.UnderlineStyle, NSNumber.FromInt32((int)NSUnderlineStyle.Single), range);
			}
			if (_textColor != null)
			{
				var attr = NSDictionary.FromObjectAndKey(_textColor.Value.ToNSUI(), NSStringAttributeKey.ForegroundColor);
				_str.AddAttributes(attr, strRange);
			}
		}
	}
}

