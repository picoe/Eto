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
			bool busy;
			void CheckForScrollbars ()
			{
				if (!busy) {
					busy = true;

					/**/
					var textSize = SWF.TextRenderer.MeasureText (Text, Font);
					bool verticalScroll = ClientSize.Height < textSize.Height; // +Convert.ToInt32 (Font.Size);
					bool horizontalScroll = ClientSize.Width < textSize.Width;

					if (verticalScroll && horizontalScroll)
						ScrollBars = SWF.ScrollBars.Both;
					else if (!verticalScroll && !horizontalScroll)
						ScrollBars = SWF.ScrollBars.None;
					else if (verticalScroll && !horizontalScroll)
						ScrollBars = SWF.ScrollBars.Vertical;
					else if (!verticalScroll && horizontalScroll)
						ScrollBars = SWF.ScrollBars.Horizontal;
					/**

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
					if (scroll != (this.ScrollBars == SWF.ScrollBars.Vertical || this.ScrollBars == SWF.ScrollBars.Both)) {

						if (this.ScrollBars == SWF.ScrollBars.Both)
							this.ScrollBars = scroll ? SWF.ScrollBars.Both : SWF.ScrollBars.Horizontal;
						else
							this.ScrollBars = scroll ? SWF.ScrollBars.Vertical : SWF.ScrollBars.None;
							
					}
					/**/
					busy = false;
				}
			}

			/*
			protected override void OnTextChanged (EventArgs e)
			{
				CheckForScrollbars ();
				base.OnTextChanged (e);
			}

			protected override void OnSizeChanged (EventArgs e)
			{
				CheckForScrollbars ();
				base.OnSizeChanged (e);
			}
			 */
		}	
		
		public TextAreaHandler ()
		{
			Control = new MyTextBox ();
			Control.Multiline = true;
			Control.AcceptsReturn = true;
			Control.AcceptsTab = true;
			Control.ScrollBars = SWF.ScrollBars.Both;
			Control.TextChanged += delegate {
				Widget.OnTextChanged (EventArgs.Empty);
			};
		}
		
		public bool ReadOnly {
			get { return Control.ReadOnly; }
			set { Control.ReadOnly = value; }
		}
		
		public bool Wrap {
			get { return Control.WordWrap; }
			set { Control.WordWrap = value; }
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
