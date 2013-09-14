using System;
using sd = System.Drawing;
using swf = System.Windows.Forms;
using Eto.Forms;

namespace Eto.Platform.Windows
{
	public class MessageBoxHandler : WidgetHandler<Widget>, IMessageBox
	{
		public string Text { get; set; }

		public string Caption { get; set; }

		public MessageBoxType Type { get; set; }

		public DialogResult ShowDialog(Control parent)
		{
			swf.Control c = (parent == null) ? null : (swf.Control)parent.ControlObject;
			var caption = Caption ?? ((parent != null && parent.ParentWindow != null) ? parent.ParentWindow.Title : null);
			swf.DialogResult result = swf.MessageBox.Show(c, Text, caption, swf.MessageBoxButtons.OK, Convert(Type));
			return result.ToEto();
		}

		public DialogResult ShowDialog(Control parent, MessageBoxButtons buttons)
		{
			var caption = Caption ?? ((parent != null && parent.ParentWindow != null) ? parent.ParentWindow.Title : null);
			swf.Control c = (parent == null) ? null : (swf.Control)parent.ControlObject;
			swf.DialogResult result = swf.MessageBox.Show(c, Text, caption, Convert(buttons), Convert(Type), DefaultButton(buttons));
			return result.ToEto();
		}

		public static swf.MessageBoxDefaultButton DefaultButton(MessageBoxButtons buttons)
		{
			switch (buttons)
			{
				default:
				case MessageBoxButtons.OK:
					return swf.MessageBoxDefaultButton.Button1;
				case MessageBoxButtons.YesNo:
				case MessageBoxButtons.OKCancel:
					return swf.MessageBoxDefaultButton.Button2;
				case MessageBoxButtons.YesNoCancel:
					return swf.MessageBoxDefaultButton.Button3;
			}
		}

		public static swf.MessageBoxButtons Convert(MessageBoxButtons buttons)
		{
			switch (buttons)
			{
				default:
				case MessageBoxButtons.OK: return swf.MessageBoxButtons.OK;
				case MessageBoxButtons.OKCancel: return swf.MessageBoxButtons.OKCancel;
				case MessageBoxButtons.YesNo: return swf.MessageBoxButtons.YesNo;
				case MessageBoxButtons.YesNoCancel: return swf.MessageBoxButtons.YesNoCancel;
			}
		}

		public static swf.MessageBoxIcon Convert(MessageBoxType type)
		{
			switch (type)
			{
				case MessageBoxType.Error: return swf.MessageBoxIcon.Error;
				case MessageBoxType.Warning: return swf.MessageBoxIcon.Warning;
				case MessageBoxType.Information: return swf.MessageBoxIcon.Information;
				case MessageBoxType.Question: return swf.MessageBoxIcon.Question;
				default:
					throw new NotSupportedException();
			}
		}
	}
}
