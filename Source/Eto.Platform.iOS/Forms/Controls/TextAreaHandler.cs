using System;
using MonoTouch.UIKit;
using Eto.Forms;

namespace Eto.Platform.iOS.Forms.Controls
{
	public class TextAreaHandler : iosControl<UITextField, TextArea>, ITextArea
	{
		public TextAreaHandler ()
		{
		}

		public override UITextField CreateControl ()
		{
			return new UITextField();
		}

		public void Append (string text, bool scrollToCursor)
		{
		}

		public void SelectAll ()
		{
		}

		public string Text {
			get { return Control.Text; }
			set { Control.Text = value; }
		}

		public bool ReadOnly {
			get;
			set;
		}

		public bool Wrap {
			get;
			set;
		}

		public string SelectedText {
			get;
			set;
		}

		public Range Selection {
			get;
			set;
		}

		public int CaretIndex {
			get;
			set;
		}
	}
}

