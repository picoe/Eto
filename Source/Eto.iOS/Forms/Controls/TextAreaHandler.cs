using System;
using MonoTouch.UIKit;
using Eto.Forms;
using MonoTouch.Foundation;
using Eto.iOS.Drawing;
using Eto.Drawing;

namespace Eto.iOS.Forms.Controls
{
	public class TextAreaHandler : IosView<UITextView, TextArea, TextArea.ICallback>, TextArea.IHandler
	{
		public class EtoTextView : UITextView
		{

			public override System.Drawing.SizeF SizeThatFits(System.Drawing.SizeF size)
			{
				var newSize = base.SizeThatFits(size);
				newSize.Width = Math.Max(newSize.Width, TextArea.DefaultSize.Width);
				newSize.Height = Math.Max(newSize.Height, TextArea.DefaultSize.Height);
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
				return new Range(range.Location, range.Location + range.Length);
			}
			set
			{
				Control.SelectedRange = new NSRange(value.Start, value.Length());
			}
		}

		public int CaretIndex
		{
			get { return Control.SelectedRange.Location; }
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

		public HorizontalAlign HorizontalAlign
		{
			get { return Control.TextAlignment.ToEto(); }
			set { Control.TextAlignment = value.ToUI(); }
		}
	}
}