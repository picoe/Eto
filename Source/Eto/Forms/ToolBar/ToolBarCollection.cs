using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Eto.Forms
{
	/// <summary>
	/// ToolBarView item collection.
	/// </summary>
	/// <copyright>(c) 2015 by Nicolas Pöhlmann</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class ToolBarCollection : Collection<ToolBar>
	{
		readonly ToolBarView parent;

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.ToolBarCollection"/> class.
		/// </summary>
		/// <param name="parent">Parent of the tool bar.</param>
		protected internal ToolBarCollection(ToolBarView parent)
		{
			this.parent = parent;
		}

		/// <summary>
		/// Adds the specified item given its order.
		/// </summary>
		/// <remarks>
		/// This will add the item into the collection based on its <see cref="ToolBar.Order"/>, keeping
		/// all items in their order.
		/// </remarks>
		/// <param name="toolbar">Tool bar to add.</param>
		public new void Add(ToolBar toolbar)
		{
			var previousItem = this.Where(c => c.Order <= toolbar.Order).OrderBy(c => c.Order).LastOrDefault();
			var previous = this.IndexOf(previousItem);

			Insert(previous + 1, toolbar);
		}

		/// <summary>
		/// Adds the specified tool bars to the collection.
		/// </summary>
		/// <param name="toolbars">Tool bar to add.</param>
		public void AddRange(IEnumerable<ToolBar> toolbars)
		{
			foreach (var toolbar in toolbars)
			{
				Add(toolbar);
			}
		}

		/// <summary>
		/// Called when the collection is cleared.
		/// </summary>
		protected override void ClearItems()
		{
			base.ClearItems();
			parent.Handler.Clear();
		}

		/// <summary>
		/// Called when an item is inserted.
		/// </summary>
		/// <param name="index">Index of the item to insert.</param>
		/// <param name="toolbar">Item to insert.</param>
		protected override void InsertItem(int index, ToolBar toolbar)
		{
			base.InsertItem(index, toolbar);
			parent.Handler.AddToolBar(toolbar, index);
		}

		/// <summary>
		/// Called when an item is removed from the collection.
		/// </summary>
		/// <param name="index">Index of the item being removed.</param>
		protected override void RemoveItem(int index)
		{
			var toolbar = this[index];
			base.RemoveItem(index);
			parent.Handler.RemoveToolBar(toolbar);
		}
	}
}
