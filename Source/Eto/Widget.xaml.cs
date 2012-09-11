#if XAML2
using System;
using System.Xaml;
using System.Collections.Generic;

namespace Eto
{
	partial class Widget : IAttachedPropertyStore
	{
		#region IAttachedPropertyStore implementation
		
		void IAttachedPropertyStore.CopyPropertiesTo (KeyValuePair<AttachableMemberIdentifier, object>[] array, int index)
		{
			((IDictionary<AttachableMemberIdentifier, object>)this.Properties).CopyTo(array, index);
		}
		
		bool IAttachedPropertyStore.RemoveProperty (AttachableMemberIdentifier attachableMemberIdentifier)
		{
			return this.Properties.Remove (attachableMemberIdentifier);
		}

		void IAttachedPropertyStore.SetProperty (AttachableMemberIdentifier attachableMemberIdentifier, object value)
		{
			this.Properties[attachableMemberIdentifier] = value;
			
		}

		bool IAttachedPropertyStore.TryGetProperty (AttachableMemberIdentifier attachableMemberIdentifier, out object value)
		{
			return this.Properties.TryGetValue (attachableMemberIdentifier, out value);
		}

		int IAttachedPropertyStore.PropertyCount
		{
			get { return this.Properties.Count; }
		}
		
		#endregion
	}
}
#endif

