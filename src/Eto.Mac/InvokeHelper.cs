using System;
namespace Eto.Mac
{
		class InvokeHelper
		{
			public Delegate Delegate { get; set; }
			public object[] Args { get; set; }
			public object Return { get; private set; }
			
			public void Action()
			{
				Return = Delegate.DynamicInvoke(Args);
			}
		}
}

