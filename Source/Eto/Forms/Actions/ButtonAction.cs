using System;
using System.Reflection;
using System.Collections;
using Eto.Drawing;

namespace Eto.Forms
{
	
	public static class ButtonActionExtensions
	{
		
		public static ButtonAction AddButton(this ActionCollection actions, string id, string text)
		{
			ButtonAction action = new ButtonAction(id, text);
			actions.Add(action);
			return action;
		}
		
		public static ButtonAction AddButton(this ActionCollection actions, string id, string text, string iconResource, EventHandler<EventArgs> activated, params Key[] accelerators)
		{
			Icon icon = null;
			if (iconResource != string.Empty) icon = new Icon(Assembly.GetCallingAssembly(), iconResource);
			ButtonAction action = new ButtonAction(id, text, icon, activated);
			action.Accelerators = accelerators;
			actions.Add(action);
			return action;
		}

		public static ButtonAction AddButton(this ActionCollection actions, string id, string text, string iconResource, EventHandler<EventArgs> activated)
		{
			Icon icon = null;
			if (iconResource != string.Empty) icon = new Icon(Assembly.GetCallingAssembly(), iconResource);
			ButtonAction action = new ButtonAction(id, text, icon, activated);
			actions.Add(action);
			return action;
		}

		public static ButtonAction AddButton(this ActionCollection actions, string id, string text, Icon icon, EventHandler<EventArgs> activated)
		{
			ButtonAction action = new ButtonAction(id, text, icon, activated);
			actions.Add(action);
			return action;
		}

		
		public static ButtonAction AddButton(this ActionCollection actions, string id, string text, EventHandler<EventArgs> activated, params Key[] accelerators)
		{
			ButtonAction action = new ButtonAction(id, text, activated);
			action.Accelerators = accelerators;
			actions.Add(action);
			return action;
		}

		public static ButtonAction AddButton(this ActionCollection actions, string id, string text, EventHandler<EventArgs> activated)
		{
			ButtonAction action = new ButtonAction(id, text, activated);
			actions.Add(action);
			return action;
		}
		
	}
	
	public partial class ButtonAction : BaseAction
	{
		
		public ButtonAction(string id, string text, Icon icon, EventHandler<EventArgs> activated)
			: base(id, text, icon, activated)
		{
			
		}
		
		public ButtonAction(string id, string text, EventHandler<EventArgs> activated)
			: base(id, text, activated)
		{
		}
		
		public ButtonAction(string id, string text)
			: base(id, text)
		{
		}
		
		public ButtonAction()
		{
		}
		
		public virtual ToolBarItem GenerateToolBar(ActionItem actionItem, ToolBar toolBar)
		{
			ToolBarButton tbb = new ToolBarButton(toolBar.Generator);
			tbb.ID = this.ID;
			tbb.Enabled = this.Enabled;
			if (ShowLabel || actionItem.ShowLabel || toolBar.TextAlign != ToolBarTextAlign.Right) tbb.Text = ToolBarText;
			//Console.WriteLine("Adding toolbar {0}", ToolBarText);
			if (Icon != null) tbb.Icon = Icon;
			new ToolBarConnector(this, tbb);
			return tbb;
		}
		
		public override void Generate(ActionItem actionItem, ToolBar toolBar)
		{
			var tbb = GenerateToolBar(actionItem, toolBar);
			toolBar.Items.Add(tbb);
		}
		
		protected class ToolBarConnector
		{
			ToolBarButton toolBarButton;
			ButtonAction action;
			public ToolBarConnector(ButtonAction action, ToolBarButton toolBarButton)
			{
				this.toolBarButton = toolBarButton;
				this.toolBarButton.Click += toolBarButton_Click;
				this.action = action;
				this.action.EnabledChanged += new EventHandler<EventArgs>(action_EnabledChanged).MakeWeak();
			}
			
			void toolBarButton_Click(Object sender, EventArgs e)
			{
				action.OnActivated(e);
			}
			
			void action_EnabledChanged(Object sender, EventArgs e)
			{
				toolBarButton.Enabled = action.Enabled;
			}
		}
		
	}
	
	
}
