using System;
using SD = System.Drawing;
using SWF = System.Windows.Forms;
using Eto.Forms;

namespace Eto.Platform.Windows
{
	public class MessageBoxHandler : IMessageBox
	{
		
		public IWidget Handler { get; set; }
		
		public void Initialize()
		{
		}
		
		#region IMessageBox Members

		public string Text { get; set; }

		public string Caption { get; set; }


		public DialogResult ShowDialog(Control parent)
		{
			SWF.Control c = (parent == null) ? null : (SWF.Control)parent.ControlObject;
			SWF.DialogResult result = SWF.MessageBox.Show(c, Text, Caption);
			return Generator.Convert(result);
		}

		public DialogResult ShowDialog(Control parent, MessageBoxButtons buttons)
		{
			SWF.Control c = (parent == null) ? null : (SWF.Control)parent.ControlObject;
			SWF.DialogResult result = SWF.MessageBox.Show(c, Text, Caption, Convert(buttons));
			return Generator.Convert(result);
		}
		
		public static SWF.MessageBoxButtons Convert(MessageBoxButtons buttons)
		{
			switch (buttons)
			{
				default:
				case MessageBoxButtons.OK: return SWF.MessageBoxButtons.OK;
				case MessageBoxButtons.OKCancel: return SWF.MessageBoxButtons.OKCancel;
				case MessageBoxButtons.YesNo: return SWF.MessageBoxButtons.YesNo;
				case MessageBoxButtons.YesNoCancel: return SWF.MessageBoxButtons.YesNoCancel;
			}
		}
		
		#endregion
	}
}
