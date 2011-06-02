using System;
using SD = System.Drawing;
using SWF = System.Windows.Forms;
using Eto.Forms;

namespace Eto.Platform.Windows
{
	public class TextBoxHandler : WindowsControl<SWF.TextBox, TextBox>, ITextBox
	{
		public TextBoxHandler()
		{
			Control = new SWF.TextBox();
		}

		#region ITextBox Members

		public bool ReadOnly
		{
			get { return Control.ReadOnly; }
			set { Control.ReadOnly = value; }
		}

		#endregion
	}
}
