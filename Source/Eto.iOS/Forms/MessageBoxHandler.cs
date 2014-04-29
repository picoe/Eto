using System;
using MonoTouch.UIKit;
using Eto.Forms;

namespace Eto.iOS.Forms
{
	public class MessageBoxHandler : WidgetHandler<Widget>, IMessageBox
	{
		public DialogResult ShowDialog (Control parent)
		{
			var alert = new UIAlertView(Caption ?? string.Empty, Text ?? string.Empty, null, "Ok");
			alert.Show ();
			return DialogResult.Ok;
		}

		public string Text { get; set; }

		public string Caption { get; set; }

		public MessageBoxType Type { get; set; }

		public MessageBoxButtons Buttons { get; set; }

		public MessageBoxDefaultButton DefaultButton { get; set; }
	}
}

