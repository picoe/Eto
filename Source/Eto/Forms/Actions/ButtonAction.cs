using System;
using System.Reflection;
using Eto.Drawing;

namespace Eto.Forms
{
	[Obsolete("Use Command and menu/toolbar apis directly instead")]
	public static class ButtonActionExtensions
	{
		
		public static ButtonAction AddButton(this ActionCollection actions, string id, string text)
		{
			var action = new ButtonAction(id, text);
			actions.Add(action);
			return action;
		}
		
		public static ButtonAction AddButton(this ActionCollection actions, string id, string text, string iconResource, EventHandler<EventArgs> activated, params Keys[] accelerators)
		{
#if WINRT
			throw new NotImplementedException("WinRT does not support Assembly.GetCallingAssembly");
#else
			Icon icon = null;
			if (!string.IsNullOrEmpty(iconResource)) icon = Icon.FromResource (Assembly.GetCallingAssembly (), iconResource);
			var action = new ButtonAction(id, text, icon, activated);
			action.Accelerators = accelerators;
			actions.Add(action);
			return action;
#endif
		}

		public static ButtonAction AddButton(this ActionCollection actions, string id, string text, string iconResource, EventHandler<EventArgs> activated)
		{
#if WINRT
			throw new NotImplementedException("WinRT does not support Assembly.GetCallingAssembly");
#else
			Icon icon = null;
			if (!string.IsNullOrEmpty(iconResource)) icon = Icon.FromResource (Assembly.GetCallingAssembly (), iconResource);
			var action = new ButtonAction(id, text, icon, activated);
			actions.Add(action);
			return action;
#endif
		}

		public static ButtonAction AddButton(this ActionCollection actions, string id, string text, Icon icon, EventHandler<EventArgs> activated)
		{
			var action = new ButtonAction(id, text, icon, activated);
			actions.Add(action);
			return action;
		}

		
		public static ButtonAction AddButton(this ActionCollection actions, string id, string text, EventHandler<EventArgs> activated, params Keys[] accelerators)
		{
			var action = new ButtonAction(id, text, activated);
			action.Accelerators = accelerators;
			actions.Add(action);
			return action;
		}

		public static ButtonAction AddButton(this ActionCollection actions, string id, string text, EventHandler<EventArgs> activated)
		{
			var action = new ButtonAction(id, text, activated);
			actions.Add(action);
			return action;
		}
		
	}
	
	[Obsolete("Use Command and menu/toolbar apis directly instead")]
	public partial class ButtonAction : BaseAction
	{
		internal Command command;

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

		public override ToolItem GenerateToolBarItem(ActionItem actionItem, Generator generator, ToolBarTextAlign textAlign)
		{
			var tbb = new ButtonToolItem(generator);
			tbb.ID = ID;
			tbb.Enabled = Enabled;
			if (ShowLabel || actionItem.ShowLabel || textAlign != ToolBarTextAlign.Right) tbb.Text = ToolBarText;
			//Console.WriteLine("Adding toolbar {0}", ToolBarText);
			if (Image != null) tbb.Image = Image;
			if (!string.IsNullOrEmpty (ToolBarItemStyle))
				tbb.Style = ToolBarItemStyle;
			new ToolBarConnector(this, tbb);
			return tbb;
		}
		
		protected class ToolBarConnector
		{
			readonly ButtonToolItem toolBarButton;
			readonly ButtonAction action;
			public ToolBarConnector(ButtonAction action, ButtonToolItem toolBarButton)
			{
				this.toolBarButton = toolBarButton;
				this.toolBarButton.Click += toolBarButton_Click;
				this.action = action;
				this.action.EnabledChanged += new EventHandler<EventArgs>(action_EnabledChanged).MakeWeak(e => this.action.EnabledChanged -= e);
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
