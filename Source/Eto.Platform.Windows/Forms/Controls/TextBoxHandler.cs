using System;
using SD = System.Drawing;
using SWF = System.Windows.Forms;
using Eto.Forms;

namespace Eto.Platform.Windows
{
	public class TextBoxHandler : WindowsControl<SWF.TextBox, TextBox>, ITextBox
	{
		public TextBoxHandler ()
		{
			Control = new SWF.TextBox ();
		}

		public bool ReadOnly {
			get { return Control.ReadOnly; }
			set { Control.ReadOnly = value; }
		}
		
		public int MaxLength {
			get { return this.Control.MaxLength; }
			set { this.Control.MaxLength = value; }
		}
	}
}
