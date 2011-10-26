using System;
using SD = System.Drawing;
using SWF = System.Windows.Forms;
using Eto.Forms;

namespace Eto.Platform.Windows
{
	public class TextAreaHandler : WindowsControl<System.Windows.Forms.TextBox, TextArea>, ITextArea
	{
		public class MyTextBox : SWF.TextBox
		{
			private void CheckForScrollbars ()
			{
				bool scroll = false;
				int lineCount = this.Lines.Length;
				if (lineCount > 1) {
					int pos0 = this.GetPositionFromCharIndex (this.GetFirstCharIndexFromLine (0)).Y;
					if (pos0 >= 32768)
						pos0 -= 65536;
					int pos1 = this.GetPositionFromCharIndex (this.GetFirstCharIndexFromLine (1)).Y;
					if (pos1 >= 32768)
						pos1 -= 65536;
					int h = pos1 - pos0;
					scroll = lineCount * h > (this.ClientSize.Height - 6);  // 6 = padding
				}
				if (scroll != (this.ScrollBars == SWF.ScrollBars.Vertical)) {
					this.ScrollBars = scroll ? SWF.ScrollBars.Vertical : SWF.ScrollBars.None;
				}
			}

			protected override void OnTextChanged (EventArgs e)
			{
				CheckForScrollbars ();
				base.OnTextChanged (e);
			}

			protected override void OnClientSizeChanged (EventArgs e)
			{
				CheckForScrollbars ();
				base.OnClientSizeChanged (e);
			}
		}		
		
		public TextAreaHandler ()
		{
			Control = new MyTextBox ();
			Control.Multiline = true;
			Control.AcceptsReturn = true;
			Control.AcceptsTab = true;
		}
		
		public bool ReadOnly {
			get { return Control.ReadOnly; }
			set { Control.ReadOnly = value; }
		}
		
		public void Append (string text, bool scrollToCursor)
		{
			Control.AppendText (text);
			if (scrollToCursor) {
				Control.SelectionStart = Control.Text.Length;
				Control.ScrollToCaret ();
			}
		}
	}
}
