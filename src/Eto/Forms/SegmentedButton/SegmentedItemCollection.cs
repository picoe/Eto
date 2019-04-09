using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Eto.Forms
{
	/// <summary>
	/// Collection of <see cref="SegmentedItem"/> objects for the <see cref="SegmentedButton"/>.
	/// </summary>
    public class SegmentedItemCollection : Collection<SegmentedItem>
    {
        SegmentedButton Parent { get; }
		SegmentedButton.IHandler Handler => Parent.Handler as SegmentedButton.IHandler;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Eto.Forms.SegmentedItemCollection"/> class.
		/// </summary>
		/// <param name="parent">Parent.</param>
		internal SegmentedItemCollection(SegmentedButton parent)
        {
            Parent = parent;
        }

		/// <summary>
		/// Clears the items.
		/// </summary>
        protected override void ClearItems()
        {
			foreach (var item in this)
			{
				item.Parent = null;
			}
			base.ClearItems();
            Handler.ClearItems();
        }

		/// <summary>
		/// Inserts the item at the specified index.
		/// </summary>
		/// <param name="index">Index to insert at.</param>
		/// <param name="item">Item to insert.</param>
		protected override void InsertItem(int index, SegmentedItem item)
        {
            base.InsertItem(index, item);
            Handler.InsertItem(index, item);
			item.Parent = Parent;
		}

		/// <summary>
		/// Removes the item at the specified index.
		/// </summary>
		/// <param name="index">Index to remove.</param>
		protected override void RemoveItem(int index)
        {
            var item = this[index];
			Handler.RemoveItem(index, item);
            base.RemoveItem(index);
			item.Parent = null;
        }

		/// <summary>
		/// Sets the item at the specified index, replacing its existing item.
		/// </summary>
		/// <param name="index">Index to replace at.</param>
		/// <param name="item">Item to replace with.</param>
		protected override void SetItem(int index, SegmentedItem item)
        {
			this[index].Parent = null;
            base.SetItem(index, item);
            Handler.SetItem(index, item);
			item.Parent = Parent;
        }

		/// <summary>
		/// Adds an enumerable of items to the collection
		/// </summary>
		/// <param name="items">Items to add.</param>
		public void AddRange(IEnumerable<SegmentedItem> items)
		{
			foreach (var item in items)
				Add(item);
		}
    }
}
