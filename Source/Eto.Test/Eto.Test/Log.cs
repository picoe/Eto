using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eto.Test
{
	public static class Log
	{
		public static void Write (object sender, string message, params object[] args)
		{
			var form = TestApplication.Instance.MainForm as MainForm;
			if (form == null) 
				return;

			var eventLog = form.EventLog;
			if (eventLog == null)
				return;

			var sb = new StringBuilder ();
			sb.AppendFormat ("[{0:HH:mm:ss}] ", DateTime.Now);
			if (sender != null)
				sb.AppendFormat ("Sender: {0}, ", sender);
			sb.AppendFormat (message, args);
			sb.Append ("\n");

			eventLog.Append (sb.ToString (), true);
		}
	}
}
