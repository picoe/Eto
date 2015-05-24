using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Eto.Forms
{
	/// <summary>
	/// ToolBar item collection.
	/// </summary>
	public class ToolItemCollection : Collection<ToolItem>
	{
		readonly ToolBar parent;

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.ToolItemCollection"/> class.
		/// </summary>
		/// <param name="parent">Parent of the tool item.</param>
		protected internal ToolItemCollection(ToolBar parent)
		{
			this.parent = parent;
		}

		/// <summary>
		/// Add a <paramref name="command"/> with the specified <paramref name="order"/>.
		/// </summary>
		/// <param name="command">Command to add.</param>
		/// <param name="order">Order to add it at.</param>
		public void Add(Command command, int order = -1)
		{
			var item = command.CreateToolItem();
			item.Order = order;
			Add(item);
		}

		/// <summary>
		/// Adds the specified item given its order.
		/// </summary>
		/// <remarks>
		/// This will add the item into the collection based on its <see cref="ToolItem.Order"/>, keeping
		/// all items in their order.
		/// </remarks>
		/// <param name="item">Item to add.</param>
		public new void Add(ToolItem item)
		{
			var previousItem = this.Where(i => i.Order <= item.Order).OrderBy(i => i.Order).LastOrDefault();
			var previous = this.IndexOf(previousItem);

			Insert(previous + 1, item);
		}

		/// <summary>
		/// Adds the specified commands to the collection starting at the specified order.
		/// </summary>
		/// <param name="commands">Commands to add.</param>
		/// <param name="order">Order of the items to add.</param>
		public void AddRange(IEnumerable<Command> commands, int order = -1)
		{
			foreach (var command in commands)
			{
				Add(command, order);
			}
		}

		/// <summary>
		/// Adds the specified tool items to the collection.
		/// </summary>
		/// <param name="items">Items to add.</param>
		public void AddRange(IEnumerable<ToolItem> items)
		{
			foreach (var item in items)
			{
				Add(item);
			}
		}

		/// <summary>
		/// Adds a separator item with the specified <paramref name="order"/> or <paramref name="type"/>
		/// </summary>
		/// <param name="order">Order to add the separator.</param>
		/// <param name="type">Type of separator.</param>
		public void AddSeparator(int order = -1, SeparatorToolItemType type = SeparatorToolItemType.Divider)
		{
			Add(new SeparatorToolItem { Order = order, Type = type });
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
		/// Called when an item is inserted.
		/// </summary>
		/// <param name="index">Index of the item to insert.</param>
		/// <param name="item">Item to insert.</param>
		protected override void InsertItem(int index, ToolItem item)
		{
			base.InsertItem(index, item);
			parent.Handler.AddButton(item, index);
		}

		/// <summary>
		/// Called when an item is removed from the collection.
		/// </summary>
		/// <param name="index">Index of the item being removed.</param>
		protected override void RemoveItem(int index)
		{
			var item = this[index];
			base.RemoveItem(index);
			parent.Handler.RemoveButton(item);
		}
	}
}
