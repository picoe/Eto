using System;

using Eto.Forms;

using SWU = System.Web.UI;

namespace Eto.Platform.Web.Forms
{
	public class MessageBoxHandler : IMessageBox
	{
		
		public DialogResult ShowDialog(Control parent, MessageBoxButtons buttons)
		{
			return DialogResult.None;
		}
		
		string text = string.Empty;
		string caption = string.Empty;
		
		
		static MessageBoxHandler()
		{
		}
		
		public MessageBoxHandler(Widget widget)
		{
		}
		
		public string Text
		{
			get
			{ return text; }
			set
			{ text = value; }
		}
		
		public string Caption
		{
			get
			{ return caption; }
			set
			{ caption = value; }
		}
		
		
		public DialogResult ShowDialog(Control parent)
		{
			return DialogResult.None;
		}
	}
}

