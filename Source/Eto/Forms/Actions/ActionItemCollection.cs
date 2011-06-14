using System;
using System.Collections;
using System.Collections.Generic;

namespace Eto.Forms
{
	public partial class ActionItemCollection : List<IActionItem>
	{
		ActionCollection actions;
		
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
				ActionItemSubMenu subMenu = actionItem as ActionItemSubMenu;
				if (subMenu != null)
				{
					ActionItemSubMenu currentSubMenu = FindAddSubMenu(subMenu.SubMenuText);
					currentSubMenu.Actions.Merge(subMenu.Actions);
				}
				else Add(actionItem);
			}
		}

		public void AddSeparator(int order = 500)
		{
			Add(new ActionItemSeparator{ Order = order });
		}

		public void AddSpace(int order = 500)
		{
			Add(new ActionItemSeparator{ Order = order, ToolBarType = SeparatorToolBarItemType.Space });
		}

		public void AddFlexibleSpace(int order = 500)
		{
			Add(new ActionItemSeparator{ Order = order, ToolBarType = SeparatorToolBarItemType.FlexibleSpace });
		}
		
		public ActionItem Add(string actionID, int order = 500)
		{
			var action = actions.Find(actionID);
			#if DEBUG
			if (action == null) Console.WriteLine("action {0} is not found", actionID);
			#endif
			return Add(action, order);
		}

		public ActionItem Add(string actionID, bool showLabel, int order = 500)
		{
			var action = actions.Find(actionID);
			#if DEBUG
			if (action == null) Console.WriteLine("action {0} is not found", actionID);
			#endif
			return Add(action, showLabel, order);
		}
		
		public ActionItem Add(BaseAction action, int order = 500)
		{
			if (action != null)
			{
				ActionItem item = new ActionItem(action);
				item.Order = order;
				this.Add(item);
				return item;
			}
			else return null;
		}

		public ActionItemSubMenu FindAddSubMenu(string subMenuText, int order = 500)
		{
			ActionItemSubMenu subMenu = null;
			foreach (IActionItem item in this)
			{
				if (item is ActionItemSubMenu && ((ActionItemSubMenu)item).SubMenuText == subMenuText)
				{
					subMenu = ((ActionItemSubMenu)item);
				}
			}
			if (subMenu == null)
			{
				subMenu = new ActionItemSubMenu(this.actions, subMenuText);
				subMenu.Order = order;
				this.Add(subMenu);
			}
			return subMenu;
		}

		public ActionItemSubMenu AddSubMenu(string subMenuText, int order = 500)
		{
			var sub = new ActionItemSubMenu(this.actions, subMenuText);
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
		
		int Compare( IActionItem x, IActionItem y )  {
			int sectionx = x.Order;
			int sectiony = y.Order;
			if (sectionx == sectiony)
			{
				sectionx = this.IndexOf(x);
				sectiony = this.IndexOf(y);
			}
			return sectionx.CompareTo(sectiony);
		}
		
		public ToolBar GenerateToolBar()
		{
			var toolBar = new ToolBar(Generator);
			Generate (toolBar);
			return toolBar;
		}

		public void Generate(ToolBar toolBar)
		{
			var list = new List<IActionItem>(this);
			list.Sort(Compare);
			foreach (IActionItem ai in list)
			{
				//ActionItem action = ai as ActionItem;
				//if (action != null) Console.WriteLine("\tGenerating toolbar item {0}", action.Action.ID);
				//else Console.WriteLine("\tGenerating action");
				ai.Generate(toolBar);
			}
		}

	}
}
