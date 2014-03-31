using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace Eto.Forms
{
	public class ToolItemCollection : Collection<ToolItem>
	{
		readonly ToolBar parent;

		protected internal ToolItemCollection(ToolBar parent)
		{
			this.parent = parent;
		}

		protected override void InsertItem(int index, ToolItem item)
		{
			base.InsertItem(index, item);
			parent.Handler.AddButton(item, index);
		}

		protected override void RemoveItem(int index)
		{
			var item = this[index];
			base.RemoveItem(index);
			parent.Handler.RemoveButton(item);
		}

		protected override void ClearItems()
		{
			base.ClearItems();
			parent.Handler.Clear();
		}

		public new void Add(ToolItem item)
		{
			int previousIndex = -1;
			for (var i = 0; i < Count; ++i)
			{
				if (this[i].Order <= item.Order)
					previousIndex = i;
				else
					break;
			}
			Insert(previousIndex + 1, item);
		}

		public void Add(Command command, int order = -1)
		{
			var item = command.CreateToolItem(parent.Generator);
			item.Order = order;
			Add(item);
		}

		public void AddSeparator(int order = -1, SeparatorToolItemType type = SeparatorToolItemType.Divider)
		{
			Add(new SeparatorToolItem(parent.Generator) { Order = order, Type = type });
		}

		public void AddRange(IEnumerable<ToolItem> items)
		{
			foreach (var item in items)
			{
				Add(item);
			}
		}

		public void AddRange(IEnumerable<Command> commands, int order = -1)
		{
			foreach (var command in commands)
			{
				Add(command, order);
			}
		}
	}
}
