using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.ObjectModel;

namespace Eto
{
	public class BindingCollection : Collection<Binding>
	{
		public void Unbind ()
		{
			foreach (var binding in this) {
				binding.Unbind ();
			}
		}
		
		public void Update ()
		{
			foreach (var binding in this) {
				binding.Update ();
			}
		}
	}
	
}
