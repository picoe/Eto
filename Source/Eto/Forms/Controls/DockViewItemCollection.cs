using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Eto.Forms
{
	/// <summary>
	/// Contains items for the <see cref="DockView"/>.
	/// </summary>
	/// <copyright>(c) 2015 by Nicolas Pöhlmann</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class DockViewItemCollection : Collection<DockViewItem>
	{
		readonly DockView parent;

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.DockViewItemCollection"/> class.
		/// </summary>
		/// <param name="parent">Parent of the control.</param>
		protected internal DockViewItemCollection(DockView parent)
		{
			this.parent = parent;
		}
		
		/// <summary>
		/// Adds the specified item given its order.
		/// </summary>
		/// <remarks>
		/// This will add the item into the collection based on its <see cref="DockViewItem.Order"/>, keeping
		/// all items in their order.
		/// </remarks>
		/// <param name="item"><see cref="DockViewItem"/> to add.</param>
		public new void Add(DockViewItem item)
		{
			var previousItem = this.Where(i => i.Order <= item.Order).OrderBy(i => i.Order).LastOrDefault();
			var previous = this.IndexOf(previousItem);

			Insert(previous + 1, item);
		}

		/// <summary>
		/// Adds the specified controls to the collection.
		/// </summary>
		/// <param name="items"><see cref="DockViewItem"/>s to add.</param>
		public void AddRange(IEnumerable<DockViewItem> items)
		{
			foreach (var item in items)
			{
				Add(item);
			}
		}

		/// <summary>
		/// Called when the collection should be cleared.
		/// </summary>
		protected override void ClearItems()
		{
			base.ClearItems();
			parent.Handler.Clear();
		}

		/// <summary>
		/// Called when a control is inserted.
		/// </summary>
		/// <param name="index">Index of the control to insert.</param>
		/// <param name="item"><see cref="DockViewItem"/> to insert.</param>
		protected override void InsertItem(int index, DockViewItem item)
		{
			base.InsertItem(index, item);
			parent.Handler.AddItem(item, index);
		}

		/// <summary>
		/// Called when a control is removed from the collection.
		/// </summary>
		/// <param name="index">Index of the <see cref="DockViewItem"/> being removed.</param>
		protected override void RemoveItem(int index)
		{
			var item = this[index];
			base.RemoveItem(index);
			parent.Handler.RemoveItem(item);
		}
	}
}
