using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if DESKTOP
using System.Xaml;
#endif

namespace Eto
{
#if DESKTOP
	/// <summary>
	/// Attachable property storage for xaml
	/// </summary>
	/// <remarks>
	/// This is used as a storage for xaml property values.
	/// </remarks>
	public class PropertyStore : IAttachedPropertyStore
	{
		IDictionary<AttachableMemberIdentifier, object> attachedProperties = new Dictionary<AttachableMemberIdentifier, object> ();

		/// <summary>
		/// Gets an attachable property value, using a default if not set to a value
		/// </summary>
		/// <typeparam name="T">Type of the property value to get</typeparam>
		/// <param name="member">Member to retrieve</param>
		/// <param name="defaultValue">Default value to use if no value is stored</param>
		/// <returns>Value of the attached property, or <paramref name="defaultValue"/> if not set</returns>
		public T Get<T> (AttachableMemberIdentifier member, T defaultValue)
		{
			object value;
			if (attachedProperties.TryGetValue (member, out value))
				return (T)value;
			else
				return defaultValue;
		}

		/// <summary>
		/// Gets an attachable property value
		/// </summary>
		/// <typeparam name="T">Type of the property value to get</typeparam>
		/// <param name="member">Member to retrieve</param>
		/// <returns>Value of the attached property</returns>
		public T Get<T> (AttachableMemberIdentifier member)
		{
			object value;
			if (attachedProperties.TryGetValue (member, out value))
				return (T)value;
			else
				return default (T);
		}

		/// <summary>
		/// Sets an attachable property value
		/// </summary>
		/// <param name="member">Member to set</param>
		/// <param name="value">Value to set to the property</param>
		public void Set (AttachableMemberIdentifier member, object value)
		{
			attachedProperties[member] = value;
		}

		/// <summary>
		/// Removes the attachable property value
		/// </summary>
		/// <param name="member">Member to remove</param>
		/// <returns>True if the value was removed, false if it was not</returns>
		public bool Remove (AttachableMemberIdentifier member)
		{
			return attachedProperties.Remove (member);
		}

		void IAttachedPropertyStore.CopyPropertiesTo (KeyValuePair<AttachableMemberIdentifier, object>[] array, int index)
		{
			attachedProperties.CopyTo (array, index);
		}

		int IAttachedPropertyStore.PropertyCount { get { return attachedProperties.Count; } }

		bool IAttachedPropertyStore.RemoveProperty (AttachableMemberIdentifier member)
		{
			return attachedProperties.Remove (member);
		}

		void IAttachedPropertyStore.SetProperty (AttachableMemberIdentifier member, object value)
		{
			attachedProperties[member] = value;
		}

		bool IAttachedPropertyStore.TryGetProperty (AttachableMemberIdentifier member, out object value)
		{
			return attachedProperties.TryGetValue (member, out value);
		}
	}
#endif
}
