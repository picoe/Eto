using System;

namespace Eto.Forms
{
	[Obsolete("Use Command and menu/toolbar apis directly instead")]
	public partial interface IActionItem
	{
		ToolItem GenerateToolBarItem(Generator generator, ToolBarTextAlign textAlign);

		int Order { get; }
	}
	
	[Obsolete("Use Command and menu/toolbar apis directly instead")]
	public abstract partial class ActionItemBase : IActionItem
	{
		int order = 500;
		
		public abstract ToolItem GenerateToolBarItem(Generator generator, ToolBarTextAlign textAlign);
		
		public int Order
		{
			get { return order; }
			set { order = value; }
		}
		
		public string ToolBarItemStyle { get; set; }
		
		public string MenuItemStyle { get; set; }
	}
	
	[Obsolete("Use Command and menu/toolbar apis directly instead")]
	public partial class ActionItemSeparator : ActionItemBase
	{
		public SeparatorToolItemType ToolBarType { get; set; }

		public override ToolItem GenerateToolBarItem(Generator generator, ToolBarTextAlign textAlign)
		{
			var tbb = new SeparatorToolItem(generator) { Type = ToolBarType };
			if (!string.IsNullOrEmpty (ToolBarItemStyle))
				tbb.Style = ToolBarItemStyle;
			return tbb;
		}

	}

	[Obsolete("Use Command and menu/toolbar apis directly instead")]
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

		public override ToolItem GenerateToolBarItem(Generator generator, ToolBarTextAlign textAlign)
		{
			return null;
		}
	}
	
	[Obsolete("Use Command and menu/toolbar apis directly instead")]
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

		public override ToolItem GenerateToolBarItem(Generator generator, ToolBarTextAlign textAlign)
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
