using System;
using System.Text;
using Eto.Forms;
using System.Diagnostics;

namespace Eto.Test
{
	public static class Log
	{
		public static void Write(object sender, string message, params object[] args)
		{
			var sb = new StringBuilder();
			sb.AppendFormat("[{0:HH:mm:ss}] ", DateTime.Now);
			if (sender != null)
				sb.AppendFormat("Sender: {0}, ", sender);
			sb.AppendFormat(message, args);
			sb.Append("\n");

			if (Platform.Instance.IsDesktop)
			{
				var form = Application.Instance != null ? Application.Instance.MainForm as MainForm : null;
				if (form == null)
					return;
			
				var eventLog = form.EventLog;
				if (eventLog == null)
					return;
				eventLog.Append(sb.ToString(), true);
			}
			else
			{
				Debug.WriteLine(sb.ToString());
			}
		}
	}
}
