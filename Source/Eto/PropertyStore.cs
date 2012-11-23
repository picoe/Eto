using System;
using System.Collections.Generic;

#if XAML
using System.Xaml;
#endif

namespace Eto
{
	/// <summary>
	/// Attachable property storage for xaml
	/// </summary>
	/// <remarks>
	/// This is used as a storage for xaml property values.
	/// </remarks>
	public class PropertyStore : Dictionary<AttachableMemberIdentifier, object>
	{
		/// <summary>
		/// Gets the parent object of this store
		/// </summary>
		public object Parent { get; private set; }

		/// <summary>
		/// Initializes a new instance of the PropertyStore class
		/// </summary>
		/// <param name="parent">Object that contains this property store</param>
		public PropertyStore (object parent)
		{
			this.Parent = parent;
		}

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
			if (this.TryGetValue (member, out value))
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
			if (this.TryGetValue (member, out value))
				return (T)value;
			else
				return default (T);
		}
	}
}
