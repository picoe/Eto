using Newtonsoft.Json.Serialization;
namespace Eto.Serialization.Json
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
