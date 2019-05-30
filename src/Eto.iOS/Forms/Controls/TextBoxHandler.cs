using System;
using UIKit;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.iOS.Forms.Controls
{
	public class TextBoxHandler : IosControl<UITextField, TextBox, TextBox.ICallback>, TextBox.IHandler
	{
		protected override SizeF GetNaturalSize(SizeF availableSize)
		{
			return SizeF.Max(base.GetNaturalSize(availableSize), new SizeF(60, 0));
		}

		protected override void Initialize()
		{
			base.Initialize();
			Control = new UITextField();
			MaxLength = Int32.MaxValue;
			Control.BorderStyle = UITextBorderStyle.RoundedRect;
			Control.ShouldChangeCharacters = delegate(UITextField textField, Foundation.NSRange range, string replacementString)
			{
				var text = textField.Text;
				if (text.Length + replacementString.Length - range.Length > MaxLength)
				{

					if (range.Length > 0)
						text = text.Remove((int)range.Location, (int)range.Length);
					replacementString = replacementString.Substring(0, (int)(MaxLength - text.Length + range.Length));
					text = text.Insert((int)range.Location, replacementString);
					//UIApplication.SharedApplication.BeginInvokeOnMainThread(delegate {

					//});
					textField.Text = text;
					return false;
				}
				return !ReadOnly;
			};
		}

		public override void AttachEvent(string handler)
		{
			switch (handler)
			{
				case TextControl.TextChangedEvent:
					Control.EditingChanged += (s, e) => Callback.OnTextChanged(Widget, e);
					break;
				default:
					base.AttachEvent(handler);
					break;
			}
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

		public int MaxLength
		{
			get;
			set;
		}

		public string PlaceholderText
		{
			get { return Control.Placeholder; }
			set { Control.Placeholder = value; }
		}

		public void SelectAll()
		{
			Control.SelectAll(Control);
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

		public Color TextColor
		{
			get { return Control.TextColor.ToEto(); }
			set { Control.TextColor = value.ToNSUI(); }
		}

		public int CaretIndex
		{
			get {
				var selectedRange = Control.SelectedTextRange;
				var selectionStart = selectedRange.Start;
				return (int)Control.GetOffsetFromPosition(Control.BeginningOfDocument, selectionStart);
			}
			set
			{
				Selection = new Range<int>(value, value - 1);
			}
		}

		public Range<int> Selection
		{
			get
			{
				var selectedRange = Control.SelectedTextRange;
				var selectionStart = selectedRange.Start;
				var selectionEnd = selectedRange.End;

				var start = (int)Control.GetOffsetFromPosition(Control.BeginningOfDocument, selectionStart);
				var end = (int)Control.GetOffsetFromPosition(Control.BeginningOfDocument, selectionEnd);
				return new Range<int>(start, end - 1);
			}
			set
			{
				var start = Control.GetPosition(Control.BeginningOfDocument, value.Start);
				var end = Control.GetPosition(Control.BeginningOfDocument, value.End);
				Control.SelectedTextRange = Control.GetTextRange(start, end);
			}
		}

		public bool ShowBorder
		{
			get { return Control.BorderStyle != UITextBorderStyle.None; }
			set { Control.BorderStyle = value ? UITextBorderStyle.RoundedRect : UITextBorderStyle.None; }
		}

		public TextAlignment TextAlignment
		{
			get { return Control.TextAlignment.ToEto(); }
			set { Control.TextAlignment = value.ToUI(); }
		}
	}
}

