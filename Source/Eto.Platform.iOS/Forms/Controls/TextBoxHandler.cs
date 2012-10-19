using System;
using MonoTouch.UIKit;
using Eto.Forms;

namespace Eto.Platform.iOS.Forms.Controls
{
	public class TextBoxHandler : iosControl<UITextField, TextBox>, ITextBox
	{
		public override UITextField CreateControl ()
		{
			return new UITextField();
		}

		public string Text {
			get { return Control.Text; }
			set { Control.Text = value; }
		}

		public bool ReadOnly {
			get { return Control.Enabled; }
			set { Control.Enabled = value; }
		}
		public int MaxLength {
			get;
			set;
		}
		public string PlaceholderText {
			get { return Control.Placeholder; }
			set { Control.Placeholder = value; }
		}
	}
}

