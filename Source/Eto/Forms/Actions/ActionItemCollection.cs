using System;
using System.Collections.Generic;
using System.Linq;

namespace Eto.Forms
{
	[Obsolete("Use Command and menu/toolbar apis directly instead")]
	public partial class ActionItemCollection : List<IActionItem>
	{
		readonly ActionCollection actions;

		public Generator Generator
		{
			get { return actions.Generator; }
		}

		public ActionItemCollection(ActionCollection actions)
		{
			this.actions = actions;
		}

		public void Merge(ActionItemCollection items)
		{
			foreach (IActionItem actionItem in items)
			{
				var subMenu = actionItem as ActionItemSubMenu;
				if (subMenu != null)
				{
					ActionItemSubMenu currentSubMenu = GetSubmenu(subMenu.SubMenuText, subMenu.Order);
					currentSubMenu.Actions.Merge(subMenu.Actions);
				}
				else
					Add(actionItem);
			}
		}

		public void AddSeparator(int order = 500)
		{
			Add(new ActionItemSeparator { Order = order });
		}

		public void AddSpace(int order = 500)
		{
			Add(new ActionItemSeparator { Order = order, ToolBarType = SeparatorToolItemType.Space });
		}

		public void AddFlexibleSpace(int order = 500)
		{
			Add(new ActionItemSeparator { Order = order, ToolBarType = SeparatorToolItemType.FlexibleSpace });
		}

		public ActionItem Add(string actionID, int order = 500)
		{
			var action = actions[actionID];
			#if DEBUG
			if (action == null)
				Console.WriteLine("action {0} is not found", actionID);
			#endif
			return Add(action, order);
		}

		public ActionItem Add(string actionID, bool showLabel, int order = 500)
		{
			var action = actions[actionID];
			#if DEBUG
			if (action == null)
				Console.WriteLine("action {0} is not found", actionID);
			#endif
			return Add(action, showLabel, order);
		}

		public ActionItem Add(BaseAction action, int order = 500)
		{
			if (action != null)
			{
				var item = new ActionItem(action);
				item.Order = order;
				Add(item);
				return item;
			}
			return null;
		}

		[Obsolete("Use GetSubmenu instead")]
		public ActionItemSubMenu FindAddSubMenu(string subMenuText, int order = 500, bool plaintextMatch = false)
		{
			return GetSubmenu(subMenuText, order, plaintextMatch, true);
		}

		public ActionItemSubMenu GetSubmenu(string subMenuText, int order = 500, bool plaintextMatch = false, bool create = true)
		{
			// replace accelerators if plaintextMatch is true
			Func<string, string> convert = s => plaintextMatch ? s.Replace("&", "") : s;

			foreach (var item in this.OfType<ActionItemSubMenu>())
			{
				if (convert(item.SubMenuText) == convert(subMenuText))
				{
					return item;
				}
			}
			if (create)
			{
				var subMenu = new ActionItemSubMenu(actions, subMenuText);
				subMenu.Order = order;
				Add(subMenu);
				return subMenu;
			}
			return null;
		}

		public ActionItemSubMenu AddSubMenu(string subMenuText, int order = 500)
		{
			var sub = new ActionItemSubMenu(actions, subMenuText);
			sub.Order = order;
			Add(sub);
			return sub;
		}

		public ActionItem Add(BaseAction action, bool showLabel, int order = 500)
		{
			var item = new ActionItem(action, showLabel);
			item.Order = order;
			Add(item);
			return item;
		}

		int Compare(IActionItem x, IActionItem y)
		{
			int sectionx = x.Order;
			int sectiony = y.Order;
			if (sectionx == sectiony)
			{
				sectionx = IndexOf(x);
				sectiony = IndexOf(y);
			}
			return sectionx.CompareTo(sectiony);
		}

		public ToolBar GenerateToolBar()
		{
			var toolBar = new ToolBar(Generator);
			Generate(toolBar);
			return toolBar;
		}

		public void Generate(ToolBar toolBar)
		{
			var list = new List<IActionItem>(this);
			list.Sort(Compare);
			var lastSeparator = false;
			for (int i = 0; i < list.Count; i++)
			{
				var ai = list[i];
				var isSeparator = (ai is ActionItemSeparator);
				
				if ((lastSeparator && isSeparator) || (isSeparator && (i == 0 || i == list.Count - 1)))
					continue;

				var tbb = ai.GenerateToolBarItem(toolBar.Generator, toolBar.TextAlign);
				if (tbb != null)
					toolBar.Items.Add(tbb);
				lastSeparator = isSeparator;	
			}
		}

		internal void ExtractMenu(ISubMenuWidget menu)
		{
			foreach (var item in menu.Items)
			{
				var button = item as ButtonMenuItem;
				if (button != null && button.Items.Count > 0)
				{
					// submenu
					var subMenu = this.GetSubmenu(button.Text, button.Order, true);
					subMenu.Actions.ExtractMenu(button);
					continue;
				}
				var separator = item as SeparatorMenuItem;
				if (separator != null)
					AddSeparator(separator.Order);

				if (item.ID != null)
					Add(item.ID, item.Order);
			}
		}
	}
}
