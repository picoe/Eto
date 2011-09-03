using System;
using Eto.Forms;
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.ObjCRuntime;

namespace Eto.Platform.Mac.Forms
{
	public class MessageBoxHandler : IMessageBox
	{
		NSAlert alert;
     
		public string Text { get; set; }

		public string Caption { get; set; }

		public IWidget Handler { get; set; }
		
		public MessageBoxType Type { get; set; }
     
		public void Initialize ()
		{
		}
     
		void Ended ()
		{
			NSApplication.SharedApplication.StopModal ();
		}

		public DialogResult ShowDialog (Control parent)
		{
			alert = new NSAlert ();
			RunDialog (parent, alert);
			return DialogResult.Ok;
		}
     
		int RunDialog (Control parent, NSAlert alert)
		{
			alert.MessageText = Caption ?? string.Empty;
			alert.InformativeText = Text ?? string.Empty;
			return MacModal.Run (alert, parent);
		}

		public DialogResult ShowDialog (Control parent, MessageBoxButtons buttons)
		{
			var alert = new NSAlert ();
         
			var OkButton = "OK";
			var CancelButton = "Cancel";
			var YesButton = "Yes";
			var NoButton = "No";
         
			switch (buttons) {
			case MessageBoxButtons.OK:
				alert.AddButton (OkButton);
				break;
			case MessageBoxButtons.OKCancel:
				alert.AddButton (OkButton);
				alert.AddButton (CancelButton);
				break;
			case MessageBoxButtons.YesNo:
				alert.AddButton (YesButton);
				alert.AddButton (NoButton);
				break;
			case MessageBoxButtons.YesNoCancel:
				alert.AddButton (YesButton);
				alert.AddButton (CancelButton);
				alert.AddButton (NoButton);
				break;
			}
			alert.AlertStyle = Convert (this.Type);
			int ret = RunDialog (parent, alert);
			switch (buttons) {
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
		
		NSAlertStyle Convert (MessageBoxType type)
		{
			switch (type) {
			case MessageBoxType.Information:
			case MessageBoxType.Question:
				return NSAlertStyle.Informational;
			case MessageBoxType.Warning:
				return NSAlertStyle.Warning;
			case MessageBoxType.Error:
				return NSAlertStyle.Critical;
			default:
				throw new NotSupportedException ();
			}
		}
		
	}
}
