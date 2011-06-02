using System;
using Eto.Forms;
using MonoMac.AppKit;

namespace Eto.Platform.Mac
{
	public class TextBoxHandler : MacText<NSTextField, TextBox>, ITextBox
	{
		public TextBoxHandler()
		{
			Control = new NSTextField();
			Control.Bezeled = true;
			Control.Editable = true;
			Control.Changed += delegate {
				Widget.OnTextChanged(EventArgs.Empty);
			};
			//Control.BezelStyle = NSTextFieldBezelStyle.Square;
			//Control.Bordered = true;
		}

		#region ITextBox Members

		public bool ReadOnly
		{
			get { return Control.Enabled; }
			set { Control.Enabled = value; }
		}

		#endregion
	}
}
