using System;
using System.Drawing;

namespace Eto.Forms.WXWidgets
{
	internal class MessageBoxHandler : IMessageBox	
	{
		string text = string.Empty;
		string caption = string.Empty;

		public MessageBoxHandler(Widget widget)
		{
		}

		#region IMessageBox Methods

		public void ShowDialog(Control parent)
		{
			if (parent != null) wx.MessageDialog.ShowModal((wx.Window)parent.ControlObject, text, caption, 0);
			else wx.MessageDialog.ShowModal(text, caption, 0);
		}

		public string Text
		{
			get { return text; }
			set { text = value; }
		}

		public string Caption
		{
			get { return caption; }
			set { caption = value; }
		}
		#endregion
	}

}
