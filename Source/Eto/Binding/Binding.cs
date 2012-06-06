using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.ObjectModel;

namespace Eto
{
	public abstract class Binding
	{
		public virtual void Unbind ()
		{
		}
		
		public virtual void Update ()
		{
		}

		protected virtual void HandleEvent (string handler)
		{
#if DEBUG
			throw new EtoException(string.Format ("This binding does not support the {0} event", handler));
#endif
		}
		
		protected virtual void RemoveEvent (string handler)
		{
		}
	}
}

