using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.ObjectModel;

namespace Eto
{
	
	public class BindingChangedEventArgs : EventArgs
	{
		public object Value { get; private set; }
		
		public BindingChangedEventArgs (object value)
		{
			this.Value = value;
		}
	}
	
}
