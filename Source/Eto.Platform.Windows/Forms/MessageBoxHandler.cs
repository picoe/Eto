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
		
		public string Text { get; set; }

		public string Caption { get; set; }
		
		public MessageBoxType Type { get; set; }


		public DialogResult ShowDialog(Control parent)
		{
			SWF.Control c = (parent == null) ? null : (SWF.Control)parent.ControlObject;
            var caption = Caption ?? ((parent != null && parent.ParentWindow != null) ? parent.ParentWindow.Title : null);
            SWF.DialogResult result = SWF.MessageBox.Show(c, Text, caption, SWF.MessageBoxButtons.OK, Convert(Type));
			return Generator.Convert(result);
		}

		public DialogResult ShowDialog(Control parent, MessageBoxButtons buttons)
		{
			var caption = Caption ?? ((parent != null && parent.ParentWindow != null) ? parent.ParentWindow.Title : null);
            SWF.Control c = (parent == null) ? null : (SWF.Control)parent.ControlObject;
			SWF.DialogResult result = SWF.MessageBox.Show(c, Text, caption, Convert(buttons), Convert(Type));
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
		
		public static SWF.MessageBoxIcon Convert(MessageBoxType type)
		{
			switch (type)
			{
			case MessageBoxType.Error: return SWF.MessageBoxIcon.Error;
			case MessageBoxType.Warning: return SWF.MessageBoxIcon.Warning;
			case MessageBoxType.Information: return SWF.MessageBoxIcon.Information;
			case MessageBoxType.Question: return SWF.MessageBoxIcon.Question;
			default:
				throw new NotSupportedException();
			}
		}
	}
}
