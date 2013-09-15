using System;
using Eto.Forms;
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.ObjCRuntime;

namespace Eto.Platform.Mac.Forms
{
	public class MessageBoxHandler : WidgetHandler<Widget>, IMessageBox
	{
		public string Text { get; set; }

		public string Caption { get; set; }

		public MessageBoxType Type { get; set; }

		public MessageBoxButtons Buttons { get; set; }

		public MessageBoxDefaultButton DefaultButton { get; set; }

		void Ended()
		{
			NSApplication.SharedApplication.StopModal();
		}

		public DialogResult ShowDialog(Control parent)
		{
			var alert = new NSAlert();

			AddButtons(alert);

			alert.AlertStyle = Convert(this.Type);
			alert.MessageText = Caption ?? string.Empty;
			alert.InformativeText = Text ?? string.Empty;
			var ret = MacModal.Run(alert, parent);
			switch (Buttons)
			{
				default:
				case MessageBoxButtons.OK:
					return DialogResult.Ok;
				case MessageBoxButtons.OKCancel:
					return (ret == 1000) ? DialogResult.Ok : DialogResult.Cancel;
				case MessageBoxButtons.YesNo:
					return (ret == 1000) ? DialogResult.Yes : DialogResult.No;
				case MessageBoxButtons.YesNoCancel:
					return (ret == 1000) ? DialogResult.Yes : (ret == 1001) ? DialogResult.Cancel : DialogResult.No;
			}
		}

		void AddButtons(NSAlert alert)
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
								ok.BecomeFirstResponder();
								break;
							case MessageBoxDefaultButton.Cancel:
							case MessageBoxDefaultButton.Default:
								cancel.BecomeFirstResponder();
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
								yes.BecomeFirstResponder();
								break;
							case MessageBoxDefaultButton.No:
							case MessageBoxDefaultButton.Default:
								no.BecomeFirstResponder();
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
								yes.BecomeFirstResponder();
								break;
							case MessageBoxDefaultButton.No:
								no.BecomeFirstResponder();
								break;
							case MessageBoxDefaultButton.Cancel:
							case MessageBoxDefaultButton.Default:
								cancel.BecomeFirstResponder();
								break;
						}
					}
					break;
			}
		}

		NSAlertStyle Convert(MessageBoxType type)
		{
			switch (type)
			{
				case MessageBoxType.Information:
				case MessageBoxType.Question:
					return NSAlertStyle.Informational;
				case MessageBoxType.Warning:
					return NSAlertStyle.Warning;
				case MessageBoxType.Error:
					return NSAlertStyle.Critical;
				default:
					throw new NotSupportedException();
			}
		}
	}
}
