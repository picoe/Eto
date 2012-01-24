using System;
using Eto.Forms;
using System.Reflection;
using System.Text;

namespace Eto.Test.Interface.Controls
{
	public class SectionBase : Panel
	{
		public TextArea EventLog { get; set; }
		
		interface ILogWrapper {
			SectionBase Section { get; set; }
			
			EventInfo Event { get; set; }
			
			string Message { get; set; }
			
		}
		
		class LogWrapper<T> : ILogWrapper
			where T: EventArgs
		{
			public SectionBase Section { get; set; }
			
			public EventInfo Event { get; set; }
			
			public string Message { get; set; }
			
			public void LogEvent (object sender, T e)
			{
				var sb = new StringBuilder();
				if (!string.IsNullOrEmpty (Message))
					sb.AppendFormat("{0}, ", Message);
				sb.AppendFormat ("Event: {0}", Event.Name);
				if (e.GetType () != typeof(EventArgs))
					sb.AppendFormat (", Args: {0}", e);
				
				Section.Log (sender, sb.ToString ());
			}
		}
		
		protected void LogEvents (object control, string message, params string[] eventNames)
		{
			var type = control.GetType ();
			foreach (var evt in eventNames) {
				LogEvent (control, evt, message);
			}
		}

		protected void LogEvents (object control, params string[] eventNames)
		{
			var type = control.GetType ();
			foreach (var evt in eventNames) {
				LogEvent (control, evt, string.Empty);
			}
		}
		
		private Type[] GetDelegateParameterTypes (Type d)
		{
			if (d.BaseType != typeof(MulticastDelegate))
				throw new ApplicationException ("Not a delegate.");

			MethodInfo invoke = d.GetMethod ("Invoke");
			if (invoke == null)
				throw new ApplicationException ("Not a delegate.");

			ParameterInfo[] parameters = invoke.GetParameters ();
			Type[] typeParameters = new Type[parameters.Length];
			for (int i = 0; i < parameters.Length; i++) {
				typeParameters [i] = parameters [i].ParameterType;
			}
			return typeParameters;
		}

		protected void LogEvent (object control, string eventName, string message)
		{
			var type = control.GetType ();
			var eventInfo = type.GetEvent (eventName);
			if (eventInfo == null)
				throw new ArgumentException (string.Format ("event {0} not found", eventName));
			
			var args = GetDelegateParameterTypes(eventInfo.EventHandlerType);
			if (args.Length != 2)
				throw new ArgumentException("Event has incorrect number of arguments. You must event manually");
			var wrapperGenericType = typeof(LogWrapper<>);
			var wrapperType = wrapperGenericType.MakeGenericType (args[1]);
			var wrapper = (ILogWrapper)Activator.CreateInstance (wrapperType);
			wrapper.Section = this;
			wrapper.Event = eventInfo;
			wrapper.Message = message;

			eventInfo.AddEventHandler (control, Delegate.CreateDelegate (eventInfo.EventHandlerType, wrapper, "LogEvent"));
		}

		public void Log (object sender, string message, params object[] args)
		{
			if (EventLog == null)
				return;

			var sb = new StringBuilder();	
			sb.AppendFormat("[{0:HH:mm:ss}] ", DateTime.Now);
			if (sender != null)
				sb.AppendFormat ("Sender: {0}, ", sender);
			sb.AppendFormat (message, args);
			sb.Append ("\n");
				
			EventLog.Append (sb.ToString (), true);
		}
	}
}

