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
		
		public static RadioAction AddRadio(this ActionCollection actions, RadioAction controller, string id, string text, string iconResource, EventHandler<EventArgs> activated, params Key[] accelerators)
		{
			Icon icon = null;
			if (iconResource != string.Empty) icon = Icon.FromResource(Assembly.GetCallingAssembly(), iconResource);
			RadioAction action = new RadioAction(controller, id, text, icon, activated);
			action.Accelerators = accelerators;
			actions.Add(action);
			return action;
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
			if (!(sender is RadioAction)) return;
			RadioAction action = (RadioAction)sender;
			action.Checked = true;
		}

		public override ToolBarItem Generate(ActionItem actionItem, ToolBar toolBar)
		{
			throw new NotImplementedException("cannot put radio buttons on the toolbar just yet");
		}
		
	}
}
