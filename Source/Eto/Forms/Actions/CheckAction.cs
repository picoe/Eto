using System;
using Eto.Drawing;
using Eto;
using System.Reflection;

namespace Eto.Forms
{

	public static class CheckActionExtensions
	{
		public static CheckAction AddCheck(this ActionCollection actions, string id, string text, string iconResource, EventHandler<EventArgs> activated)
		{
			return AddCheck(actions, id, text, iconResource, activated, null);
		}

		public static CheckAction AddCheck(this ActionCollection actions, string id, string text, EventHandler<EventArgs> activated)
		{
			return AddCheck(actions, id, text, string.Empty, activated, null);
		}

		public static CheckAction AddCheck(this ActionCollection actions, string id, string text, string iconResource, EventHandler<EventArgs> activated, params Key[] accelerators)
		{
			Icon icon = null;
			if (!string.IsNullOrEmpty(iconResource)) icon = Icon.FromResource (Assembly.GetCallingAssembly (), iconResource);
			var action = new CheckAction(id, text, icon, activated);
			action.Accelerators = accelerators;
			actions.Add(action);
			return action;
		}
		
		public static bool RemoveCheckHandler(this ActionCollection actions, string actionID, EventHandler<EventArgs> checkChangedHandler)
		{
			var action = actions[actionID] as CheckAction;
			if (action != null)
			{
				action.CheckedChanged -= checkChangedHandler;
				return true;
			}
			return false;
		}
		
		
	}
	
	public partial class CheckAction : BaseAction
	{
		bool isChecked;

		public event EventHandler<EventArgs> CheckedChanged;

		public CheckAction()
		{
		}

		public CheckAction(string id, string text, Icon icon, EventHandler<EventArgs> activated) : base(id, text, icon, activated)
		{
		}

		public void OnCheckedChanged(EventArgs e)
		{
			if (CheckedChanged != null) CheckedChanged(this, e);
		}

		public bool Checked
		{
			get { return isChecked; }
			set
			{
				if (isChecked != value)
				{
					isChecked = value;
					OnCheckedChanged(EventArgs.Empty);
				}
			}
		}

		public static void Toggle(object sender, EventArgs args)
		{
			var action = sender as CheckAction;
			if (action != null)
			{
				action.Checked = !action.Checked;
			}
		}

		public override ToolBarItem GenerateToolBarItem(ActionItem actionItem, Generator generator, ToolBarTextAlign textAlign)
		{
			var tbb = new CheckToolBarButton(generator);
			tbb.ID = ID;
			tbb.Checked = Checked;
			tbb.Enabled = Enabled;
			if (ShowLabel || actionItem.ShowLabel || textAlign != ToolBarTextAlign.Right) tbb.Text = ToolBarText;
			if (Image != null) tbb.Image = Image;
			if (!string.IsNullOrEmpty (ToolBarItemStyle))
				tbb.Style = ToolBarItemStyle;
			new ToolBarConnector(this, tbb);
			return tbb;
		}

		class ToolBarConnector
		{
			readonly CheckToolBarButton toolBarButton;
			readonly CheckAction action;
			bool changing;
			
			public ToolBarConnector(CheckAction action, CheckToolBarButton toolBarButton)
			{
				this.toolBarButton = toolBarButton;
				this.toolBarButton.Click += toolBarButton_Click;
				this.action = action;
				this.action.EnabledChanged += new EventHandler<EventArgs>(action_EnabledChanged).MakeWeak(e => this.action.EnabledChanged -= e);
				this.action.CheckedChanged += new EventHandler<EventArgs>(action_CheckedChanged).MakeWeak(e => this.action.CheckedChanged -= e);
			}
			
			void toolBarButton_Click(Object sender, EventArgs e)
			{
				if (!changing) action.OnActivated(e);
			}
			
			void action_CheckedChanged(Object sender, EventArgs e)
			{
				changing = true;
				toolBarButton.Checked = action.Checked;
				changing = false;
			}
			
			void action_EnabledChanged(Object sender, EventArgs e)
			{
				toolBarButton.Enabled = action.Enabled;
			}
		}

	}
}
