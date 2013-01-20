using System;
using System.Collections;

namespace Eto.Forms
{
	public partial interface IActionItem
	{
		void Generate(ToolBar toolBar);

		int Order { get; }
	}
	
	public abstract partial class ActionItemBase : IActionItem
	{
		int order = 500;
		
		public abstract void Generate(ToolBar toolBar);
		
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

		public override void Generate(ToolBar toolBar)
		{
			var tbb = new SeparatorToolBarItem(toolBar.Generator) { Type = this.ToolBarType };
			if (!string.IsNullOrEmpty (ToolBarItemStyle))
				tbb.Style = ToolBarItemStyle;
			toolBar.Items.Add(tbb);
		}

	}

	public partial class ActionItemSubMenu : ActionItemBase
	{
		public ActionItemSubMenu(ActionCollection actions, string subMenuText)
		{
			this.Actions = new ActionItemCollection(actions);
			this.SubMenuText = subMenuText;
		}

		public string Icon { get; set; }

		public string ID { get; set; }

		public string SubMenuText { get; set; }
		
		public ActionItemCollection Actions { get; private set; }

		public override void Generate(ToolBar toolBar)
		{
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
		
		public override void Generate(ToolBar toolBar)
		{
			var item = Action.Generate(this, toolBar);
			if (item != null) {
				if (!string.IsNullOrEmpty (ToolBarItemStyle))
					item.Style = ToolBarItemStyle;
				toolBar.Items.Add (item);
			}
		}

	}
}
