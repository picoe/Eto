using System;
using Eto.Forms;

namespace Eto.Test.UnitTests.Handlers
{
	public class TestWidgetHandler : Widget.IHandler
	{
		protected object Callback
		{
			get { return ((ICallbackSource)Widget).Callback; }
		}

		public string ID { get; set; }

		public Widget Widget { get; set; }

		public void Initialize()
		{
		}

		public void HandleEvent(string id, bool defaultEvent = false)
		{
		}
	}
	
}
