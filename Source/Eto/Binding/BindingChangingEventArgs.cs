using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.ObjectModel;

namespace Eto
{
	
	public class BindingChangingEventArgs : CancelEventArgs
	{
		public object Value { get; set; }
		
		public BindingChangingEventArgs (object value)
		{
			this.Value = value;
		}
	}
	
}
