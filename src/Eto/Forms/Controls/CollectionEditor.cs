using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Eto.Forms
{
	/// <summary>
	/// Control to edit a collection of objects
	/// </summary>
	/// <remarks>
	/// This control allows the user to edit the specified <see cref="CollectionEditor.DataStore"/> by adding
	/// and removing entries of <see cref="ElementType"/>.
	/// </remarks>
	[Handler(typeof(IHandler))]
	public class CollectionEditor : Control
	{
		new IHandler Handler => (IHandler)base.Handler;

		/// <summary>
		/// Data store of the items to edit
		/// </summary>
		public IEnumerable<object> DataStore
		{
			get => Handler.DataStore;
			set => Handler.DataStore = value;
		}

		/// <summary>
		/// Gets or sets the type of element to create when adding new elements to the data store
		/// </summary>
		public Type ElementType
		{
			get => Handler.ElementType;
			set => Handler.ElementType = value;
		}

		/// <summary>
		/// Handler for the <see cref="CollectionEditor"/>.
		/// </summary>
		public new interface IHandler : Control.IHandler
		{
			/// <summary>
			/// Data store of the items to edit
			/// </summary>
			IEnumerable<object> DataStore { get; set; }

			/// <summary>
			/// Gets or sets the type of element to create when adding new elements to the data store
			/// </summary>
			Type ElementType { get; set; }
		}
	}
}
