using System;
using sd = System.Drawing;
using swf = System.Windows.Forms;
using Eto.Forms;

namespace Eto.WinForms.Forms
{
	public class MessageBoxHandler : WidgetHandler<Widget>, MessageBox.IHandler
	{
		public string Text { get; set; }

		public string Caption { get; set; }

		public MessageBoxType Type { get; set; }

		public MessageBoxButtons Buttons { get; set; }

		public MessageBoxDefaultButton DefaultButton { get; set; }

		public DialogResult ShowDialog(Control parent)
		{
			var caption = Caption ?? ((parent != null && parent.ParentWindow != null) ? parent.ParentWindow.Title : null);
			swf.Control c = (parent == null) ? null : (swf.Control)parent.ControlObject;
			swf.DialogResult result = swf.MessageBox.Show(c, Text, caption, Convert(Buttons), Convert(Type), Convert(DefaultButton, Buttons));
			return result.ToEto();
		}

		public static swf.MessageBoxDefaultButton Convert(MessageBoxDefaultButton defaultButton, MessageBoxButtons buttons)
		{
			switch (defaultButton)
			{
				case MessageBoxDefaultButton.OK:
					return swf.MessageBoxDefaultButton.Button1;
				case MessageBoxDefaultButton.No:
				case MessageBoxDefaultButton.Cancel:
					return swf.MessageBoxDefaultButton.Button2;
				case MessageBoxDefaultButton.Default:
					switch (buttons)
					{
						case MessageBoxButtons.OK:
							return swf.MessageBoxDefaultButton.Button1;
						case MessageBoxButtons.YesNo:
						case MessageBoxButtons.OKCancel:
							return swf.MessageBoxDefaultButton.Button2;
						case MessageBoxButtons.YesNoCancel:
							return swf.MessageBoxDefaultButton.Button3;
						default:
							throw new NotSupportedException();
					}
				default:
					throw new NotSupportedException();
			}
		}

		public static swf.MessageBoxButtons Convert(MessageBoxButtons buttons)
		{
			switch (buttons)
			{
				case MessageBoxButtons.OK: return swf.MessageBoxButtons.OK;
				case MessageBoxButtons.OKCancel: return swf.MessageBoxButtons.OKCancel;
				case MessageBoxButtons.YesNo: return swf.MessageBoxButtons.YesNo;
				case MessageBoxButtons.YesNoCancel: return swf.MessageBoxButtons.YesNoCancel;
				default:
					throw new NotSupportedException();
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
