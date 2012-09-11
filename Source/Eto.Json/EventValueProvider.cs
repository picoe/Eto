using System;
using System.IO;
using json = Newtonsoft.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;
using System.Runtime.Serialization;

namespace Eto.Json
{
	class EventValueProvider : IValueProvider
	{
		public EventInfo EventInfo { get; set; }
		
		public void SetValue (object target, object value)
		{
			if (value != null)
				EventInfo.AddEventHandler (target, (Delegate)value);
		}
		
		public object GetValue (object target)
		{
			return null;
		}
	}
	
}
