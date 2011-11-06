using System;
using MonoTouch.UIKit;
using Eto.Forms;

namespace Eto.Platform.iOS.Forms
{
	public class MessageBoxHandler : WidgetHandler, IMessageBox
	{
		public DialogResult ShowDialog (Control parent)
		{
			var alert = new UIAlertView(Caption, Text, null, null);
			alert.Show ();
			return DialogResult.Ok;
		}

		public DialogResult ShowDialog (Control parent, MessageBoxButtons buttons)
		{
			var alert = new UIAlertView(Caption, Text, null, null);
			alert.Show ();
			return DialogResult.Ok;
		}

		public string Text { get; set; }

		public string Caption { get; set; }

		public MessageBoxType Type { get; set; }
	}
}

