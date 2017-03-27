using System;
using System.Text;
using Eto.Forms;
using System.Diagnostics;

namespace Eto.Test
{
	public static class Log
	{
		static StringBuilder s_deferredLog;

		public static void Write(object sender, string message, params object[] args)
		{
			var sb = new StringBuilder();
			sb.AppendFormat("[{0:HH:mm:ss}] ", DateTime.Now);
			if (sender != null)
				sb.AppendFormat("Sender: {0}, ", sender);
			if (args?.Length > 0)
				sb.AppendFormat(message, args);
			else
				sb.Append(message);
			sb.Append("\n");

			if (Platform.Instance.IsDesktop)
			{
				var form = Application.Instance?.MainForm as MainForm;
				var eventLog = form?.EventLog;
				if (eventLog != null)
				{
					eventLog.Append(sb.ToString(), true);
					return;
				}

				if (Application.Instance != null)
				{
					if (s_deferredLog == null)
					{
						// so we can show log events before the main form is shown
						Application.Instance.Initialized += (s2, e) =>
						{
							var eventLog2 = (Application.Instance.MainForm as MainForm)?.EventLog;
							if (eventLog2 != null)
							{
								eventLog2.Append(s_deferredLog.ToString(), true);
								s_deferredLog.Clear();
							}
						};
						s_deferredLog = new StringBuilder();
					}
					s_deferredLog.Append(sb.ToString());
				}
			}

			Debug.WriteLine(sb.ToString());
		}
	}
}
