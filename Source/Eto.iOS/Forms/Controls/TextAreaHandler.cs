using System;
using UIKit;
using Eto.Forms;
using Foundation;
using Eto.iOS.Drawing;
using Eto.Drawing;

namespace Eto.iOS.Forms.Controls
{
	public class TextAreaHandler : IosView<UITextView, TextArea, TextArea.ICallback>, TextArea.IHandler
	{
		public class EtoTextView : UITextView
		{

			public override CoreGraphics.CGSize SizeThatFits(CoreGraphics.CGSize size)
			{
				var newSize = base.SizeThatFits(size);
				newSize.Width = (nfloat)Math.Max(newSize.Width, 100);
				newSize.Height = (nfloat)Math.Max(newSize.Height, 60);
				return newSize;
			}
		}

		protected override void Initialize()
		{
			base.Initialize();
			Control = new EtoTextView();
			Control.Layer.BorderWidth = 1f;
			Control.Layer.BorderColor = UIColor.Gray.CGColor;
			Control.Layer.CornerRadius = 2f;
		}

		public void Append(string text, bool scrollToCursor)
		{
			Control.SelectedTextRange = Control.GetTextRange(Control.EndOfDocument, Control.EndOfDocument);
			Control.InsertText(text);
			if (scrollToCursor)
			{
				Control.ScrollRangeToVisible(Control.SelectedRange);
			}
		}

		public void SelectAll()
		{
			Control.SelectAll(Control);
		}

		public string Text
		{
			get { return Control.Text; }
			set { Control.Text = value; }
		}

		public bool ReadOnly
		{
			get;
			set;
		}

		public bool Wrap
		{
			get;
			set;
		}

		public override Font Font
		{
			get { return base.Font; }
			set
			{
				base.Font = value;
				Control.Font = value.ToUI();
			}
		}

		public string SelectedText
		{
			get { return Control.TextInRange(Control.SelectedTextRange); }
			set { Control.ReplaceText(Control.SelectedTextRange, value); }
		}

		public Range<int> Selection
		{
			get
			{ 
				var range = Control.SelectedRange;
				return new Range<int>((int)range.Location, (int)(range.Location + range.Length - 1));
			}
			set
			{
				Control.SelectedRange = new NSRange(value.Start, value.Length());
			}
		}

		public int CaretIndex
		{
			get { return (int)Control.SelectedRange.Location; }
			set
			{
				Control.SelectedRange = new NSRange(value, 0);
			}
		}

		public Color TextColor
		{
			get { return Control.TextColor.ToEto(); }
			set { Control.TextColor = value.ToNSUI(); }
		}

		public bool AcceptsTab
		{
			get;
			set;
		}

		public bool AcceptsReturn
		{
			get;
			set;
		}

		public TextAlignment TextAlignment
		{
			get { return Control.TextAlignment.ToEto(); }
			set { Control.TextAlignment = value.ToUI(); }
		}

		public bool SpellCheck
		{
			get { return Control.SpellCheckingType != UITextSpellCheckingType.No; }
			set
			{
				Control.SpellCheckingType = value ? UITextSpellCheckingType.Yes : UITextSpellCheckingType.No;
			}
		}

		public bool SpellCheckIsSupported { get { return true; } }

		public TextReplacements TextReplacements
		{
			get { return TextReplacements.None; }
			set { }
		}

		public TextReplacements SupportedTextReplacements
		{
			get { return TextReplacements.None; }
		}
	}
}