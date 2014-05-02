using System;
using System.Text;
using Eto.Forms;

namespace Eto.Test
{
	public static class Log
	{
		public static void Write (object sender, string message, params object[] args)
		{
			var sb = new StringBuilder ();
			sb.AppendFormat ("[{0:HH:mm:ss}] ", DateTime.Now);
			if (sender != null)
				sb.AppendFormat ("Sender: {0}, ", sender);
			sb.AppendFormat (message, args);
			sb.Append ("\n");

#if DESKTOP
			var form = Application.Instance.MainForm as MainForm;
			if (form == null) 
				return;
			
			var eventLog = form.EventLog;
			if (eventLog == null)
				return;
			eventLog.Append (sb.ToString (), true);
#else
			Console.WriteLine (sb.ToString ());
#endif
		}
	}
}
