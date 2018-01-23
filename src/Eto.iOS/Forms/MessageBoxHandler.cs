using System;
using UIKit;
using Eto.Forms;

namespace Eto.iOS.Forms
{
	public class MessageBoxHandler : WidgetHandler<Widget>, MessageBox.IHandler
	{
		public DialogResult ShowDialog (Control parent)
		{
			var alert = new UIAlertView(Caption ?? string.Empty, Text ?? string.Empty, null, null);
			AddButtons(alert);
			alert.Show ();
			return DialogResult.Ok;
		}

		void AddButtons(UIAlertView alert)
		{
			var OkButton = "OK";
			var CancelButton = "Cancel";
			var YesButton = "Yes";
			var NoButton = "No";

			switch (Buttons)
			{
				case MessageBoxButtons.OK:
					alert.AddButton(OkButton);
					break;
				case MessageBoxButtons.OKCancel:
					{
						var ok = alert.AddButton(OkButton);
						var cancel = alert.AddButton(CancelButton);
						switch (DefaultButton)
						{
							case MessageBoxDefaultButton.OK:
								alert.CancelButtonIndex = ok;
								break;
							case MessageBoxDefaultButton.Cancel:
							case MessageBoxDefaultButton.Default:
								alert.CancelButtonIndex = cancel;
								break;
						}
					}
					break;
				case MessageBoxButtons.YesNo:
					{
						var yes = alert.AddButton(YesButton);
						var no = alert.AddButton(NoButton);
						switch (DefaultButton)
						{
							case MessageBoxDefaultButton.Yes:
								alert.CancelButtonIndex = yes;
								break;
							case MessageBoxDefaultButton.No:
							case MessageBoxDefaultButton.Default:
								alert.CancelButtonIndex = no;
								break;
						}
					}
					break;
				case MessageBoxButtons.YesNoCancel:
					{
						var yes = alert.AddButton(YesButton);
						var cancel = alert.AddButton(CancelButton);
						var no = alert.AddButton(NoButton);
						switch (DefaultButton)
						{
							case MessageBoxDefaultButton.Yes:
								alert.CancelButtonIndex = yes;
								break;
							case MessageBoxDefaultButton.No:
								alert.CancelButtonIndex = no;
								break;
							case MessageBoxDefaultButton.Cancel:
							case MessageBoxDefaultButton.Default:
								alert.CancelButtonIndex = cancel;
								break;
						}
					}
					break;
			}
		}


		public string Text { get; set; }

		public string Caption { get; set; }

		public MessageBoxType Type { get; set; }

		public MessageBoxButtons Buttons { get; set; }

		public MessageBoxDefaultButton DefaultButton { get; set; }
	}
}

