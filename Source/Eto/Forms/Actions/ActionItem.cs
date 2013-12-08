using System;

namespace Eto.Forms
{
	public partial interface IActionItem
	{
		ToolBarItem GenerateToolBarItem(Generator generator, ToolBarTextAlign textAlign);

		int Order { get; }
	}
	
	public abstract partial class ActionItemBase : IActionItem
	{
		int order = 500;
		
		public abstract ToolBarItem GenerateToolBarItem(Generator generator, ToolBarTextAlign textAlign);
		
		public int Order
		{
			get { return order; }
			set { order = value; }
		}
		
		public string ToolBarItemStyle { get; set; }
		
		public string MenuItemStyle { get; set; }
	}
	
	public partial class ActionItemSeparator : ActionItemBase
	{
		public SeparatorToolBarItemType ToolBarType { get; set; }

		public override ToolBarItem GenerateToolBarItem(Generator generator, ToolBarTextAlign textAlign)
		{
			var tbb = new SeparatorToolBarItem(generator) { Type = ToolBarType };
			if (!string.IsNullOrEmpty (ToolBarItemStyle))
				tbb.Style = ToolBarItemStyle;
			return tbb;
		}

	}

	public partial class ActionItemSubMenu : ActionItemBase
	{
		public ActionItemSubMenu(ActionCollection actions, string subMenuText)
		{
			this.Actions = new ActionItemCollection(actions);
			this.SubMenuText = subMenuText;
		}

		public string ID { get; set; }

		public string SubMenuText { get; set; }
		
		public ActionItemCollection Actions { get; private set; }

		public override ToolBarItem GenerateToolBarItem(Generator generator, ToolBarTextAlign textAlign)
		{
			return null;
		}
	}
	
	public partial class ActionItem : ActionItemBase
	{
		
		public ActionItem(BaseAction action) : this(action, false)
		{

		}

		
		public ActionItem(BaseAction action, bool showLabel)
		{
			if (action == null) throw new ArgumentNullException("action", "Action cannot be null for an action item");
			this.Action = action;
			this.ShowLabel = showLabel;
		}

		public BaseAction Action { get; set; }

		public bool ShowLabel { get; set; }

		public override ToolBarItem GenerateToolBarItem(Generator generator, ToolBarTextAlign textAlign)
		{
			var item = Action.GenerateToolBarItem(this, generator, textAlign);
			if (item != null) {
				if (!string.IsNullOrEmpty (ToolBarItemStyle))
					item.Style = ToolBarItemStyle;
			}
			return item;
		}
	}
}
