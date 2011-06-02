using System;
using System.Collections;

namespace Eto.Forms
{
	public interface IMenu : IInstanceWidget
	{
		void AddMenu(int index, MenuItem item);
		void RemoveMenu(MenuItem item);
		void Clear();
	}

	public abstract class Menu : InstanceWidget
	{
		MenuItemCollection items;
		IMenu inner;

		protected Menu(Generator g, Type type) : base(g, type)
		{
			items = new MenuItemCollection(this);
			inner = (IMenu)base.Handler;
		}

		internal protected void AddMenu(int index, MenuItem item)
		{
			inner.AddMenu(index, item);
		}

		internal protected void RemoveMenu(MenuItem item)
		{
			inner.RemoveMenu(item);
		}

		internal protected void Clear()
		{
			inner.Clear();
		}

		public MenuItemCollection MenuItems
		{
			get { return items; }
		}

		public void GenerateActions(ActionItemCollection actionItems)
		{
			foreach (IActionItem ai in actionItems)
			{
				ai.Generate(this);
			}
		}

	}
}
