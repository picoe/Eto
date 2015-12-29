using System;
using System.Collections;
using SD = System.Drawing;
using SWF = System.Windows.Forms;
using Eto.Forms;
using System.Collections.Generic;
using System.Linq;

namespace Eto.WinForms.Forms.Menu
{
	public class RadioMenuItemHandler : MenuItemHandler<SWF.ToolStripMenuItem, RadioMenuItem, RadioMenuItem.ICallback>, RadioMenuItem.IHandler
	{
		List<RadioMenuItem> group;

		public RadioMenuItemHandler()
		{
			Control = new SWF.ToolStripMenuItem();
			Control.Click += control_Click;
		}

		void control_Click(object sender, EventArgs e)
		{
			UncheckGroup();
			Checked = true;
			Callback.OnClick(Widget, e);
		}

		void UncheckGroup()
		{
			if (group != null)
			{
				var checkedItem = group.FirstOrDefault(r => r.Checked && r != Widget);
				if (checkedItem != null)
					checkedItem.Checked = false;
			}
		}

		public void Create(RadioMenuItem controller)
		{
			if (controller != null)
			{
				var controllerInner = (RadioMenuItemHandler)controller.Handler;
				if (controllerInner.group == null)
				{
					controllerInner.group = new List<RadioMenuItem>();
					controllerInner.group.Add(controller);
				}
				controllerInner.group.Add(Widget);
				group = controllerInner.group;
			}
		}

		public bool Checked
		{
			get { return Control.Checked; }
			set
			{
				if (value)
					UncheckGroup();
                Control.Checked = value;
			}
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case RadioMenuItem.CheckedChangedEvent:
					Control.CheckedChanged += (sender, e) => Callback.OnCheckedChanged(Widget, EventArgs.Empty);
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}
	}
}
