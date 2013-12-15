using System;
using Eto.Drawing;
using System.Reflection;

namespace Eto.Forms
{
	public static class RadioActionExtensions
	{
		public static RadioAction AddRadio(this ActionCollection actions, RadioAction controller, string id, string text)
		{
			return AddRadio(actions, controller, id, text, string.Empty, null, null);
		}
		
		public static RadioAction AddRadio(this ActionCollection actions, RadioAction controller, string id, string text, string iconResource, EventHandler<EventArgs> activated, params Keys[] accelerators)
		{
#if WINRT
			throw new NotImplementedException();
#else
			Icon icon = null;
			if (!string.IsNullOrEmpty(iconResource)) icon = Icon.FromResource(Assembly.GetCallingAssembly(), iconResource);
			var action = new RadioAction(controller, id, text, icon, activated);
			action.Accelerators = accelerators;
			actions.Add(action);
			return action;
#endif
		}
		
	}
	
	public partial class RadioAction : BaseAction
	{
		bool isChecked;
		RadioAction Controller { get; set; }

		public event EventHandler<EventArgs> CheckedChanged;

		public RadioAction(RadioAction controller)
		{
			this.Controller = controller;
		}
		
		public RadioAction(RadioAction controller, string id, string text, Icon icon, EventHandler<EventArgs> activated) : base(id, text, icon, activated)
		{
			this.Controller = controller;
		}

		public virtual void OnCheckedChanged(EventArgs e)
		{
			if (CheckedChanged != null) CheckedChanged(this, e);
		}

		public bool Checked
		{
			get { return isChecked; }
			set
			{
				isChecked = value;
				OnCheckedChanged(EventArgs.Empty);
			}
		}

		public static void Toggle(object sender, EventArgs args)
		{
			var action = sender as RadioAction;
			if (action != null)
			{
				action.Checked = true;
			}
		}

		public override ToolItem GenerateToolBarItem(ActionItem actionItem, Generator generator, ToolBarTextAlign textAlign)
		{
			throw new NotImplementedException("cannot put radio buttons on the toolbar just yet");
		}		
	}
}
